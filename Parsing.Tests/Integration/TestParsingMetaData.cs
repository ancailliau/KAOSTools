using System;
using System.Linq;
using NUnit.Framework;
using KAOSTools.Parsing;
using KAOSTools.Core;
using KAOSTools.Parsing.Parsers;

namespace UCLouvain.KAOSTools.Parsing.Tests
{
    [TestFixture()]
    public class TestParsingMetaData
    {
        private static ModelBuilder parser = new ModelBuilder ();

        [TestCase(@"@author ""Antoine Cailliau""", "Antoine Cailliau")]
        [TestCase(@"@author ""antoine.cailliau@uclouvain.be""", "antoine.cailliau@uclouvain.be")]
        [TestCase(@"@author ""ancailliau""", "ancailliau")]
        [TestCase(@"@author "" """, " ")]
        [TestCase(@"@author ""$*""", "$*")]
        public void TestAuthor (string input, string expectedAuthor)
        {
            var model = parser.Parse (input);
            model.Author.ShallEqual(expectedAuthor);
        }

        [TestCase(@"@title ""My Model""", "My Model")]
        public void TestTitle(string input, string expectedAuthor)
        {
            var model = parser.Parse(input);
            model.Title.ShallEqual(expectedAuthor);
        }

        [TestCase(@"@version ""V1""", "V1")]
        [TestCase(@"@version ""0.1""", "0.1")]
        [TestCase(@"@version ""0.0.1-beta""", "0.0.1-beta")]
        public void TestVersion(string input, string expectedAuthor)
        {
            var model = parser.Parse(input);
            model.Version.ShallEqual(expectedAuthor);
        }

        [TestCase(@"@author """"")]
        [TestCase(@"@author """"""")]
        [TestCase(@"@author ancailliau")]
        public void TestAuthorFail(string input)
        {
            Assert.Catch(() => {
                parser.Parse(input);
            });
        }

        [TestCase(@"@title """"")]
        [TestCase(@"@title """"""")]
        [TestCase(@"@title my_title")]
        [TestCase(@"@title ""My Model""
                    @title ""My Model""")]
        public void TestTitleFail(string input)
        {
            Assert.Catch(() => {
                parser.Parse(input);
            });
        }

        [TestCase(@"@version """"")]
        [TestCase(@"@version """"""")]
        [TestCase(@"@version 0.1")]
        public void TestVersionFail(string input)
        {
            Assert.Catch(() => {
                parser.Parse(input);
            });
        }

        [TestCase(@"@my_attribute ""My value""", "my_attribute", "My value")]
        [TestCase(@"@my.attribute ""My value""", "my.attribute", "My value")]
        [TestCase(@"@my-attribute ""My value""", "my-attribute", "My value")]
        [TestCase(@"@my\\attribute ""My value""", @"my\\attribute", "My value")]
        public void TestModelAttribute(string input, string expectedKey, string expectedValue)
        {
            var model = parser.Parse(input);
            model.Parameters[expectedKey].ShallEqual(expectedValue);
        }

        [TestCase(@"@ ""My value""")]
        [TestCase(@"@$ ""My value""")]
        public void TestFailModelAttribute(string input)
        {
            Assert.Catch(() => {
                var model = parser.Parse(input);
            });
        }

        [TestCase(@"@author ""Antoine Cailliau""
                    @author ""Antoine PasCailliau""")]
        public void TestDuplicateModelAttribute(string input)
        {
            var e = Assert.Catch(() => {
                parser.Parse(input);
            });

            Assert.IsInstanceOf(typeof(BuilderException), e, "Exception is not BuilderException.");
        }
    }

}

