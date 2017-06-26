using System.Collections.Generic;
using System.IO;
using System.Linq;
using KAOSTools.Core;
using KAOSTools.Parsing;
using NUnit.Framework;
using UCLouvain.KAOSTools.Core.SatisfactionRates;
using UCLouvain.KAOSTools.Propagators.BDD;

namespace UCLouvain.KAOSTools.Propagators.Tests
{
    [TestFixture ()]
    public class TestBDDBasedPropagator : TestPropagator
    {
        protected override IPropagator GetPropagator (KAOSModel model)
        {
            return new BDDBasedPropagator (model);
        }

        [TestCase (0, 0, 1)]
        [TestCase (0.5, 0.5, 0.5 * 0.5)]
        [TestCase (1, 0, 0)]
        [TestCase (0, 1, 0)]
        [TestCase (.99, .99, (1 - .99) * (1 - .99))]
        [TestCase (.01, .01, (1 - .01) * (1 - .01))]
        public void TestMilestoneGoalRefinement (double o1_esr, double o2_esr, double expected_root_esr)
        {
            TestMilestone (o1_esr, o2_esr, expected_root_esr);
        }

        [TestCase (0,   0,   .5, .5, 1)]
        [TestCase (0.5, .5,  .5, .5, 0.5 * 0.5)]
        [TestCase (1,   0,   .5, .5, 0)]
        [TestCase (0,   1,   .5, .5, 0)]
        [TestCase (.99, .99, .5, .5, (1 - .99) * (1 - .99))]
        [TestCase (.01, .01, .5, .5, (1 - .01) * (1 - .01))]
        [TestCase (0,   0,   .3, .7, 1)]
        [TestCase (0.5, .5,  .3, .7, 0.5 * 0.5)]
        [TestCase (1,   0,   .3, .7, 0)]
        [TestCase (0,   1,   .3, .7, 0)]
        [TestCase (.9,  .8,  .3, .7, (1 - .90) * (1 - .80))]
        [TestCase (.02, .01, .3, .7, (1 - .02) * (1 - .01))]
        public void TestCaseRefinement (double o1_esr, double o2_esr, double c1, double c2, double expected_root_esr)
        {
            TestCase (o1_esr, o2_esr, c1, c2, expected_root_esr);
        }
        
        [TestCase (0, 0, 0, 1)]
        [TestCase (0.5, 0.5, .5, 0.5 * 0.5 * .5)]
        [TestCase (1, 0, 0, 0)]
        [TestCase (0, 1, 0, 0)]
        [TestCase (0, 0, 1, 0)]
        [TestCase (1, 1, 0, 0)]
        [TestCase (1, 0, 1, 0)]
        [TestCase (.99, .99, .99, (1 - .99) * (1 - .99) * (1 - .99))]
        [TestCase (.01, .01, .01, (1 - .01) * (1 - .01) * (1 - 0.01))]
        public void TestIntroduceGuardGoalRefinement (double o1_esr, double o2_esr, double o3_esr, double expected_root_esr)
        {
            TestIntroduceGuard (o1_esr, o2_esr, o3_esr, expected_root_esr);
        }
        
        [TestCase (0, 0, 1)]
        [TestCase (0.5, 0.5, 0.5 * 0.5)]
        [TestCase (1, 0, 0)]
        [TestCase (0, 1, 0)]
        [TestCase (.99, .99, (1 - .99) * (1 - .99))]
        [TestCase (.01, .01, (1 - .01) * (1 - .01))]
        public void TestDivideAndConquerGoalRefinement (double o1_esr, double o2_esr, double expected_root_esr)
        {
            TestDivideAndConquer (o1_esr, o2_esr, expected_root_esr);
        }
        
        [TestCase (0, 0, 1)]
        [TestCase (0.5, 0.5, 0.5 * 0.5)]
        [TestCase (1, 0, 0)]
        [TestCase (0, 1, 0)]
        [TestCase (.99, .99, (1 - .99) * (1 - .99))]
        [TestCase (.01, .01, (1 - .01) * (1 - .01))]
        public void TestUnmonitorabilityGoalRefinement (double o1_esr, double o2_esr, double expected_root_esr)
        {
            TestUnmonitorability (o1_esr, o2_esr, expected_root_esr);
        }
        
        [TestCase (0, 0, 1)]
        [TestCase (0.5, 0.5, 0.5 * 0.5)]
        [TestCase (1, 0, 0)]
        [TestCase (0, 1, 0)]
        [TestCase (.99, .99, (1 - .99) * (1 - .99))]
        [TestCase (.01, .01, (1 - .01) * (1 - .01))]
        public void TestUncontrollabilityGoalRefinement (double o1_esr, double o2_esr, double expected_root_esr)
        {
            TestUncontrollability (o1_esr, o2_esr, expected_root_esr);
        }
        
        [Test()]
        public void TestRemoveObstructedGoal ()
        {
            var input = 
                @"declare goal [ anchor ] refinedby child1, child2 end
                  declare goal [ child1 ] obstructedby o1 end
                  declare goal [ child2 ] obstructedby o2 end
                  declare obstacle [ o1 ] probability .1 resolvedby [substitution:anchor] cm1 end
                  declare obstacle [ o2 ] refinedby so1 refinedby so2 end
                  declare obstacle [ so1 ] probability .2 resolvedby [substitution:anchor] cm2 end
                  declare obstacle [ so2 ] probability .3 end
                  declare goal [ cm1 ] end
                  declare goal [ cm2 ] obstructedby o_cm end
                  declare obstacle [ o_cm ] probability .4 end";
            
            var parser = new ModelBuilder ();
            var model = parser.Parse (input);

            var resolutions = model.Resolutions ();
            foreach (var r in resolutions) {
                r.Name = "R_" + r.ResolvingGoalIdentifier;
            }
            var anchor = model.Goal ("anchor");
            
            var p1 = new BDDBasedPropagator (model);
            System.Console.WriteLine (p1.GetESR (anchor));
            
            var p2 = new BDDBasedResolutionPropagator (model);
            System.Console.WriteLine (p2.GetESR (anchor));

            HashSet<Resolution> hashSet = new HashSet<Resolution> (model.Resolutions ());
            System.Console.WriteLine (p2.GetESR (anchor, hashSet));
        }
        
        [Test()]
        public void TestKeepObstructedGoal ()
        {
            var input = 
                @"declare goal [ anchor ] refinedby child1, child2 end
                  declare goal [ child1 ] obstructedby o1 end
                  declare goal [ child2 ] obstructedby o2 end
                  declare obstacle [ o1 ] probability .1 resolvedby [restoration:anchor] cm1 end
                  declare obstacle [ o2 ] refinedby so1 refinedby so2 end
                  declare obstacle [ so1 ] probability .2 resolvedby [restoration:anchor] cm2 end
                  declare obstacle [ so2 ] probability .3 end
                  declare goal [ cm1 ] end
                  declare goal [ cm2 ] obstructedby o_cm end
                  declare obstacle [ o_cm ] probability .4 end";
            
            var parser = new ModelBuilder ();
            var model = parser.Parse (input);

            var resolutions = model.Resolutions ();
            foreach (var r in resolutions) {
                r.Name = "R_" + r.ResolvingGoalIdentifier;
            }
            var anchor = model.Goal ("anchor");
            
            var p1 = new BDDBasedPropagator (model);
            System.Console.WriteLine (p1.GetESR (anchor));
            
            var p2 = new BDDBasedResolutionPropagator (model);
            System.Console.WriteLine (p2.GetESR (anchor));

            HashSet<Resolution> hashSet = new HashSet<Resolution> (model.Resolutions ());
            System.Console.WriteLine (p2.GetESR (anchor, hashSet));
        }
        
    }
}
