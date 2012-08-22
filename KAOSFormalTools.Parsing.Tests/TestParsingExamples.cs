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
            var model = parser.Parse (input);

            Assert.IsNotNull (model);
        }

        [Test()]
        public void TestCarPooling ()
        {
            string input = File.ReadAllText ("../../Examples/carpooling.kaos");
            var model = parser.Parse (input);

            Assert.IsNotNull (model);
        }
    }

}

