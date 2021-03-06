﻿using System;
using System.Linq;
using NUnit.Framework;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Parsing;
using UCLouvain.KAOSTools.Parsing.Parsers;

namespace UCLouvain.KAOSTools.Parsing.Tests
{
    [TestFixture()]
    public class TestParsingType
    {
        private static ModelBuilder parser = new ModelBuilder ();

        [TestCase(@"declare type[ test ]
                    end", "test")]
		[TestCase(@"declare type[ test_long_identifier ]
                    end", "test_long_identifier")]
		[TestCase(@"declare type[ test-long-identifier ]
                    end", "test-long-identifier")]
        [TestCase(@"declare type[ test12 ]
                    end", "test12")]
        public void TestIdentifier (string input, string identifier)
        {
            var model = parser.Parse (input);
            model.GivenTypes()
                .Where (x => x.Identifier == identifier)
                .ShallBeSingle ();
        }


        [TestCase(@"declare type [] end")]
        [TestCase(@"declare type [-] end")]
        [TestCase(@"declare type [_] end")]
        [TestCase(@"declare type [$] end")]
        public void TestInvalidIdentifier (string input)
        {
            Assert.Throws<ParserException> (() => {
                parser.Parse (input);
            });
        }

		[TestCase(@"declare type [ test ]
                        name ""test""
                    end", "test")]
		[TestCase(@"declare type [ test ]
                        name ""Long name with spaces and numbers 123""
                    end", "Long name with spaces and numbers 123")]
		[TestCase(@"declare type [ test ]
                        name ""[-_-]""
                    end", "[-_-]")]
        public void TestName (string input, string expectedName)
        {
            var model = parser.Parse (input);
            model.GivenTypes()
                .Where (x => x.Name == expectedName)
                .ShallBeSingle ();
        }

		[TestCase(@"declare type [ test ]
                        name """"""
                    end")]
        public void TestInvalidName (string input)
        {
            Assert.Throws<ParserException> (() => {
                parser.Parse (input);
            });
        }

        [TestCase(@"declare type[ test ]
                        definition ""My description""
                    end", "My description")]
        [TestCase(@"declare type[ test ]
                        definition """"
                    end", "")]
        [TestCase(@"declare type[ test ]
                        definition ""multi
                                     line""
                    end", "multi line")]
        public void TestDefinition (string input, string expectedDescription)
        {
            var model = parser.Parse (input);
            model.GivenTypes()
                .Where (x => x.Identifier == "test")
                .ShallBeSuchThat (x => x.Definition == expectedDescription);
        }

		[TestCase(@"declare type [ test ]
                        definition "" "" "" 
                    end")]
        public void TestInvalidDescription (string input)
        {
            Assert.Throws<ParserException> (() => {
                parser.Parse (input);
            });
        }

        [TestCase(@"declare type [ test ] $my_attribute ""my_value"" end",
                  "my_attribute", "my_value")]
        public void TestCustomAttribute(string input, string key, string value)
        {
            var model = parser.Parse(input);
            var v = model.GivenTypes()
                .Where(x => x.Identifier == "test")
                .ShallBeSingle();
            v.CustomData.Keys.ShallContain(key);
            v.CustomData[key].ShallEqual(value);
        }
    }
}

