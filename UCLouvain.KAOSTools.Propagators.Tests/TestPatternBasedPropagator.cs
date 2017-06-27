using NUnit.Framework;
using System;
using System.IO;
using UCLouvain.KAOSTools.Parsing;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Core.SatisfactionRates;
using System.Linq;

namespace UCLouvain.KAOSTools.Propagators.Tests
{
    [TestFixture ()]
    public class TestPatternBasedPropagator : TestPropagator
    {
        protected override IPropagator GetPropagator (KAOSModel model)
        {
            return new PatternBasedPropagator (model);
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
        [TestCase (0.5, .5,  .5, .5, 0.5 * 0.5 + 0.5 * 0.5)]
        [TestCase (1,   0,   .5, .5, 0.5)]
        [TestCase (0,   1,   .5, .5, 0.5)]
        [TestCase (.99, .99, .5, .5, 0.5 * (1 - .99) + 0.5 * (1 - .99))]
        [TestCase (.01, .01, .5, .5, 0.5 * (1 - .01) + 0.5 * (1 - .01))]
        [TestCase (0,   0,   .3, .7, 1)]
        [TestCase (0.5, .5,  .3, .7, 0.3 * 0.5 + 0.5 * 0.7)]
        [TestCase (1,   0,   .3, .7, 0.7)]
        [TestCase (0,   1,   .3, .7, 0.3)]
        [TestCase (.9,  .8,  .3, .7, 0.3 * (1 - .90) + 0.7 * (1 - .80))]
        [TestCase (.02, .01, .3, .7, 0.3 * (1 - .02) + 0.7 * (1 - .01))]
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
    }
}
