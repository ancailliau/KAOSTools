using System;
using System.Linq;
using NUnit.Framework;
using KAOSFormalTools.Parsing;
using LtlSharp;
using ShallTests;

namespace KAOSFormalTools.Parsing.Tests
{
    [TestFixture()]
    public class TestParsingAlternative
    {
        private static Parser parser = new Parser ();
        
        [TestCase(@"declare alternative
                        id test
                    end", "test")]
        [TestCase(@"declare alternative
                        id _test
                    end", "_test")]
        [TestCase(@"declare alternative
                        id -test
                    end", "-test")]
        [TestCase(@"declare alternative
                        id $test
                    end", "$test")]
        [TestCase(@"declare alternative
                        id test_long_identifier
                    end", "test_long_identifier")]
        [TestCase(@"declare alternative
                        id test-long-identifier
                    end", "test-long-identifier")]
        [TestCase(@"declare alternative
                        id test12
                    end", "test12")]
        [TestCase(@"declare alternative
                        id 0
                    end", "0")]
        [TestCase(@"declare alternative
                        id test2
                        id test
                    end", "test")]
        [TestCase(@"declare alternative
                        id test
                        id test
                    end", "test")]
        public void TestIdentifier (string input, string expectedIdentifier)
        {
            var model = parser.Parse (input);
            model.GoalModel.Alternatives.Where (x => x.Identifier == expectedIdentifier).ShallBeSingle ();
        }
        
        [TestCase(@"declare alternative id   end")]
        [TestCase(@"declare alternative id - end")]
        [TestCase(@"declare alternative id _ end")]
        [TestCase(@"declare alternative id $ end")]
        public void TestInvalidIdentifier (string input)
        {
            Assert.Throws<ParsingException> (() => {
                parser.Parse (input);
            });
        }
        
        [TestCase(@"declare alternative
                        name ""test""
                    end", "test")]
        [TestCase(@"declare alternative
                        name ""Long name with spaces and numbers 123""
                    end", "Long name with spaces and numbers 123")]
        [TestCase(@"declare alternative
                        name ""[-_-]""
                    end", "[-_-]")]
        public void TestName (string input, string expectedName)
        {
            var model = parser.Parse (input);
            model.GoalModel.Alternatives
                .Where (x => x.Name == expectedName)
                    .ShallBeSingle ();
        }
        
        [TestCase(@"declare alternative
                        name """"
                    end")]
        [TestCase(@"declare alternative
                        name """"""
                    end")]
        public void TestInvalidName (string input)
        {
            Assert.Throws<ParsingException> (() => {
                parser.Parse (input);
            });
        }
           
        [TestCase(@"declare alternative id test  end
                    declare alternative id test2 end")]
        public void TestMultiple (string input)
        {
            var model = parser.Parse (input);
            model.GoalModel.Alternatives.ShallContain (x => x.Identifier == "test");
            model.GoalModel.Alternatives.ShallContain (x => x.Identifier == "test2");
        }

        
        [TestCase(@"declare goal 
                        id test
                        refinedby[""Alternative 1""] child1, child2
                        refinedby[""Alternative 2""] child1, child2
                    end

                    declare goal
                        id test2
                        refinedby[""Alternative 3""] child3
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

                    declare alternative id alt1 name ""Alternative 1"" end
                    declare alternative id alt2 name ""Alternative 2"" end
                    declare alternative id alt3 name ""Alternative 3"" end")]
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
            
            model.GoalModel.Alternatives.Select (x => x.Identifier)
                .ShallOnlyContain (new string[] { "alt1" , "alt2", "alt3" });
            
            var goalTest = model.GoalModel.Goals
                .ShallContain (x => x.Identifier == "test")
                    .ShallBeSingle ();
            
            goalTest.Refinements.Select (x => x.AlternativeIdentifier).Select (x => x.Identifier)
                .ShallOnlyContain (new string[] { "alt1" , "alt2" });
            
            var goalTest2 = model.GoalModel.Goals
                .ShallContain (x => x.Identifier == "test2")
                    .ShallBeSingle ();
            
            goalTest2.Refinements.Select (x => x.AlternativeIdentifier).Select (x => x.Identifier)
                .ShallOnlyContain (new string[] { "alt3" });
        }
    }

}
