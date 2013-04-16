using System;
using System.Linq;
using NUnit.Framework;
using ShallTests;

namespace KAOSFormalTools.Parsing.Tests
{
    [TestFixture()]
    public class TestParsingPredicate
    {
        private static Parser parser = new Parser ();

        [TestCase(@"declare predicate
                        name ""Test""
                        definition ""My definition""
                        signature ""Test(c:Test)""
                    end")]
        public void TestSimplePredicate (string input)
        {
            var model = parser.Parse (input);
            model.Predicates.Keys.ShallOnlyContain (new string[] { "Test" });
            model.Predicates["Test"].ShallBeSuchThat (x => x.Definition == "My definition" & x.Signature == "Test(c:Test)");
        }
    }
}

