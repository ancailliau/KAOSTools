﻿using NUnit.Framework;
using System;
using KAOSTools.Parsing;
using System.IO;
using KAOSTools.Core;
using UCLouvain.KAOSTools.Propagators.BDD;
using UCLouvain.KAOSTools.Integrators;
using System.Linq;
using UCLouvain.KAOSTools.Utils.FileExporter;

namespace KAOSTools.UCLouvain.Thesis.Tests
{
    [TestFixture ()]
    public class Test
    {
        [Test ()]
        public void TestCase ()
        {
            var filename = "/Users/acailliau/Google Drive/PhD/Dissertation/running-example-sfp/model_bug.kaos";
            
            ModelBuilder parser = new ModelBuilder ();
            string input = File.ReadAllText (filename);
            var model = parser.Parse (input, filename);

            var root = model.Goal ("make_up_water_provided");

            var propagator = new BDDBasedPropagator (model);
            var propagator2 = new BDDBasedResolutionPropagator (model);
            propagator.PreBuildObstructionSet (root);
            propagator2.PreBuildObstructionSet (root);
            
            Console.WriteLine ("SatRate for " + root.Identifier + ": " + propagator.GetESR (root));
            Console.WriteLine ("SatRate for " + root.Identifier + ": " + propagator2.GetESR (root));

            var o1 = model.Obstacle ("alarm_not_raised");
            var r1 = model.Resolutions ().Where (x => x.ObstacleIdentifier == o1.Identifier).First ();
            
            var o2 = model.Obstacle ("no_response");
            var r2 = model.Resolutions ().Where (x => x.ObstacleIdentifier == o2.Identifier).First ();
            
            Console.WriteLine ("SatRate for " + root.Identifier + " (r1): " + propagator2.GetESR (root, new [] { r1 }));
            Console.WriteLine ("SatRate for " + root.Identifier + " (r1, r2): " + propagator2.GetESR (root, new [] { r1, r2 }));
            
            var integrator = new ResolutionIntegrator (model);
            
            integrator.Integrate (r1);
            
            propagator.PreBuildObstructionSet (root);
            Console.WriteLine ("SatRate for " + root.Identifier + " (r1): " + propagator.GetESR (root));
            

            var fileExporter = new KAOSFileExporter (model);
            //Console.WriteLine (fileExporter.Export ());
            
            integrator.Integrate (r2);
            
            propagator.PreBuildObstructionSet (root);
            Console.WriteLine ("SatRate for " + root.Identifier + " (r1, r2): " + propagator.GetESR (root));
            
            //Console.WriteLine (fileExporter.Export ());
            
        }
        
        [Test ()]
        public void TestCase2 ()
        {
            var filename = "/Users/acailliau/Google Drive/PhD/Dissertation/running-example-sfp/model.kaos";
            
            ModelBuilder parser = new ModelBuilder ();
            string input = File.ReadAllText (filename);
            var model = parser.Parse (input, filename);

            var root = model.Goal ("make_up_water_provided");

            var propagator = new BDDBasedPropagator (model);
            var propagator2 = new BDDBasedResolutionPropagator (model);
            propagator.PreBuildObstructionSet (root);
            propagator2.PreBuildObstructionSet (root);
            
            Console.WriteLine ("SatRate for " + root.Identifier + ": " + propagator.GetESR (root));
            Console.WriteLine ("SatRate for " + root.Identifier + ": " + propagator2.GetESR (root));

            var o1 = model.Obstacle ("alarm_not_raised");
            var r1 = model.Resolutions ().Where (x => x.ObstacleIdentifier == o1.Identifier).First ();
            
            var o2 = model.Obstacle ("no_response");
            var r2 = model.Resolutions ().Where (x => x.ObstacleIdentifier == o2.Identifier).First ();
            
            Console.WriteLine ("SatRate for " + root.Identifier + " (r1): " + propagator2.GetESR (root, new [] { r1 }));
            Console.WriteLine ("SatRate for " + root.Identifier + " (r1, r2): " + propagator2.GetESR (root, new [] { r1, r2 }));
            
            var integrator = new ResolutionIntegrator (model);
            
            integrator.Integrate (r1);
            
            propagator.PreBuildObstructionSet (root);
            Console.WriteLine ("SatRate for " + root.Identifier + " (r1): " + propagator.GetESR (root));
            

            var fileExporter = new KAOSFileExporter (model);
            //Console.WriteLine (fileExporter.Export ());
            
            integrator.Integrate (r2);
            
            propagator.PreBuildObstructionSet (root);
            Console.WriteLine ("SatRate for " + root.Identifier + " (r1, r2): " + propagator.GetESR (root));
            
            //Console.WriteLine (fileExporter.Export ());
            
            
            
            
        }
    }
}
