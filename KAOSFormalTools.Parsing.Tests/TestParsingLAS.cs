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
    public class TestParsingLAS
    {
        private static Parser parser = new Parser ();
        
        [Test()]
        public void TestFromFile ()
        {
            string input = File.ReadAllText ("../../las.kaos");
            var model = parser.Parse (input);

            Assert.IsNotNull (model);
        }
    }

}

