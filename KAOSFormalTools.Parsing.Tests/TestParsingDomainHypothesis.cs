using System;
using System.Linq;
using NUnit.Framework;
using KAOSFormalTools.Parsing;
using LtlSharp;
using ShallTests;

namespace KAOSFormalTools.Parsing.Tests
{
    [TestFixture()]
    public class TestParsingDomainHypothesis
    {
        private static Parser parser = new Parser ();
        
        [TestCase(@"begin domainhypothesis
                        id test
                    end", "test")]
        [TestCase(@"begin domainhypothesis
                        id _test
                    end", "_test")]
        [TestCase(@"begin domainhypothesis
                        id -test
                    end", "-test")]
        [TestCase(@"begin domainhypothesis
                        id $test
                    end", "$test")]
        [TestCase(@"begin domainhypothesis
                        id test_long_identifier
                    end", "test_long_identifier")]
        [TestCase(@"begin domainhypothesis
                        id test-long-identifier
                    end", "test-long-identifier")]
        [TestCase(@"begin domainhypothesis
                        id test12
                    end", "test12")]
        [TestCase(@"begin domainhypothesis
                        id 0
                    end", "0")]
        [TestCase(@"begin domainhypothesis
                        id test2
                        id test
                    end", "test")]
        [TestCase(@"begin domainhypothesis
                        id test
                        id test
                    end", "test")]
        public void TestIdentifier (string input, string expectedIdentifier)
        {
            var model = parser.Parse (input);
            model.GoalModel.DomainHypotheses.Where (x => x.Identifier == expectedIdentifier).ShallBeSingle ();
        }
        
        [TestCase(@"begin domainhypothesis id   end")]
        [TestCase(@"begin domainhypothesis id - end")]
        [TestCase(@"begin domainhypothesis id _ end")]
        [TestCase(@"begin domainhypothesis id $ end")]
        public void TestInvalidIdentifier (string input)
        {
            Assert.Throws<ParsingException> (() => {
                parser.Parse (input);
            });
        }
        
        [TestCase(@"begin domainhypothesis
                        name ""test""
                    end", "test")]
        [TestCase(@"begin domainhypothesis
                        name ""Long name with spaces and numbers 123""
                    end", "Long name with spaces and numbers 123")]
        [TestCase(@"begin domainhypothesis
                        name ""[-_-]""
                    end", "[-_-]")]
        public void TestName (string input, string expectedName)
        {
            var model = parser.Parse (input);
            model.GoalModel.DomainHypotheses
                .Where (x => x.Name == expectedName)
                    .ShallBeSingle ();
        }
        
        [TestCase(@"begin domainhypothesis
                        name """"
                    end")]
        [TestCase(@"begin domainhypothesis
                        name """"""
                    end")]
        public void TestInvalidName (string input)
        {
            Assert.Throws<ParsingException> (() => {
                parser.Parse (input);
            });
        }        
            
        [TestCase(@"begin domhyp
                        id test
                        name ""old name""
                        definition ""old definition""
                    end

                    begin domhyp
                        id test
                        name ""new name""
                        definition ""new definition""
                    end")]
        [TestCase(@"begin domhyp
                        id test
                    end

                    begin domhyp
                        id test
                        name ""old name""
                        definition ""old definition""
                    end")]
        public void TestMerge (string input)
        {
            var model = parser.Parse (input);
            
            var domhyp = model.GoalModel.DomainHypotheses.Where (x => x.Identifier == "test").ShallBeSingle ();
            domhyp.Name.ShallEqual ("old name");
            domhyp.Definition.ShallEqual ("old definition");
        }
        
        [TestCase(@"begin domainhypothesis id test  end
                    begin domainhypothesis id test2 end")]
        [TestCase(@"begin domhyp id test  end
                    begin domhyp id test2 end")]
        public void TestMultiple (string input)
        {
            var model = parser.Parse (input);
            model.GoalModel.DomainHypotheses.ShallContain (x => x.Identifier == "test");
            model.GoalModel.DomainHypotheses.ShallContain (x => x.Identifier == "test2");
        }

        [TestCase(@"begin goal id g refinedby test end
                    begin domhyp id test end")]
        [TestCase(@"begin goal id g refinedby begin domhyp id test end end")]
        public void TestRefinementWithDomainHypothesis (string input)
        {
            var model = parser.Parse (input);
            var g = model.GoalModel.GetGoalByIdentifier ("g");
            Console.WriteLine (g.Refinements.Count);
            g.Refinements.ShallBeSingle ().DomainHypotheses.Select (x => x.Identifier).ShallOnlyContain (new string[] { "test" });
        }

    }

}

