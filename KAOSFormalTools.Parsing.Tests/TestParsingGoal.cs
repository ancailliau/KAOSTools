using System;
using System.Linq;
using NUnit.Framework;
using KAOSFormalTools.Parsing;
using LtlSharp;
using ShallTests;

namespace KAOSFormalTools.Parsing.Tests
{
    [TestFixture()]
    public class TestParsingGoal
    {
        private static Parser parser = new Parser ();
       
        [TestCase(@"begin goal
                        # test
                        id test
                    end")]
        [TestCase(@"begin goal
                        # test
                        id # test
                        test
                    end")]
        [TestCase(@"begin goal
                        # test
                        # id test
                    end")]
        [TestCase(@"# begin goal
                        # test
                        # id test
                        begin goal id test end #
                    # end")]
        public void TestComment (string input)
        {
            var model = parser.Parse (input);
            model.GoalModel.Goals.ShallBeSingle ();
        }

        [TestCase(@"begin goal
                        id test
                    end", "test")]
        [TestCase(@"begin goal
                        id _test
                    end", "_test")]
        [TestCase(@"begin goal
                        id -test
                    end", "-test")]
        [TestCase(@"begin goal
                        id $test
                    end", "$test")]
        [TestCase(@"begin goal
                        id test_long_identifier
                    end", "test_long_identifier")]
        [TestCase(@"begin goal
                        id test-long-identifier
                    end", "test-long-identifier")]
        [TestCase(@"begin goal
                        id test12
                    end", "test12")]
        [TestCase(@"begin goal
                        id 0
                    end", "0")]
        [TestCase(@"begin goal
                        id test2
                        id test
                    end", "test")]
        [TestCase(@"begin goal
                        id test
                        id test
                    end", "test")]
        public void TestIdentifier (string input, string expectedIdentifier)
        {
            var model = parser.Parse (input);
            model.GoalModel.Goals.Where (x => x.Identifier == expectedIdentifier).ShallBeSingle ();
        }

        [TestCase(@"begin goal
                        id 
                    end")]
        [TestCase(@"begin goal
                        id -
                    end")]
        [TestCase(@"begin goal
                        id _
                    end")]
        [TestCase(@"begin goal
                        id $
                    end")]
        public void TestInvalidIdentifier (string input)
        {
            Assert.Throws<ParsingException> (() => {
                parser.Parse (input);
            });
        }

        [TestCase(@"begin goal
                        name ""test""
                    end", "test")]
        [TestCase(@"begin goal
                        name ""Long name with spaces and numbers 123""
                    end", "Long name with spaces and numbers 123")]
        [TestCase(@"begin goal
                        name ""[-_-]""
                    end", "[-_-]")]
        public void TestName (string input, string expectedName)
        {
            var model = parser.Parse (input);
            model.GoalModel.Goals
                .Where (x => x.Name == expectedName)
                    .ShallBeSingle ();
        }
        
        [TestCase(@"begin goal
                        name """"
                    end")]
        [TestCase(@"begin goal
                        name """"""
                    end")]
        public void TestInvalidName (string input)
        {
            Assert.Throws<ParsingException> (() => {
                parser.Parse (input);
            });
        }

        [Test()]
        public void TestOverride ()
        {
            var input = @"begin goal
                            id test
                            name ""old name""
                            definition ""old definition""
                            formalspec ""old""
                            refinedby old_child1, old_child2
                            obstructedby old_obstacle
                            assignedto old_agent
                        end

                        override goal
                            id test
                            name ""new name""
                            definition ""new definition""
                            formalspec ""new""
                            refinedby new_child1, new_child2
                            obstructedby new_obstacle
                            assignedto new_agent
                        end";
            
            var model = parser.Parse (input);

            var goal = model.GoalModel.Goals.Where (x => x.Identifier == "test").ShallBeSingle ();
            goal.Name.ShallEqual ("new name");
            goal.Definition.ShallEqual ("new definition");
            goal.FormalSpec.ShallBeSuchThat (x => (x as LtlSharp.Proposition).Name == "new");
            goal.Refinements.ShallBeSingle ()
                .Children.Select (x => x.Identifier)
                .ShallOnlyContain (new string[] { "new_child1", "new_child2" });

            goal.Obstruction
                .Select (x => x.Identifier)
                .ShallOnlyContain (new string[] { "new_obstacle" });
            
            goal.AssignedAgents
                .SelectMany (x => x.Agents)
                .Select (x => x.Identifier)
                .ShallOnlyContain (new string[] { "new_agent" });
        }

        [TestCase(@"begin goal
                        id test
                        name ""old name""
                        definition ""old definition""
                        formalspec ""old""
                        refinedby old_child1, old_child2
                        obstructedby old_obstacle
                        assignedto old_agent
                    end

                    begin goal
                        id test
                        name ""new name""
                        definition ""new definition""
                        formalspec ""new""
                        refinedby new_child1, new_child2
                        obstructedby new_obstacle
                        assignedto new_agent
                    end")]
        [TestCase(@"begin goal
                        id test
                    end

                    begin goal 
                        id test
                        name ""old name""
                        definition ""old definition""
                        formalspec ""old""
                        refinedby old_child1, old_child2
                        obstructedby old_obstacle
                        assignedto old_agent
                    end

                    begin goal
                        id test
                        name ""new name""
                        definition ""new definition""
                        formalspec ""new""
                        refinedby new_child1, new_child2
                        obstructedby new_obstacle
                        assignedto new_agent
                    end")]
        public void TestMerge (string input)
        {
            var model = parser.Parse (input);
            
            var goal = model.GoalModel.Goals.Where (x => x.Identifier == "test").ShallBeSingle ();
            goal.Name.ShallEqual ("old name");
            goal.Definition.ShallEqual ("old definition");
            goal.FormalSpec.ShallBeSuchThat (x => (x as LtlSharp.Proposition).Name == "old");

            goal.Refinements.ShallContain (y => y.Children.Select (x => x.Identifier)
                .OnlyContains (new string[] { "old_child1", "old_child2" }));

            goal.Refinements.ShallContain (y => y.Children.Select (x => x.Identifier)
                .OnlyContains (new string[] { "new_child1", "new_child2" }));

            goal.Obstruction
                .Select (x => x.Identifier)
                .ShallOnlyContain (new string[] { "new_obstacle", "old_obstacle" });
            
            goal.AssignedAgents
                .SelectMany (x => x.Agents)
                .Select (x => x.Identifier)
                .ShallOnlyContain (new string[] { "new_agent", "old_agent" });
        }

        [Test()]
        public void TestFormalSpec ()
        {
            var input = @"begin goal
                              id          test
                              formalspec  ""pif""
                          end";
            
            var model = parser.Parse (input);
            var root = model.GoalModel.Goals.Where (x => x.Identifier == "test").ShallBeSingle ();
            Assert.IsNotNull (root.FormalSpec);
        }

        [Test()]
        public void TestMultipleGoals ()
        {
            var input = @"begin goal id test  end
                          begin goal id test2 end";

            var model = parser.Parse (input);

            model.GoalModel.Goals.Count.ShallEqual (2);
            model.GoalModel.Goals.ShallContain (x => x.Identifier == "test");
            model.GoalModel.Goals.ShallContain (x => x.Identifier == "test2");
        }
                
        [TestCase(@"begin goal 
                        id test
                        refinedby child1, child2
                    end")]
        [TestCase(@"begin goal 
                        id test
                        refinedby begin goal id child1 end, child2
                    end")]
        [TestCase(@"begin goal 
                        id test
                        refinedby begin goal id child1 end, begin goal id child2 end
                    end")]
        public void TestRefinement (string input)
        {
            var model = parser.Parse (input);

            var goal = model.GoalModel.Goals
                .ShallContain (x => x.Identifier == "test")
                .ShallBeSingle ();

            goal.Refinements
                .ShallContain (x => x.Children.Select(y => y.Identifier)
                                              .OnlyContains ( new string [] { "child1" , "child2" }));
        }

        [TestCase(@"begin goal 
                        id test
                        refinedby child1, child2
                    end
                    begin goal id child1 refinedby child3, child4 end")]
        [TestCase(@"begin goal 
                        id test
                        refinedby begin goal id child1 refinedby child3, child4 end, child2
                    end")]
        [TestCase(@"begin goal 
                        id test
                        refinedby begin goal id child1 refinedby begin goal id child3 end, child4 end, child2
                    end")]
        public void TestRefinementRecursive (string input)
        {
            var model = parser.Parse (input);
            
            var test = model.GoalModel.Goals
                .ShallContain (x => x.Identifier == "test")
                .ShallBeSingle ();
            
            test.Refinements
                .ShallContain (x => x.Children.Select(y => y.Identifier)
                                              .OnlyContains ( new string [] { "child1" , "child2" }));

            var child1 = model.GoalModel.Goals
                .ShallContain (x => x.Identifier == "child1")
                .ShallBeSingle ();
            
            child1.Refinements
                .ShallContain (x => x.Children.Select(y => y.Identifier)
                                              .OnlyContains ( new string [] { "child3" , "child4" }));
        }

        [TestCase(@"begin goal
                        id         test
                        refinedby  test2, domprop
                    end
                    begin domainproperty id domprop end")]
        [TestCase(@"begin goal
                        id         test
                        refinedby  test2, ""domprop""
                    end
                    begin domainproperty id domprop name ""domprop"" end")]
        public void TestRefinementWithDomainProperty (string input)
        {
            var model = parser.Parse (input);

            var test = model.GoalModel.Goals.ShallContain (x => x.Identifier == "test").ShallBeSingle ();
            test.Refinements.ShallBeSingle ().DomainProperties.Select (x => x.Identifier).ShallOnlyContain (new string [] { "domprop" });
        }
            
        [TestCase(@"begin goal 
                        id test
                        refinedby child1, ""child2""
                    end
                    begin goal id child1 name ""child1"" end")]
        [TestCase(@"begin goal 
                        id test
                        refinedby ""child1"", ""child2""
                    end")]
        [TestCase(@"begin goal 
                        id test
                        refinedby begin goal name ""child1"" end, ""child2""
                    end")]
        [TestCase(@"begin goal 
                        id test
                        refinedby begin goal name ""child1"" end, begin goal name ""child2"" end
                    end")]
        public void TestRefinementByName (string input)
        {
            var model = parser.Parse (input);
            
            var goal = model.GoalModel.Goals
                .ShallContain (x => x.Identifier == "test")
                    .ShallBeSingle ();
            
            goal.Refinements
                .ShallContain (x => x.Children.Select(y => y.Name)
                               .OnlyContains ( new string [] { "child1" , "child2" }));
        }

        [TestCase(@"begin goal 
                        id test
                        refinedby[""Alternative 1""] child1, child2
                        refinedby[""Alternative 2""] child1, child2
                    end

                    begin goal
                        id test2
                        refinedby[""Alternative 3""] child3
                    end")]
        public void TestRefinementAlternative (string input)
        {
            var model = parser.Parse (input);

            model.GoalModel.Alternatives.Select (x => x.Name)
                .ShallOnlyContain (new string[] { "Alternative 1" , "Alternative 2", "Alternative 3" });

            var goalTest = model.GoalModel.Goals
                .ShallContain (x => x.Identifier == "test")
                    .ShallBeSingle ();

            goalTest.Refinements.Select (x => x.AlternativeIdentifier).Select (x => x.Name)
                .ShallOnlyContain (new string[] { "Alternative 1" , "Alternative 2" });

            var goalTest2 = model.GoalModel.Goals
                .ShallContain (x => x.Identifier == "test2")
                    .ShallBeSingle ();
            
            goalTest2.Refinements.Select (x => x.AlternativeIdentifier).Select (x => x.Name)
                .ShallOnlyContain (new string[] { "Alternative 3" });
        }

        [TestCase(@"begin goal id test rds 0.95 end", 0.95)]
        [TestCase(@"begin goal id test rds 1    end", 1)]
        [TestCase(@"begin goal id test rds 0    end", 0)]
        [TestCase(@"begin goal id test rds .01  end", .01)]
        public void TestRequiredDegreeOfSatisfaction (string input, double expected)
        {
            var model = parser.Parse (input);
            model.GoalModel.Goals.ShallContain (x => x.Identifier == "test").ShallBeSingle ().RDS.ShallEqual (expected);
        }

    }

}

