using System;
using NUnit.Framework;
using KAOSTools.Core;
using KAOSTools.Parsing;
using KAOSTools.Parsing.Parsers;
using KAOSTools.Parsing.Parsers.Declarations;

namespace UCLouvain.KAOSTools.Parsing.Tests.Unit
{
    [TestFixture()]
    public class TestDeclareParser
    {
        [Test()]
        public void TestFailWhenUnknownAttribute() 
        {
            var dg = new GoalDeclareParser();
            var e = Assert.Catch(() => {
                dg.ParsedAttribute("my_attribute", null, null);
            });
            Assert.IsInstanceOf(typeof(NotImplementedException), e);
            Assert.That(e.Message.Contains("my_attribute"));
            Assert.That(e.Message.Contains(dg.GetIdentifier()));
        }
    }
}
