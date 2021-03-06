﻿using System;
using System.Linq;
using NUnit.Framework;
using UCLouvain.KAOSTools.Parsing;
using UCLouvain.KAOSTools.Core;

namespace UCLouvain.KAOSTools.Parsing.Tests
{
    [TestFixture()]
    public class TestParsingFormalSpec
    {
        static ModelBuilder parser = new ModelBuilder ();
       
        [TestCase(@"declare goal [ test ]
                              formalspec  forall amb: Ambulance . Test(amb)
                          end", new string[] { "amb" })]
        [TestCase(@"declare goal [ test ]
                              formalspec  forall amb1: Ambulance, amb2: Ambulance . Test(amb1, amb2)
                          end", new string[] { "amb1", "amb2" })]
        public void TestForAll (string input, string[] variables)
        {
            var model = parser.Parse (input);
            var root = model.Goals().Where (x => x.Identifier == "test").ShallBeSingle ();
            var expression = root.FormalSpec as Forall;

            Assert.NotNull (expression);

            expression.Declarations.Select(x => x.Name).ShallOnlyContain (variables);
            Assert.NotNull (expression.Enclosed);
            
            model.Entities().Count (x => x.Identifier == "Ambulance").ShallEqual (1);
        }

        [TestCase(@"declare goal [ test ]
                              formalspec  exists amb: Ambulance . Test(amb)
                          end", new string[] { "amb" })]
        [TestCase(@"declare goal [ test ]
                              formalspec  exists amb1: Ambulance, amb2: Ambulance . Test(amb1, amb2)
                          end", new string[] { "amb1", "amb2" })]
        public void TestExists (string input, string[] variables)
        {
            var model = parser.Parse (input);
            var root = model.Goals().Where (x => x.Identifier == "test").ShallBeSingle ();
            var expression = root.FormalSpec as Exists;

            Assert.NotNull (expression);

            expression.Declarations.Select(x => x.Name).ShallOnlyContain (variables);
            Assert.NotNull (expression.Enclosed);

            model.Entities().Count (x => x.Identifier == "Ambulance").ShallEqual (1);
        }

        [TestCase(@"declare goal [ test ]
                              formalspec  when Allocated() then Mobilized()
                          end")]
        public void TestStrongImply (string input)
        {
            var model = parser.Parse (input);
            var root = model.Goals().Where (x => x.Identifier == "test").ShallBeSingle ();
            var expression = root.FormalSpec as StrongImply;

            Assert.NotNull (expression);
            Assert.NotNull (expression.Left);
            Assert.NotNull (expression.Right);
        }

        [TestCase(@"declare goal [ test ]
                              formalspec  if Allocated() then Mobilized()
                          end")]
        public void TestImplication (string input)
        {
            var model = parser.Parse (input);
            var root = model.Goals().Where (x => x.Identifier == "test").ShallBeSingle ();
            var expression = root.FormalSpec as Imply;

            Assert.NotNull (expression);
            Assert.NotNull (expression.Left);
            Assert.NotNull (expression.Right);
        }

        [TestCase(@"declare goal [ test ]
                              formalspec  Allocated() iff Mobilized()
                          end")]
        public void TestEquivalence (string input)
        {
            var model = parser.Parse (input);
            var root = model.Goals().Where (x => x.Identifier == "test").ShallBeSingle ();
            var expression = root.FormalSpec as Equivalence;

            Assert.NotNull (expression);
            Assert.NotNull (expression.Left);
            Assert.NotNull (expression.Right);
        }
        
        [TestCase(@"declare goal [ test ]
                              formalspec  Allocated() until Mobilized()
                          end")]
        public void TestUntil (string input)
        {
            var model = parser.Parse (input);
            var root = model.Goals().Where (x => x.Identifier == "test").ShallBeSingle ();
            var expression = root.FormalSpec as Until;
            
            Assert.NotNull (expression, root.FormalSpec.GetType() + " is not expected");
            Assert.NotNull (expression.Left);
            Assert.NotNull (expression.Right);
        }
        
        [TestCase(@"declare goal [ test ]
                              formalspec  Allocated() release Mobilized()
                          end")]
        public void TestRelease (string input)
        {
            var model = parser.Parse (input);
            var root = model.Goals().Where (x => x.Identifier == "test").ShallBeSingle ();
            var expression = root.FormalSpec as Release;

            Assert.NotNull (expression, root.FormalSpec.GetType() + " is not expected");
            Assert.NotNull (expression.Left);
            Assert.NotNull (expression.Right);
        }
        
        [TestCase(@"declare goal [ test ]
                              formalspec  Allocated() unless Mobilized()
                          end")]
        public void TestUnless (string input)
        {
            var model = parser.Parse (input);
            var root = model.Goals().Where (x => x.Identifier == "test").ShallBeSingle ();
            var expression = root.FormalSpec as Unless;
            
            Assert.NotNull (expression, root.FormalSpec.GetType() + " is not expected");
            Assert.NotNull (expression.Left);
            Assert.NotNull (expression.Right);
        }
        
        [TestCase(@"declare goal [ test ]
                              formalspec  Allocated() and Mobilized()
                          end")]
        public void TestAnd (string input)
        {
            var model = parser.Parse (input);
            var root = model.Goals().Where (x => x.Identifier == "test").ShallBeSingle ();
            var expression = root.FormalSpec as And;

            Assert.NotNull (expression, root.FormalSpec.GetType() + " is not expected");
            Assert.NotNull (expression.Left);
            Assert.NotNull (expression.Right);
        }
        
        [TestCase(@"declare goal [ test ]
                              formalspec  Allocated() or Mobilized()
                          end")]
        public void TestOr (string input)
        {
            var model = parser.Parse (input);
            var root = model.Goals().Where (x => x.Identifier == "test").ShallBeSingle ();
            var expression = root.FormalSpec as Or;

            Assert.NotNull (expression, root.FormalSpec.GetType() + " is not expected");
            Assert.NotNull (expression.Left);
            Assert.NotNull (expression.Right);
        }

        [TestCase(@"declare goal [ test ]
                              formalspec  not Mobilized()
                          end")]
        public void TestNot (string input)
        {
            var model = parser.Parse (input);
            var root = model.Goals().Where (x => x.Identifier == "test").ShallBeSingle ();
            var expression = root.FormalSpec as Not;

            Assert.NotNull (expression, root.FormalSpec.GetType() + " is not expected");
            Assert.NotNull (expression.Enclosed);
        }

        [TestCase(@"declare goal [ test ]
                              formalspec  next Mobilized()
                          end")]
        public void TestNext (string input)
        {
            var model = parser.Parse (input);
            var root = model.Goals().Where (x => x.Identifier == "test").ShallBeSingle ();
            var expression = root.FormalSpec as Next;

            Assert.NotNull (expression, root.FormalSpec.GetType() + " is not expected");
            Assert.NotNull (expression.Enclosed);
        }

        [TestCase(@"declare goal [ test ]
                              formalspec  sooner-or-later Mobilized()
                          end")]
        [TestCase(@"declare goal [ test ]
                              formalspec  eventually Mobilized()
                          end")]
        public void TestEventually (string input)
        {
            var model = parser.Parse (input);
            var root = model.Goals().Where (x => x.Identifier == "test").ShallBeSingle ();
            var expression = root.FormalSpec as Eventually;

            Assert.NotNull (expression, root.FormalSpec.GetType() + " is not expected");
            Assert.NotNull (expression.Enclosed);
        }

        [TestCase(@"declare goal [ test ]
                              formalspec  always Mobilized()
                          end")]
        [TestCase(@"declare goal [ test ]
                              formalspec  globally Mobilized()
                          end")]
        public void TestGlobally (string input)
        {
            var model = parser.Parse (input);
            var root = model.Goals().Where (x => x.Identifier == "test").ShallBeSingle ();
            var expression = root.FormalSpec as Globally;

            Assert.NotNull (expression, root.FormalSpec.GetType() + " is not expected");
            Assert.NotNull (expression.Enclosed);
        }

        [TestCase(@"declare goal [ test ]
                              formalspec  when Mobilized() then sooner-or-later, before 12 minutes, OnScene()
                          end", TimeComparator.less)]
        [TestCase(@"declare goal [ test ]
                              formalspec  when Mobilized() then sooner-or-later, strictly before 12 minutes, OnScene()
                          end", TimeComparator.strictly_less)]
        [TestCase(@"declare goal [ test ]
                              formalspec  when Mobilized() then sooner-or-later, in 12 minutes, OnScene()
                          end", TimeComparator.equal)]
        [TestCase(@"declare goal [ test ]
                              formalspec  when Mobilized() then sooner-or-later, after 12 minutes, OnScene()
                          end", TimeComparator.greater)]
        [TestCase(@"declare goal [ test ]
                              formalspec  when Mobilized() then sooner-or-later, strictly after 12 minutes, OnScene()
                          end", TimeComparator.strictly_greater)]
        public void TestEventuallyTimeBound (string input, TimeComparator comparator)
        {
            var model = parser.Parse (input);
            var root = model.Goals().Where (x => x.Identifier == "test").ShallBeSingle ();

            var implication = root.FormalSpec as StrongImply;
            Assert.NotNull (implication);

            var sooner_or_later = implication.Right as Eventually;
            Assert.NotNull (sooner_or_later);

            sooner_or_later.TimeBound.Comparator.ShallEqual (comparator);
            sooner_or_later.TimeBound.Bound.ShallEqual (new TimeSpan (0, 12, 0));
        }

        [TestCase(@"declare obstacle [ test ]
                              formalspec  sooner-or-later ( Mobilized() and always, for more than 12 minutes, not OnScene() )
                          end", TimeComparator.greater)]
        [TestCase(@"declare obstacle [ test ]
                              formalspec  sooner-or-later ( Mobilized() and always, for strictly more than 12 minutes, not OnScene() )
                          end", TimeComparator.strictly_greater)]
        [TestCase(@"declare obstacle [ test ]
                              formalspec  sooner-or-later ( Mobilized() and always, for 12 minutes, not OnScene() )
                          end", TimeComparator.equal)]
        [TestCase(@"declare obstacle [ test ]
                              formalspec  sooner-or-later ( Mobilized() and always, for less than 12 minutes, not OnScene() )
                          end", TimeComparator.less)]
        [TestCase(@"declare obstacle [ test ]
                              formalspec  sooner-or-later ( Mobilized() and always, for strictly less than 12 minutes, not OnScene() )
                          end", TimeComparator.strictly_less)]
        public void TestGloballyTimeBound (string input, TimeComparator comparator)
        {
            var model = parser.Parse (input);
            var root = model.Obstacles().Where (x => x.Identifier == "test").ShallBeSingle ();
            
            var sooner_or_later = root.FormalSpec as Eventually;
            Assert.NotNull (sooner_or_later);

            var and = sooner_or_later.Enclosed as And;
            Assert.NotNull (and);

            var globally = and.Right as Globally;
            Assert.NotNull (globally);

            globally.TimeBound.Comparator.ShallEqual (comparator);
            globally.TimeBound.Bound.ShallEqual (new TimeSpan (0, 12, 0));
        }

        [TestCase(@"declare goal [ test ]
                        formalspec predicate()
                    end", "predicate", null)]
        public void TestPredicateReference (string input, string id, string name)
        {
            var model = parser.Parse (input);
            var root = model.Goals().Where (x => x.Identifier == "test").ShallBeSingle ();
            var predicateReference = root.FormalSpec as PredicateReference;

            if (id != null)
                Assert.AreEqual (id, predicateReference.PredicateIdentifier);

            model.Predicates().Count().ShallEqual(1);
        }

        [TestCase("declare goal [ test ] formalspec exists arg1 : my_type . predicate(arg1) end", "predicate", null)]
        public void TestPredicateReferenceWithParameters (string input, string id, string name)
        {
            var model = parser.Parse (input);
            var root = model.Goals().Where (x => x.Identifier == "test").ShallBeSingle ();
            var predicateReference = (root.FormalSpec as Exists).Enclosed as PredicateReference;

            if (id != null)
                Assert.AreEqual (id, predicateReference.PredicateIdentifier);

            predicateReference.ActualArguments.ShallOnlyContain (new string[] {"arg1"});
            
            model.Predicates().Count().ShallEqual(1);
        }

        [TestCase("declare goal [ test ] formalspec exists arg1 : my_type . (arg1) in predicate end", "predicate", null)]
        public void TestRelationReferenceWithParameters (string input, string id, string name)
        {
            var model = parser.Parse (input);
            var root = model.Goals().Where (x => x.Identifier == "test").ShallBeSingle ();
            var expression = (root.FormalSpec as Exists).Enclosed as RelationReference;

            if (id != null)
                Assert.AreEqual (id, expression.Relation);

            expression.ActualArguments.ShallOnlyContain (new string[] {"arg1"});

            model.Relations().Count().ShallEqual(1);
        }

        [TestCase("declare goal [ test ] formalspec exists arg1 : my_type . arg1.test end", "test", null)]
        public void TestAttributeReference (string input, string id, string name)
        {
            var model = parser.Parse (input);
            var root = model.Goals().Where (x => x.Identifier == "test").ShallBeSingle ();
            var expression = (root.FormalSpec as Exists).Enclosed as AttributeReference;

			Assert.NotNull(expression);
			Assert.NotNull(expression.Attribute);

            if (id != null)
                Assert.AreEqual(id, expression.Attribute);

			var entity = model.Entities().Single(x => x.Identifier == "my_type");
            model.Attributes().Where (x => x.EntityIdentifier == entity.Identifier).Count().ShallEqual(1);
        }
        
        [TestCase("declare goal [ test ] formalspec exists arg1 : my_type . arg1.test == 0.5 end", 
                  ComparisonCriteria.Equals, 0.5)]
        [TestCase("declare goal [ test ] formalspec exists arg1 : my_type . arg1.test == .5 end", 
                  ComparisonCriteria.Equals, 0.5)]
        [TestCase("declare goal [ test ] formalspec exists arg1 : my_type . arg1.test == 124 end", 
                  ComparisonCriteria.Equals, 124)]
        [TestCase("declare goal [ test ] formalspec exists arg1 : my_type . arg1.test == 0 end", 
                  ComparisonCriteria.Equals, 0)]
        [TestCase("declare goal [ test ] formalspec exists arg1 : my_type . arg1.test > 0 end", 
                  ComparisonCriteria.BiggerThan, 0)]
        [TestCase("declare goal [ test ] formalspec exists arg1 : my_type . arg1.test >= 0 end", 
                  ComparisonCriteria.BiggerThanOrEquals, 0)]
        [TestCase("declare goal [ test ] formalspec exists arg1 : my_type . arg1.test < 0 end", 
                  ComparisonCriteria.LessThan, 0)]
        [TestCase("declare goal [ test ] formalspec exists arg1 : my_type . arg1.test <= 0 end", 
                  ComparisonCriteria.LessThanOrEquals, 0)]
        [TestCase("declare goal [ test ] formalspec exists arg1 : my_type . arg1.test != 0 end", 
                  ComparisonCriteria.NotEquals, 0)]
        public void TestComparisonPredicateNumeric (string input, ComparisonCriteria criteria, double value)
        {
            var model = parser.Parse (input);
            var root = model.Goals().Where (x => x.Identifier == "test").ShallBeSingle ();
            var expression = (root.FormalSpec as Exists).Enclosed as ComparisonPredicate;

            Assert.NotNull (expression);

            expression.Criteria.ShallEqual (criteria);
            (expression.Right as NumericConstant).Value.ShallEqual (value);
        }

        [TestCase("declare goal [ test ] formalspec exists arg1 : my_type . arg1.test == \"This is \"\"my quote!\"\"\" end", 
                  ComparisonCriteria.Equals, "This is \"my quote!\"")]
        [TestCase("declare goal [ test ] formalspec exists arg1 : my_type . arg1.test == \"\" end", 
                  ComparisonCriteria.Equals, "")]
        [TestCase("declare goal [ test ] formalspec exists arg1 : my_type . arg1.test == \"0\" end", 
                  ComparisonCriteria.Equals, "0")]
        [TestCase("declare goal [ test ] formalspec exists arg1 : my_type . arg1.test > \"0\" end", 
                  ComparisonCriteria.BiggerThan, "0")]
        [TestCase("declare goal [ test ] formalspec exists arg1 : my_type . arg1.test >= \"0\" end", 
                  ComparisonCriteria.BiggerThanOrEquals, "0")]
        [TestCase("declare goal [ test ] formalspec exists arg1 : my_type . arg1.test < \"0\" end", 
                  ComparisonCriteria.LessThan, "0")]
        [TestCase("declare goal [ test ] formalspec exists arg1 : my_type . arg1.test <= \"0\" end", 
                  ComparisonCriteria.LessThanOrEquals, "0")]
        [TestCase("declare goal [ test ] formalspec exists arg1 : my_type . arg1.test != \"0\" end", 
                  ComparisonCriteria.NotEquals, "0")]
        public void TestComparisonPredicateString (string input, ComparisonCriteria criteria, string value)
        {
            var model = parser.Parse (input);
            var root = model.Goals().Where (x => x.Identifier == "test").ShallBeSingle ();
            var expression = (root.FormalSpec as Exists).Enclosed as ComparisonPredicate;

            Assert.NotNull (expression);
            
            expression.Criteria.ShallEqual (criteria);
            (expression.Right as StringConstant).Value.ShallEqual (value);

        }
        
        [TestCase("declare goal [ test ] formalspec exists arg1 : my_type . arg1.test == false end", 
                  ComparisonCriteria.Equals, false)]
        [TestCase("declare goal [ test ] formalspec exists arg1 : my_type . arg1.test != true end", 
                  ComparisonCriteria.NotEquals, true)]
        public void TestComparisonPredicateBool (string input, ComparisonCriteria criteria, bool value)
        {
            var model = parser.Parse (input);
            var root = model.Goals().Where (x => x.Identifier == "test").ShallBeSingle ();
            var expression = (root.FormalSpec as Exists).Enclosed as ComparisonPredicate;

            Assert.NotNull (expression);

            expression.Criteria.ShallEqual (criteria);
            (expression.Right as BoolConstant).Value.ShallEqual (value);

        }

        [TestCase(@"declare goal 
                      [ test ]
                      formalspec exists arg1 : T,  arg2 : T . arg1 != arg2
                    end", 
                  ComparisonCriteria.NotEquals)]
        public void TestComparisonVariable (string input, ComparisonCriteria criteria)
        {
            var model = parser.Parse (input);
            var root = model.Goals().Where (x => x.Identifier == "test").ShallBeSingle ();
            var expression = (root.FormalSpec as Exists).Enclosed as ComparisonPredicate;

            Assert.NotNull (expression);

            expression.Criteria.ShallEqual (criteria);
            (expression.Left as VariableReference).Name.ShallEqual ("arg1");
            (expression.Right as VariableReference).Name.ShallEqual ("arg2");
        }
        
        [TestCase("declare goal [ test ] formalspec exists arg1 : my_type . \"MyString\" == true end", 
                  ComparisonCriteria.Equals)]
        [TestCase("declare goal [ test ] formalspec exists arg1 : my_type . 0 != true end", 
                  ComparisonCriteria.NotEquals)]
        [TestCase("declare goal [ test ] formalspec exists arg1 : my_type . 0 >= 1 end", 
                  ComparisonCriteria.BiggerThanOrEquals)]
        public void TestComparisonPredicateReverse (string input, ComparisonCriteria criteria)
        {
            var model = parser.Parse (input);
            var root = model.Goals().Where (x => x.Identifier == "test").ShallBeSingle ();
            var expression = (root.FormalSpec as Exists).Enclosed as ComparisonPredicate;

            Assert.NotNull (expression);

            expression.Criteria.ShallEqual (criteria);
        }

        [TestCase("declare goal [ test ] formalspec exists arg1 : my_type . arg1.test != arg1.test end", 
                  ComparisonCriteria.NotEquals)]
        public void TestComparisonPredicateOtherAttribute (string input, ComparisonCriteria criteria)
        {
            var model = parser.Parse (input);
            var root = model.Goals().Where (x => x.Identifier == "test").ShallBeSingle ();
            var expression = (root.FormalSpec as Exists).Enclosed as ComparisonPredicate;

            Assert.NotNull (expression);

            expression.Criteria.ShallEqual (criteria);
            (expression.Right as AttributeReference).Variable.ShallEqual ("arg1");
            (expression.Right as AttributeReference).Attribute.ShallEqual ("test");

        }

        [TestCase("declare goal [ test ] formalspec exists arg1 : my_type . arg1.test != predicate(arg1) end", 
                  ComparisonCriteria.NotEquals)]
        public void TestComparisonPredicateOtherPredicate (string input, ComparisonCriteria criteria)
        {
            var model = parser.Parse (input);
            var root = model.Goals().Where (x => x.Identifier == "test").ShallBeSingle ();
            var expression = (root.FormalSpec as Exists).Enclosed as ComparisonPredicate;

            Assert.NotNull (expression);

            expression.Criteria.ShallEqual (criteria);
			(expression.Right as PredicateReference).PredicateIdentifier.ShallEqual("predicate");
            (expression.Right as PredicateReference).ActualArguments.ShallOnlyContain (new string[] { "arg1" });

        }

        [TestCase(@"declare domhyp [ test ]
                              formalspec  forall amb: Ambulance . Test(amb)
                          end", new string[] { "amb" })]
        [TestCase(@"declare domhyp [ test ]
                              formalspec  forall amb1: Ambulance, amb2: Ambulance . Test(amb1, amb2)
                          end", new string[] { "amb1", "amb2" })]
        public void TestFormalSpecDomHyp(string input, string[] variables)
        {
            var model = parser.Parse(input);
            var root = model.DomainHypotheses().Where(x => x.Identifier == "test").ShallBeSingle();
            var expression = root.FormalSpec as Forall;
            Assert.NotNull(expression);
        }
    }

}

