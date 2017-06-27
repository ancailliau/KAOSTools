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
    public class TestKeepObstructedGoalIntegration
    {
        [Test ()]
        public void TestIntegrateObstaclePrevention ()
        {
            var input = @"declare goal [ anchor ] 
                            formalspec when Current() then sooner-or-later Target()
                            refinedby child1, child2 
                          end
                          declare goal [ child1 ] 
                            formalspec when Current() then sooner-or-later Milestone()
                            obstructedby o
                          end
                          declare goal [ child2 ]
                            formalspec when Milestone() then sooner-or-later Target()
                          end
                          declare obstacle [ o ] 
                            formalspec sooner-or-later (Current() and always not Milestone())
                            resolvedby [prevention:anchor] cm
                          end
                          declare goal [ cm ] end";
                          
            ModelBuilder parser = new ModelBuilder ();
            var model = parser.Parse (input);

            var integrator = new ResolutionIntegrator (model);

            var r = model.Resolutions ().Single ();
            integrator.Integrate (r);

            var e = new KAOSFileExporter (model);
            Console.WriteLine (e.Export ());

        }
        
        [Test ()]
        public void TestIntegrateObstaclePreventionProbabilityComputation ()
        {
            var input = @"declare goal [ root ] 
                            refinedby anchor
                          end
                          declare goal [ anchor ] 
                            formalspec when Current() then sooner-or-later Target()
                            refinedby child1, child2 
                          end
                          declare goal [ child1 ] 
                            formalspec when Current() then sooner-or-later Milestone()
                            obstructedby o
                          end
                          declare goal [ child2 ]
                            formalspec when Milestone() then sooner-or-later Target()
                          end
                          declare obstacle [ o ] 
                            probability .4
                            formalspec sooner-or-later (Current() and always not Milestone())
                            resolvedby [prevention:anchor] cm
                          end
                          declare goal [ cm ] obstructedby o2 end
                          declare obstacle [ o2 ] probability .1 end";
                          
            ModelBuilder parser = new ModelBuilder ();
            var model = parser.Parse (input);

            var propagator = new BDDBasedResolutionPropagator (model);
            var computed = ((DoubleSatisfactionRate) propagator.GetESR (model.Goal ("root"), model.Resolutions ())).SatisfactionRate;
            
            var integrator = new ResolutionIntegrator (model);
            var r = model.Resolutions ().Single ();
            integrator.Integrate (r);

            var obstruction = model.Obstructions (x => x.ObstacleIdentifier == "o").Single ();
            model.obstacleRepository.Remove (obstruction);

            propagator = new BDDBasedResolutionPropagator (model);
            var expected = ((DoubleSatisfactionRate)propagator.GetESR (model.Goal ("root"))).SatisfactionRate;
            Assert.AreEqual (expected, computed);

        }
    }
}
