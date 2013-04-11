using System;
using System.Linq;
using NUnit.Framework;
using ShallTests;

namespace KAOSFormalTools.Parsing.Tests
{
    [TestFixture()]
    public class TestParsingAgent
    {
        private static Parser parser = new Parser ();

        [TestCase(@"begin software agent
                        id test
                    end", KAOSFormalTools.Domain.AgentType.Software)]
        [TestCase(@"begin environment agent
                        id test
                    end", KAOSFormalTools.Domain.AgentType.Environment)]
        public void TestTypeOfAgent (string input, KAOSFormalTools.Domain.AgentType type)
        {
            var model = parser.Parse (input);
            model.GoalModel.Agents
                .Where (x => x.Type == type)
                .ShallBeSingle ();
        }

        [TestCase(@"begin agent
                        id test
                    end", "test")]
        [TestCase(@"begin agent
                        id _test
                    end", "_test")]
        [TestCase(@"begin agent
                        id -test
                    end", "-test")]
        [TestCase(@"begin agent
                        id $test
                    end", "$test")]
        [TestCase(@"begin agent
                        id test_long_identifier
                    end", "test_long_identifier")]
        [TestCase(@"begin agent
                        id test-long-identifier
                    end", "test-long-identifier")]
        [TestCase(@"begin agent
                        id test12
                    end", "test12")]
        [TestCase(@"begin agent
                        id 0
                    end", "0")]
        public void TestIdentifier (string input, string identifier)
        {
            var model = parser.Parse (input);
            model.GoalModel.Agents
                .Where (x => x.Identifier == identifier)
                .ShallBeSingle ();
        }
        
        [TestCase(@"begin agent
                        id 
                    end")]
        [TestCase(@"begin agent
                        id -
                    end")]
        [TestCase(@"begin agent
                        id _
                    end")]
        [TestCase(@"begin agent
                        id $
                    end")]
        public void TestInvalidIdentifier (string input)
        {
            Assert.Throws<ParsingException> (() => {
                parser.Parse (input);
            });
        }

        [TestCase(@"begin agent
                        name ""test""
                    end", "test")]
        [TestCase(@"begin agent
                        name ""Long name with spaces and numbers 123""
                    end", "Long name with spaces and numbers 123")]
        [TestCase(@"begin agent
                        name ""[-_-]""
                    end", "[-_-]")]
        public void TestName (string input, string expectedName)
        {
            var model = parser.Parse (input);
            model.GoalModel.Agents
                .Where (x => x.Name == expectedName)
                .ShallBeSingle ();
        }
        
        [TestCase(@"begin agent
                        name """"
                    end")]
        [TestCase(@"begin agent
                        name """"""
                    end")]
        public void TestInvalidName (string input)
        {
            Assert.Throws<ParsingException> (() => {
                parser.Parse (input);
            });
        }

        [TestCase(@"begin agent
                        id test
                        description ""My description""
                    end", "My description")]
        [TestCase(@"begin agent
                        id test
                        description """"
                    end", "")]
        [TestCase(@"begin agent
                        id test
                        definition ""My description""
                    end", "My description")]
        [TestCase(@"begin agent
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

        [TestCase(@"begin agent
                        description 
                    end")]
        public void TestInvalidDescription (string input)
        {
            Assert.Throws<ParsingException> (() => {
                parser.Parse (input);
            });
        }

        [TestCase(@"begin goal
                        id goal
                        assignedto agent
                    end")]
        [TestCase(@"begin goal
                        id goal
                        assignedto begin agent
                                     id agent
                                   end
                    end")]
        public void TestAssignedTo (string input)
        {
            var model = parser.Parse (input);
            model.GoalModel.Goals
                .Where (x => x.Identifier == "goal" & x.AssignedAgents.Count() == 1)
                .Select (x => x.AssignedAgents)
                .ShallBeSingle ()
                .ShallBeSuchThat (x => x.Identifier == "agent");
        }

        [TestCase(@"begin goal
                        id goal
                        assignedto agent1, agent2
                    end")]
        [TestCase(@"begin goal
                        id goal
                        assignedto begin agent
                                     id agent1
                                   end, agent2
                    end")]
        [TestCase(@"begin goal
                        id goal
                        assignedto begin agent
                                     id agent1
                                   end, begin agent
                                     id agent2
                                   end
                    end")]
        public void TestAssignedToMultipleAgents (string input)
        {
            var model = parser.Parse (input);

            model.GoalModel.Goals
                .Where (x => x.Identifier == "goal")
                .SelectMany (x => x.AssignedAgents)
                .Select (x => x.Identifier)
                .ShallOnlyContain (new string[] { "agent1", "agent2" });
        }
    }
}

