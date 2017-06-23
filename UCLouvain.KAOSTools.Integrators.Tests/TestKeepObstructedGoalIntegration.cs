using NUnit.Framework;
using System;
using KAOSTools.Parsing;
using KAOSTools.Core;
using System.Linq;
using UCLouvain.KAOSTools.Utils.FileExporter;

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
    }
}
