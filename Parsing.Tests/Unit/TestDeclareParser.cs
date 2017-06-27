using System;
using NUnit.Framework;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Parsing;
using UCLouvain.KAOSTools.Parsing.Parsers;
using UCLouvain.KAOSTools.Parsing.Parsers.Declarations;

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
