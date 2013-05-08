using System;
using System.Linq;
using NUnit.Framework;
using ShallTests;

namespace KAOSTools.Parsing.Tests
{
    [TestFixture()]
    public class TestParsingRelation
    {
        private static ModelBuilder parser = new ModelBuilder ();

        [TestCase(@"declare relation
                        id test
                    end", "test")]
        [TestCase(@"declare relation
                        id _test
                    end", "_test")]
        [TestCase(@"declare relation
                        id -test
                    end", "-test")]
        [TestCase(@"declare relation
                        id $test
                    end", "$test")]
        [TestCase(@"declare relation
                        id test_long_identifier
                    end", "test_long_identifier")]
        [TestCase(@"declare relation
                        id test-long-identifier
                    end", "test-long-identifier")]
        [TestCase(@"declare relation
                        id test12
                    end", "test12")]
        [TestCase(@"declare relation
                        id 0
                    end", "0")]
        public void TestIdentifier (string input, string identifier)
        {
            var model = parser.Parse (input);
            model.Relations
                .Where (x => x.Identifier == identifier)
                .ShallBeSingle ();
        }

        [TestCase(@"declare association
                        id 0
                    end", "0")]
        public void TestAssociation (string input, string identifier)
        {
            var model = parser.Parse (input);
            model.Relations
                .Where (x => x.Identifier == identifier)
                    .ShallBeSingle ();
        }
        
        [TestCase(@"declare relation
                        id 
                    end")]
        [TestCase(@"declare relation
                        id -
                    end")]
        [TestCase(@"declare relation
                        id _
                    end")]
        [TestCase(@"declare relation
                        id $
                    end")]
        public void TestInvalidIdentifier (string input)
        {
            Assert.Throws<CompilationException> (() => {
                parser.Parse (input);
            });
        }

        [TestCase(@"declare relation
                        name ""test""
                    end", "test")]
        [TestCase(@"declare relation
                        name ""Long name with spaces and numbers 123""
                    end", "Long name with spaces and numbers 123")]
        [TestCase(@"declare relation
                        name ""[-_-]""
                    end", "[-_-]")]
        public void TestName (string input, string expectedName)
        {
            var model = parser.Parse (input);
            model.Relations
                .Where (x => x.Name == expectedName)
                .ShallBeSingle ();
        }
        
        [TestCase(@"declare relation
                        name """"
                    end")]
        [TestCase(@"declare relation
                        name """"""
                    end")]
        public void TestInvalidName (string input)
        {
            Assert.Throws<CompilationException> (() => {
                parser.Parse (input);
            });
        }

        [TestCase(@"declare relation
                        id test
                        definition ""My description""
                    end", "My description")]
        [TestCase(@"declare relation
                        id test
                        definition """"
                    end", "")]
        [TestCase(@"declare relation
                        id test
                        definition ""multi
                                     line""
                    end", "multi line")]
        public void TestDefinition (string input, string expectedDescription)
        {
            var model = parser.Parse (input);
            model.Relations
                .Where (x => x.Identifier == "test")
                .ShallBeSuchThat (x => x.Definition == expectedDescription);
        }

        [TestCase(@"declare relation
                        definition "" "" "" 
                    end")]
        public void TestInvalidDescription (string input)
        {
            Assert.Throws<CompilationException> (() => {
                parser.Parse (input);
            });
        }

        [TestCase(@"declare relation
                        id test
                        attribute ""My attribute""
                    end", new string[] { "My attribute" })]
        [TestCase(@"declare relation
                        id test
                        attribute ""My attribute"" : ""My Type""
                    end", new string[] { "My attribute" })]
        [TestCase(@"declare relation
                        id test
                        attribute ""My attribute 1"" : ""My Type""
                        attribute ""My attribute 2"" : ""My Type""
                    end", new string[] { "My attribute 1", "My attribute 2" })]
        [TestCase(@"declare relation
                        id test
                        attribute ""My attribute 1""
                        attribute ""My attribute 2""
                    end", new string[] { "My attribute 1", "My attribute 2" })]
        [TestCase(@"declare relation
                        id test
                        attribute ""test""
                        attribute ""test""
                    end", new string[] { "test", "test" })]
        public void TestAttribute (string input, string[] attributes)
        {
            var model = parser.Parse (input);
            var relation = model.Relations.Single (x => x.Identifier == "test");
            relation.Attributes.Select (x => x.Name).ShallOnlyContain (attributes);
        }
        
        [TestCase(@"declare relation
                        id test
                        attribute """" : ""My Type""
                    end")]
        public void TestInvalidAttribute (string input)
        {
            Assert.Throws<CompilationException> (() => {
                parser.Parse (input);
            });
        }

        [TestCase(@"declare relation
                        id test
                        link entity1
                        link entity2
                    end")]
        [TestCase(@"declare relation
                        id test
                        link ""Entity 1""
                        link ""Entity 2""
                    end

                    declare entity name ""Entity 1"" id entity1 end
                    declare entity name ""Entity 2"" id entity2 end")]
        public void TestLink (string input)
        {
            var model = parser.Parse (input);
            var relation1 = model.Relations.Single (x => x.Identifier == "test");

            relation1.Links.Select (x => x.Target.Identifier).ShallOnlyContain (new string[] { "entity1", "entity2" });
        }
    }
}

