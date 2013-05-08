using System;
using System.Linq;
using NUnit.Framework;
using KAOSTools.Parsing;
using LtlSharp;
using ShallTests;
using KAOSTools.MetaModel;

namespace KAOSTools.Parsing.Tests
{
    [TestFixture()]
    public class TestParsingDomainProperty
    {
        private static ModelBuilder parser = new ModelBuilder ();
        
        [TestCase(@"declare domainproperty
                        id test
                    end", "test")]
        [TestCase(@"declare domainproperty
                        id _test
                    end", "_test")]
        [TestCase(@"declare domainproperty
                        id -test
                    end", "-test")]
        [TestCase(@"declare domainproperty
                        id $test
                    end", "$test")]
        [TestCase(@"declare domainproperty
                        id test_long_identifier
                    end", "test_long_identifier")]
        [TestCase(@"declare domainproperty
                        id test-long-identifier
                    end", "test-long-identifier")]
        [TestCase(@"declare domainproperty
                        id test12
                    end", "test12")]
        [TestCase(@"declare domainproperty
                        id 0
                    end", "0")]
        public void TestIdentifier (string input, string expectedIdentifier)
        {
            var model = parser.Parse (input);
            model.GoalModel.DomainProperties.Where (x => x.Identifier == expectedIdentifier).ShallBeSingle ();
        }
        
        [TestCase(@"declare domainproperty id   end")]
        [TestCase(@"declare domainproperty id - end")]
        [TestCase(@"declare domainproperty id _ end")]
        [TestCase(@"declare domainproperty id $ end")]
        public void TestInvalidIdentifier (string input)
        {
            Assert.Throws<CompilationException> (() => {
                parser.Parse (input);
            });
        }
        
        [TestCase(@"declare domainproperty
                        name ""test""
                    end", "test")]
        [TestCase(@"declare domainproperty
                        name ""Long name with spaces and numbers 123""
                    end", "Long name with spaces and numbers 123")]
        [TestCase(@"declare domainproperty
                        name ""[-_-]""
                    end", "[-_-]")]
        public void TestName (string input, string expectedName)
        {
            var model = parser.Parse (input);
            model.GoalModel.DomainProperties
                .Where (x => x.Name == expectedName)
                    .ShallBeSingle ();
        }

        [TestCase(@"declare domainproperty
                        name """"""
                    end")]
        public void TestInvalidName (string input)
        {
            Assert.Throws<CompilationException> (() => {
                parser.Parse (input);
            });
        }        
        
        [Test()]
        public void TestFormalSpec ()
        {
            var input = @"declare domainproperty
                              id          test
                              formalspec  ""paf""
                          end";
            
            var model = parser.Parse (input);
            var test = model.GoalModel.DomainProperties.Where (x => x.Identifier == "test").ShallBeSingle ();
            Assert.IsNotNull (test.FormalSpec);
        }
                
        [TestCase(@"declare domprop
                        id test
                        name ""old name""
                        definition ""old definition""
                    end

                    override domprop
                        id test
                        name ""new name""
                        definition ""new definition""
                        formalspec ""new""
                    end")]
        [TestCase(@"declare domprop
                        id test
                    end

                    override domprop
                        id test
                        name ""new name""
                        definition ""new definition""
                        formalspec ""new""
                    end")]
        public void TestMerge (string input)
        {
            var model = parser.Parse (input);
            
            var domprop = model.GoalModel.DomainProperties.Where (x => x.Identifier == "test").ShallBeSingle ();
            domprop.Name.ShallEqual ("new name");
            domprop.Definition.ShallEqual ("new definition");
            ((PredicateReference) domprop.FormalSpec).Predicate.Signature.ShallEqual ("new");
        }
        
        [TestCase(@"declare domainproperty id test  end
                    declare domainproperty id test2 end")]
        [TestCase(@"declare domprop id test  end
                    declare domprop id test2 end")]
        public void TestMultiple (string input)
        {
            var model = parser.Parse (input);
            model.GoalModel.DomainProperties.ShallContain (x => x.Identifier == "test");
            model.GoalModel.DomainProperties.ShallContain (x => x.Identifier == "test2");
        }

        [TestCase(@"declare domprop id test probability 0.95 end", 0.95)]
        [TestCase(@"declare domprop id test probability 1    end", 1)]
        [TestCase(@"declare domprop id test probability 0    end", 0)]
        [TestCase(@"declare domprop id test probability .01  end", .01)]
        [TestCase(@"declare domprop id test eps 0.95 end", 0.95)]
        [TestCase(@"declare domprop id test eps 1    end", 1)]
        [TestCase(@"declare domprop id test eps 0    end", 0)]
        [TestCase(@"declare domprop id test eps .01  end", .01)]
        public void TestProbability (string input, double expected)
        {
            var model = parser.Parse (input);
            model.GoalModel.DomainProperties.ShallContain (x => x.Identifier == "test").ShallBeSingle ().EPS.ShallEqual (expected);
        }

    }

}

