using System;
using System.Linq;
using NUnit.Framework;
using KAOSTools.Parsing;
using ShallTests;
using KAOSTools.MetaModel;

namespace KAOSTools.Parsing.Tests
{
    [TestFixture()]
    public class TestParsingSystem
    {
        private static ModelBuilder parser = new ModelBuilder ();
        
        [TestCase(@"declare system
                        id test
                    end", "test")]
        [TestCase(@"declare system
                        id test_long_identifier
                    end", "test_long_identifier")]
        [TestCase(@"declare system
                        id test-long-identifier
                    end", "test-long-identifier")]
        [TestCase(@"declare system
                        id test12
                    end", "test12")]
        public void TestIdentifier (string input, string expectedIdentifier)
        {
            var model = parser.Parse (input);
            model.AlternativeSystems().Where (x => x.Identifier == expectedIdentifier).ShallBeSingle ();
        }
        
        [TestCase(@"declare system id   end")]
        [TestCase(@"declare system id - end")]
        [TestCase(@"declare system id _ end")]
        [TestCase(@"declare system id $ end")]
        public void TestInvalidIdentifier (string input)
        {
            Assert.Throws<ParserException> (() => {
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
            model.AlternativeSystems()
                .Where (x => x.Name == expectedName)
                    .ShallBeSingle ();
        }

        [TestCase(@"declare system
                        name """"""
                    end")]
        public void TestInvalidName (string input)
        {
            Assert.Throws<ParserException> (() => {
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
            Assert.Throws<BuilderException> (() => {
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
            model.AlternativeSystems().Count().ShallEqual (1);
        }
           
        [TestCase(@"declare system id test  end
                    declare system id test2 end")]
        public void TestMultiple (string input)
        {
            var model = parser.Parse (input);
            model.AlternativeSystems().ShallContain (x => x.Identifier == "test");
            model.AlternativeSystems().ShallContain (x => x.Identifier == "test2");
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
            
            model.AlternativeSystems().Select (x => x.Name)
                .ShallOnlyContain (new string[] { "system 1" , "system 2", "system 3" });
            
            var goalTest = model.Goals()
                .ShallContain (x => x.Identifier == "test")
                    .ShallBeSingle ();
            
            goalTest.Refinements().Select (x => x.SystemReference()).Select (x => x.Name)
                .ShallOnlyContain (new string[] { "system 1" , "system 2" });
            
            var goalTest2 = model.Goals()
                .ShallContain (x => x.Identifier == "test2")
                    .ShallBeSingle ();
            
            goalTest2.Refinements().Select (x => x.SystemReference()).Select (x => x.Name)
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
            
            model.AlternativeSystems().Select (x => x.Identifier)
                .ShallOnlyContain (new string[] { "alt1" , "alt2", "alt3" });
            
            var goalTest = model.Goals()
                .ShallContain (x => x.Identifier == "test")
                    .ShallBeSingle ();
            
            goalTest.Refinements().Select (x => x.SystemReference()).Select (x => x.Identifier)
                .ShallOnlyContain (new string[] { "alt1" , "alt2" });
            
            var goalTest2 = model.Goals()
                .ShallContain (x => x.Identifier == "test2")
                    .ShallBeSingle ();
            
            goalTest2.Refinements().Select (x => x.SystemReference()).Select (x => x.Identifier)
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

            model.AlternativeSystems().Count().ShallEqual (3);
            model.RootSystems().Count.ShallEqual (1);

            var root = model.RootSystems().Single ();
            root.Alternatives.Select (a => a.Identifier).ShallOnlyContain (new string[] { "sys1", "sys2" });
        }
    }

}

