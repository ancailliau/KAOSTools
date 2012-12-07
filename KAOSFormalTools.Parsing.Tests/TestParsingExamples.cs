using System;
using System.Linq;
using System.Text;
using NUnit.Framework;
using KAOSFormalTools.Parsing;
using LtlSharp;
using System.Net;
using System.IO;

namespace KAOSFormalTools.Parsing.Tests
{
    [TestFixture()]
    public class TestParsingExamples
    {
        private static Parser parser = new Parser ();
        
        [Test()]
        public void TestLAS ()
        {
            string input = File.ReadAllText ("../../Examples/las.kaos");
            var model = parser.Parse (input, "../../Examples/las.kaos");

            Assert.IsNotNull (model);
        }

        [Test()]
        public void TestInclude ()
        {
            string input = File.ReadAllText ("../../Examples/include.kaos");
            var model = parser.Parse (input, "../../Examples/include.kaos");
            
            Assert.IsNotNull (model);
            Assert.IsNotEmpty (model.Goals.First ().Name);
            Assert.IsNotEmpty (model.Goals.First ().Definition);
        }

        [Test()]
        public void TestIssue7 ()
        {
            string input = File.ReadAllText ("../../Examples/issue7.kaos");
            var model = parser.Parse (input);
            
            Assert.IsNotNull (model);

            Assert.AreEqual (2, model.Goals.Count);
            Assert.AreEqual (1, model.DomainProperties.Count);

            Assert.AreEqual (2, model.RootGoals.First ().Refinements.Count);
            
            var domain_refinement = model.RootGoals.First ().Refinements[0];
            Assert.AreEqual ("driver_now_route", domain_refinement.DomainProperties.First ().Identifier);

            var goal_refinement = model.RootGoals.First ().Refinements[1];
            Assert.AreEqual ("achieve_destination_reached_if_gps_support", goal_refinement.Children.First ().Identifier);
        }

        /*
        [Test()]
        public void TestCarPooling ()
        {
            string input = File.ReadAllText ("../../Examples/carpooling.kaos");
            var model = parser.Parse (input);

            Assert.IsNotNull (model);
        }
        */
    }

}

