using System;
using System.Linq;
using NUnit.Framework;
using KAOSFormalTools.Parsing;
using LtlSharp;

namespace KAOSFormalTools.Parsing.Tests
{
    [TestFixture()]
    public class TestParsingDomainProperty
    {
        private static Parser parser = new Parser ();
        
        [Test()]
        public void TestMissingIdentifier ()
        {
            var input = @"begin domainproperty end";

            Assert.Throws (typeof(KAOSFormalTools.Parsing.ParsingException), () => {
                parser.Parse (input);
            });
        }

        [Test()]
        public void TestIdentifier ()
        {
            var input = @"
begin domainproperty
    id test
end
";
            var gm = parser.Parse (input);
            Assert.AreEqual ("test", gm.DomainProperties.First().Identifier);
        }

        [Test()]
        public void TestName ()
        {
            var input = @"
begin domainproperty
    id test
    name ""My domain property name""
end
";
            
            var gm = parser.Parse (input);
            var root = gm.DomainProperties.First ();
            Assert.AreEqual ("My domain property name", root.Name);
        }

        [Test()]
        public void TestFormalSpec ()
        {
            var input = @"
begin domainproperty
    id          test
    name        ""My goal name""
    formalspec  ""G (incidentReported -> F ambulanceOnScene)""
end
";
            
            var gm = parser.Parse (input);
            var root = gm.DomainProperties.First ();
            Assert.IsInstanceOf (typeof(Globally), root.FormalSpec);
        }
    }

}

