using System;
using System.Collections.Generic;
using System.Diagnostics;
using LtlSharp;
using LtlSharp.Monitoring;
using NLog;
using UCLouvain.KAOSTools.Core;
namespace UCLouvain.KAOSTools.Monitoring
{
	public class ObstacleMonitor : KAOSCoreElementMonitor
	{
		protected Obstacle obstacle;

		protected IStateInformationStorage storage;

        static Logger logger = LogManager.GetCurrentClassLogger();

		public ObstacleMonitor(Obstacle obstacle,
						   KAOSModel model,
						   HashSet<string> projection,
						   IStateInformationStorage storage,
						   TimeSpan monitoringDelay) : base(model, obstacle, projection, monitoringDelay)
		{
			this.storage = storage;
			this.obstacle = obstacle;

			Initialize();
		}

		void Initialize()
		{
			Ready = false;

			var w = Stopwatch.StartNew();
			logger.Info($"Building an obstacle monitor for '{obstacle.FriendlyName}'.");

			ITLFormula translatedFormula;
			if (obstacle.FormalSpec is Eventually e) {
				translatedFormula = TranslateToLtlSharp(e.Enclosed);
			} else {
				throw new NotSupportedException(
					"Obstacles must follow the pattern F(phi) where phi is an LTL formula."
				);
			}

			logger.Trace("Formula {0} converted to {1}", obstacle.FormalSpec, translatedFormula.Normalize());
			monitor = new ProbabilisticLTLMonitor(translatedFormula, storage);

			w.Stop();
			logger.Info("Time to build monitor: {0}ms", w.ElapsedMilliseconds);
		}
	}
}
