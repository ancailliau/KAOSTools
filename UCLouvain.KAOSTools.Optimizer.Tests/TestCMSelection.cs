using NUnit.Framework;
using System;
using KAOSTools.Parsing;
using KAOSTools.Core;
using UCLouvain.KAOSTools.Propagators.BDD;
using System.Linq;

namespace UCLouvain.KAOSTools.Optimizer.Tests
{
    [TestFixture ()]
    public class TestCMSelection
    {
        [Test ()]
        public void TestBestCountermeasureSelection ()
        {
            var input = @"declare goal [ anchor ] rsr .75 refinedby child1, child2 end
                          declare goal [ child1 ] obstructedby o1 end
                          declare goal [ child2 ] obstructedby o2 end
                          declare obstacle [ o1 ] probability .2 resolvedby [substitution:anchor] cm1 end
                          declare obstacle [ so1 ] probability .1 resolvedby [substitution:anchor] cm2 end
                          declare obstacle [ so2 ] probability .3 resolvedby [substitution:anchor] cm3 end
                          declare obstacle [ o2 ] refinedby so1 refinedby so2 end";
                          
            ModelBuilder parser = new ModelBuilder ();
            var model = parser.Parse (input);

            var propagator = new BDDBasedResolutionPropagator (model);
            
            var optimizer = new NaiveCountermeasureSelectionOptimizer (model);
            var anchor = model.Goal ("anchor");
            
            var minimalCost = optimizer.GetMinimalCost (anchor, propagator);
            Assert.AreEqual (1, minimalCost);

            var optimalSelections = optimizer.GetOptimalSelections (minimalCost, anchor, propagator);
            Assert.AreEqual (2, optimalSelections.Count());
            Assert.That (optimalSelections.Any(x => x.Resolutions.All (y => y.ResolvingGoalIdentifier == "cm2")));
            Assert.That (optimalSelections.Any(x => x.Resolutions.All (y => y.ResolvingGoalIdentifier == "cm3")));
        }
    }
}
