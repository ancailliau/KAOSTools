using System.Linq;
using KAOSTools.Core;
using NUnit.Framework;
using UCLouvain.KAOSTools.Core.Agents;
using KAOSTools.Parsing;
using KAOSTools.Parsing.Parsers;

namespace UCLouvain.KAOSTools.Parsing.Tests
{
    [TestFixture()]
    public class TestParsingAgent
    {
        private static ModelBuilder parser = new ModelBuilder ();

		[TestCase(@"declare agent [ test ]
                        type software
                    end", AgentType.Software)]
        [TestCase(@"declare agent [ test ]
                        type environment
                    end", AgentType.Environment)]
        public void TestTypeOfAgent (string input, AgentType type)
        {
            var model = parser.Parse (input);
            model.Agents()
                .Where (x => x.Type == type)
                .ShallBeSingle ();
        }

        [TestCase(@"declare agent [ test ]
                    end", "test")]
		[TestCase(@"declare agent [ test_long_identifier ]
                    end", "test_long_identifier")]
		[TestCase(@"declare agent [ test-long-identifier ]
                    end", "test-long-identifier")]
		[TestCase(@"declare agent [ test12 ]
                    end", "test12")]
        public void TestIdentifier (string input, string identifier)
        {
            var model = parser.Parse (input);
            model.Agents()
                .Where (x => x.Identifier == identifier)
                .ShallBeSingle ();
        }
        
        [TestCase(@"declare agent [] end")]
		[TestCase(@"declare agent [-] end")]
		[TestCase(@"declare agent [_] end")]
		[TestCase(@"declare agent [end] end")]
        public void TestInvalidIdentifier (string input)
        {
			Assert.Throws<ParserException> (() => {
                parser.Parse (input);
            });
        }

        [TestCase(@"declare agent [ test ]
                        name ""test""
                    end", "test")]
		[TestCase(@"declare agent [ test ]
                        name ""Long name with spaces and numbers 123""
                    end", "Long name with spaces and numbers 123")]
		[TestCase(@"declare agent [ test ]
                        name ""[-_-]""
                    end", "[-_-]")]
        public void TestName (string input, string expectedName)
        {
            var model = parser.Parse (input);
            model.Agents()
                .Where (x => x.Name == expectedName)
                .ShallBeSingle ();
        }

		[TestCase(@"declare agent [ test ]
                        name """"""
                    end")]
        public void TestInvalidName (string input)
        {
            Assert.Throws<ParserException> (() => {
                parser.Parse (input);
            });
        }

        [TestCase(@"declare agent [ test ]
                        definition ""My description""
                    end", "My description")]
        [TestCase(@"declare agent [ test ]
                        definition """"
                    end", "")]
        [TestCase(@"declare agent [ test ]
                        definition ""multi
                                     line""
                    end", "multi line")]
        public void TestDefinition (string input, string expectedDescription)
        {
            var model = parser.Parse (input);
            model.Agents()
                .Where (x => x.Identifier == "test")
                .ShallBeSuchThat (x => x.Definition == expectedDescription);
        }

		[TestCase(@"declare agent [ test ]
                        definition "" "" "" 
                    end")]
        public void TestInvalidDescription (string input)
        {
            Assert.Throws<ParserException> (() => {
                parser.Parse (input);
            });
        }

		[TestCase(@"declare goal [ goal ]
                        assignedto agent
                    end
                    declare agent [ agent ] end")]
        public void TestAssignedTo (string input)
        {
            var model = parser.Parse (input);
            model.Goals()
                .Where (x => x.Identifier == "goal" & x.AgentAssignments().Count() == 1)
                .SelectMany (x => x.AgentAssignments())
                .SelectMany (x => x.Agents())
                .ShallBeSingle ()
                .ShallBeSuchThat (x => x.Identifier == "agent");
        }

        [TestCase(@"declare goal [ goal ]
                        assignedto agent1, agent2
                    end
                    declare agent [ agent1 ] end
                    declare agent [ agent2 ] end")]
        public void TestAssignedToMultipleAgents (string input)
        {
            var model = parser.Parse (input);

            model.Goals()
                .Where (x => x.Identifier == "goal")
                .SelectMany (x => x.AgentAssignments())
                .SelectMany (x => x.Agents())
                .Select (x => x.Identifier)
                .ShallOnlyContain (new string[] { "agent1", "agent2" });
        }
    }
}

