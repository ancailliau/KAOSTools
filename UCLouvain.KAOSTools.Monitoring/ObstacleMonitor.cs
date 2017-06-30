using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LtlSharp;
using LtlSharp.Monitoring;
using MoreLinq;
using NLog;
using UCLouvain.KAOSTools.Core;
namespace UCLouvain.KAOSTools.Monitoring
{
	public class ObstacleMonitor : KAOSCoreElementMonitor
	{
		protected Obstacle obstacle;
		
		public Obstacle Obstacle {
			get { return obstacle;  }
		}

		protected IStateInformationStorage storage;

        static Logger logger = LogManager.GetCurrentClassLogger();

		public ObstacleMonitor(Obstacle obstacle,
						   KAOSModel model,
						   IStateInformationStorage storage,
						   TimeSpan monitoringDelay) : base(model, obstacle, monitoringDelay)
		{
			this.storage = storage;
			this.obstacle = obstacle;

			SetProjection();
			Initialize();
		}
		
		void SetProjection ()
		{	
			if (obstacle.CustomData.ContainsKey("hashProjection")) {
				projection = obstacle.CustomData["hashProjection"].Split(',').ToHashSet();
			} else {
				projection = obstacle.FormalSpec.PredicateReferences.Select(x => x.PredicateIdentifier).ToHashSet();
			}
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
			
			Run();
		}

		public IStateInformation MonitoredSatisfactionRate => monitor.Max;
	}
}
