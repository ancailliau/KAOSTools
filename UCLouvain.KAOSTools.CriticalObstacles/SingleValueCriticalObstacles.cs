using System;
using System.Collections.Generic;
using System.Linq;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Propagators.BDD;
using UCLouvain.KAOSTools.Core.SatisfactionRates;
using MoreLinq;

namespace UCLouvain.KAOSTools.CriticalObstacles
{
	public class CriticalScore
	{
	
		public double ViolationSeverity {
			get;
			set;
		}
		
		public double CombinationProbability { get; set; }

		public CriticalScore(double combinationProbability, double violationSeverity)
		{
			CombinationProbability = combinationProbability;
			ViolationSeverity = violationSeverity;
		}

	}
	
	public class SingleValueCriticalObstacles : AbstractCriticalObstacles
	{
		readonly BDDBasedPropagator _propagator;

		readonly double rsr;

		public SingleValueCriticalObstacles(KAOSModel model, Goal root) : base(model, root)
		{
			_propagator = new BDDBasedPropagator(model);
			_propagator.PreBuildObstructionSet(root);

			rsr = root.RDS;
		}
		
		public Dictionary<HashSet<Obstacle>, CriticalScore> GetObstacleScores (int combination_size)
		{
			var dict = new Dictionary<HashSet<Obstacle>, CriticalScore>();

			var obstacles = _model.LeafObstacles().ToList();

			foreach (var combination in GetAllCombinations(obstacles, combination_size)) {
				var d = (DoubleSatisfactionRate)_propagator.GetESR(_root, combination);
				var key = combination.ToHashSet();
				double combination_probability = combination.Aggregate(1d, (arg1, arg2) =>
				{
					var sat_rate = (DoubleSatisfactionRate) _model.satisfactionRateRepository.GetObstacleSatisfactionRate(arg2.Identifier);
					if (sat_rate == null) {
						throw new InvalidOperationException($"The obstacle '{arg2.Identifier}' was not estimated");
					}
					return arg1 * sat_rate.SatisfactionRate;
				});
				var val = new CriticalScore(combination_probability, d.ViolationSeverity(rsr));
				dict.Add(key, val);
			}

			return dict;
		}
	}
}
