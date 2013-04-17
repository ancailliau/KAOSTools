using System;
using System.Linq;
using NUnit.Framework;
using KAOSTools.Parsing;
using LtlSharp;
using ShallTests;

namespace KAOSTools.Parsing.Tests
{
    [TestFixture()]
    public class TestParsingGoal
    {
        private static Parser parser = new Parser ();
       
        [TestCase(@"declare goal
                        # test
                        id test
                    end")]
        [TestCase(@"declare goal
                        # test
                        id # test
                        test
                    end")]
        [TestCase(@"declare goal
                        # test
                        # id test
                    end")]
        [TestCase(@"# declare goal
                        # test
                        # id test
                        declare goal id test end #
                    # end")]
        public void TestComment (string input)
        {
            var model = parser.Parse (input);
            model.GoalModel.Goals.ShallBeSingle ();
        }

        [TestCase(@"declare goal
                        id test
                    end", "test")]
        [TestCase(@"declare goal
                        id _test
                    end", "_test")]
        [TestCase(@"declare goal
                        id -test
                    end", "-test")]
        [TestCase(@"declare goal
                        id $test
                    end", "$test")]
        [TestCase(@"declare goal
                        id test_long_identifier
                    end", "test_long_identifier")]
        [TestCase(@"declare goal
                        id test-long-identifier
                    end", "test-long-identifier")]
        [TestCase(@"declare goal
                        id test12
                    end", "test12")]
        [TestCase(@"declare goal
                        id 0
                    end", "0")]
        [TestCase(@"declare goal
                        id test2
                        id test
                    end", "test")]
        [TestCase(@"declare goal
                        id test
                        id test
                    end", "test")]
        public void TestIdentifier (string input, string expectedIdentifier)
        {
            var model = parser.Parse (input);
            model.GoalModel.Goals.Where (x => x.Identifier == expectedIdentifier).ShallBeSingle ();
        }

        [TestCase(@"declare goal
                        id 
                    end")]
        [TestCase(@"declare goal
                        id -
                    end")]
        [TestCase(@"declare goal
                        id _
                    end")]
        [TestCase(@"declare goal
                        id $
                    end")]
        public void TestInvalidIdentifier (string input)
        {
            Assert.Throws<ParsingException> (() => {
                parser.Parse (input);
            });
        }

        [TestCase(@"declare goal
                        name ""test""
                    end", "test")]
        [TestCase(@"declare goal
                        name ""Long name with spaces and numbers 123""
                    end", "Long name with spaces and numbers 123")]
        [TestCase(@"declare goal
                        name ""[-_-]""
                    end", "[-_-]")]
        public void TestName (string input, string expectedName)
        {
            var model = parser.Parse (input);
            model.GoalModel.Goals
                .Where (x => x.Name == expectedName)
                    .ShallBeSingle ();
        }
        
        [TestCase(@"declare goal
                        name """"
                    end")]
        [TestCase(@"declare goal
                        name """"""
                    end")]
        public void TestInvalidName (string input)
        {
            Assert.Throws<ParsingException> (() => {
                parser.Parse (input);
            });
        }

        [TestCase(@"declare goal
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
                    end")]
        [TestCase(@"declare goal
                        id test
                    end

                    override goal 
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
                    end")]
        public void TestMerge (string input)
        {
            var model = parser.Parse (input);
            
            var goal = model.GoalModel.Goals.Where (x => x.Identifier == "test").ShallBeSingle ();
            goal.Name.ShallEqual ("old name");
            goal.Definition.ShallEqual ("old definition");
            goal.FormalSpec.ShallBeSuchThat (x => (x as LtlSharp.Proposition).Name == "old");

            foreach (var r in goal.Refinements) {
                Console.WriteLine (string.Join (",", r.Children.Select (x => x.Identifier)));
            }

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
            var input = @"declare goal
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
            var input = @"declare goal id test  end
                          declare goal id test2 end";

            var model = parser.Parse (input);

            model.GoalModel.Goals.Count.ShallEqual (2);
            model.GoalModel.Goals.ShallContain (x => x.Identifier == "test");
            model.GoalModel.Goals.ShallContain (x => x.Identifier == "test2");
        }
                
        [TestCase(@"declare goal 
                        id test
                        refinedby child1, child2
                    end")]
        [TestCase(@"declare goal 
                        id test
                        refinedby declare goal id child1 end, child2
                    end")]
        [TestCase(@"declare goal 
                        id test
                        refinedby declare goal id child1 end, declare goal id child2 end
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

        [TestCase(@"declare goal 
                        id test
                        refinedby child1, child2
                    end
                    declare goal id child1 refinedby child3, child4 end")]
        [TestCase(@"declare goal 
                        id test
                        refinedby declare goal id child1 refinedby child3, child4 end, child2
                    end")]
        [TestCase(@"declare goal 
                        id test
                        refinedby declare goal id child1 refinedby declare goal id child3 end, child4 end, child2
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

        [TestCase(@"declare goal
                        id         test
                        refinedby  test2, domprop
                    end
                    declare domainproperty id domprop end")]
        [TestCase(@"declare goal
                        id         test
                        refinedby  test2, ""domprop""
                    end
                    declare domainproperty id domprop name ""domprop"" end")]
        public void TestRefinementWithDomainProperty (string input)
        {
            var model = parser.Parse (input);

            var test = model.GoalModel.Goals.ShallContain (x => x.Identifier == "test").ShallBeSingle ();
            test.Refinements.ShallBeSingle ().DomainProperties.Select (x => x.Identifier).ShallOnlyContain (new string [] { "domprop" });
        }
            
        [TestCase(@"declare goal 
                        id test
                        refinedby child1, ""child2""
                    end
                    declare goal id child1 name ""child1"" end")]
        [TestCase(@"declare goal 
                        id test
                        refinedby ""child1"", ""child2""
                    end")]
        [TestCase(@"declare goal 
                        id test
                        refinedby declare goal name ""child1"" end, ""child2""
                    end")]
        [TestCase(@"declare goal 
                        id test
                        refinedby declare goal name ""child1"" end, declare goal name ""child2"" end
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


        [TestCase(@"declare goal id test rds 0.95 end", 0.95)]
        [TestCase(@"declare goal id test rds 1    end", 1)]
        [TestCase(@"declare goal id test rds 0    end", 0)]
        [TestCase(@"declare goal id test rds .01  end", .01)]
        public void TestRequiredDegreeOfSatisfaction (string input, double expected)
        {
            var model = parser.Parse (input);
            model.GoalModel.Goals.ShallContain (x => x.Identifier == "test").ShallBeSingle ().RDS.ShallEqual (expected);
        }

    }

}
