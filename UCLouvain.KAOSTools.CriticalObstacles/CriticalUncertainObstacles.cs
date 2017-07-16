using System;
using System.Collections.Generic;
using System.Linq;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Propagators;
using UCLouvain.KAOSTools.Propagators.BDD;
using UCLouvain.KAOSTools.Core.SatisfactionRates;
using MoreLinq;

namespace UCLouvain.KAOSTools.CriticalObstacles
{
	public class UncertaintyCriticalScore
	{
	
		public double UncertaintySpread {
			get;
			private set;
		}
		
		public double ViolationUncertainty {
			get;
			set;
		}
		
		public UncertaintyCriticalScore(double uncertaintySpread, double violationUncertainty)
		{
			UncertaintySpread = uncertaintySpread;
			ViolationUncertainty = violationUncertainty;
		}

	}

	public class CriticalUncertainObstacles : AbstractCriticalObstacles
	{
		readonly BDDBasedUncertaintyPropagator _propagator;

		readonly double rsr;

		int n_samples = 1000;

		public CriticalUncertainObstacles(KAOSModel model, Goal root) : base(model, root)
		{
			_propagator = new BDDBasedUncertaintyPropagator(model, n_samples);
			_propagator.PreBuildObstructionSet(root);

			rsr = root.RDS;
		}

		public Dictionary<HashSet<Obstacle>, UncertaintyCriticalScore> GetObstacleScores ()
		{
			var dict = new Dictionary<HashSet<Obstacle>, UncertaintyCriticalScore>();

			var obstacles = _model.LeafObstacles().ToList();

			foreach (var combination in GetAllCombinations(obstacles, 2)) {
				var d = (SimulatedSatisfactionRate)_propagator.GetESR(_root, combination);
				var key = combination.ToHashSet();
				var val = new UncertaintyCriticalScore (d.UncertaintySpread(rsr), d.ViolationUncertainty(rsr));
				dict.Add(key, val);
			}

			return dict;
		}
	}
}
