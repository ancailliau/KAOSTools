using System;
using System.Collections.Generic;
using KAOSTools.Core;
using System.Linq;
using UCLouvain.KAOSTools.Core.SatisfactionRates;
using UCLouvain.KAOSTools.Propagators;
using System.Diagnostics;
using UCLouvain.KAOSTools.Integrators;

namespace UCLouvain.KAOSTools.Optimizer
{
    public class NaiveCountermeasureSelectionOptimizer
    {
        KAOSModel _model;
        OptimizationStatistics stats;

        SoftResolutionIntegrator integrator;

        const double EPSILON = 1E-15;

        public NaiveCountermeasureSelectionOptimizer (KAOSModel model)
        {
            _model = model;
            stats = new OptimizationStatistics ();
            integrator = new SoftResolutionIntegrator (model);
        }

        /// <summary>
        /// Returns the minimal cost ensuring the goal RDS. Returns -1 if no
        /// selection satisfy the RDS.
        /// </summary>
        /// <param name="goal">The goal</param>
        /// <returns>The minimal cost.</returns>
        public double GetMinimalCost (Goal goal, IPropagator propagator)
        {
            var timer = Stopwatch.StartNew ();
            int minCost = -1;

            var sr = (DoubleSatisfactionRate) propagator.GetESR (goal);
            if (sr.SatisfactionRate > goal.RDS)
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
                
                sr = (DoubleSatisfactionRate) propagator.GetESR (goal);
                
                // Console.WriteLine ( new OptimalSelection (r, cost, sr.SatisfactionRate) );

                foreach (var resolution in r) {
                    integrator.Remove (resolution);
                }

                if (sr.SatisfactionRate > goal.RDS) {
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

        public IEnumerable<OptimalSelection> GetOptimalSelections (double minCost, Goal goal, IPropagator propagator)
        {
            var timer = Stopwatch.StartNew ();
            var optimalSelections = new List<OptimalSelection> ();
            double bestSR = 0;
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
                
                var sr = (DoubleSatisfactionRate) propagator.GetESR (goal);

                foreach (var resolution in activeResolutions) {
                    integrator.Remove (resolution);
                }
                
                if (sr.SatisfactionRate > bestSR + EPSILON) {
                    bestSR = sr.SatisfactionRate;
                    optimalSelections = new List<OptimalSelection> ();
                    optimalSelections.Add (new OptimalSelection (activeResolutions, cost, sr.SatisfactionRate));
                } else if (Math.Abs (sr.SatisfactionRate - bestSR) < EPSILON) {
                    optimalSelections.Add (new OptimalSelection (activeResolutions, cost, sr.SatisfactionRate));
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
