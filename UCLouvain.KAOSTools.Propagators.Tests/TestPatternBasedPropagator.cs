using NUnit.Framework;
using System;
using System.IO;
using KAOSTools.Parsing;
using KAOSTools.Core;

namespace UCLouvain.KAOSTools.Propagators.Tests
{
    [TestFixture ()]
    public class TestPatternBasedPropagator
    {
        [TestCase (0, 0, 1)]
        [TestCase (0.5, 0.5, 0.5 * 0.5)]
        [TestCase (1, 0, 0)]
        [TestCase (0, 1, 0)]
        [TestCase (.99, .99, .99 * .99)]
        [TestCase (.01, .01, .01 * .01)]
        public void TestMilestoneGoalRefinement (double o1_esr, double o2_esr, double expected_root_esr)
        {
            Directory.SetCurrentDirectory (TestContext.CurrentContext.TestDirectory);
            const string filename = "./Examples/MilestoneModel.kaos";

            ModelBuilder parser = new ModelBuilder ();
            string input = File.ReadAllText (filename);
            var model = parser.Parse (input, filename);

            var root = model.Goal ("root");
            var o1 = model.Obstacle ("o1");
            var o2 = model.Obstacle ("o2");

            o1.EPS = o1_esr;
            o2.EPS = o2_esr;

            var propagator = new PatternBasedPropagator (model);
            Assert.AreEqual ((1 - o1_esr) * (1 - o2_esr), propagator.GetESR (root));
        }
    }
}
