using System;
using System.Linq;
using NUnit.Framework;
using UCLouvain.KAOSTools.Parsing;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Parsing.Parsers;

namespace UCLouvain.KAOSTools.Parsing.Tests
{
    [TestFixture()]
    public class TestParsingSoftGoal
    {
        private static ModelBuilder parser = new ModelBuilder ();
       
        [TestCase(@"declare softgoal [ test ]
                    end", "test")]
        [TestCase(@"declare softgoal [ test_long_identifier ]
                    end", "test_long_identifier")]
        [TestCase(@"declare softgoal [ test-long-identifier ]
                    end", "test-long-identifier")]
        [TestCase(@"declare softgoal [ test12 ]
                    end", "test12")]
        public void TestIdentifier (string input, string expectedIdentifier)
        {
            var model = parser.Parse (input);
            model.SoftGoals().Where (x => x.Identifier == expectedIdentifier).ShallBeSingle ();
        }

        [TestCase(@"declare softgoal [] end")]
        [TestCase(@"declare softgoal [-] end")]
        [TestCase(@"declare softgoal [_] end")]
        [TestCase(@"declare softgoal [$] end")]
        public void TestInvalidIdentifier (string input)
        {
            Assert.Throws<ParserException> (() => {
                parser.Parse (input);
            });
        }

        [TestCase(@"declare softgoal [ test ]
                        name ""test""
                    end", "test")]
		[TestCase(@"declare softgoal [ test ]
                        name ""Long name with spaces and numbers 123""
                    end", "Long name with spaces and numbers 123")]
		[TestCase(@"declare softgoal [ test ]
                        name ""[-_-]""
                    end", "[-_-]")]
		[TestCase(@"declare softgoal [ test ]
                        name ""multi
                               line""
                    end", "multi line")]
		[TestCase("declare softgoal [ test ] name \"quoted \"\"name\"\"\" end", @"quoted ""name""")]
        public void TestName (string input, string expectedName)
        {
            var model = parser.Parse (input);
            model.SoftGoals().Where (x => x.Name == expectedName).ShallBeSingle ();
        }

        [TestCase(@"declare softgoal [ test ]
                        name """"""
                    end")]
        public void TestInvalidName (string input)
        {
            Assert.Throws<ParserException> (() => {
                parser.Parse (input);
            });
        }

		[TestCase(@"declare softgoal [ test ]
                        definition ""test""
                    end", "test")]
		[TestCase(@"declare softgoal [ test ]
                        definition """"
                    end", "")]
		[TestCase(@"declare softgoal [ test ]
                        definition ""on multiple
                                     lines.""
                    end", "on multiple lines.")]
        [TestCase("declare softgoal [ test ] definition \"with a \"\"quote\"\" !\" end", "with a \"quote\" !")]
        public void TestDefinition (string input, string expectedDefinition)
        {
            var model = parser.Parse (input);
            var g = model.SoftGoals().Single (x => x.Identifier == "test");
            g.Definition.ShallEqual (expectedDefinition);
        }

        [Test()]
        public void TestMultipleGoals ()
        {
            var input = @"declare softgoal [ test ] end
                          declare softgoal [ test2 ] end";

            var model = parser.Parse (input);

            model.SoftGoals().Count().ShallEqual (2);
            model.SoftGoals().ShallContain (x => x.Identifier == "test");
            model.SoftGoals().ShallContain (x => x.Identifier == "test2");
        }

        [TestCase(@"declare softgoal [ test ]
                        $custom ""My string""
                    end", "test")]
        public void TestCustomAttribute (string input, string expectedIdentifier)
        {
            var model = parser.Parse (input);
            var g = model.SoftGoals(x => x.Identifier == expectedIdentifier).ShallBeSingle ();
            g.CustomData.Count.ShallEqual (1);
            g.CustomData["custom"].ShallEqual("My string");
        }
    }

}

