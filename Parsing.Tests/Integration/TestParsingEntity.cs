using System;
using System.Linq;
using NUnit.Framework;
using KAOSTools.Core;
using KAOSTools.Parsing;
using KAOSTools.Parsing.Parsers;

namespace UCLouvain.KAOSTools.Parsing.Tests
{
    [TestFixture()]
    public class TestParsingEntity
    {
        private static ModelBuilder parser = new ModelBuilder ();

        [TestCase(@"declare entity [ test ]
                    end", "test")]
        [TestCase(@"declare entity [ test_long_identifier ]
                    end", "test_long_identifier")]
        [TestCase(@"declare entity [ test-long-identifier ]
                    end", "test-long-identifier")]
        [TestCase(@"declare entity [ test12 ]
                    end", "test12")]
        public void TestIdentifier (string input, string identifier)
        {
            var model = parser.Parse (input);
            model.Entities()
                .Where (x => x.Identifier == identifier)
                .ShallBeSingle ();
        }

        [TestCase(@"declare object [ test ]
                    end", "test")]
        public void TestObject (string input, string identifier)
        {
            var model = parser.Parse (input);
            model.Entities()
                .Where (x => x.Identifier == identifier)
                    .ShallBeSingle ();
        }
        
        [TestCase(@"declare entity [] end")]
		[TestCase(@"declare entity [-] end")]
		[TestCase(@"declare entity [_] end")]
		[TestCase(@"declare entity [end] end")]
        public void TestInvalidIdentifier (string input)
        {
            Assert.Throws<ParserException> (() => {
                parser.Parse (input);
            });
        }

        [TestCase(@"declare entity [ test ]
                        name ""test""
                    end", "test")]
		[TestCase(@"declare entity [ test ]
                        name ""Long name with spaces and numbers 123""
                    end", "Long name with spaces and numbers 123")]
		[TestCase(@"declare entity [ test ]
                        name ""[-_-]""
                    end", "[-_-]")]
        public void TestName (string input, string expectedName)
        {
            var model = parser.Parse (input);
            model.Entities()
                .Where (x => x.Name == expectedName)
                .ShallBeSingle ();
        }

		[TestCase(@"declare entity [ test ]
                        name """"""
                    end")]
        public void TestInvalidName (string input)
        {
            Assert.Throws<ParserException> (() => {
                parser.Parse (input);
            });
        }

        [TestCase(@"declare entity [ test ]
                        definition ""My description""
                    end", "My description")]
        [TestCase(@"declare entity [ test ]
                        definition """"
                    end", "")]
        [TestCase(@"declare entity [ test ]
                        definition ""multi
                                     line""
                    end", "multi line")]
        public void TestDefinition (string input, string expectedDescription)
        {
            var model = parser.Parse (input);
            model.Entities()
                .Where (x => x.Identifier == "test")
                .ShallBeSuchThat (x => x.Definition == expectedDescription);
        }

		[TestCase(@"declare entity [ test ]
                        definition "" "" "" 
                    end")]
        public void TestInvalidDescription (string input)
        {
            Assert.Throws<ParserException> (() => {
                parser.Parse (input);
            });
        }

        [TestCase(@"declare entity [ test ]
                        attribute my_attribute
                    end", new string[] { "my_attribute" })]
        [TestCase(@"declare entity [ test ]
                        attribute my_attribute:my_type
                    end", new string[] { "my_attribute" })]
        [TestCase(@"declare entity [ test ]
                        attribute my_attribute1 : my_type1
                        attribute my_attribute2 : my_type1
                    end", new string[] { "my_attribute1", "my_attribute2" })]
        public void TestAttribute (string input, string[] attributes)
        {
            var model = parser.Parse (input);
            var entity = model.Entities().Single (x => x.Identifier == "test");
            entity.Attributes().Select (x => x.Identifier).ShallOnlyContain (attributes);
        }

        [TestCase(@"declare entity [ test ]
                        isa test2
                    end
                    declare entity [ test2 ] end", false)]
		[TestCase(@"declare entity [ test ]
                        isa test2
                    end", true)]
        public void TestIsA (string input, bool implicitParent)
        {
            var model = parser.Parse (input);
            var entity1 = model.Entities().Single (x => x.Identifier == "test");
            entity1.ParentIdentifiers.ShallOnlyContain (new string[] { "test2" });

            var parentEntity = model.Entities().Single (x => x.Identifier == "test2");
            parentEntity.Implicit.ShallEqual (implicitParent);
        }

        [TestCase(@"declare entity [ test ]
                        type software
                    end", EntityType.Software)]
        [TestCase(@"declare entity [ test ]
                        type environment
                    end", EntityType.Environment)]
        public void TestEntityType (string input, EntityType type)
        {
            var model = parser.Parse (input);
            var entity = model.Entities().Single (x => x.Identifier == "test");
            entity.Type.ShallEqual (type);
        }

        [TestCase(@"declare entity [ test ] $my_attribute ""my_value"" end",
                  "my_attribute", "my_value")]
        public void TestCustomAttribute(string input, string key, string value)
        {
            var model = parser.Parse(input);
            var v = model.Entities()
                .Where(x => x.Identifier == "test")
                .ShallBeSingle();
            v.CustomData.Keys.ShallContain(key);
            v.CustomData[key].ShallEqual(value);
        }
    }
}

