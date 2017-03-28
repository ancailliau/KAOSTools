using System;
using System.Linq;
using NUnit.Framework;
using KAOSTools.Core;
using KAOSTools.Parsing;
using KAOSTools.Parsing.Parsers;

namespace UCLouvain.KAOSTools.Parsing.Tests
{
    [TestFixture()]
    public class TestParsingRelation
    {
        private static ModelBuilder parser = new ModelBuilder ();

        [TestCase(@"declare relation [ test ]
                    end", "test")]
        [TestCase(@"declare relation [ test_long_identifier ]
                    end", "test_long_identifier")]
        [TestCase(@"declare relation [ test-long-identifier ]
                    end", "test-long-identifier")]
        [TestCase(@"declare relation [ test12 ]
                    end", "test12")]
        public void TestIdentifier (string input, string identifier)
        {
            var model = parser.Parse (input);
            model.Relations()
                .Where (x => x.Identifier == identifier)
                .ShallBeSingle ();
        }

        [TestCase(@"declare association[ test ]
                    end", "test")]
        public void TestAssociation (string input, string identifier)
        {
            var model = parser.Parse (input);
            model.Relations()
                .Where (x => x.Identifier == identifier)
                    .ShallBeSingle ();
        }

		[TestCase(@"declare relation [] end")]
		[TestCase(@"declare relation [-] end")]
		[TestCase(@"declare relation [_] end")]
		[TestCase(@"declare relation [$] end")]
        public void TestInvalidIdentifier (string input)
        {
            Assert.Throws<ParserException> (() => {
                parser.Parse (input);
            });
        }

        [TestCase(@"declare relation [ test ]
                        name ""test""
                    end", "test")]
		[TestCase(@"declare relation [ test ]
                        name ""Long name with spaces and numbers 123""
                    end", "Long name with spaces and numbers 123")]
		[TestCase(@"declare relation [ test ]
                        name ""[-_-]""
                    end", "[-_-]")]
        public void TestName (string input, string expectedName)
        {
            var model = parser.Parse (input);
            model.Relations()
                .Where (x => x.Name == expectedName)
                .ShallBeSingle ();
        }
        
        [TestCase(@"declare relation [ test ]
                        name """"""
                    end")]
        public void TestInvalidName (string input)
        {
            Assert.Throws<ParserException> (() => {
                parser.Parse (input);
            });
        }

        [TestCase(@"declare relation[ test ]
                        definition ""My description""
                    end", "My description")]
        [TestCase(@"declare relation[ test ]
                        definition """"
                    end", "")]
        [TestCase(@"declare relation[ test ]
                        definition ""multi
                                     line""
                    end", "multi line")]
        public void TestDefinition (string input, string expectedDescription)
        {
            var model = parser.Parse (input);
            model.Relations()
                .Where (x => x.Identifier == "test")
                .ShallBeSuchThat (x => x.Definition == expectedDescription);
        }

		[TestCase(@"declare relation [ test ]
                        definition "" "" "" 
                    end")]
        public void TestInvalidDescription (string input)
        {
            Assert.Throws<ParserException> (() => {
                parser.Parse (input);
            });
        }

        [TestCase(@"declare relation[ test ]
                        attribute my_attribute
                    end", new string[] { "my_attribute" })]
        [TestCase(@"declare relation[ test ]
                        attribute my_attribute : my_type
                    end
                    declare type [ my_type ] end", new string[] { "my_attribute" })]
        [TestCase(@"declare relation[ test ]
                        attribute my_attribute1
                        attribute my_attribute2
                    end
                    declare type [ my_type ] end", new string[] { "my_attribute1", "my_attribute2" })]
		[TestCase(@"declare relation[ test ]
                        attribute my_attribute1 : my_type
                        attribute my_attribute2 : my_type
                    end
                    declare type [ my_type ] end", new string[] { "my_attribute1", "my_attribute2" })]
        public void TestAttribute (string input, string[] attributes)
        {
            var model = parser.Parse (input);
            var relation = model.Relations().Single (x => x.Identifier == "test");
            relation.Attributes().Select (x => x.Identifier).ShallOnlyContain (attributes);
        }

        [TestCase(@"declare relation[ test ]
                        link entity1
                        link entity2
                    end")]
        [TestCase(@"declare relation[ test ]
                        link entity1
                        link entity2
                    end
                    declare entity [ entity1 ] end
                    declare entity [ entity2 ] end")]
        public void TestLink (string input)
        {
            var model = parser.Parse (input);
            var relation1 = model.Relations().Single (x => x.Identifier == "test");

            relation1.Links.Select (x => x.Target.Identifier).ShallOnlyContain (new string[] { "entity1", "entity2" });
        }
    }
}

