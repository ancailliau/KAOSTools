using System;
using System.Linq;
using NUnit.Framework;
using ShallTests;
using KAOSTools.MetaModel;

namespace KAOSTools.Parsing.Tests
{
    [TestFixture()]
    public class TestParsingEntity
    {
        private static ModelBuilder parser = new ModelBuilder ();

        [TestCase(@"declare entity
                        id test
                    end", "test")]
        [TestCase(@"declare entity
                        id _test
                    end", "_test")]
        [TestCase(@"declare entity
                        id -test
                    end", "-test")]
        [TestCase(@"declare entity
                        id $test
                    end", "$test")]
        [TestCase(@"declare entity
                        id test_long_identifier
                    end", "test_long_identifier")]
        [TestCase(@"declare entity
                        id test-long-identifier
                    end", "test-long-identifier")]
        [TestCase(@"declare entity
                        id test12
                    end", "test12")]
        [TestCase(@"declare entity
                        id 0
                    end", "0")]
        public void TestIdentifier (string input, string identifier)
        {
            var model = parser.Parse (input);
            model.Entities
                .Where (x => x.Identifier == identifier)
                .ShallBeSingle ();
        }

        [TestCase(@"declare object
                        id 0
                    end", "0")]
        public void TestObject (string input, string identifier)
        {
            var model = parser.Parse (input);
            model.Entities
                .Where (x => x.Identifier == identifier)
                    .ShallBeSingle ();
        }
        
        [TestCase(@"declare entity
                        id 
                    end")]
        [TestCase(@"declare entity
                        id -
                    end")]
        [TestCase(@"declare entity
                        id _
                    end")]
        [TestCase(@"declare entity
                        id $
                    end")]
        public void TestInvalidIdentifier (string input)
        {
            Assert.Throws<CompilationException> (() => {
                parser.Parse (input);
            });
        }

        [TestCase(@"declare entity
                        name ""test""
                    end", "test")]
        [TestCase(@"declare entity
                        name ""Long name with spaces and numbers 123""
                    end", "Long name with spaces and numbers 123")]
        [TestCase(@"declare entity
                        name ""[-_-]""
                    end", "[-_-]")]
        public void TestName (string input, string expectedName)
        {
            var model = parser.Parse (input);
            model.Entities
                .Where (x => x.Name == expectedName)
                .ShallBeSingle ();
        }
        
        [TestCase(@"declare entity
                        name """"
                    end")]
        [TestCase(@"declare entity
                        name """"""
                    end")]
        public void TestInvalidName (string input)
        {
            Assert.Throws<CompilationException> (() => {
                parser.Parse (input);
            });
        }

        [TestCase(@"declare entity
                        id test
                        definition ""My description""
                    end", "My description")]
        [TestCase(@"declare entity
                        id test
                        definition """"
                    end", "")]
        [TestCase(@"declare entity
                        id test
                        definition ""multi
                                     line""
                    end", "multi line")]
        public void TestDefinition (string input, string expectedDescription)
        {
            var model = parser.Parse (input);
            model.Entities
                .Where (x => x.Identifier == "test")
                .ShallBeSuchThat (x => x.Definition == expectedDescription);
        }

        [TestCase(@"declare entity
                        definition "" "" "" 
                    end")]
        public void TestInvalidDescription (string input)
        {
            Assert.Throws<CompilationException> (() => {
                parser.Parse (input);
            });
        }

        [TestCase(@"declare entity
                        id test
                        attribute ""My attribute""
                    end", new string[] { "My attribute" })]
        [TestCase(@"declare entity
                        id test
                        attribute ""My attribute"" : ""My Type""
                    end", new string[] { "My attribute" })]
        [TestCase(@"declare entity
                        id test
                        attribute ""My attribute 1"" : ""My Type""
                        attribute ""My attribute 2"" : ""My Type""
                    end", new string[] { "My attribute 1", "My attribute 2" })]
        [TestCase(@"declare entity
                        id test
                        attribute ""My attribute 1""
                        attribute ""My attribute 2""
                    end", new string[] { "My attribute 1", "My attribute 2" })]
        [TestCase(@"declare entity
                        id test
                        attribute ""test""
                        attribute ""test""
                    end", new string[] { "test", "test" })]
        public void TestAttribute (string input, string[] attributes)
        {
            var model = parser.Parse (input);
            var entity = model.Entities.Single (x => x.Identifier == "test");
            entity.Attributes.Select (x => x.Name).ShallOnlyContain (attributes);
        }
        
        [TestCase(@"declare entity
                        id test
                        attribute """" : ""My Type""
                    end")]
        public void TestInvalidAttribute (string input)
        {
            Assert.Throws<CompilationException> (() => {
                parser.Parse (input);
            });
        }

        [TestCase(@"declare entity
                        id test
                        is test2
                    end", true)]
        [TestCase(@"declare entity
                        id test
                        is ""Entity 2""
                    end

                    declare entity
                        id test2
                        name ""Entity 2""
                    end", false)]
        [TestCase(@"declare entity
                        id test
                        is test2
                    end

                    declare entity
                        id test2
                    end", false)]
        public void TestIsA (string input, bool implicitParent)
        {
            var model = parser.Parse (input);
            var entity1 = model.Entities.Single (x => x.Identifier == "test");
            entity1.Parents.Select (x => x.Identifier).ShallOnlyContain (new string[] { "test2" });

            var parentEntity = model.Entities.Single (x => x.Identifier == "test2");
            parentEntity.Implicit.ShallEqual (implicitParent);
        }

        [TestCase(@"declare entity
                        id test
                        type software
                    end", EntityType.Software)]
        [TestCase(@"declare entity
                        id test
                        type environment
                    end", EntityType.Environment)]
        public void TestEntityType (string input, EntityType type)
        {
            var model = parser.Parse (input);
            var entity = model.Entities.Single (x => x.Identifier == "test");
            entity.Type.ShallEqual (type);
        }
    }
}

