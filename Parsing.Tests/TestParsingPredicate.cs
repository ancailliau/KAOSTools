using System;
using System.Linq;
using NUnit.Framework;
using ShallTests;
using KAOSTools.MetaModel;

namespace KAOSTools.Parsing.Tests
{
    [TestFixture()]
    public class TestParsingPredicate
    {
        private static ModelBuilder parser = new ModelBuilder ();

        
        [TestCase(@"declare predicate
                        id test
                    end", "test")]
        [TestCase(@"declare predicate
                        id _test
                    end", "_test")]
        [TestCase(@"declare predicate
                        id -test
                    end", "-test")]
        [TestCase(@"declare predicate
                        id $test
                    end", "$test")]
        [TestCase(@"declare predicate
                        id test_long_identifier
                    end", "test_long_identifier")]
        [TestCase(@"declare predicate
                        id test-long-identifier
                    end", "test-long-identifier")]
        [TestCase(@"declare predicate
                        id test12
                    end", "test12")]
        [TestCase(@"declare predicate
                        id 0
                    end", "0")]
        public void TestIdentifier (string input, string expectedIdentifier)
        {
            var model = parser.Parse (input);
            model.Predicates.Where (x => x.Identifier == expectedIdentifier).ShallBeSingle ();
        }
        
        [TestCase(@"declare predicate
                        id 
                    end")]
        [TestCase(@"declare predicate
                        id -
                    end")]
        [TestCase(@"declare predicate
                        id _
                    end")]
        [TestCase(@"declare predicate
                        id $
                    end")]
        public void TestInvalidIdentifier (string input)
        {
            Assert.Throws<CompilationException> (() => {
                parser.Parse (input);
            });
        }
        
        [TestCase(@"declare predicate
                        name ""test""
                    end", "test")]
        [TestCase(@"declare predicate
                        name ""Long name with spaces and numbers 123""
                    end", "Long name with spaces and numbers 123")]
        [TestCase(@"declare predicate
                        name ""[-_-]""
                    end", "[-_-]")]
        public void TestName (string input, string expectedName)
        {
            var model = parser.Parse (input);
            model.Predicates.Where (x => x.Name == expectedName)
                    .ShallBeSingle ();
        }

        [TestCase(@"declare predicate
                        name """"""
                    end")]
        public void TestInvalidName (string input)
        {
            Assert.Throws<CompilationException> (() => {
                parser.Parse (input);
            });
        }
        
        [TestCase(@"declare predicate
                        id test
                        name ""old name""
                        definition ""old definition""
                        formalspec ""old""
                    end

                    override predicate
                        id test
                        name ""new name""
                        definition ""new definition""
                        formalspec ""new""
                    end")]
        [TestCase(@"declare predicate
                        id test
                    end

                    override predicate 
                        id test
                        name ""old name""
                        formalspec ""old""
                    end

                    override predicate
                        id test
                        name ""new name""
                        definition ""new definition""
                        formalspec ""new""
                    end")]
        public void TestMerge (string input)
        {
            var model = parser.Parse (input);
            
            var predicate = model.Predicates.Where (x => x.Identifier == "test").ShallBeSingle ();
            predicate.Name.ShallEqual ("new name");
            predicate.Definition.ShallEqual ("new definition");
            ((PredicateReference) predicate.FormalSpec).Predicate.Signature.ShallEqual ("new");
        }

        [TestCase(@"declare predicate
                        name ""Test""
                        argument ""c"": ""MyType""
                    end")]
        [TestCase(@"declare predicate
                        name ""Test""
                        argument ""c"": mytype
                    end

                    declare object
                        id mytype
                        name ""MyType""
                    end")]
        [TestCase(@"declare predicate
                        name ""Test""
                        argument ""c"": declare object id myType name ""MyType"" end
                    end")]
        public void TestArgument (string input)
        {
            var model = parser.Parse (input);
            var predicate = model.Predicates.Single (x => x.Name == "Test");

            predicate.Arguments.Select (x => x.Name).ShallOnlyContain (new string[] { "c" });
            predicate.Arguments.Select (x => x.Type).Select (x => x.Name).ShallOnlyContain (new string[] { "MyType" });

            model.Entities.Select (x => x.Name).ShallOnlyContain (new string[] { "MyType" });
        }
        
        [TestCase(@"declare predicate
                        name ""Test""
                        argument ""c"": ""MyType""
                        formalspec ""c.MyAttribute""
                    end")]
        public void TestFormalSpec (string input)
        {
            var model = parser.Parse (input);
            var predicate = model.Predicates.Single (x => x.Name == "Test");
            
            var entity = model.Entities.Single (x => x.Name == "MyType");
            var attribute = entity.Attributes.Single (x => x.Name == "MyAttribute");

            ((AttributeReference) predicate.FormalSpec).Variable.ShallEqual ("c");
            ((AttributeReference) predicate.FormalSpec).Entity.ShallEqual (entity);
            ((AttributeReference) predicate.FormalSpec).Attribute.ShallEqual (attribute);
        }

        [TestCase(@"declare predicate
                        id test
                        signature ""mypredicate""
                    end", "mypredicate")]
        public void TestSignature (string input, string signature)
        {
            var model = parser.Parse (input);
            var predicate = model.Predicates.Single (x => x.Identifier == "test");
            predicate.Signature.ShallEqual (signature);
        }
    }
}

