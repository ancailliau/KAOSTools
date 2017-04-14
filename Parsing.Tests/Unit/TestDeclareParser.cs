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
            var e = Assert.Catch(() => {
                var dg = new GoalDeclareParser();
                dg.ParsedAttribute("attribute", null, null);
            });
            Assert.IsInstanceOf(typeof(NotImplementedException), e);
        }
    }
}
