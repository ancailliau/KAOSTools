using System;
using System.Linq;
using NUnit.Framework;
using KAOSTools.Parsing;
using ShallTests;
using KAOSTools.MetaModel;

namespace KAOSTools.Parsing.Tests
{
    [TestFixture()]
    public class TestParsingAntiGoal
    {
        private static ModelBuilder parser = new ModelBuilder ();
       
        [TestCase(@"declare antigoal
                        id test
                    end", "test")]
        [TestCase(@"declare antigoal
                        id test_long_identifier
                    end", "test_long_identifier")]
        [TestCase(@"declare antigoal
                        id test-long-identifier
                    end", "test-long-identifier")]
        [TestCase(@"declare antigoal
                        id test12
                    end", "test12")]
        public void TestIdentifier (string input, string expectedIdentifier)
        {
            var model = parser.Parse (input);
            model.GoalModel.AntiGoals.Where (x => x.Identifier == expectedIdentifier).ShallBeSingle ();
        }

        [TestCase(@"declare antigoal
                        id 
                    end")]
        [TestCase(@"declare antigoal
                        id -
                    end")]
        [TestCase(@"declare antigoal
                        id _
                    end")]
        [TestCase(@"declare antigoal
                        id $
                    end")]
        public void TestInvalidIdentifier (string input)
        {
            Assert.Throws<ParserException> (() => {
                parser.Parse (input);
            });
        }

        [TestCase(@"declare antigoal
                        name ""test""
                    end", "test")]
        [TestCase(@"declare antigoal
                        name ""Long name with spaces and numbers 123""
                    end", "Long name with spaces and numbers 123")]
        [TestCase(@"declare antigoal
                        name ""[-_-]""
                    end", "[-_-]")]
        [TestCase(@"declare antigoal
                        name ""multi
                               line""
                    end", "multi line")]
        [TestCase("declare antigoal name \"quoted \"\"name\"\"\" end", @"quoted ""name""")]
        public void TestName (string input, string expectedName)
        {
            var model = parser.Parse (input);
            model.GoalModel.AntiGoals
                .Where (x => x.Name == expectedName)
                    .ShallBeSingle ();
        }

        [TestCase(@"declare antigoal
                        name """"""
                    end")]
        public void TestInvalidName (string input)
        {
            Assert.Throws<ParserException> (() => {
                parser.Parse (input);
            });
        }

        [TestCase(@"declare antigoal
                        id test
                        definition ""test""
                    end", "test")]
        [TestCase(@"declare antigoal
                        id test
                        definition """"
                    end", "")]
        [TestCase(@"declare antigoal
                        id test
                        definition ""on multiple
                                     lines.""
                    end", "on multiple lines.")]
        [TestCase("declare antigoal id test definition \"with a \"\"quote\"\" !\" end", "with a \"quote\" !")]
        public void TestDefinition (string input, string expectedDefinition)
        {
            var model = parser.Parse (input);
            var g = model.GoalModel.AntiGoals.Single (x => x.Identifier == "test");
            g.Definition.ShallEqual (expectedDefinition);
        }

        [TestCase(@"declare antigoal
                        id test
                        name ""old name""
                        definition ""old definition""
                        refinedby old_child1, old_child2
                        assignedto old_agent
                    end

                    override antigoal
                        id test
                        name ""new name""
                        definition ""new definition""
                        refinedby new_child1, new_child2
                        assignedto new_agent
                    end")]
        [TestCase(@"declare antigoal
                        id test
                    end

                    override antigoal 
                        id test
                        name ""old name""
                        definition ""old definition""
                        refinedby old_child1, old_child2
                        assignedto old_agent
                    end

                    override antigoal
                        id test
                        name ""new name""
                        definition ""new definition""
                        refinedby new_child1, new_child2
                        assignedto new_agent
                    end")]
        public void TestMerge (string input)
        {
            var model = parser.Parse (input);
            
            var goal = model.GoalModel.AntiGoals.Where (x => x.Identifier == "test").ShallBeSingle ();
            goal.Name.ShallEqual ("new name");
            goal.Definition.ShallEqual ("new definition");

            goal.Refinements.ShallContain (y => y.SubAntiGoals.Select (x => x.Identifier)
                .OnlyContains (new string[] { "old_child1", "old_child2" }));

            goal.Refinements.ShallContain (y => y.SubAntiGoals.Select (x => x.Identifier)
                .OnlyContains (new string[] { "new_child1", "new_child2" }));


            goal.AgentAssignments
                .SelectMany (x => x.Agents)
                .Select (x => x.Identifier)
                .ShallOnlyContain (new string[] { "new_agent", "old_agent" });
        }

        [Test()]
        public void TestMultipleGoals ()
        {
            var input = @"declare antigoal id test  end
                          declare antigoal id test2 end";

            var model = parser.Parse (input);

            model.GoalModel.AntiGoals.Count.ShallEqual (2);
            model.GoalModel.AntiGoals.ShallContain (x => x.Identifier == "test");
            model.GoalModel.AntiGoals.ShallContain (x => x.Identifier == "test2");
        }
            
        [TestCase(@"declare antigoal 
                        id test
                        refinedby child1, child2
                    end")]
        public void TestImplicit (string input)
        {
            var model = parser.Parse (input);
            
            var goal = model.GoalModel.AntiGoals.Single (x => x.Identifier == "test");
            var refinement = goal.Refinements.Single ();
            foreach (var item in refinement.SubAntiGoals) {
                item.Implicit.ShallBeTrue ();
            }
        }

        [TestCase(@"declare antigoal 
                        id test
                        refinedby child1, child2
                    end")]
        [TestCase(@"declare antigoal 
                        id test
                        refinedby declare antigoal id child1 end, child2
                    end")]
        [TestCase(@"declare antigoal 
                        id test
                        refinedby declare antigoal id child1 end, declare antigoal id child2 end
                    end")]
        public void TestRefinement (string input)
        {
            var model = parser.Parse (input);

            var goal = model.GoalModel.AntiGoals
                .ShallContain (x => x.Identifier == "test")
                .ShallBeSingle ();

            goal.Refinements
                .ShallContain (x => x.SubAntiGoals.Select(y => y.Identifier)
                                              .OnlyContains ( new string [] { "child1" , "child2" }));
        }

        [TestCase(@"declare antigoal
                        id         test
                        refinedby  test2, domprop
                    end
                    declare domainproperty id domprop end")]
        [TestCase(@"declare antigoal
                        id         test
                        refinedby  test2, ""domprop""
                    end
                    declare domainproperty id domprop name ""domprop"" end")]
        [TestCase(@"declare antigoal
                        id         test
                        refinedby  test2, declare domainproperty id domprop name ""domprop"" end
                    end")]
        public void TestRefinementWithDomainProperty (string input)
        {
            var model = parser.Parse (input);

            var test = model.GoalModel.AntiGoals.ShallContain (x => x.Identifier == "test").ShallBeSingle ();
            test.Refinements.ShallBeSingle ().DomainProperties.Select (x => x.Identifier).ShallOnlyContain (new string [] { "domprop" });
        }

        
        [TestCase(@"declare antigoal
                        id         test
                        refinedby  test2, obstacle
                    end
                    declare obstacle id obstacle end")]
        [TestCase(@"declare antigoal
                        id         test
                        refinedby  test2, ""obstacle""
                    end
                    declare obstacle id obstacle name ""obstacle"" end")]
        [TestCase(@"declare antigoal
                        id         test
                        refinedby  test2, declare obstacle id obstacle name ""obstacle"" end
                    end")]
        public void TestRefinementWithObstacle (string input)
        {
            var model = parser.Parse (input);

            var test = model.GoalModel.AntiGoals.ShallContain (x => x.Identifier == "test").ShallBeSingle ();
            test.Refinements.ShallBeSingle ().Obstacles.Select (x => x.Identifier).ShallOnlyContain (new string [] { "obstacle" });
        }

        
        [TestCase(@"declare antigoal
                        id         test
                        refinedby  test2, domhyp
                    end
                    declare domhyp id domhyp end")]
        [TestCase(@"declare antigoal
                        id         test
                        refinedby  test2, ""domhyp""
                    end
                    declare domhyp id domhyp name ""domhyp"" end")]
        [TestCase(@"declare antigoal
                        id         test
                        refinedby  test2, declare domhyp id domhyp name ""domhyp"" end
                    end")]
        public void TestRefinementWithDomainHypothesis (string input)
        {
            var model = parser.Parse (input);
            
            var test = model.GoalModel.AntiGoals.ShallContain (x => x.Identifier == "test").ShallBeSingle ();
            test.Refinements.ShallBeSingle ().DomainHypotheses.Select (x => x.Identifier).ShallOnlyContain (new string [] { "domhyp" });
        }
            
        [TestCase(@"declare antigoal 
                        id test
                        refinedby child1, ""child2""
                    end
                    declare antigoal id child1 name ""child1"" end")]
        [TestCase(@"declare antigoal 
                        id test
                        refinedby ""child1"", ""child2""
                    end")]
        [TestCase(@"declare antigoal 
                        id test
                        refinedby declare antigoal name ""child1"" end, ""child2""
                    end")]
        [TestCase(@"declare antigoal 
                        id test
                        refinedby declare antigoal name ""child1"" end, declare antigoal name ""child2"" end
                    end")]
        public void TestRefinementByName (string input)
        {
            var model = parser.Parse (input);
            
            var goal = model.GoalModel.AntiGoals
                .ShallContain (x => x.Identifier == "test")
                    .ShallBeSingle ();
            
            goal.Refinements
                .ShallContain (x => x.SubAntiGoals.Select(y => y.Name)
                               .OnlyContains ( new string [] { "child1" , "child2" }));
        }

    }

}

