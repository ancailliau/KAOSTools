﻿using NUnit.Framework;
using System;
using System.IO;
using KAOSTools.Parsing;
using KAOSTools.Core;
using UCLouvain.KAOSTools.Core.SatisfactionRates;
using System.Linq;
using UCLouvain.KAOSTools.Parsing;
using UCLouvain.KAOSTools.Propagators.BDD;

namespace UCLouvain.KAOSTools.Propagators.Tests
{
    [TestFixture ()]
    public class TestPropagatorRandomModels
    {
        [Test()]
        [Repeat(1)]
        public void TestMilestoneGoalRefinement ()
        {
            var options = new RandomModelOptions {
                MinGoalBranchingFactor = 2,
                MaxGoalBranchingFactor = 4,
                GoalMaxHeight = 2,
                
                MinObstacleANDBranchingFactor = 2,
                MaxObstacleANDBranchingFactor = 4,
            };
        
            var generator = new RandomModelGenerator (options);   
            var model = generator.Generate ();
            
            var p1 = new PatternBasedPropagator (model);
            var p2 = new BDDBasedPropagator (model);
            
            Console.WriteLine ("Generated goals: " + model.Goals ().Count ());
            Console.WriteLine ("Generated obstacles: " + model.Obstacles ().Count ());
            
            var root = model.RootGoals ().Single ();
            var sr1 = (DoubleSatisfactionRate) p1.GetESR (root);
            var sr2 = (DoubleSatisfactionRate) p2.GetESR (root);
            
            Assert.AreEqual (sr1.SatisfactionRate, sr2.SatisfactionRate, 0.0001);
        }
    }
}
