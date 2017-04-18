using System;
using KAOSTools.Parsing.Parsers;
using KAOSTools.Parsing.Parsers.Attributes;
using NUnit.Framework;
using UCLouvain.KAOSTools.Parsing.Parsers.Exceptions;
using System.Linq;

namespace UCLouvain.KAOSTools.Parsing.Tests.Unit.Attributes
{
    [TestFixture]
    public class TestArgumentAttributeParser
    {

        [Test]
        public void TestInvalidLeftColonValue ()
        {
            ParsedElement left = new NameExpression ("left");
            ParsedElement right = new IdentifierExpression ("right");
            TestInvalidColonValueHelper (left, right);
        }

        [Test]
        public void TestInvalidRightColonValue ()
        {
            ParsedElement left = new IdentifierExpression ("left");
            ParsedElement right = new NameExpression ("right");
            TestInvalidColonValueHelper (left, right);
        }

        [Test]
        public void TestInvalidColonValues ()
        {
            ParsedElement left = new NameExpression ("left");
            ParsedElement right = new NameExpression ("right");
            TestInvalidColonValueHelper (left, right);
        }

        private static void TestInvalidColonValueHelper (ParsedElement left, ParsedElement right)
        {
            var ap = new ArgumentAttributeParser ();
            const string attIdentifier = "argument";

            var e = Assert.Catch (() => {
                ap.ParsedAttribute (attIdentifier,
                                    null,
                                    new NParsedAttributeColon (left, right));
            });

            Assert.IsInstanceOf (typeof (InvalidAttributeValueException), e);
            StringAssert.AreEqualIgnoringCase (
                string.Format (InvalidAttributeValueException.IDENTIFIER, attIdentifier),
                e.Message
            );
        }

        [Test]
        public void TestNullParameter ()
        {
            var ap = new ArgumentAttributeParser ();
            const string attIdentifier = "argument";

            var e = Assert.Catch(() => {
                ap.ParsedAttribute(attIdentifier,
                                   new NParsedAttributeAtomic(new ParsedFloat()), 
                                   new NParsedAttributeAtomic(new IdentifierExpression("software")));
            });

            Assert.IsInstanceOf(typeof(InvalidParameterAttributeException), e);
            StringAssert.AreEqualIgnoringCase(
                string.Format(InvalidParameterAttributeException.NO_PARAM, attIdentifier),
                e.Message
            );
        }
 
        [Test]
        public void TestAtomicValue ()
        {
            TestNotColonValue (new NParsedAttributeAtomic ());
        }

        [Test]
        public void TestBracketValue ()
        {
            TestNotColonValue (new NParsedAttributeBracket ());
        }

        [Test]
        public void TestListValue ()
        {
            TestNotColonValue (new NParsedAttributeList (Enumerable.Empty<ParsedElement> ()));
        }

        public void TestNotColonValue (NParsedAttributeValue v)
        {
            var ap = new ArgumentAttributeParser ();
            const string attIdentifier = "argument";

            var e = Assert.Catch (() => {
                ap.ParsedAttribute (attIdentifier,
                                   null,
                                   v);
            });

            Assert.IsInstanceOf (typeof (InvalidAttributeValueException), e);
            StringAssert.AreEqualIgnoringCase (
                string.Format (InvalidAttributeValueException.COLON_ONLY, attIdentifier),
                e.Message
            );
        }
    }
}
