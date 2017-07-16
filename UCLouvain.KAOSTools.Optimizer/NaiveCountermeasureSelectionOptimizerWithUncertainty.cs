using System;
using System.Collections.Generic;
using UCLouvain.KAOSTools.Core;
using System.Linq;
using UCLouvain.KAOSTools.Core.SatisfactionRates;
using UCLouvain.KAOSTools.Propagators;
using System.Diagnostics;
using UCLouvain.KAOSTools.Integrators;
using UCLouvain.KAOSTools.Propagators.BDD;

namespace UCLouvain.KAOSTools.Optimizer
{
    public class NaiveCountermeasureSelectionOptimizerWithUncertainty
    {
        KAOSModel _model;
        OptimizationStatistics stats;

        SoftResolutionIntegrator integrator;

        const double EPSILON = 1E-15;

		double threshold;

        public NaiveCountermeasureSelectionOptimizerWithUncertainty (KAOSModel model, double threshold)
        {
            _model = model;
            stats = new OptimizationStatistics ();
            integrator = new SoftResolutionIntegrator (model);
			this.threshold = threshold;
        }

        /// <summary>
        /// Returns the minimal cost ensuring the goal RDS. Returns -1 if no
        /// selection satisfy the RDS.
        /// </summary>
        /// <param name="goal">The goal</param>
        /// <returns>The minimal cost.</returns>
        public double GetMinimalCost (Goal goal, BDDBasedUncertaintyPropagator propagator)
        {
            var timer = Stopwatch.StartNew ();
            int minCost = -1;

            var sr = (SimulatedSatisfactionRate) propagator.GetESR (goal);
            if (sr.ViolationUncertainty(goal.RDS) < threshold)
                return 0;

            int count = 0;
            int tested_count = 0;
            int safe_count = 0;

            var countermeasure_goals = _model.Resolutions ().Select (x => x.ResolvingGoalIdentifier).Distinct ();
            
            foreach (var cg in GetAllCombinations (countermeasure_goals.ToList ())) {
                count++;
                var cost = cg.Count ();
                //if (minCost >= 0 && cost > minCost) continue;
                tested_count++;

                var r = _model.Resolutions ().Where (x => cg.Contains (x.ResolvingGoalIdentifier));

                foreach (var resolution in r) {
                    integrator.Integrate (resolution);
                }
                
                sr = (SimulatedSatisfactionRate) propagator.GetESR (goal);
                
                // Console.WriteLine ( new OptimalSelection (r, cost, sr.SatisfactionRate) );

                foreach (var resolution in r) {
                    integrator.Remove (resolution);
                }

                if (sr.ViolationUncertainty(goal.RDS) < threshold) {
                    safe_count++;
                    stats.MaxSafeCost = Math.Max (stats.MaxSafeCost, cost);
                    
                    if (minCost == -1 || cost < minCost) {
                        minCost = cost;
                    }
                }
            }
            timer.Stop ();
            stats.TimeToComputeMinimalCost = timer.Elapsed;
            stats.NbResolvingGoals = countermeasure_goals.Count ();
            stats.NbSelections = count;
            stats.NbTestedSelections = tested_count;
            stats.NbSafeSelections = safe_count;
            
            return minCost;
        }

        public IEnumerable<OptimalSelectionWithUncertainty> GetOptimalSelections (double minCost, Goal goal, BDDBasedUncertaintyPropagator propagator)
        {
            var timer = Stopwatch.StartNew ();
            var optimalSelections = new List<OptimalSelectionWithUncertainty> ();
            double bestVU = 1;
            double bestUS = double.MaxValue;
            int tested_count = 0;

            var countermeasure_goals = _model.Resolutions ().Select (x => x.ResolvingGoalIdentifier).Distinct ();
            
            foreach (var cg in GetAllCombinations (countermeasure_goals.ToList ())) {
            
                var cost = cg.Count ();
                if (minCost > 0 && cost > minCost) continue;
                tested_count++;
                
                var activeResolutions = _model.Resolutions ().Where (x => cg.Contains (x.ResolvingGoalIdentifier));
                
                foreach (var resolution in activeResolutions) {
                    integrator.Integrate (resolution);
                }
                
                var sr = (SimulatedSatisfactionRate) propagator.GetESR (goal);

                foreach (var resolution in activeResolutions) {
                    integrator.Remove (resolution);
                }

				double vu = sr.ViolationUncertainty(goal.RDS);
				double us = sr.UncertaintySpread(goal.RDS);
				if (vu < (bestVU - EPSILON)) {
                    bestVU = vu;
                    optimalSelections = new List<OptimalSelectionWithUncertainty> ();
                    optimalSelections.Add (new OptimalSelectionWithUncertainty (activeResolutions, cost, vu, us));
                } else if (Math.Abs (vu - bestVU) < EPSILON) {
                    optimalSelections.Add (new OptimalSelectionWithUncertainty (activeResolutions, cost, vu, us));
                }
            }
            timer.Stop ();
            stats.TimeToComputeSelection = timer.Elapsed;
            stats.NbTestedSelectionsForOptimality = tested_count;

            return optimalSelections;
        }

        // Code from Stackoverflow
        // https://stackoverflow.com/questions/7802822/all-possible-combinations-of-a-list-of-values
        static List<List<T>> GetAllCombinations<T> (List<T> list)
        {
            List<List<T>> result = new List<List<T>> ();
            result.Add (new List<T> ());
            result.Last ().Add (list [0]);
            if (list.Count == 1)
                return result;
            List<List<T>> tailCombos = GetAllCombinations (list.Skip (1).ToList ());
            tailCombos.ForEach (combo => {
                result.Add (new List<T> (combo));
                combo.Add (list [0]);
                result.Add (new List<T> (combo));
            });
            return result;
        }

        public OptimizationStatistics GetStatistics ()
        {
            return stats;
        }
    }
}
