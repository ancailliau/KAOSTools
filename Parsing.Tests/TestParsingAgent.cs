using System;
using System.Linq;
using NUnit.Framework;
using ShallTests;

namespace KAOSTools.Parsing.Tests
{
    [TestFixture()]
    public class TestParsingAgent
    {
        private static ModelBuilder parser = new ModelBuilder ();

        [TestCase(@"declare agent
                        id test
                        type software
                    end", KAOSTools.MetaModel.AgentType.Software)]
        [TestCase(@"declare agent
                        id test
                        type environment
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
                        id test_long_identifier
                    end", "test_long_identifier")]
        [TestCase(@"declare agent
                        id test-long-identifier
                    end", "test-long-identifier")]
        [TestCase(@"declare agent
                        id test12
                    end", "test12")]
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
            Assert.Throws<ParserException> (() => {
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
                        name """"""
                    end")]
        public void TestInvalidName (string input)
        {
            Assert.Throws<ParserException> (() => {
                parser.Parse (input);
            });
        }

        [TestCase(@"declare agent
                        id test
                        definition ""My description""
                    end", "My description")]
        [TestCase(@"declare agent
                        id test
                        definition """"
                    end", "")]
        [TestCase(@"declare agent
                        id test
                        definition ""multi
                                     line""
                    end", "multi line")]
        public void TestDefinition (string input, string expectedDescription)
        {
            var model = parser.Parse (input);
            model.GoalModel.Agents
                .Where (x => x.Identifier == "test")
                .ShallBeSuchThat (x => x.Definition == expectedDescription);
        }

        [TestCase(@"declare agent
                        definition "" "" "" 
                    end")]
        public void TestInvalidDescription (string input)
        {
            Assert.Throws<ParserException> (() => {
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

        [TestCase(@"declare goal 
                        id test
                        assignedto agent1
                    end")]
        public void TestImplicit (string input)
        {
            var model = parser.Parse (input);
            
            var agent = model.GoalModel.Agents.Single (x => x.Identifier == "agent1");
            agent.Implicit.ShallBeTrue ();
        }
    }
}

