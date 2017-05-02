using System;
using UCLouvain.KAOSTools.Propagators.BDD;
using UCLouvain.KAOSTools.Propagators.Tests;
using KAOSTools.Core;
using System.Linq;
using UCLouvain.KAOSTools.Core.SatisfactionRates;
using BenchmarkDotNet.Attributes;

namespace UCLouvain.KAOSTools.Propagators.Benchmark
{
    public class PatternVSBDD
    {
        KAOSModel model;
        Goal root;
        BDDBasedPropagator p3;
        
        [Params(1,2,3,4)]
        public int GoalMaxHeight { get; set; }
    
        public PatternVSBDD ()
        {
            var options = new RandomModelOptions {
                MinGoalBranchingFactor = 2,
                MaxGoalBranchingFactor = 4,
                GoalMaxHeight = GoalMaxHeight,
                
                MinObstacleANDBranchingFactor = 2,
                MaxObstacleANDBranchingFactor = 4,
            };
        
            var generator = new RandomModelGenerator (options);   
            model = generator.Generate ();
            root = model.RootGoals ().Single ();
            
            p3 = new BDDBasedPropagator (model);
            p3.PreBuildObstructionSet (root);
        }
        
        [Benchmark]
        public DoubleSatisfactionRate PatternBasedComputation ()
        {
            var p1 = new PatternBasedPropagator (model);
            return (DoubleSatisfactionRate) p1.GetESR (root);
        }
        
        [Benchmark]
        public DoubleSatisfactionRate BDDBasedComputation ()
        {
            var p2 = new BDDBasedPropagator (model);
            return (DoubleSatisfactionRate) p2.GetESR (root);
        }
        
        [Benchmark]
        public DoubleSatisfactionRate BDDBasedComputationPrebuilt ()
        {
            return (DoubleSatisfactionRate) p3.GetESR (root);
        }
    }
}
