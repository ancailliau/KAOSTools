using System;
using System.Linq;
using NUnit.Framework;
using KAOSTools.Parsing;
using KAOSTools.Core;
using KAOSTools.Parsing.Parsers;

namespace UCLouvain.KAOSTools.Parsing.Tests
{
    [TestFixture()]
    public class TestParsingDomainHypothesis
    {
        private static ModelBuilder parser = new ModelBuilder ();
        
        [TestCase(@"declare domainhypothesis [ test ]
                    end", "test")]
		[TestCase(@"declare domainhypothesis [ test_long_identifier ]
                    end", "test_long_identifier")]
		[TestCase(@"declare domainhypothesis [ test-long-identifier ]
                    end", "test-long-identifier")]
		[TestCase(@"declare domainhypothesis [ test12 ]
                    end", "test12")]
        public void TestIdentifier (string input, string expectedIdentifier)
        {
            var model = parser.Parse (input);
            model.DomainHypotheses().Where (x => x.Identifier == expectedIdentifier).ShallBeSingle ();
        }
        
        [TestCase(@"declare domainhypothesis [] end")]
		[TestCase(@"declare domainhypothesis [-] end")]
		[TestCase(@"declare domainhypothesis [_] end")]
		[TestCase(@"declare domainhypothesis [end] end")]
        public void TestInvalidIdentifier (string input)
        {
            Assert.Throws<ParserException> (() => {
                parser.Parse (input);
            });
        }
        
        [TestCase(@"declare domainhypothesis [ test ]
                        name ""test""
                    end", "test")]
		[TestCase(@"declare domainhypothesis [ test ]
                        name ""Long name with spaces and numbers 123""
                    end", "Long name with spaces and numbers 123")]
		[TestCase(@"declare domainhypothesis [ test ]
                        name ""[-_-]""
                    end", "[-_-]")]
        public void TestName (string input, string expectedName)
        {
            var model = parser.Parse (input);
            model.DomainHypotheses()
                .Where (x => x.Name == expectedName)
                    .ShallBeSingle ();
        }

		[TestCase(@"declare domainhypothesis [ test ]
                        name """"""
                    end")]
        public void TestInvalidName (string input)
        {
            Assert.Throws<ParserException> (() => {
                parser.Parse (input);
            });
        }        
            
        [TestCase(@"declare domhyp [ test ]
                        name ""old name""
                        definition ""old definition""
                    end

                    override domhyp [ test ]
                        name ""new name""
                        definition ""new definition""
                    end")]
        [TestCase(@"declare domhyp [ test ]
                    end

                    override domhyp [ test ]
                        name ""new name""
                        definition ""new definition""
                    end")]
        public void TestMerge (string input)
        {
            var model = parser.Parse (input);
            
            var domhyp = model.DomainHypotheses().Where (x => x.Identifier == "test").ShallBeSingle ();
            domhyp.Name.ShallEqual ("new name");
            domhyp.Definition.ShallEqual ("new definition");
        }
        
        [TestCase(@"declare domainhypothesis [ test ] end
                    declare domainhypothesis [ test2 ] end")]
		[TestCase(@"declare domhyp [ test ] end
                    declare domhyp [ test2 ] end")]
        public void TestMultiple (string input)
        {
            var model = parser.Parse (input);
            model.DomainHypotheses().ShallContain (x => x.Identifier == "test");
            model.DomainHypotheses().ShallContain (x => x.Identifier == "test2");
        }

		[TestCase(@"declare goal [ g ] refinedby test end
                    declare domhyp [ test ] end")]
        public void TestRefinementWithDomainHypothesis (string input)
        {
            var model = parser.Parse (input);
            var g = model.Goals().Single (x => x.Identifier == "g");
            g.Refinements().ShallBeSingle ().DomainHypothesisIdentifiers.ShallOnlyContain (new string[] { "test" });
        }

    }

}

