﻿using System;
using System.Linq;
using System.Text;
using NUnit.Framework;
using UCLouvain.KAOSTools.Parsing;
using System.Net;
using System.IO;
using UCLouvain.KAOSTools.Core;

namespace UCLouvain.KAOSTools.Parsing.Tests
{
    [TestFixture()]
    public class TestParsingExamples
    {
        private static ModelBuilder parser = new ModelBuilder ();

        //[Test()]
        //public void TestBCMS ()
        //{
        //    string input = File.ReadAllText ("/Users/acailliau/Dropbox/bCMS/model/Goal Model/Z-whole-model.kaos");
        //    var model = parser.Parse (input, "/Users/acailliau/Dropbox/bCMS/model/Goal Model/Z-whole-model.kaos");

        //    Assert.IsNotNull (model);
        //}


        [Test()]
        public void TestLAS ()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
            string input = File.ReadAllText ("./Examples/las.kaos");
            var model = parser.Parse (input, "./Examples/las.kaos");

            Assert.IsNotNull (model);
        }

        [Test()]
        public void TestInclude ()
        {
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
            string input = File.ReadAllText (@"./Examples/include.kaos");
			var model = parser.Parse(input, @"./Examples/include.kaos");
            
            Assert.IsNotNull (model);
            Assert.IsNotEmpty (model.Goals().First ().Name);
            Assert.IsNotEmpty (model.Goals().First ().Definition);
        }

        [Test()]
        public void TestIssue7 ()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
            string input = File.ReadAllText ("./Examples/issue7.kaos");
            var model = parser.Parse (input, "./Examples/issue7.kaos");
            
            Assert.IsNotNull (model);

            Assert.AreEqual (2, model.Goals().Count());
            Assert.AreEqual (1, model.DomainProperties().Count());

            Assert.AreEqual (2, model.RootGoals().First ().Refinements().Count());
            
            model.RootGoals()
                .SelectMany (x => x.Refinements())
                .SelectMany (r => r.DomainProperties())
                .Select (d => d.Identifier)
                .ShallContain ("driver_now_route");

            model.RootGoals()
                .SelectMany (x => x.Refinements())
                .SelectMany (r => r.SubGoals())
                .Select (d => d.Identifier)
                .ShallContain ("achieve_destination_reached_if_gps_support");
        }
    }

}

