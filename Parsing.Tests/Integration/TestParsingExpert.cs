using System;
using System.Linq;
using NUnit.Framework;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Parsing;
using UCLouvain.KAOSTools.Parsing.Parsers;

namespace UCLouvain.KAOSTools.Parsing.Tests
{
    [TestFixture()]
    public class TestParsingExpert
    {
        private static ModelBuilder parser = new ModelBuilder ();

        
        [TestCase(@"declare expert [ test ]
                    end", "test")]
        [TestCase(@"declare expert [ test_long_identifier ]
                    end", "test_long_identifier")]
        [TestCase(@"declare expert [ test-long-identifier ]
                    end", "test-long-identifier")]
        [TestCase(@"declare expert [ test12 ]
                    end", "test12")]
        public void TestIdentifier (string input, string expectedIdentifier)
        {
            var model = parser.Parse (input);
            model.Experts().Where (x => x.Identifier == expectedIdentifier).ShallBeSingle ();
		}

		[TestCase(@"declare expert [] end")]
		[TestCase(@"declare expert [-] end")]
		[TestCase(@"declare expert [_] end")]
		[TestCase(@"declare expert [$] end")]
        public void TestInvalidIdentifier (string input)
        {
            Assert.Throws<ParserException> (() => {
                parser.Parse (input);
            });
        }
        
        [TestCase(@"declare expert [ test ]
                        name ""test""
                    end", "test")]
		[TestCase(@"declare expert [ test ]
                        name ""Long name with spaces and numbers 123""
                    end", "Long name with spaces and numbers 123")]
		[TestCase(@"declare expert [ test ]
                        name ""[-_-]""
                    end", "[-_-]")]
        public void TestName (string input, string expectedName)
        {
            var model = parser.Parse (input);
            model.Experts().Where (x => x.Name == expectedName)
                    .ShallBeSingle ();
        }

		[TestCase(@"declare expert [ test ]
                        name """"""
                    end")]
        public void TestInvalidName (string input)
        {
            Assert.Throws<ParserException> (() => {
                parser.Parse (input);
            });
        }

        [TestCase(@"declare expert [ test ] $my_attribute ""my_value"" end",
                  "my_attribute", "my_value")]
        public void TestCustomAttribute(string input, string key, string value)
        {
            var model = parser.Parse(input);
            var v = model.Experts()
                .Where(x => x.Identifier == "test")
                .ShallBeSingle();
            v.CustomData.Keys.ShallContain(key);
            v.CustomData[key].ShallEqual(value);
        }
    }
}

