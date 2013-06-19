using System;
using System.Linq;
using NUnit.Framework;
using KAOSTools.Parsing;
using ShallTests;
using KAOSTools.MetaModel;

namespace KAOSTools.Parsing.Tests
{
    [TestFixture()]
    public class TestParsingDomainHypothesis
    {
        private static ModelBuilder parser = new ModelBuilder ();
        
        [TestCase(@"declare domainhypothesis
                        id test
                    end", "test")]
        [TestCase(@"declare domainhypothesis
                        id test_long_identifier
                    end", "test_long_identifier")]
        [TestCase(@"declare domainhypothesis
                        id test-long-identifier
                    end", "test-long-identifier")]
        [TestCase(@"declare domainhypothesis
                        id test12
                    end", "test12")]
        public void TestIdentifier (string input, string expectedIdentifier)
        {
            var model = parser.Parse (input);
            model.DomainHypotheses.Where (x => x.Identifier == expectedIdentifier).ShallBeSingle ();
        }
        
        [TestCase(@"declare domainhypothesis id   end")]
        [TestCase(@"declare domainhypothesis id - end")]
        [TestCase(@"declare domainhypothesis id _ end")]
        [TestCase(@"declare domainhypothesis id $ end")]
        public void TestInvalidIdentifier (string input)
        {
            Assert.Throws<ParserException> (() => {
                parser.Parse (input);
            });
        }
        
        [TestCase(@"declare domainhypothesis
                        name ""test""
                    end", "test")]
        [TestCase(@"declare domainhypothesis
                        name ""Long name with spaces and numbers 123""
                    end", "Long name with spaces and numbers 123")]
        [TestCase(@"declare domainhypothesis
                        name ""[-_-]""
                    end", "[-_-]")]
        public void TestName (string input, string expectedName)
        {
            var model = parser.Parse (input);
            model.DomainHypotheses
                .Where (x => x.Name == expectedName)
                    .ShallBeSingle ();
        }

        [TestCase(@"declare domainhypothesis
                        name """"""
                    end")]
        public void TestInvalidName (string input)
        {
            Assert.Throws<ParserException> (() => {
                parser.Parse (input);
            });
        }        
            
        [TestCase(@"declare domhyp
                        id test
                        name ""old name""
                        definition ""old definition""
                    end

                    override domhyp
                        id test
                        name ""new name""
                        definition ""new definition""
                    end")]
        [TestCase(@"declare domhyp
                        id test
                    end

                    override domhyp
                        id test
                        name ""new name""
                        definition ""new definition""
                    end")]
        public void TestMerge (string input)
        {
            var model = parser.Parse (input);
            
            var domhyp = model.DomainHypotheses.Where (x => x.Identifier == "test").ShallBeSingle ();
            domhyp.Name.ShallEqual ("new name");
            domhyp.Definition.ShallEqual ("new definition");
        }
        
        [TestCase(@"declare domainhypothesis id test  end
                    declare domainhypothesis id test2 end")]
        [TestCase(@"declare domhyp id test  end
                    declare domhyp id test2 end")]
        public void TestMultiple (string input)
        {
            var model = parser.Parse (input);
            model.DomainHypotheses.ShallContain (x => x.Identifier == "test");
            model.DomainHypotheses.ShallContain (x => x.Identifier == "test2");
        }

        [TestCase(@"declare goal id g refinedby test end
                    declare domhyp id test end")]
        [TestCase(@"declare goal id g refinedby declare domhyp id test end end")]
        public void TestRefinementWithDomainHypothesis (string input)
        {
            var model = parser.Parse (input);
            var g = model.Goals.Single (x => x.Identifier == "g");
            g.Refinements().ShallBeSingle ().DomainHypotheses.Select (x => x.Identifier).ShallOnlyContain (new string[] { "test" });
        }

    }

}

