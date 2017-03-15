using System;
using KAOSTools.MetaModel;
using System.Collections.Generic;
using System.Linq;
using UncertaintySimulation;

namespace ObstaclePriotirizer
{
    public class EffectCalculator
    {
        KAOSModel model;

        public EffectCalculator(KAOSModel model)
        {
            this.model = model;
        }

        public Dictionary<List<Obstacle>, double> GetEffects(Goal g)
        {
            ComputeObstructionSets();
            var effects = new Dictionary<List<Obstacle>, double> ();
            Populate(g, obstructions[g].mapping.Keys.Cast<Obstacle>(), Enumerable.Empty<Obstacle>(), effects);
            return effects;
        }

        void Populate(Goal g, IEnumerable<Obstacle> toVisit, IEnumerable<Obstacle> active, Dictionary<List<Obstacle>, double> effects)
        {
            var os = obstructions[g];
            if (toVisit.Count() == 0) {
                var sample = new Dictionary<int, double> ();
                foreach (var o in active) {
                    if (os.mapping.ContainsKey(o)) {
                        var index = os.mapping[o];
                        sample.Add(index, o.EPS);
                    } else {
                        Console.WriteLine("Obstacle {0} not in obstruction superset for {1}", o.FriendlyName, g.FriendlyName);
                    }
                }
                var effect = 1 - os.GetProbability(sample);
                if (effect < 1) {
                    effects.Add(active.ToList(), effect);
                } else {
                    // ignore combination leading to no effect
                }
                
            } else {
                var current = toVisit.First();
                if (current.EPS > 0) {
                    Populate(g, toVisit.Skip(1), active.Union(new[] { current }), effects);
                }
                Populate(g, toVisit.Skip(1), active, effects);
            }
        }

        // Copy/paste

        Dictionary<Goal, ObstructionSuperset> obstructions;
        Dictionary<Obstacle, ObstructionSuperset> obstructionObstacles;

        void ComputeObstructionSets()
        {
            obstructions = new Dictionary<Goal, ObstructionSuperset>();
            // Get the obstruction set
            var goals = model.Goals();
            foreach (var goal in goals) {
                ObstructionSuperset obstructionSuperset;
                obstructionSuperset = goal.GetObstructionSuperset(true);
                obstructions.Add(goal, obstructionSuperset);
            }

            obstructionObstacles = new Dictionary<Obstacle, ObstructionSuperset>();
            // Get the obstruction set
            var obstacles = model.Obstacles();
            foreach (var obstacle in obstacles) {
                ObstructionSuperset obstructionSuperset;
                obstructionSuperset = obstacle.GetObstructionSuperset();
                obstructionObstacles.Add(obstacle, obstructionSuperset);
            }
        }

        public void ComputeCPS()
        {
            ComputeObstructionSets();

            foreach (var kv in obstructionObstacles) {
                var sampleVector = new Dictionary<int, double>();
                foreach (var o in kv.Value.mapping) {
                    sampleVector.Add(o.Value, ((Obstacle)o.Key).EPS);
                }
                var p = kv.Value.GetProbability(sampleVector);
                kv.Key.CPS = p;
                Console.WriteLine("Computing probability for {0}: {1}", kv.Key.FriendlyName, kv.Key.CPS);
            }

            foreach (var kv in obstructions) {
                var sampleVector = new Dictionary<int, double>();
                foreach (var o in kv.Value.mapping) {
                    sampleVector.Add(o.Value, ((Obstacle)o.Key).EPS);
                }
                var p = 1 - kv.Value.GetProbability(sampleVector);
                kv.Key.CPS = p;
                Console.WriteLine("Computing probability for {0}: {1}", kv.Key.FriendlyName, kv.Key.CPS);
            }
        }
   }
}
