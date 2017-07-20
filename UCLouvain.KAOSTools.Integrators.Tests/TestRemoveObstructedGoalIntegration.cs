using NUnit.Framework;
using System;
using UCLouvain.KAOSTools.Parsing;
using UCLouvain.KAOSTools.Core;
using System.Linq;
using UCLouvain.KAOSTools.Utils.FileExporter;
using UCLouvain.KAOSTools.Propagators.BDD;
using UCLouvain.KAOSTools.Core.SatisfactionRates;

namespace UCLouvain.KAOSTools.Integrators.Tests
{
    [TestFixture ()]
    public class TestRemoveObstructedGoalIntegration
    {
        [Test ()]
        public void TestIntegrateSubstitution ()
        {
            var input = @"declare goal [ anchor ] refinedby child1, child2 end
                          declare goal [ child1 ] obstructedby o end
                          declare goal [ child2 ] end
                          declare obstacle [ o ] resolvedby [substitution:anchor] cm end
                          declare goal [ cm ] end";
                          
            ModelBuilder parser = new ModelBuilder ();
            var model = parser.Parse (input);

            var integrator = new ResolutionIntegrator (model);
            var r = model.Resolutions ().Single ();
            integrator.Integrate (r);

            var refinement = model.GoalRefinements ().Single ();
            Assert.That (refinement.SubGoalIdentifiers.Any (x => x.Identifier == "child2"));
            Assert.That (refinement.SubGoalIdentifiers.Any (x => x.Identifier == "cm"));
            Assert.AreEqual (2, refinement.SubGoalIdentifiers.Count ());
        }
        
        [Test ()]
        public void TestIntegrateSubstitutionProbabilityComputation ()
        {
            var input = @"declare goal [ anchor ] refinedby child1, child2 end
                          declare goal [ child1 ] obstructedby o end
                          declare goal [ child2 ] end
                          declare obstacle [ o ] probability .4 resolvedby [substitution:anchor] cm end
                          declare goal [ cm ] obstructedby o2 end
                          declare obstacle [ o2 ] probability .1 end";
                          
            ModelBuilder parser = new ModelBuilder ();
            var model = parser.Parse (input);
            
            
            var integrator = new ResolutionIntegrator (model);
            var r = model.Resolutions ().Single ();
            integrator.Integrate (r);

            var propagator = new BDDBasedPropagator (model);
            var expected = ((DoubleSatisfactionRate)propagator.GetESR (model.Goal ("anchor"))).SatisfactionRate;
        }
        
        [Test ()]
        public void TestIntegrateSubstitutionHighLevelAnchor ()
        {
            var input = @"declare goal [ anchor ] refinedby child1, child2 end
                          declare goal [ child1 ] refinedby child3, child4 end
                          declare goal [ child3 ] obstructedby o end
                          declare obstacle [ o ] resolvedby [substitution:anchor] cm end
                          declare goal [ cm ] end";
                          
            ModelBuilder parser = new ModelBuilder ();
            var model = parser.Parse (input);

            var integrator = new ResolutionIntegrator (model);

            var r = model.Resolutions ().Single ();
            integrator.Integrate (r);

            var e = new KAOSFileExporter (model);
            Console.WriteLine (e.Export ());

            var refinement = model.GoalRefinements (x => x.ParentGoalIdentifier == "anchor").Single ();
            Assert.That (refinement.SubGoalIdentifiers.Any (x => x.Identifier == "child2"));
            Assert.That (refinement.SubGoalIdentifiers.Any (x => x.Identifier == "cm"));
            Assert.AreEqual (2, refinement.SubGoalIdentifiers.Count ());
        }
        
        [Test ()]
        public void TestIntegrateSubstitutionHighLevelAnchorProbabilityComputation ()
        {
            var input = @"declare goal [ anchor ] refinedby child1, child2 end
                          declare goal [ child1 ] refinedby child3, child4 end
                          declare goal [ child3 ] obstructedby o end
                          declare obstacle [ o ] probability .4 resolvedby [substitution:anchor] cm end
                          declare goal [ cm ] obstructedby o2 end
                          declare obstacle [ o2 ] probability .1 end";
                          
            ModelBuilder parser = new ModelBuilder ();
            var model = parser.Parse (input);
            
        }
    }
}
