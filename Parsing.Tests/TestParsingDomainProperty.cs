using System;
using System.Linq;
using NUnit.Framework;
using KAOSTools.Parsing;
using KAOSTools.Core;
using KAOSTools.Parsing.Parsers;
using UCLouvain.KAOSTools.Core.SatisfactionRates;

namespace UCLouvain.KAOSTools.Parsing.Tests
{
    [TestFixture()]
    public class TestParsingDomainProperty
    {
        private static ModelBuilder parser = new ModelBuilder ();
        
        [TestCase(@"declare domainproperty [ test ]
                    end", "test")]
		[TestCase(@"declare domainproperty [ test_long_identifier ]
                    end", "test_long_identifier")]
		[TestCase(@"declare domainproperty [ test-long-identifier ]
                    end", "test-long-identifier")]
        [TestCase(@"declare domainproperty [ test12 ]
                    end", "test12")]
        public void TestIdentifier (string input, string expectedIdentifier)
        {
            var model = parser.Parse (input);
            model.DomainProperties().Where (x => x.Identifier == expectedIdentifier).ShallBeSingle ();
        }
        
        [TestCase(@"declare domainproperty [] end")]
        [TestCase(@"declare domainproperty [-] end")]
        [TestCase(@"declare domainproperty [_] end")]
        [TestCase(@"declare domainproperty [end] end")]
        public void TestInvalidIdentifier (string input)
        {
            Assert.Throws<ParserException> (() => {
                parser.Parse (input);
            });
        }
        
        [TestCase(@"declare domainproperty [ test ]
                        name ""test""
                    end", "test")]
		[TestCase(@"declare domainproperty [ test ]
                        name ""Long name with spaces and numbers 123""
                    end", "Long name with spaces and numbers 123")]
		[TestCase(@"declare domainproperty [ test ]
                        name ""[-_-]""
                    end", "[-_-]")]
        public void TestName (string input, string expectedName)
        {
            var model = parser.Parse (input);
            model.DomainProperties()
                .Where (x => x.Name == expectedName)
                    .ShallBeSingle ();
        }

		[TestCase(@"declare domainproperty [ test ]
                        name """"""
                    end")]
        public void TestInvalidName (string input)
        {
            Assert.Throws<ParserException> (() => {
                parser.Parse (input);
            });
        }      
                
        [TestCase(@"declare domprop [ test ]
                        name ""old name""
                        definition ""old definition""
                    end

                    override domprop [ test ]
                        name ""new name""
                        definition ""new definition""
                    end")]
        [TestCase(@"declare domprop [ test ]
                    end

                    override domprop [ test ]
                        name ""new name""
                        definition ""new definition""
                    end")]
        public void TestMerge (string input)
        {
            var model = parser.Parse (input);
            
            var domprop = model.DomainProperties().Where (x => x.Identifier == "test").ShallBeSingle ();
            domprop.Name.ShallEqual ("new name");
            domprop.Definition.ShallEqual ("new definition");
        }
        
        [TestCase(@"declare domainproperty [ test ] end
                    declare domainproperty [ test2 ] end")]
		[TestCase(@"declare domprop [ test ] end
                    declare domprop [ test2 ] end")]
        public void TestMultiple (string input)
        {
            var model = parser.Parse (input);
            model.DomainProperties().ShallContain (x => x.Identifier == "test");
            model.DomainProperties().ShallContain (x => x.Identifier == "test2");
        }

        [TestCase(@"declare domprop [ test ] probability 0.95 end", 0.95)]
        [TestCase(@"declare domprop [ test ] probability 1    end", 1)]
        [TestCase(@"declare domprop [ test ] probability 0    end", 0)]
        [TestCase(@"declare domprop [ test ] probability .01  end", .01)]
        [TestCase(@"declare domprop [ test ] esr 0.95 end", 0.95)]
        [TestCase(@"declare domprop [ test ] esr 1    end", 1)]
        [TestCase(@"declare domprop [ test ] esr 0    end", 0)]
        [TestCase(@"declare domprop [ test ] esr .01  end", .01)]
        public void TestProbability (string input, double expected)
        {
			var model = parser.Parse(input);

            var sr = model.satisfactionRateRepository.GetDomPropSatisfactionRate("test");
			Assert.IsInstanceOf(typeof(DoubleSatisfactionRate), sr);
			Assert.AreEqual(expected, ((DoubleSatisfactionRate)sr).SatisfactionRate);
        }

    }

}

