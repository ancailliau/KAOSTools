using System;
using System.Linq;
using NUnit.Framework;
using UCLouvain.KAOSTools.Parsing;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Parsing.Parsers;

namespace UCLouvain.KAOSTools.Parsing.Tests
{
    [TestFixture()]
    public class TestParsingGoal
    {
        private static ModelBuilder parser = new ModelBuilder ();
       
        [TestCase(@"declare goal [ test ]
                    end", "test")]
        [TestCase(@"declare goal [ test_long_identifier ]
                    end", "test_long_identifier")]
        [TestCase(@"declare goal [ test-long-identifier ]
                    end", "test-long-identifier")]
        [TestCase(@"declare goal [ test12 ]
                    end", "test12")]
        public void TestIdentifier (string input, string expectedIdentifier)
        {
            var model = parser.Parse (input);
            model.Goals().Where (x => x.Identifier == expectedIdentifier).ShallBeSingle ();
        }

        [TestCase(@"declare goal [] end")]
        [TestCase(@"declare goal [-] end")]
        [TestCase(@"declare goal [_] end")]
        [TestCase(@"declare goal [$] end")]
        public void TestInvalidIdentifier (string input)
        {
            Assert.Throws<ParserException> (() => {
                parser.Parse (input);
            });
        }

        [TestCase(@"declare goal [ test ]
                        name ""test""
                    end", "test")]
		[TestCase(@"declare goal [ test ]
                        name ""Long name with spaces and numbers 123""
                    end", "Long name with spaces and numbers 123")]
		[TestCase(@"declare goal [ test ]
                        name ""[-_-]""
                    end", "[-_-]")]
		[TestCase(@"declare goal [ test ]
                        name ""multi
                               line""
                    end", "multi line")]
		[TestCase("declare goal [ test ] name \"quoted \"\"name\"\"\" end", @"quoted ""name""")]
        public void TestName (string input, string expectedName)
        {
            var model = parser.Parse (input);
            model.Goals().Where (x => x.Name == expectedName).ShallBeSingle ();
        }

        [TestCase(@"declare goal [ test ]
                        name """"""
                    end")]
        public void TestInvalidName (string input)
        {
            Assert.Throws<ParserException> (() => {
                parser.Parse (input);
            });
        }

		[TestCase(@"declare goal [ test ]
                        definition ""test""
                    end", "test")]
		[TestCase(@"declare goal [ test ]
                        definition """"
                    end", "")]
		[TestCase(@"declare goal [ test ]
                        definition ""on multiple
                                     lines.""
                    end", "on multiple lines.")]
        [TestCase("declare goal [ test ] definition \"with a \"\"quote\"\" !\" end", "with a \"quote\" !")]
        public void TestDefinition (string input, string expectedDefinition)
        {
            var model = parser.Parse (input);
            var g = model.Goals().Single (x => x.Identifier == "test");
            g.Definition.ShallEqual (expectedDefinition);
        }

		[TestCase(@"declare goal [ test ]
                        name ""old name""
                        definition ""old definition""
                        refinedby old_child1, old_child2
                        obstructedby old_obstacle
                        assignedto old_agent
                    end

                    override goal [ test ]
                        name ""new name""
                        definition ""new definition""
                        refinedby new_child1, new_child2
                        obstructedby new_obstacle
                        assignedto new_agent
                    end

                    declare goal [ old_child1 ] end 
                    declare goal [ new_child1 ] end
                    declare goal [ old_child2 ] end 
                    declare goal [ new_child2 ] end

                    declare obstacle [ old_obstacle ] end 
                    declare obstacle [ new_obstacle ] end

                    declare agent [ old_agent ] end 
                    declare agent [ new_agent ] end")]
        public void TestMerge (string input)
        {
            var model = parser.Parse (input);
            
            var goal = model.Goals().Where (x => x.Identifier == "test").ShallBeSingle ();
            goal.Name.ShallEqual ("new name");
            goal.Definition.ShallEqual ("new definition");

            goal.Refinements().ShallContain (y => y.SubGoalIdentifiers.Select (x => x.Identifier)
                .OnlyContains (new string[] { "old_child1", "old_child2" }));

            goal.Refinements().ShallContain (y => y.SubGoalIdentifiers.Select (x => x.Identifier)
                .OnlyContains (new string[] { "new_child1", "new_child2" }));

            goal.Obstructions()
                .Select (x => x.ObstacleIdentifier)
                .ShallOnlyContain (new string[] { "new_obstacle", "old_obstacle" });
            
            goal.AgentAssignments()
                .SelectMany (x => x.Agents())
                .Select (x => x.Identifier)
                .ShallOnlyContain (new string[] { "new_agent", "old_agent" });
        }

        [Test()]
        public void TestMultipleGoals ()
        {
            var input = @"declare goal [ test ] end
                          declare goal [ test2 ] end";

            var model = parser.Parse (input);

            model.Goals().Count().ShallEqual (2);
            model.Goals().ShallContain (x => x.Identifier == "test");
            model.Goals().ShallContain (x => x.Identifier == "test2");
        }
            
        [TestCase(@"declare goal [ test ]
                        refinedby child1, child2
                    end
                    declare goal [ child1 ] end
                    declare goal [ child2 ] end")]
		[TestCase(@"declare goal [ test ]
                        refinedby child1, child2
                    end")]
        public void TestRefinement (string input)
        {
            var model = parser.Parse (input);

            var goal = model.Goals()
                .ShallContain (x => x.Identifier == "test")
                .ShallBeSingle ();

            var refinement = goal.Refinements().Single ();
            refinement.SubGoalIdentifiers.Select (x => x.Identifier).ShallOnlyContain ( new string [] { "child1" , "child2" });
        }


		[TestCase(@"declare goal [ test ]
                        refinedby child1, child2
                    end
                    declare goal [ child1 ] refinedby child3, child4 end
                    declare goal [ child2 ] end
                    declare goal [ child3 ] end
                    declare goal [ child4 ] end")]
		[TestCase(@"declare goal [ test ]
                        refinedby child1, child2
                    end
                    declare goal [ child1 ] refinedby child3, child4 end
                    declare goal [ child2 ] end")]
        public void TestRefinementRecursive (string input)
        {
            var model = parser.Parse (input);
            
            var test = model.Goals()
                .ShallContain (x => x.Identifier == "test")
                .ShallBeSingle ();
            
            test.Refinements()
                .ShallContain (x => x.SubGoalIdentifiers.Select (y => y.Identifier)
                                              .OnlyContains ( new string [] { "child1" , "child2" }));

            var child1 = model.Goals()
                .ShallContain (x => x.Identifier == "child1")
                .ShallBeSingle ();
            
            child1.Refinements()
                .ShallContain (x => x.SubGoalIdentifiers.Select (y => y.Identifier)
                                              .OnlyContains ( new string [] { "child3" , "child4" }));
        }

		[TestCase(@"declare goal [ test ]
                        refinedby  child2, domprop
                    end
                    declare goal [ child2 ] end
                    declare domainproperty [ domprop ] end")]
        public void TestRefinementWithDomainProperty (string input)
        {
            var model = parser.Parse (input);

            var test = model.Goals().ShallContain (x => x.Identifier == "test").ShallBeSingle ();
            test.Refinements().ShallBeSingle ().DomainPropertyIdentifiers.Select (x => x.Identifier).ShallOnlyContain (new string [] { "domprop" });
        }

        
        [TestCase(@"declare goal [ test ]
                        refinedby  child2, domhyp
                    end
                    declare domhyp [ domhyp ] end
                    declare goal [ child2 ] end")]
        public void TestRefinementWithDomainHypothesis (string input)
        {
            var model = parser.Parse (input);
            
            var test = model.Goals().ShallContain (x => x.Identifier == "test").ShallBeSingle ();
            test.Refinements().ShallBeSingle ().DomainHypothesisIdentifiers.Select (x => x.Identifier).ShallOnlyContain (new string [] { "domhyp" });
        }

        [TestCase(@"declare goal [ test ]
                        refinedby [ milestone ] goal1, goal2
                    end
                    declare goal [ goal1 ] end
                    declare goal [ goal2 ] end", RefinementPattern.Milestone)]
        [TestCase(@"declare goal [ test ]
                        refinedby [ introduce_guard ] goal1, goal2
                    end
                    declare goal [ goal1 ] end
                    declare goal [ goal2 ] end", RefinementPattern.IntroduceGuard)]
        [TestCase(@"declare goal [ test ]
                        refinedby [ divide_and_conquer ] goal1, goal2
                    end
                    declare goal [ goal1 ] end
                    declare goal [ goal2 ] end", RefinementPattern.DivideAndConquer)]
        [TestCase(@"declare goal [ test ]
                        refinedby [ unmonitorability ] goal1, goal2
                    end
                    declare goal [ goal1 ] end
                    declare goal [ goal2 ] end", RefinementPattern.Unmonitorability)]
        [TestCase(@"declare goal [ test ]
                        refinedby [ uncontrollability ] goal1, goal2
                    end
                    declare goal [ goal1 ] end
                    declare goal [ goal2 ] end", RefinementPattern.Uncontrollability)]
        public void TestRefinementPatterns (string input, RefinementPattern pattern)
        {
            var model = parser.Parse (input);

            var goal = model.Goals()
                .ShallContain (x => x.Identifier == "test")
                    .ShallBeSingle ();

            var refinement = goal.Refinements().Single ();
            refinement.RefinementPattern.ShallEqual (pattern);
        }
        
        [TestCase(@"declare goal [ test ]
                        refinedby [ case ] goal1 [.5], goal2 [.5]
                    end
                    declare goal [ goal1 ] end
                    declare goal [ goal2 ] end", .5, .5)]
        [TestCase(@"declare goal [ test ]
                        refinedby [ case ] goal1 [.1], goal2 [.9]
                    end
                    declare goal [ goal1 ] end
                    declare goal [ goal2 ] end", .1, .9)]
        [TestCase(@"declare goal [ test ]
                        refinedby [ case ] goal1 [0], goal2 [1]
                    end
                    declare goal [ goal1 ] end
                    declare goal [ goal2 ] end", 0, 1)]
        [TestCase(@"declare goal [ test ]
                        refinedby [ case ] goal1 [1], goal2 [0]
                    end
                    declare goal [ goal1 ] end
                    declare goal [ goal2 ] end", 1, 0)]
        public void TestCaseRefinementPatterns (string input, double e1, double e2)
        {
            var model = parser.Parse (input);

            var goal = model.Goals()
                .ShallContain (x => x.Identifier == "test")
                    .ShallBeSingle ();

            var refinement = goal.Refinements().Single ();
            refinement.RefinementPattern.ShallEqual (RefinementPattern.Case);
            
            AssertParameter (refinement, "goal1", e1);
            AssertParameter (refinement, "goal2", e2);
        }

        private static void AssertParameter (GoalRefinement refinement, string goalId, double expected)
        {
            IRefineeParameter p1 = refinement.SubGoalIdentifiers.Single (x => x.Identifier == goalId).Parameters;
            Assert.IsInstanceOf (typeof (PrimitiveRefineeParameter<double>), p1);
            var cp1 = (PrimitiveRefineeParameter<double>)p1;
            Assert.AreEqual (expected, cp1.Value);
        }

        [TestCase(@"declare goal [ test ] rsr 0.95    end", 0.95)]
        [TestCase(@"declare goal [ test ] rsr 1    end", 1)]
        [TestCase(@"declare goal [ test ] rsr 0    end", 0)]
        [TestCase (@"declare goal [ test ] rsr .01  end", .01)]
        [TestCase (@"declare goal [ test ] rsr 99%  end", .99)]
        public void TestRequiredDegreeOfSatisfaction (string input, double expected)
        {
            var model = parser.Parse (input);
            model.Goals().ShallContain (x => x.Identifier == "test").ShallBeSingle ().RDS.ShallEqual (expected);
        }

        [TestCase(@"declare goal [ test ]
                        $custom ""My string""
                    end", "test")]
        public void TestCustomAttribute (string input, string expectedIdentifier)
        {
            var model = parser.Parse (input);
            var g = model.Goals(x => x.Identifier == expectedIdentifier).ShallBeSingle ();
            g.CustomData.Count.ShallEqual (1);
            g.CustomData["custom"].ShallEqual("My string");
        }

        [TestCase(@"declare goal [ test ]
                        except [obst] goal2
                    end", "obst", "goal2")]
        public void TestExceptAttribute (string input, string expectedObstacle, string expectedCM)
        {
            var model = parser.Parse (input);
            var g = model.Goals(x => x.Identifier == "test").ShallBeSingle ();
            g.Exceptions().Count().ShallEqual(1);

			var e = g.Exceptions().SingleOrDefault();
			e.ResolvingGoalIdentifier.ShallEqual(expectedCM);
			e.ResolvedObstacleIdentifier.ShallEqual(expectedObstacle);
        }

        [TestCase(@"declare goal [ test ]
                        providednot [obstacle] mycondition() and otherCondition()
                    end", "obstacle")]
        public void TestProvidedNotAttribute (string input, string expectedObstacle)
        {
            var model = parser.Parse (input);
            var g = model.Goals(x => x.Identifier == "test").ShallBeSingle ();
            g.ProvidedNotAnnotations().Count().ShallEqual(1);

			var e = g.ProvidedNotAnnotations().SingleOrDefault();
			e.ObstacleIdentifier.ShallEqual(expectedObstacle);
			Assert.IsInstanceOf(typeof(And), e.Formula);
		}

        [TestCase(@"declare goal [ test ]
                        replaces [ obstacle ] test2
                    end", "test2")]
        public void TestReplacesAttribute (string input, string expectedGoal)
        {
            var model = parser.Parse (input);
            var g = model.Goals(x => x.Identifier == "test").ShallBeSingle ();
            g.Replacements().Count().ShallEqual(1);

			var e = g.Replacements().SingleOrDefault();
			e.ResolvingGoalIdentifier.ShallEqual(expectedGoal);
        }

        [TestCase(@"declare goal [ test ]
                        provided [obstacle] mycondition() and otherCondition()
                    end", "obstacle")]
        public void TestProvidedAttribute (string input, string expectedObstacle)
        {
            var model = parser.Parse (input);
            var g = model.Goals(x => x.Identifier == "test").ShallBeSingle ();
            g.ProvidedAnnotations().Count().ShallEqual(1);

			var e = g.ProvidedAnnotations().SingleOrDefault();
			e.ObstacleIdentifier.ShallEqual(expectedObstacle);
			Assert.IsInstanceOf(typeof(And), e.Formula);
        }

        [TestCase(@"declare goal [ test ]
                        relaxedTo [obstacle] mycondition() and otherCondition()
                    end", "obstacle")]
        public void TestRelaxedToAttribute (string input, string expectedObstacle)
        {
            var model = parser.Parse (input);
            var g = model.Goals(x => x.Identifier == "test").ShallBeSingle ();
            g.RelaxedToAnnotations().Count().ShallEqual(1);

			var e = g.RelaxedToAnnotations().SingleOrDefault();
			e.ObstacleIdentifier.ShallEqual(expectedObstacle);
			Assert.IsInstanceOf(typeof(And), e.Formula);
        }
    }

}

