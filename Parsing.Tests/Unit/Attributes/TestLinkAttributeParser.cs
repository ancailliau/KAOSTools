using System;
using KAOSTools.Parsing.Parsers;
using KAOSTools.Parsing.Parsers.Attributes;
using NUnit.Framework;
using UCLouvain.KAOSTools.Parsing.Parsers.Exceptions;
using System.Linq;

namespace UCLouvain.KAOSTools.Parsing.Tests.Unit.Attributes
{
    [TestFixture]
    public class TestLinkAttributeParser
    {
        #region Test parameter

        [TestCase (-1)]
        [TestCase (Int16.MinValue)]
        public void TestParseMultiplicityInteger (int v)
        {
            const string attIdentifier = "link";

            var e = Assert.Catch (() => {
                LinkAttributeParser.ParseMultiplicity (attIdentifier, new NParsedAttributeAtomic (new ParsedInteger (v)));
            });

            Assert.IsInstanceOf (typeof (InvalidParameterAttributeException), e);
            StringAssert.AreEqualIgnoringCase (
                string.Format (InvalidParameterAttributeException.INVALID_VALUE, attIdentifier),
                e.Message
            );
        }

        [TestCase ("")]
        [TestCase ("    ")]
        public void TestParseMultiplicityString (string v)
        {
            const string attIdentifier = "link";

            var e = Assert.Catch (() => {
                LinkAttributeParser.ParseMultiplicity (attIdentifier, new NParsedAttributeAtomic (new IdentifierExpression (v)));
            });

            Assert.IsInstanceOf (typeof (InvalidParameterAttributeException), e);
            StringAssert.AreEqualIgnoringCase (
                string.Format (InvalidParameterAttributeException.INVALID_VALUE, attIdentifier),
                e.Message
            );
        }

        [Test ()]
        public void TestParseInvalidSingleMultiplicityName ()
        {
            const string attIdentifier = "link";

            var e = Assert.Catch (() => {
                LinkAttributeParser.ParseMultiplicity (attIdentifier, new NParsedAttributeAtomic (new NameExpression ("test")));
            });

            Assert.IsInstanceOf (typeof (InvalidParameterAttributeException), e);
            StringAssert.AreEqualIgnoringCase (
                string.Format (InvalidParameterAttributeException.INVALID_VALUE, attIdentifier),
                e.Message
            );
        }

        [Test ()]
        public void TestParseInvalidSingleMultiplicityNotAtomic ()
        {
            const string attIdentifier = "link";

            var e = Assert.Catch (() => {
                LinkAttributeParser.ParseMultiplicity (attIdentifier, (ParsedElement) new NParsedAttributeColon ());
            });

            Assert.IsInstanceOf (typeof (InvalidParameterAttributeException), e);
            StringAssert.AreEqualIgnoringCase (
                string.Format (InvalidParameterAttributeException.ATOMIC_ONLY, attIdentifier),
                e.Message
            );
        }

        [Test ()]
        public void TestParseInvalidNumberOfParameters ()
        {
            var ap = new LinkAttributeParser ();
            const string attIdentifier = "link";

            var e = Assert.Catch (() => {
                ap.ParsedAttribute (attIdentifier,
                                    new NParsedAttributeList (new [] { 
                                        new IdentifierExpression ("M"), 
                                        new IdentifierExpression ("M"), 
                    new IdentifierExpression ("M") }),
                                                              new NParsedAttributeAtomic(new IdentifierExpression("test")));
            });

            Assert.IsInstanceOf (typeof (InvalidParameterAttributeException), e);
            StringAssert.AreEqualIgnoringCase (
                string.Format (InvalidParameterAttributeException.INVALID_VALUE, attIdentifier),
                e.Message
            );
        }

        [Test ()]
        public void TestParseInvalidParameters ()
        {
            var ap = new LinkAttributeParser ();
            const string attIdentifier = "link";

            var e = Assert.Catch (() => {
                ap.ParsedAttribute (attIdentifier,
                                    new NParsedAttributeColon (),
                                   new NParsedAttributeAtomic (new IdentifierExpression ("test")));
            });

            Assert.IsInstanceOf (typeof (InvalidParameterAttributeException), e);
            StringAssert.AreEqualIgnoringCase (
                string.Format (InvalidParameterAttributeException.ATOMIC_OR_LIST, attIdentifier),
                e.Message
            );
        }

        #endregion

        #region Test value

        [Test]
        public void TestInvalidAtomicValue ()
        {
            var ap = new LinkAttributeParser ();
            const string attIdentifier = "link";

            var e = Assert.Catch (() => {
                ap.ParsedAttribute (attIdentifier,
                                    null,
                                    new NParsedAttributeAtomic (new NameExpression ("")));
            });

            Assert.IsInstanceOf (typeof (InvalidAttributeValueException), e);
            StringAssert.AreEqualIgnoringCase (
                string.Format (InvalidAttributeValueException.IDENTIFIER, attIdentifier),
                e.Message
            );
        }

        [Test]
        public void TestColonValue ()
        {
            TestNotAtomicValue (new NParsedAttributeColon ());
        }

        [Test]
        public void TestBracketValue ()
        {
            TestNotAtomicValue (new NParsedAttributeBracket ());
        }

        [Test]
        public void TestListValue ()
        {
            TestNotAtomicValue (new NParsedAttributeList (Enumerable.Empty<ParsedElement> ()));
        }

        public void TestNotAtomicValue (NParsedAttributeValue v)
        {
            var ap = new LinkAttributeParser ();
            const string attIdentifier = "link";

            var e = Assert.Catch (() => {
                ap.ParsedAttribute (attIdentifier,
                                   null,
                                   v);
            });

            Assert.IsInstanceOf (typeof (InvalidAttributeValueException), e);
            StringAssert.AreEqualIgnoringCase (
                string.Format (InvalidAttributeValueException.ATOMIC_ONLY, attIdentifier),
                e.Message
            );
        }

        #endregion

    }
}
