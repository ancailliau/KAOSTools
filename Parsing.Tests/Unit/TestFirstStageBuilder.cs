using System;
using NUnit.Framework;
using KAOSTools.Core;
using KAOSTools.Parsing;
using KAOSTools.Parsing.Parsers;

namespace UCLouvain.KAOSTools.Parsing.Tests.Unit
{
    [TestFixture()]
    public class TestFirstStageBuilder
    {
        [Test()]
        public void TestFailWhenParsedDeclareUnknow() 
        {
            var model = new KAOSModel();
            var path = new Uri("/tmp/fake");
            var builder = new FirstStageBuilder(model, path);

            Assert.Catch(() => {
                builder.BuildDeclare(new UnknownParsedDeclare("unknown"));
            });
        }

        [Test()]
        public void TestParsedDeclareGoal()
        {
            var model = new KAOSModel();
            var path = new Uri("/tmp/fake");
            var builder = new FirstStageBuilder(model, path);

            builder.BuildDeclare(new ParsedGoal("my-goal"));
            model.Goals(x => x.Identifier == "my-goal").ShallBeSingle();
        }

        class UnknownParsedDeclare : ParsedDeclare
        {
            public UnknownParsedDeclare(string identifier) : base(identifier)
            {
            }
        }
    }
}
