using System;
using System.Linq;
using System.Text;
using NUnit.Framework;
using KAOSTools.Parsing;
using LtlSharp;
using System.Net;
using System.IO;
using ShallTests;

namespace KAOSTools.Parsing.Tests
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
            Assert.IsNotEmpty (model.GoalModel.Goals.First ().Name);
            Assert.IsNotEmpty (model.GoalModel.Goals.First ().Definition);
        }

        [Test()]
        public void TestIssue7 ()
        {
            string input = File.ReadAllText ("../../Examples/issue7.kaos");
            var model = parser.Parse (input);
            
            Assert.IsNotNull (model);

            Assert.AreEqual (2, model.GoalModel.Goals.Count);
            Assert.AreEqual (1, model.GoalModel.DomainProperties.Count);

            Assert.AreEqual (2, model.GoalModel.RootGoals.First ().Refinements.Count);
            
            model.GoalModel.RootGoals
                .SelectMany (x => x.Refinements)
                .SelectMany (r => r.DomainProperties)
                .Select (d => d.Identifier)
                .ShallContain ("driver_now_route");

            model.GoalModel.RootGoals
                .SelectMany (x => x.Refinements)
                .SelectMany (r => r.Children)
                .Select (d => d.Identifier)
                .ShallContain ("achieve_destination_reached_if_gps_support");
        }
    }

}

