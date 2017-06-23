using System;
using System.Linq;
using NUnit.Framework;
using KAOSTools.Core;
using KAOSTools.Parsing;
using KAOSTools.Parsing.Parsers;

namespace UCLouvain.KAOSTools.Parsing.Tests
{
    [TestFixture()]
    public class TestParsingPredicate
    {
        private static ModelBuilder parser = new ModelBuilder ();

        
        [TestCase(@"declare predicate [ test ]
                    end", "test")]
        [TestCase(@"declare predicate [ test_long_identifier ]
                    end", "test_long_identifier")]
        [TestCase(@"declare predicate [ test-long-identifier ]
                    end", "test-long-identifier")]
        [TestCase(@"declare predicate [ test12 ]
                    end", "test12")]
        public void TestIdentifier (string input, string expectedIdentifier)
        {
            var model = parser.Parse (input);
            model.Predicates().Where (x => x.Identifier == expectedIdentifier).ShallBeSingle ();
		}

		[TestCase(@"declare predicate [] end")]
		[TestCase(@"declare predicate [-] end")]
		[TestCase(@"declare predicate [_] end")]
		[TestCase(@"declare predicate [$] end")]
        public void TestInvalidIdentifier (string input)
        {
            Assert.Throws<ParserException> (() => {
                parser.Parse (input);
            });
        }
        
        [TestCase(@"declare predicate [ test ]
                        name ""test""
                    end", "test")]
		[TestCase(@"declare predicate [ test ]
                        name ""Long name with spaces and numbers 123""
                    end", "Long name with spaces and numbers 123")]
		[TestCase(@"declare predicate [ test ]
                        name ""[-_-]""
                    end", "[-_-]")]
        public void TestName (string input, string expectedName)
        {
            var model = parser.Parse (input);
            model.Predicates().Where (x => x.Name == expectedName)
                    .ShallBeSingle ();
        }

		[TestCase(@"declare predicate [ test ]
                        name """"""
                    end")]
        public void TestInvalidName (string input)
        {
            Assert.Throws<ParserException> (() => {
                parser.Parse (input);
            });
        }

        [TestCase(@"declare predicate [ test ]
                        definition ""test""
                    end", "test")]
        [TestCase(@"declare predicate [ test ]
                        definition """"
                    end", "")]
        [TestCase(@"declare predicate [ test ]
                        definition ""on multiple
                                     lines.""
                    end", "on multiple lines.")]
        [TestCase("declare predicate [ test ] definition \"with a \"\"quote\"\" !\" end", "with a \"quote\" !")]
        public void TestDefinition (string input, string expectedDefinition)
        {
            var model = parser.Parse (input);
            var g = model.Predicates().Single (x => x.Identifier == "test");
            g.Definition.ShallEqual (expectedDefinition);
        }

        [TestCase(@"declare predicate [ test ]
                        name ""old name""
                        definition ""old definition""
                    end

                    override predicate [ test ]
                        name ""new name""
                        definition ""new definition""
                    end")]
		[TestCase(@"declare predicate [ test ]
                    end

                    override predicate  [ test ]
                        name ""old name""
                    end

                    override predicate [ test ]
                        name ""new name""
                        definition ""new definition""
                    end")]
        public void TestMerge (string input)
        {
            var model = parser.Parse (input);
            
            var predicate = model.Predicates().Where (x => x.Identifier == "test").ShallBeSingle ();
            predicate.Name.ShallEqual ("new name");
            predicate.Definition.ShallEqual ("new definition");
        }

		[TestCase(@"declare predicate [ test ]
                        name ""Test""
                        argument c: mytype
                    end

                    declare object [ mytype ]
                        name ""MyType""
                    end")]
		[TestCase(@"declare predicate [ test ]
                        name ""Test""
                        argument c: mytype
                    end")]
        public void TestArgument (string input)
        {
            var model = parser.Parse (input);
            var predicate = model.Predicates().Single (x => x.Name == "Test");

            predicate.Arguments.Select (x => x.Name).ShallOnlyContain (new string[] { "c" });
			predicate.Arguments.Select(x => x.Type).Select(x => x.Identifier).ShallOnlyContain(new string[] { "mytype" });

			model.Entities().Select(x => x.Identifier).ShallOnlyContain(new string[] { "mytype" });
        }
        
        [TestCase(@"declare predicate [ test ]
                        name ""Test""
                        argument c: my_type
                        formalspec c.my_attribute
                    end
                    declare entity [ my_type ] end")]
        public void TestFormalSpec (string input)
        {
            var model = parser.Parse (input);
            var predicate = model.Predicates().Single (x => x.Name == "Test");
            
            var entity = model.Entities().Single(x => x.Identifier == "my_type");
            var attribute = entity.Attributes().Single(x => x.Identifier == "my_attribute");

            ((AttributeReference) predicate.FormalSpec).Variable.ShallEqual ("c");
            ((AttributeReference) predicate.FormalSpec).Entity.ShallEqual (entity.Identifier);
            ((AttributeReference) predicate.FormalSpec).Attribute.ShallEqual (attribute.Identifier);
        }

        [TestCase(@"declare predicate [ test ] $my_attribute ""my_value"" end",
                  "my_attribute", "my_value")]
        public void TestCustomAttribute(string input, string key, string value)
        {
            var model = parser.Parse(input);
            var v = model.Predicates()
                .Where(x => x.Identifier == "test")
                .ShallBeSingle();
            v.CustomData.Keys.ShallContain(key);
            v.CustomData[key].ShallEqual(value);
        }

        [TestCase(@"declare predicate [ test ] default true end", true)]
        [TestCase(@"declare predicate [ test ] default false end", false)]
        public void TestDefaultAttribute(string input, bool expected)
        {
            var model = parser.Parse(input);
            var v = model.Predicates()
                .Where(x => x.Identifier == "test")
                .ShallBeSingle();
            v.DefaultValue.ShallEqual(expected);
        }
    }
}

