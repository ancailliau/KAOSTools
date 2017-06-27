using System;
using System.Collections.Generic;
using System.Diagnostics;
using LtlSharp;
using LtlSharp.Monitoring;
using MoreLinq;
using NLog;
using UCLouvain.KAOSTools.Core;
namespace UCLouvain.KAOSTools.Monitoring
{
	public class GoalMonitor : KAOSCoreElementMonitor
	{
		protected Goal goal;

		protected IStateInformationStorage storage;

        static Logger logger = LogManager.GetCurrentClassLogger();

		public GoalMonitor(Goal goal,
						   KAOSModel model,
						   HashSet<string> projection,
						   IStateInformationStorage storage,
						   TimeSpan monitoringDelay) : base(model, goal, projection, monitoringDelay)
		{
			this.goal = goal;
			this.storage = storage;

			Initialize();
		}

		void Initialize()
		{
			Ready = false;

			var w = Stopwatch.StartNew();
			logger.Info($"Building a goal monitor for '{goal.FriendlyName}'.");

			ITLFormula translatedFormula;
			if (goal.FormalSpec is StrongImply si) {
				translatedFormula = TranslateToLtlSharp(new Imply(si.Left, si.Right));
			} else if (goal.FormalSpec is KAOSTools.Core.Globally g) {
				translatedFormula = TranslateToLtlSharp(g.Enclosed);

			} else {
				throw new NotSupportedException(
					"Goals must follow the pattern G(phi) where phi is an LTL formula."
				);
			}

			logger.Trace("Formula {0} converted to {1}", goal.FormalSpec, translatedFormula.Normalize());
			monitor = new ProbabilisticLTLMonitor(translatedFormula, storage);

			w.Stop();
			logger.Info("Time to build monitor: {0}ms", w.ElapsedMilliseconds);
		}
	}
}
