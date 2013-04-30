using System;
using System.Linq;
using NUnit.Framework;
using ShallTests;

namespace KAOSTools.Parsing.Tests
{
    [TestFixture()]
    public class TestParsingAgent
    {
        private static Parser parser = new Parser ();

        [TestCase(@"declare software agent
                        id test
                    end", KAOSTools.MetaModel.AgentType.Software)]
        [TestCase(@"declare environment agent
                        id test
                    end", KAOSTools.MetaModel.AgentType.Environment)]
        public void TestTypeOfAgent (string input, KAOSTools.MetaModel.AgentType type)
        {
            var model = parser.Parse (input);
            model.GoalModel.Agents
                .Where (x => x.Type == type)
                .ShallBeSingle ();
        }

        [TestCase(@"declare agent
                        id test
                    end", "test")]
        [TestCase(@"declare agent
                        id _test
                    end", "_test")]
        [TestCase(@"declare agent
                        id -test
                    end", "-test")]
        [TestCase(@"declare agent
                        id $test
                    end", "$test")]
        [TestCase(@"declare agent
                        id test_long_identifier
                    end", "test_long_identifier")]
        [TestCase(@"declare agent
                        id test-long-identifier
                    end", "test-long-identifier")]
        [TestCase(@"declare agent
                        id test12
                    end", "test12")]
        [TestCase(@"declare agent
                        id 0
                    end", "0")]
        public void TestIdentifier (string input, string identifier)
        {
            var model = parser.Parse (input);
            model.GoalModel.Agents
                .Where (x => x.Identifier == identifier)
                .ShallBeSingle ();
        }
        
        [TestCase(@"declare agent
                        id 
                    end")]
        [TestCase(@"declare agent
                        id -
                    end")]
        [TestCase(@"declare agent
                        id _
                    end")]
        [TestCase(@"declare agent
                        id $
                    end")]
        public void TestInvalidIdentifier (string input)
        {
            Assert.Throws<ParsingException> (() => {
                parser.Parse (input);
            });
        }

        [TestCase(@"declare agent
                        name ""test""
                    end", "test")]
        [TestCase(@"declare agent
                        name ""Long name with spaces and numbers 123""
                    end", "Long name with spaces and numbers 123")]
        [TestCase(@"declare agent
                        name ""[-_-]""
                    end", "[-_-]")]
        public void TestName (string input, string expectedName)
        {
            var model = parser.Parse (input);
            model.GoalModel.Agents
                .Where (x => x.Name == expectedName)
                .ShallBeSingle ();
        }
        
        [TestCase(@"declare agent
                        name """"
                    end")]
        [TestCase(@"declare agent
                        name """"""
                    end")]
        public void TestInvalidName (string input)
        {
            Assert.Throws<ParsingException> (() => {
                parser.Parse (input);
            });
        }

        [TestCase(@"declare agent
                        id test
                        description ""My description""
                    end", "My description")]
        [TestCase(@"declare agent
                        id test
                        description """"
                    end", "")]
        [TestCase(@"declare agent
                        id test
                        definition ""My description""
                    end", "My description")]
        [TestCase(@"declare agent
                        id test
                        definition """"
                    end", "")]
        public void TestDescription (string input, string expectedDescription)
        {
            var model = parser.Parse (input);
            model.GoalModel.Agents
                .Where (x => x.Identifier == "test")
                .ShallBeSuchThat (x => x.Description == expectedDescription);
        }

        [TestCase(@"declare agent
                        description 
                    end")]
        public void TestInvalidDescription (string input)
        {
            Assert.Throws<ParsingException> (() => {
                parser.Parse (input);
            });
        }

        [TestCase(@"declare goal
                        id goal
                        assignedto agent
                    end")]
        [TestCase(@"declare goal
                        id goal
                        assignedto declare agent
                                     id agent
                                   end
                    end")]
        public void TestAssignedTo (string input)
        {
            var model = parser.Parse (input);
            model.GoalModel.Goals
                .Where (x => x.Identifier == "goal" & x.AgentAssignments.Count() == 1)
                .SelectMany (x => x.AgentAssignments)
                .SelectMany (x => x.Agents)
                .ShallBeSingle ()
                .ShallBeSuchThat (x => x.Identifier == "agent");
        }

        [TestCase(@"declare goal
                        id goal
                        assignedto agent1, agent2
                    end")]
        [TestCase(@"declare goal
                        id goal
                        assignedto declare agent
                                     id agent1
                                   end, agent2
                    end")]
        [TestCase(@"declare goal
                        id goal
                        assignedto declare agent
                                     id agent1
                                   end, declare agent
                                     id agent2
                                   end
                    end")]
        public void TestAssignedToMultipleAgents (string input)
        {
            var model = parser.Parse (input);

            model.GoalModel.Goals
                .Where (x => x.Identifier == "goal")
                .SelectMany (x => x.AgentAssignments)
                .SelectMany (x => x.Agents)
                .Select (x => x.Identifier)
                .ShallOnlyContain (new string[] { "agent1", "agent2" });
        }
    }
}

