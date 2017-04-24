using System;
using System.Linq;
using NUnit.Framework;
using KAOSTools.Parsing;
using System.Collections.Generic;
using KAOSTools.Core;
using KAOSTools.Parsing.Parsers;
using UCLouvain.KAOSTools.Core.SatisfactionRates;

namespace UCLouvain.KAOSTools.Parsing.Tests
{
    [TestFixture()]
    public class TestParsingObstacle
    {
        private static ModelBuilder parser = new ModelBuilder ();

        [TestCase(@"declare obstacle[ test ]
                    end", "test")]
        [TestCase(@"declare obstacle[ test_long_identifier ]
                    end", "test_long_identifier")]
        [TestCase(@"declare obstacle[ test-long-identifier ]
                    end", "test-long-identifier")]
        [TestCase(@"declare obstacle[ test12 ]
                    end", "test12")]
        public void TestIdentifier (string input, string expectedIdentifier)
        {
            var model = parser.Parse (input);
            model.Obstacles().Where (x => x.Identifier == expectedIdentifier).ShallBeSingle ();
        }

		[TestCase(@"declare obstacle [] end")]
		[TestCase(@"declare obstacle [-] end")]
		[TestCase(@"declare obstacle [_] end")]
		[TestCase(@"declare obstacle [$] end")]
        public void TestInvalidIdentifier (string input)
        {
            Assert.Throws<ParserException> (() => {
                parser.Parse (input);
            });
        }
        
        [TestCase(@"declare obstacle [ test ]
                        name ""test""
                    end", "test")]
		[TestCase(@"declare obstacle [ test ]
                        name ""Long name with spaces and numbers 123""
                    end", "Long name with spaces and numbers 123")]
		[TestCase(@"declare obstacle [ test ]
                        name ""[-_-]""
                    end", "[-_-]")]
        public void TestName (string input, string expectedName)
        {
            var model = parser.Parse (input);
            model.Obstacles()
                .Where (x => x.Name == expectedName)
                .ShallBeSingle ();
        }

		[TestCase(@"declare obstacle [ test ]
                        name """"""
                    end")]
        public void TestInvalidName (string input)
        {
            Assert.Throws<ParserException> (() => {
                parser.Parse (input);
            });
        }

        [Test()]
        public void TestMultiple ()
        {
            var input = @"declare obstacle [ test ] end
                          declare obstacle [ test2 ] end";
            
            var model = parser.Parse (input);
            
            model.Obstacles().Count().ShallEqual (2);
            model.Obstacles().ShallContain (x => x.Identifier == "test");
            model.Obstacles().ShallContain (x => x.Identifier == "test2");
        }
        
        [TestCase(@"declare obstacle [ test ]
                        refinedby child1, child2
                    end
                    declare obstacle [ child1 ] end declare obstacle [ child2 ] end")]
		[TestCase(@"declare obstacle [ test ]
                        refinedby child1, child2
                    end")]
        public void TestRefinement (string input)
        {
            var model = parser.Parse (input);
            
            var obstacle = model.Obstacles()
                .ShallContain (x => x.Identifier == "test")
                    .ShallBeSingle ();
            
            obstacle.Refinements()
                .ShallContain (x => x.SubobstacleIdentifiers.Select (y => y.Identifier)
                               .OnlyContains ( new string [] { "child1" , "child2" }));
        }

        [TestCase(@"declare obstacle [ test ]
                        refinedby child1, child2
                        refinedby child3, child4
                    end
                    declare obstacle [ child1 ] end declare obstacle [ child2 ] end
                    declare obstacle [ child3 ] end declare obstacle [ child4 ] end")]
        public void TestAlternatives (string input)
        {
            var model = parser.Parse (input);
            
            var obstacle = model.Obstacles()
                .ShallContain (x => x.Identifier == "test")
                    .ShallBeSingle ();

            Console.WriteLine("***");
            foreach (var re in obstacle.Refinements()) {
                Console.WriteLine(string.Join(",", re.SubobstacleIdentifiers));
			}
			Console.WriteLine("***");

            obstacle.Refinements().ShallContain (x => x.SubobstacleIdentifiers.Select (y => y.Identifier)
                                                              .OnlyContains ( new string [] { "child1" , "child2" }));
            obstacle.Refinements().ShallContain (x => x.SubobstacleIdentifiers.Select (y => y.Identifier)
                                                              .OnlyContains ( new string [] { "child3" , "child4" }));
        }

		[TestCase(@"declare obstacle[ test ]
                        refinedby  test2, domprop
                    end
                    declare obstacle [ test2 ] end
                    declare domainproperty [ domprop ] end")]
        public void TestRefinementWithDomainProperty (string input)
        {
            var model = parser.Parse (input);
            
            var test = model.Obstacles().ShallContain (x => x.Identifier == "test").ShallBeSingle ();
            test.Refinements().ShallBeSingle ().DomainPropertyIdentifiers.Select (y => y.Identifier).ShallOnlyContain (new string [] { "domprop" });
        }

		[TestCase(@"declare obstacle[ test ]
                        refinedby  test2, domhyp
                    end
                    declare obstacle [ test2 ] end
                    declare domhyp [ domhyp ] end")]
        public void TestRefinementWithDomainHypothesis (string input)
        {
            var model = parser.Parse (input);
            
            var test = model.Obstacles().ShallContain (x => x.Identifier == "test").ShallBeSingle ();
            var refinement = test.Refinements().ShallBeSingle ();

            refinement.DomainHypothesisIdentifiers.Select (y => y.Identifier).ShallOnlyContain (new string [] { "domhyp" });
        }
        

        [Test()]
        public void TestMerge ()
        {
            var input = @"declare obstacle [ test ]
                            name ""old name""
                            definition ""old definition""
                            refinedby old_child1, old_child2
                            resolvedby old_goal
                        end
                        declare obstacle [ old_child1 ] end
                        declare obstacle [ old_child2 ] end
                        declare goal [ old_goal ] end

                        override obstacle [ test ]
                            name ""new name""
                            definition ""new definition""
                            refinedby new_child1, new_child2
                            resolvedby new_goal
                        end
                        declare obstacle [ new_child1 ] end
                        declare obstacle [ new_child2 ] end
                        declare goal [ new_goal ] end";
            
            var model = parser.Parse (input);
            
            var obstacle = model.Obstacles().Where (x => x.Identifier == "test").ShallBeSingle ();
            obstacle.Name.ShallEqual ("new name");
            obstacle.Definition.ShallEqual ("new definition");

            obstacle.Refinements().ShallContain (x => x.SubobstacleIdentifiers.Select (y => y.Identifier)
                                           .OnlyContains (new string[] { "old_child1", "old_child2" }));
            
            obstacle.Refinements().ShallContain (x => x.SubobstacleIdentifiers.Select (y => y.Identifier)
                                           .OnlyContains (new string[] { "new_child1", "new_child2" }));
            
            obstacle.Resolutions()
                .Select (x => x.ResolvingGoalIdentifier)
                    .ShallOnlyContain (new string[] { "new_goal", "old_goal" });
        }

        [TestCase(@"declare goal[ test ]
                        obstructedby obstacle_1
                        obstructedby obstacle_2
                    end")]
        [TestCase(@"declare goal[ test ]
                        obstructedby obstacle_1
                        obstructedby obstacle_2
                    end
                    declare obstacle [ obstacle_1 ] end
                    declare obstacle [ obstacle_2 ] end")]
        public void TestObstruction (string input)
        {
            var model = parser.Parse (input);
            var test = model.Goals().Where (x => x.Identifier == "test").ShallBeSingle ();

            test.Obstructions().Select (x => x.ObstacleIdentifier).ShallOnlyContain (new string[] { "obstacle_1", "obstacle_2" });
        }
        
        [TestCase(@"declare obstacle[ test ]
                        resolvedby goal_1
                        resolvedby goal_2
                    end
                    declare goal [ goal_1 ] end
                    declare goal [ goal_2 ] end")]
		[TestCase(@"declare obstacle[ test ]
                        resolvedby goal_1
                        resolvedby goal_2
                    end")]
        public void TestResolution (string input)
        {
            var model = parser.Parse (input);
            var test = model.Obstacles().Where (x => x.Identifier == "test").ShallBeSingle ();
            test.Resolutions().Select (x => x.ResolvingGoalIdentifier).ShallOnlyContain (new string[] { "goal_1", "goal_2" });
        }

        [TestCase(@"declare goal [ obstructedgoal ] obstructedby test end
                    declare obstacle[ test ]
                        resolvedby [prevention] goal
                    end
                    declare goal [ goal ] end")]
        public void TestResolutionPattern (string input)
        {
            var model = parser.Parse (input);
            //model.IntegrateResolutions ();

            var test = model.Obstacles().Where (x => x.Identifier == "test").ShallBeSingle ();
            var resolution = test.Resolutions().Single ();
            resolution.ResolvingGoalIdentifier.ShallEqual ("goal");
            resolution.ResolutionPattern.ShallEqual (ResolutionPattern.ObstaclePrevention);

            //var obstructedGoal = model.Goals().Single (x => x.Identifier == "obstructedgoal");
            //var e = obstructedGoal.Assumptions.Single ();
            //e.Implicit.ShallBeTrue ();
            //Assert.AreEqual ("goal", e.Assumed.Identifier);
            //Assert.Fail ();
        }

        //[TestCase(@"declare goal id obstructedgoal obstructedby test end
        //            declare obstacle[ test ]
        //                resolvedby(weak_mitigation[anchor]) goal
        //            end")]
        //public void TestResolutionPatternWithAnchor (string input)
        //{
        //    var model = parser.Parse (input);
        //    model.IntegrateResolutions ();

        //    var test = model.Obstacles().Where (x => x.Identifier == "test").ShallBeSingle ();
        //    var resolution = test.Resolutions().Single ();
        //    resolution.ResolvingGoalIdentifier.ShallEqual ("goal");
        //    resolution.ResolutionPattern.ShallEqual (ResolutionPattern.ObstacleWeakMitigation);
        //    (resolution.Parameters.First() as Goal).Identifier.ShallEqual ("anchor");

        //    // Console.WriteLine (string.Join(",", model.Goals().Select(x => x.Identifier + "("+ x.Exceptions.Count + ")")));

        //    var obstructedGoal = model.Goals().Single (x => x.Identifier == "anchor");
        //    var e = obstructedGoal.Exceptions().Single ();
        //    e.Implicit.ShallBeTrue ();
        //    e.ResolvedObstacleIdentifier.ShallEqual ("test");
        //    e.ResolvingGoalIdentifier.ShallEqual ("goal");

        //}

        [TestCase(@"declare obstacle [ test ] probability 0.95 end", 0.95)]
        [TestCase(@"declare obstacle [ test ] probability 1    end", 1)]
        [TestCase(@"declare obstacle [ test ] probability 0    end", 0)]
        [TestCase(@"declare obstacle [ test ] probability .01  end", .01)]
        [TestCase(@"declare obstacle [ test ] esr 0.95 end", 0.95)]
        [TestCase(@"declare obstacle [ test ] esr 1    end", 1)]
        [TestCase(@"declare obstacle [ test ] esr 0    end", 0)]
        [TestCase(@"declare obstacle [ test ] esr .01  end", .01)]
        public void TestProbability (string input, double expected)
        {
            var model = parser.Parse (input);
            // model.Obstacles().ShallContain (x => x.Identifier == "test").ShallBeSingle ().EPS.ShallEqual (expected);

            var sr = model.satisfactionRateRepository.GetObstacleSatisfactionRate ("test");
            Assert.IsInstanceOf(typeof(DoubleSatisfactionRate), sr);
            Assert.AreEqual(expected, ((DoubleSatisfactionRate)sr).SatisfactionRate);
        }

        [TestCase(@"declare obstacle [ test ] $my_attribute ""my_value"" end",
                  "my_attribute", "my_value")]
        public void TestCustomAttribute(string input, string key, string value)
        {
            var model = parser.Parse(input);
            var v = model.Obstacles()
                .Where(x => x.Identifier == "test")
                .ShallBeSingle();
            v.CustomData.Keys.ShallContain(key);
            v.CustomData[key].ShallEqual(value);
        }
    }
}

