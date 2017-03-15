using System;
using KAOSTools.Core;
using System.Collections.Generic;
using System.Linq;
using UncertaintySimulation;

namespace ObstaclePriotirizer
{
    public class CPSCalculator
    {
        Dictionary<Goal, ObstructionSuperset> obstructions;
        Dictionary<Obstacle, ObstructionSuperset> obstructionObstacles;

        KAOSModel model;

        public CPSCalculator(KAOSModel model)
        {
            this.model = model;
        }

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
