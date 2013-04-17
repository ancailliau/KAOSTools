using System;
using System.Linq;
using NUnit.Framework;
using KAOSTools.Parsing;
using LtlSharp;
using System.Collections.Generic;
using ShallTests;

namespace KAOSTools.Parsing.Tests
{
    [TestFixture()]
    public class TestParsingObstacle
    {
        private static Parser parser = new Parser ();

        [TestCase(@"declare obstacle
                        id test
                    end", "test")]
        [TestCase(@"declare obstacle
                        id _test
                    end", "_test")]
        [TestCase(@"declare obstacle
                        id -test
                    end", "-test")]
        [TestCase(@"declare obstacle
                        id $test
                    end", "$test")]
        [TestCase(@"declare obstacle
                        id test_long_identifier
                    end", "test_long_identifier")]
        [TestCase(@"declare obstacle
                        id test-long-identifier
                    end", "test-long-identifier")]
        [TestCase(@"declare obstacle
                        id test12
                    end", "test12")]
        [TestCase(@"declare obstacle
                        id 0
                    end", "0")]
        [TestCase(@"declare obstacle
                        id test2
                        id test
                    end", "test")]
        [TestCase(@"declare obstacle
                        id test
                        id test
                    end", "test")]
        public void TestIdentifier (string input, string expectedIdentifier)
        {
            var model = parser.Parse (input);
            model.GoalModel.Obstacles.Where (x => x.Identifier == expectedIdentifier).ShallBeSingle ();
        }
        
        [TestCase(@"declare obstacle
                        id 
                    end")]
        [TestCase(@"declare obstacle
                        id -
                    end")]
        [TestCase(@"declare obstacle
                        id _
                    end")]
        [TestCase(@"declare obstacle
                        id $
                    end")]
        public void TestInvalidIdentifier (string input)
        {
            Assert.Throws<ParsingException> (() => {
                parser.Parse (input);
            });
        }
        
        [TestCase(@"declare obstacle
                        name ""test""
                    end", "test")]
        [TestCase(@"declare obstacle
                        name ""Long name with spaces and numbers 123""
                    end", "Long name with spaces and numbers 123")]
        [TestCase(@"declare obstacle
                        name ""[-_-]""
                    end", "[-_-]")]
        public void TestName (string input, string expectedName)
        {
            var model = parser.Parse (input);
            model.GoalModel.Obstacles
                .Where (x => x.Name == expectedName)
                .ShallBeSingle ();
        }
        
        [TestCase(@"declare obstacle
                        name """"
                    end")]
        [TestCase(@"declare obstacle
                        name """"""
                    end")]
        public void TestInvalidName (string input)
        {
            Assert.Throws<ParsingException> (() => {
                parser.Parse (input);
            });
        }

        
        [Test()]
        public void TestFormalSpec ()
        {
            var input = @"declare obstacle
                              id          test
                              formalspec  ""pouf""
                          end";
            
            var model = parser.Parse (input);
            var test = model.GoalModel.Obstacles.Where (x => x.Identifier == "test").ShallBeSingle ();
            Assert.IsNotNull (test.FormalSpec);
        }
        
        [Test()]
        public void TestMultiple ()
        {
            var input = @"declare obstacle id test  end
                          declare obstacle id test2 end";
            
            var model = parser.Parse (input);
            
            model.GoalModel.Obstacles.Count.ShallEqual (2);
            model.GoalModel.Obstacles.ShallContain (x => x.Identifier == "test");
            model.GoalModel.Obstacles.ShallContain (x => x.Identifier == "test2");
        }
        
        [TestCase(@"declare obstacle 
                        id test
                        refinedby child1, child2
                    end")]
        [TestCase(@"declare obstacle 
                        id test
                        refinedby declare obstacle id child1 end, child2
                    end")]
        [TestCase(@"declare obstacle 
                        id test
                        refinedby declare obstacle id child1 end, declare obstacle id child2 end
                    end")]
        public void TestRefinement (string input)
        {
            var model = parser.Parse (input);
            
            var obstacle = model.GoalModel.Obstacles
                .ShallContain (x => x.Identifier == "test")
                    .ShallBeSingle ();
            
            obstacle.Refinements
                .ShallContain (x => x.Children.Select(y => y.Identifier)
                               .OnlyContains ( new string [] { "child1" , "child2" }));
        }

        [TestCase(@"declare obstacle 
                        id test
                        refinedby child1, child2
                        refinedby child3, child4
                    end")]
        [TestCase(@"declare obstacle 
                        id test
                        refinedby child1, child2
                        refinedby declare obstacle id child3 end, child4
                    end")]
        public void TestAlternatives (string input)
        {
            var model = parser.Parse (input);
            
            var obstacle = model.GoalModel.Obstacles
                .ShallContain (x => x.Identifier == "test")
                    .ShallBeSingle ();
            
            obstacle.Refinements.ShallContain (x => x.Children.Select(y => y.Identifier)
                                                              .OnlyContains ( new string [] { "child1" , "child2" }));
            obstacle.Refinements.ShallContain (x => x.Children.Select(y => y.Identifier)
                                                              .OnlyContains ( new string [] { "child3" , "child4" }));
        }
        
        [TestCase(@"declare obstacle 
                        id test
                        refinedby child1, child2
                    end
                    declare obstacle id child1 refinedby child3, child4 end")]
        [TestCase(@"declare obstacle 
                        id test
                        refinedby declare obstacle id child1 refinedby child3, child4 end, child2
                    end")]
        [TestCase(@"declare obstacle 
                        id test
                        refinedby declare obstacle id child1 refinedby declare obstacle id child3 end, child4 end, child2
                    end")]
        public void TestRefinementRecursive (string input)
        {
            var model = parser.Parse (input);
            
            var test = model.GoalModel.Obstacles
                .ShallContain (x => x.Identifier == "test")
                    .ShallBeSingle ();
            
            test.Refinements
                .ShallContain (x => x.Children.Select(y => y.Identifier)
                               .OnlyContains ( new string [] { "child1" , "child2" }));
            
            var child1 = model.GoalModel.Obstacles
                .ShallContain (x => x.Identifier == "child1")
                    .ShallBeSingle ();
            
            child1.Refinements
                .ShallContain (x => x.Children.Select(y => y.Identifier)
                               .OnlyContains ( new string [] { "child3" , "child4" }));
        }
        
        [TestCase(@"declare obstacle
                        id         test
                        refinedby  test2, domprop
                    end
                    declare domainproperty id domprop end")]
        [TestCase(@"declare obstacle
                        id         test
                        refinedby  test2, ""domprop""
                    end
                    declare domainproperty id domprop name ""domprop"" end")]
        public void TestRefinementWithDomainProperty (string input)
        {
            var model = parser.Parse (input);
            
            var test = model.GoalModel.Obstacles.ShallContain (x => x.Identifier == "test").ShallBeSingle ();
            test.Refinements.ShallBeSingle ().DomainProperties.Select (x => x.Identifier).ShallOnlyContain (new string [] { "domprop" });
        }
        
        [TestCase(@"declare obstacle 
                        id test
                        refinedby child1, ""child2""
                    end
                    declare obstacle id child1 name ""child1"" end")]
        [TestCase(@"declare obstacle 
                        id test
                        refinedby ""child1"", ""child2""
                    end")]
        [TestCase(@"declare obstacle 
                        id test
                        refinedby declare obstacle name ""child1"" end, ""child2""
                    end")]
        [TestCase(@"declare obstacle 
                        id test
                        refinedby declare obstacle name ""child1"" end, declare obstacle name ""child2"" end
                    end")]
        public void TestRefinementByName (string input)
        {
            var model = parser.Parse (input);
            
            var goal = model.GoalModel.Obstacles
                .ShallContain (x => x.Identifier == "test")
                    .ShallBeSingle ();
            
            goal.Refinements
                .ShallContain (x => x.Children.Select(y => y.Name)
                               .OnlyContains ( new string [] { "child1" , "child2" }));
        }

        [Test()]
        public void TestMerge ()
        {
            var input = @"declare obstacle
                            id test
                            name ""old name""
                            definition ""old definition""
                            formalspec ""old""
                            refinedby old_child1, old_child2
                            resolvedby old_goal
                        end

                        override obstacle
                            id test
                            name ""new name""
                            definition ""new definition""
                            formalspec ""new""
                            refinedby new_child1, new_child2
                            resolvedby new_goal
                        end";
            
            var model = parser.Parse (input);
            
            var obstacle = model.GoalModel.Obstacles.Where (x => x.Identifier == "test").ShallBeSingle ();
            obstacle.Name.ShallEqual ("old name");
            obstacle.Definition.ShallEqual ("old definition");
            obstacle.FormalSpec.ShallBeSuchThat (x => (x as LtlSharp.Proposition).Name == "old");
            
            obstacle.Refinements.ShallContain (y => y.Children.Select (x => x.Identifier)
                                           .OnlyContains (new string[] { "old_child1", "old_child2" }));
            
            obstacle.Refinements.ShallContain (y => y.Children.Select (x => x.Identifier)
                                           .OnlyContains (new string[] { "new_child1", "new_child2" }));
            
            obstacle.Resolutions
                .Select (x => x.Identifier)
                    .ShallOnlyContain (new string[] { "new_goal", "old_goal" });
        }

        [TestCase(@"declare goal
                        id test
                        obstructedby obstacle_1, obstacle_2
                    end")]
        [TestCase(@"declare goal
                        id test
                        obstructedby declare obstacle id obstacle_1 end, obstacle_2
                    end")]
        [TestCase(@"declare goal
                        id test
                        obstructedby declare obstacle id obstacle_1 end, declare obstacle id obstacle_2 end
                    end")]
        [TestCase(@"declare goal
                        id test
                        obstructedby obstacle_1
                        obstructedby obstacle_2
                    end")]
        public void TestObstruction (string input)
        {
            var model = parser.Parse (input);
            var test = model.GoalModel.Goals.Where (x => x.Identifier == "test").ShallBeSingle ();
            test.Obstruction.Select (x => x.Identifier).ShallOnlyContain (new string[] { "obstacle_1", "obstacle_2" });
        }
        
        [TestCase(@"declare obstacle
                        id test
                        resolvedby goal_1, goal_2
                    end")]
        [TestCase(@"declare obstacle
                        id test
                        resolvedby declare goal id goal_1 end, goal_2
                    end")]
        [TestCase(@"declare obstacle
                        id test
                        resolvedby declare goal id goal_1 end, declare goal id goal_2 end
                    end")]
        [TestCase(@"declare obstacle
                        id test
                        resolvedby goal_1
                        resolvedby goal_2
                    end")]
        public void TestResolution (string input)
        {
            var model = parser.Parse (input);
            var test = model.GoalModel.Obstacles.Where (x => x.Identifier == "test").ShallBeSingle ();
            test.Resolutions.Select (x => x.Identifier).ShallOnlyContain (new string[] { "goal_1", "goal_2" });
        }

        [TestCase(@"declare obstacle id test probability 0.95 end", 0.95)]
        [TestCase(@"declare obstacle id test probability 1    end", 1)]
        [TestCase(@"declare obstacle id test probability 0    end", 0)]
        [TestCase(@"declare obstacle id test probability .01  end", .01)]
        [TestCase(@"declare obstacle id test eps 0.95 end", 0.95)]
        [TestCase(@"declare obstacle id test eps 1    end", 1)]
        [TestCase(@"declare obstacle id test eps 0    end", 0)]
        [TestCase(@"declare obstacle id test eps .01  end", .01)]
        public void TestProbability (string input, double expected)
        {
            var model = parser.Parse (input);
            model.GoalModel.Obstacles.ShallContain (x => x.Identifier == "test").ShallBeSingle ().EPS.ShallEqual (expected);
        }
    }
}

