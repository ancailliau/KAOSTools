using System;
using System.Linq;
using NUnit.Framework;
using KAOSFormalTools.Parsing;
using LtlSharp;
using ShallTests;

namespace KAOSFormalTools.Parsing.Tests
{
    [TestFixture()]
    public class TestParsingDomainProperty
    {
        private static Parser parser = new Parser ();
        
        [TestCase(@"begin domainproperty
                        id test
                    end", "test")]
        [TestCase(@"begin domainproperty
                        id _test
                    end", "_test")]
        [TestCase(@"begin domainproperty
                        id -test
                    end", "-test")]
        [TestCase(@"begin domainproperty
                        id $test
                    end", "$test")]
        [TestCase(@"begin domainproperty
                        id test_long_identifier
                    end", "test_long_identifier")]
        [TestCase(@"begin domainproperty
                        id test-long-identifier
                    end", "test-long-identifier")]
        [TestCase(@"begin domainproperty
                        id test12
                    end", "test12")]
        [TestCase(@"begin domainproperty
                        id 0
                    end", "0")]
        [TestCase(@"begin domainproperty
                        id test2
                        id test
                    end", "test")]
        [TestCase(@"begin domainproperty
                        id test
                        id test
                    end", "test")]
        public void TestIdentifier (string input, string expectedIdentifier)
        {
            var model = parser.Parse (input);
            model.GoalModel.DomainProperties.Where (x => x.Identifier == expectedIdentifier).ShallBeSingle ();
        }
        
        [TestCase(@"begin domainproperty id   end")]
        [TestCase(@"begin domainproperty id - end")]
        [TestCase(@"begin domainproperty id _ end")]
        [TestCase(@"begin domainproperty id $ end")]
        public void TestInvalidIdentifier (string input)
        {
            Assert.Throws<ParsingException> (() => {
                parser.Parse (input);
            });
        }
        
        [TestCase(@"begin domainproperty
                        name ""test""
                    end", "test")]
        [TestCase(@"begin domainproperty
                        name ""Long name with spaces and numbers 123""
                    end", "Long name with spaces and numbers 123")]
        [TestCase(@"begin domainproperty
                        name ""[-_-]""
                    end", "[-_-]")]
        public void TestName (string input, string expectedName)
        {
            var model = parser.Parse (input);
            model.GoalModel.DomainProperties
                .Where (x => x.Name == expectedName)
                    .ShallBeSingle ();
        }
        
        [TestCase(@"begin domainproperty
                        name """"
                    end")]
        [TestCase(@"begin domainproperty
                        name """"""
                    end")]
        public void TestInvalidName (string input)
        {
            Assert.Throws<ParsingException> (() => {
                parser.Parse (input);
            });
        }        
        
        [Test()]
        public void TestFormalSpec ()
        {
            var input = @"begin domainproperty
                              id          test
                              formalspec  ""paf""
                          end";
            
            var model = parser.Parse (input);
            var test = model.GoalModel.DomainProperties.Where (x => x.Identifier == "test").ShallBeSingle ();
            Assert.IsNotNull (test.FormalSpec);
        }
                
        [TestCase(@"begin domprop
                        id test
                        name ""old name""
                        definition ""old definition""
                        formalspec ""old""
                    end

                    begin domprop
                        id test
                        name ""new name""
                        definition ""new definition""
                        formalspec ""new""
                    end")]
        [TestCase(@"begin domprop
                        id test
                    end

                    begin domprop
                        id test
                        name ""old name""
                        definition ""old definition""
                        formalspec ""old""
                    end")]
        public void TestMerge (string input)
        {
            var model = parser.Parse (input);
            
            var domprop = model.GoalModel.DomainProperties.Where (x => x.Identifier == "test").ShallBeSingle ();
            domprop.Name.ShallEqual ("old name");
            domprop.Definition.ShallEqual ("old definition");
            domprop.FormalSpec.ShallBeSuchThat (x => (x as LtlSharp.Proposition).Name == "old");
        }
        
        [TestCase(@"begin domainproperty id test  end
                    begin domainproperty id test2 end")]
        [TestCase(@"begin domprop id test  end
                    begin domprop id test2 end")]
        public void TestMultiple (string input)
        {
            var model = parser.Parse (input);
            model.GoalModel.DomainProperties.ShallContain (x => x.Identifier == "test");
            model.GoalModel.DomainProperties.ShallContain (x => x.Identifier == "test2");
        }

        [TestCase(@"begin domprop id test probability 0.95 end", 0.95)]
        [TestCase(@"begin domprop id test probability 1    end", 1)]
        [TestCase(@"begin domprop id test probability 0    end", 0)]
        [TestCase(@"begin domprop id test probability .01  end", .01)]
        [TestCase(@"begin domprop id test eps 0.95 end", 0.95)]
        [TestCase(@"begin domprop id test eps 1    end", 1)]
        [TestCase(@"begin domprop id test eps 0    end", 0)]
        [TestCase(@"begin domprop id test eps .01  end", .01)]
        public void TestProbability (string input, double expected)
        {
            var model = parser.Parse (input);
            model.GoalModel.DomainProperties.ShallContain (x => x.Identifier == "test").ShallBeSingle ().EPS.ShallEqual (expected);
        }

    }

}

