using System;
using System.Linq;
using NUnit.Framework;
using ShallTests;

namespace KAOSTools.Parsing.Tests
{
    [TestFixture()]
    public class TestParsingType
    {
        private static ModelBuilder parser = new ModelBuilder ();

        [TestCase(@"declare type
                        id test
                    end", "test")]
        [TestCase(@"declare type
                        id test_long_identifier
                    end", "test_long_identifier")]
        [TestCase(@"declare type
                        id test-long-identifier
                    end", "test-long-identifier")]
        [TestCase(@"declare type
                        id test12
                    end", "test12")]
        public void TestIdentifier (string input, string identifier)
        {
            var model = parser.Parse (input);
            model.GivenTypes
                .Where (x => x.Identifier == identifier)
                .ShallBeSingle ();
        }
        
        [TestCase(@"declare type
                        id 
                    end")]
        [TestCase(@"declare type
                        id -
                    end")]
        [TestCase(@"declare type
                        id _
                    end")]
        [TestCase(@"declare type
                        id $
                    end")]
        public void TestInvalidIdentifier (string input)
        {
            Assert.Throws<ParserException> (() => {
                parser.Parse (input);
            });
        }

        [TestCase(@"declare type
                        name ""test""
                    end", "test")]
        [TestCase(@"declare type
                        name ""Long name with spaces and numbers 123""
                    end", "Long name with spaces and numbers 123")]
        [TestCase(@"declare type
                        name ""[-_-]""
                    end", "[-_-]")]
        public void TestName (string input, string expectedName)
        {
            var model = parser.Parse (input);
            model.GivenTypes
                .Where (x => x.Name == expectedName)
                .ShallBeSingle ();
        }

        [TestCase(@"declare type
                        name """"""
                    end")]
        public void TestInvalidName (string input)
        {
            Assert.Throws<ParserException> (() => {
                parser.Parse (input);
            });
        }

        [TestCase(@"declare type
                        id test
                        definition ""My description""
                    end", "My description")]
        [TestCase(@"declare type
                        id test
                        definition """"
                    end", "")]
        [TestCase(@"declare type
                        id test
                        definition ""multi
                                     line""
                    end", "multi line")]
        public void TestDefinition (string input, string expectedDescription)
        {
            var model = parser.Parse (input);
            model.GivenTypes
                .Where (x => x.Identifier == "test")
                .ShallBeSuchThat (x => x.Definition == expectedDescription);
        }

        [TestCase(@"declare type
                        definition "" "" "" 
                    end")]
        public void TestInvalidDescription (string input)
        {
            Assert.Throws<ParserException> (() => {
                parser.Parse (input);
            });
        }

        [TestCase(@"declare entity 
                        id test
                        attribute ""myagent"": type1
                    end")]
        public void TestImplicit (string input)
        {
            var model = parser.Parse (input);
            
            var agent = model.GivenTypes.Single (x => x.Identifier == "type1");
            agent.Implicit.ShallBeTrue ();
        }
    }
}

