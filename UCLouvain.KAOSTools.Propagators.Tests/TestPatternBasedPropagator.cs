using NUnit.Framework;
using System;
using System.IO;
using KAOSTools.Parsing;
using KAOSTools.Core;
using UCLouvain.KAOSTools.Core.SatisfactionRates;
using System.Linq;

namespace UCLouvain.KAOSTools.Propagators.Tests
{
    [TestFixture ()]
    public class TestPatternBasedPropagator
    {
        [TestCase (0, 0, 1)]
        [TestCase (0.5, 0.5, 0.5 * 0.5)]
        [TestCase (1, 0, 0)]
        [TestCase (0, 1, 0)]
        [TestCase (.99, .99, (1 - .99) * (1 - .99))]
        [TestCase (.01, .01, (1 - .01) * (1 - .01))]
        public void TestMilestoneGoalRefinement (double o1_esr, double o2_esr, double expected_root_esr)
        {
            Directory.SetCurrentDirectory (TestContext.CurrentContext.TestDirectory);
            const string filename = "./Examples/MilestoneModel.kaos";

            ModelBuilder parser = new ModelBuilder ();
            string input = File.ReadAllText (filename);
            var model = parser.Parse (input, filename);

            var root = model.Goal ("root");

            model.satisfactionRateRepository.AddObstacleSatisfactionRate ("o1", new DoubleSatisfactionRate (o1_esr));
            model.satisfactionRateRepository.AddObstacleSatisfactionRate ("o2", new DoubleSatisfactionRate (o2_esr));

            var propagator = new PatternBasedPropagator (model);
            ISatisfactionRate esr = propagator.GetESR (root);
            Assert.IsInstanceOf (typeof(DoubleSatisfactionRate), esr);
            DoubleSatisfactionRate esrd = (DoubleSatisfactionRate)esr;
            Assert.AreEqual (expected_root_esr, esrd.SatisfactionRate);
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
            Directory.SetCurrentDirectory (TestContext.CurrentContext.TestDirectory);
            const string filename = "./Examples/CaseModel.kaos";

            ModelBuilder parser = new ModelBuilder ();
            string input = File.ReadAllText (filename);
            var model = parser.Parse (input, filename);

            var root = model.Goal ("root");

            var refinement = model.GoalRefinements ().Single ();
            var p1 = (PrimitiveRefineeParameter<double>) refinement.SubGoalIdentifiers.Single (x => x.Identifier == "m1").Parameters;
            var p2 = (PrimitiveRefineeParameter<double>) refinement.SubGoalIdentifiers.Single (x => x.Identifier == "m2").Parameters;
            p1.Value = c1;
            p2.Value = c2;

            model.satisfactionRateRepository.AddObstacleSatisfactionRate ("o1", new DoubleSatisfactionRate (o1_esr));
            model.satisfactionRateRepository.AddObstacleSatisfactionRate ("o2", new DoubleSatisfactionRate (o2_esr));

            var propagator = new PatternBasedPropagator (model);
            ISatisfactionRate esr = propagator.GetESR (root);
            Assert.IsInstanceOf (typeof(DoubleSatisfactionRate), esr);
            DoubleSatisfactionRate esrd = (DoubleSatisfactionRate)esr;
            Assert.AreEqual (expected_root_esr, esrd.SatisfactionRate);
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
            Directory.SetCurrentDirectory (TestContext.CurrentContext.TestDirectory);
            const string filename = "./Examples/IntroduceGuardModel.kaos";

            ModelBuilder parser = new ModelBuilder ();
            string input = File.ReadAllText (filename);
            var model = parser.Parse (input, filename);

            var root = model.Goal ("root");

            model.satisfactionRateRepository.AddObstacleSatisfactionRate ("o1", new DoubleSatisfactionRate (o1_esr));
            model.satisfactionRateRepository.AddObstacleSatisfactionRate ("o2", new DoubleSatisfactionRate (o2_esr));
            model.satisfactionRateRepository.AddObstacleSatisfactionRate ("o3", new DoubleSatisfactionRate (o3_esr));

            var propagator = new PatternBasedPropagator (model);
            ISatisfactionRate esr = propagator.GetESR (root);
            Assert.IsInstanceOf (typeof(DoubleSatisfactionRate), esr);
            DoubleSatisfactionRate esrd = (DoubleSatisfactionRate)esr;
            Assert.AreEqual (expected_root_esr, esrd.SatisfactionRate);
        }
        
        [TestCase (0, 0, 1)]
        [TestCase (0.5, 0.5, 0.5 * 0.5)]
        [TestCase (1, 0, 0)]
        [TestCase (0, 1, 0)]
        [TestCase (.99, .99, (1 - .99) * (1 - .99))]
        [TestCase (.01, .01, (1 - .01) * (1 - .01))]
        public void TestDivideAndConquerGoalRefinement (double o1_esr, double o2_esr, double expected_root_esr)
        {
            Directory.SetCurrentDirectory (TestContext.CurrentContext.TestDirectory);
            const string filename = "./Examples/DivideAndConquerModel.kaos";

            ModelBuilder parser = new ModelBuilder ();
            string input = File.ReadAllText (filename);
            var model = parser.Parse (input, filename);

            var root = model.Goal ("root");

            model.satisfactionRateRepository.AddObstacleSatisfactionRate ("o1", new DoubleSatisfactionRate (o1_esr));
            model.satisfactionRateRepository.AddObstacleSatisfactionRate ("o2", new DoubleSatisfactionRate (o2_esr));

            var propagator = new PatternBasedPropagator (model);
            ISatisfactionRate esr = propagator.GetESR (root);
            Assert.IsInstanceOf (typeof(DoubleSatisfactionRate), esr);
            DoubleSatisfactionRate esrd = (DoubleSatisfactionRate)esr;
            Assert.AreEqual (expected_root_esr, esrd.SatisfactionRate);
        }
        
        [TestCase (0, 0, 1)]
        [TestCase (0.5, 0.5, 0.5 * 0.5)]
        [TestCase (1, 0, 0)]
        [TestCase (0, 1, 0)]
        [TestCase (.99, .99, (1 - .99) * (1 - .99))]
        [TestCase (.01, .01, (1 - .01) * (1 - .01))]
        public void TestUnmonitorabilityGoalRefinement (double o1_esr, double o2_esr, double expected_root_esr)
        {
            Directory.SetCurrentDirectory (TestContext.CurrentContext.TestDirectory);
            const string filename = "./Examples/UnmonitorabilityModel.kaos";

            ModelBuilder parser = new ModelBuilder ();
            string input = File.ReadAllText (filename);
            var model = parser.Parse (input, filename);

            var root = model.Goal ("root");

            model.satisfactionRateRepository.AddObstacleSatisfactionRate ("o1", new DoubleSatisfactionRate (o1_esr));
            model.satisfactionRateRepository.AddObstacleSatisfactionRate ("o2", new DoubleSatisfactionRate (o2_esr));

            var propagator = new PatternBasedPropagator (model);
            ISatisfactionRate esr = propagator.GetESR (root);
            Assert.IsInstanceOf (typeof(DoubleSatisfactionRate), esr);
            DoubleSatisfactionRate esrd = (DoubleSatisfactionRate)esr;
            Assert.AreEqual (expected_root_esr, esrd.SatisfactionRate);
        }
        
        [TestCase (0, 0, 1)]
        [TestCase (0.5, 0.5, 0.5 * 0.5)]
        [TestCase (1, 0, 0)]
        [TestCase (0, 1, 0)]
        [TestCase (.99, .99, (1 - .99) * (1 - .99))]
        [TestCase (.01, .01, (1 - .01) * (1 - .01))]
        public void TestUncontrollabilityGoalRefinement (double o1_esr, double o2_esr, double expected_root_esr)
        {
            Directory.SetCurrentDirectory (TestContext.CurrentContext.TestDirectory);
            const string filename = "./Examples/UncontrollabilityModel.kaos";

            ModelBuilder parser = new ModelBuilder ();
            string input = File.ReadAllText (filename);
            var model = parser.Parse (input, filename);

            var root = model.Goal ("root");

            model.satisfactionRateRepository.AddObstacleSatisfactionRate ("o1", new DoubleSatisfactionRate (o1_esr));
            model.satisfactionRateRepository.AddObstacleSatisfactionRate ("o2", new DoubleSatisfactionRate (o2_esr));

            var propagator = new PatternBasedPropagator (model);
            ISatisfactionRate esr = propagator.GetESR (root);
            Assert.IsInstanceOf (typeof(DoubleSatisfactionRate), esr);
            DoubleSatisfactionRate esrd = (DoubleSatisfactionRate)esr;
            Assert.AreEqual (expected_root_esr, esrd.SatisfactionRate);
        }
        
    }
}
