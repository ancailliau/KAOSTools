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
    public abstract class TestPropagator
    {
        protected abstract IPropagator GetPropagator (KAOSModel model);
    
        protected void TestMilestone (double o1_esr, double o2_esr, double expected_root_esr)
        {
            Directory.SetCurrentDirectory (TestContext.CurrentContext.TestDirectory);
            const string filename = "./Examples/MilestoneModel.kaos";

            ModelBuilder parser = new ModelBuilder ();
            string input = File.ReadAllText (filename);
            var model = parser.Parse (input, filename);

            var root = model.Goal ("root");

            model.satisfactionRateRepository.AddObstacleSatisfactionRate ("o1", new DoubleSatisfactionRate (o1_esr));
            model.satisfactionRateRepository.AddObstacleSatisfactionRate ("o2", new DoubleSatisfactionRate (o2_esr));

            var _propagator = GetPropagator (model);
            ISatisfactionRate esr = _propagator.GetESR (root);
            Assert.IsInstanceOf (typeof(DoubleSatisfactionRate), esr);
            DoubleSatisfactionRate esrd = (DoubleSatisfactionRate)esr;
            Assert.AreEqual (expected_root_esr, esrd.SatisfactionRate, 0.0001);
        }

        protected void TestCase (double o1_esr, double o2_esr, double c1, double c2, double expected_root_esr)
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

            var _propagator = GetPropagator (model);
            ISatisfactionRate esr = _propagator.GetESR (root);
            Assert.IsInstanceOf (typeof(DoubleSatisfactionRate), esr);
            DoubleSatisfactionRate esrd = (DoubleSatisfactionRate)esr;
            Assert.AreEqual (expected_root_esr, esrd.SatisfactionRate, 0.0001);
        }
        
        protected void TestIntroduceGuard (double o1_esr, double o2_esr, double o3_esr, double expected_root_esr)
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

            var _propagator = GetPropagator (model);
            ISatisfactionRate esr = _propagator.GetESR (root);
            Assert.IsInstanceOf (typeof(DoubleSatisfactionRate), esr);
            DoubleSatisfactionRate esrd = (DoubleSatisfactionRate)esr;
            Assert.AreEqual (expected_root_esr, esrd.SatisfactionRate, 0.0001);
        }
        
        protected void TestDivideAndConquer (double o1_esr, double o2_esr, double expected_root_esr)
        {
            Directory.SetCurrentDirectory (TestContext.CurrentContext.TestDirectory);
            const string filename = "./Examples/DivideAndConquerModel.kaos";

            ModelBuilder parser = new ModelBuilder ();
            string input = File.ReadAllText (filename);
            var model = parser.Parse (input, filename);

            var root = model.Goal ("root");

            model.satisfactionRateRepository.AddObstacleSatisfactionRate ("o1", new DoubleSatisfactionRate (o1_esr));
            model.satisfactionRateRepository.AddObstacleSatisfactionRate ("o2", new DoubleSatisfactionRate (o2_esr));

            var _propagator = GetPropagator (model);
            ISatisfactionRate esr = _propagator.GetESR (root);
            Assert.IsInstanceOf (typeof(DoubleSatisfactionRate), esr);
            DoubleSatisfactionRate esrd = (DoubleSatisfactionRate)esr;
            Assert.AreEqual (expected_root_esr, esrd.SatisfactionRate, 0.0001);
        }
        
        protected void TestUnmonitorability (double o1_esr, double o2_esr, double expected_root_esr)
        {
            Directory.SetCurrentDirectory (TestContext.CurrentContext.TestDirectory);
            const string filename = "./Examples/UnmonitorabilityModel.kaos";

            ModelBuilder parser = new ModelBuilder ();
            string input = File.ReadAllText (filename);
            var model = parser.Parse (input, filename);

            var root = model.Goal ("root");

            model.satisfactionRateRepository.AddObstacleSatisfactionRate ("o1", new DoubleSatisfactionRate (o1_esr));
            model.satisfactionRateRepository.AddObstacleSatisfactionRate ("o2", new DoubleSatisfactionRate (o2_esr));

            var _propagator = GetPropagator (model);
            ISatisfactionRate esr = _propagator.GetESR (root);
            Assert.IsInstanceOf (typeof(DoubleSatisfactionRate), esr);
            DoubleSatisfactionRate esrd = (DoubleSatisfactionRate)esr;
            Assert.AreEqual (expected_root_esr, esrd.SatisfactionRate, 0.0001);
        }
        
        protected void TestUncontrollability (double o1_esr, double o2_esr, double expected_root_esr)
        {
            Directory.SetCurrentDirectory (TestContext.CurrentContext.TestDirectory);
            const string filename = "./Examples/UncontrollabilityModel.kaos";

            ModelBuilder parser = new ModelBuilder ();
            string input = File.ReadAllText (filename);
            var model = parser.Parse (input, filename);

            var root = model.Goal ("root");

            model.satisfactionRateRepository.AddObstacleSatisfactionRate ("o1", new DoubleSatisfactionRate (o1_esr));
            model.satisfactionRateRepository.AddObstacleSatisfactionRate ("o2", new DoubleSatisfactionRate (o2_esr));

            var _propagator = GetPropagator (model);
            ISatisfactionRate esr = _propagator.GetESR (root);
            Assert.IsInstanceOf (typeof(DoubleSatisfactionRate), esr);
            DoubleSatisfactionRate esrd = (DoubleSatisfactionRate)esr;
            Assert.AreEqual (expected_root_esr, esrd.SatisfactionRate, 0.0001);
        }
        
    }
}
