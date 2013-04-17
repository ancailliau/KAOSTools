using System;
using System.Linq;
using NUnit.Framework;
using KAOSTools.Parsing;
using LtlSharp;
using ShallTests;

namespace KAOSTools.Parsing.Tests
{
    [TestFixture()]
    public class TestParsingSystem
    {
        private static Parser parser = new Parser ();
        
        [TestCase(@"declare system
                        id test
                    end", "test")]
        [TestCase(@"declare system
                        id _test
                    end", "_test")]
        [TestCase(@"declare system
                        id -test
                    end", "-test")]
        [TestCase(@"declare system
                        id $test
                    end", "$test")]
        [TestCase(@"declare system
                        id test_long_identifier
                    end", "test_long_identifier")]
        [TestCase(@"declare system
                        id test-long-identifier
                    end", "test-long-identifier")]
        [TestCase(@"declare system
                        id test12
                    end", "test12")]
        [TestCase(@"declare system
                        id 0
                    end", "0")]
        [TestCase(@"declare system
                        id test2
                        id test
                    end", "test")]
        [TestCase(@"declare system
                        id test
                        id test
                    end", "test")]
        public void TestIdentifier (string input, string expectedIdentifier)
        {
            var model = parser.Parse (input);
            model.GoalModel.Systems.Where (x => x.Identifier == expectedIdentifier).ShallBeSingle ();
        }
        
        [TestCase(@"declare system id   end")]
        [TestCase(@"declare system id - end")]
        [TestCase(@"declare system id _ end")]
        [TestCase(@"declare system id $ end")]
        public void TestInvalidIdentifier (string input)
        {
            Assert.Throws<ParsingException> (() => {
                parser.Parse (input);
            });
        }
        
        [TestCase(@"declare system
                        name ""test""
                    end", "test")]
        [TestCase(@"declare system
                        name ""Long name with spaces and numbers 123""
                    end", "Long name with spaces and numbers 123")]
        [TestCase(@"declare system
                        name ""[-_-]""
                    end", "[-_-]")]
        public void TestName (string input, string expectedName)
        {
            var model = parser.Parse (input);
            model.GoalModel.Systems
                .Where (x => x.Name == expectedName)
                    .ShallBeSingle ();
        }
        
        [TestCase(@"declare system
                        name """"
                    end")]
        [TestCase(@"declare system
                        name """"""
                    end")]
        public void TestInvalidName (string input)
        {
            Assert.Throws<ParsingException> (() => {
                parser.Parse (input);
            });
        }

        [TestCase(@"declare system
                        id test
                    end

                    declare system
                        id test
                    end")]
        [TestCase(@"declare system
                        name ""test""
                    end

                    declare system
                        name ""test""
                    end")]
        public void TestFailedOverride (string input)
        {
            Assert.Throws<ParsingException> (() => {
                parser.Parse (input);
            });
        }

        [TestCase(@"declare system
                        id test
                    end

                    override system
                        id test
                    end")]
        [TestCase(@"declare system
                        name ""test""
                    end

                    override system
                        name ""test""
                    end")]
        public void TestOverride (string input)
        {
            var model = parser.Parse (input);
            model.GoalModel.Systems.Count.ShallEqual (1);
        }
           
        [TestCase(@"declare system id test  end
                    declare system id test2 end")]
        public void TestMultiple (string input)
        {
            var model = parser.Parse (input);
            model.GoalModel.Systems.ShallContain (x => x.Identifier == "test");
            model.GoalModel.Systems.ShallContain (x => x.Identifier == "test2");
        }

        
        [TestCase(@"declare goal 
                        id test
                        refinedby[""system 1""] child1, child2
                        refinedby[""system 2""] child1, child2
                    end

                    declare goal
                        id test2
                        refinedby[""system 3""] child3
                    end")]
        [TestCase(@"declare goal 
                        id test
                        refinedby[alt1] child1, child2
                        refinedby[alt2] child1, child2
                    end

                    declare goal
                        id test2
                        refinedby[alt3] child3
                    end

                    declare system id alt1 name ""system 1"" end
                    declare system id alt2 name ""system 2"" end
                    declare system id alt3 name ""system 3"" end")]
        public void TestRefinementsystem (string input)
        {
            var model = parser.Parse (input);
            
            model.GoalModel.Systems.Select (x => x.Name)
                .ShallOnlyContain (new string[] { "system 1" , "system 2", "system 3" });
            
            var goalTest = model.GoalModel.Goals
                .ShallContain (x => x.Identifier == "test")
                    .ShallBeSingle ();
            
            goalTest.Refinements.Select (x => x.SystemIdentifier).Select (x => x.Name)
                .ShallOnlyContain (new string[] { "system 1" , "system 2" });
            
            var goalTest2 = model.GoalModel.Goals
                .ShallContain (x => x.Identifier == "test2")
                    .ShallBeSingle ();
            
            goalTest2.Refinements.Select (x => x.SystemIdentifier).Select (x => x.Name)
                .ShallOnlyContain (new string[] { "system 3" });
        }

        [TestCase(@"declare goal 
                        id test
                        refinedby[alt1] child1, child2
                        refinedby[alt2] child1, child2
                    end

                    declare goal
                        id test2
                        refinedby[alt3] child3
                    end")]
        public void TestRefinementAlternativeIdentifier (string input)
        {
            var model = parser.Parse (input);
            
            model.GoalModel.Systems.Select (x => x.Identifier)
                .ShallOnlyContain (new string[] { "alt1" , "alt2", "alt3" });
            
            var goalTest = model.GoalModel.Goals
                .ShallContain (x => x.Identifier == "test")
                    .ShallBeSingle ();
            
            goalTest.Refinements.Select (x => x.SystemIdentifier).Select (x => x.Identifier)
                .ShallOnlyContain (new string[] { "alt1" , "alt2" });
            
            var goalTest2 = model.GoalModel.Goals
                .ShallContain (x => x.Identifier == "test2")
                    .ShallBeSingle ();
            
            goalTest2.Refinements.Select (x => x.SystemIdentifier).Select (x => x.Identifier)
                .ShallOnlyContain (new string[] { "alt3" });
        }

        [TestCase(@"declare system 
                        id test
                        alternative sys1
                        alternative sys2
                    end")]
        [TestCase(@"declare system 
                        id test
                        alternative declare system id sys1 end
                        alternative sys2
                    end")]
        [TestCase(@"declare system 
                        id test
                        alternative declare system id sys1 end
                        alternative declare system id sys2 end
                    end")]
        public void TestSystemRefinement (string input)
        {
            var model = parser.Parse (input);

            model.GoalModel.Systems.Count.ShallEqual (3);
            model.GoalModel.RootSystems.Count.ShallEqual (1);

            var root = model.GoalModel.RootSystems.Single ();
            root.Alternatives.Select (a => a.Identifier).ShallOnlyContain (new string[] { "sys1", "sys2" });
        }
    }

}

