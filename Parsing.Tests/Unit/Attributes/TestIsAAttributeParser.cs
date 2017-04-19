using System;
using KAOSTools.Parsing.Parsers;
using KAOSTools.Parsing.Parsers.Attributes;
using NUnit.Framework;
using UCLouvain.KAOSTools.Parsing.Parsers.Exceptions;
using System.Linq;

namespace UCLouvain.KAOSTools.Parsing.Tests.Unit.Attributes
{
    [TestFixture]
    public class TestIsAAttributeParser
    {
        [Test]
        public void TestNullParameter ()
        {
            var ap = new IsAAttributeParser ();
            const string attIdentifier = "isa";

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
        public void TestInvalidAtomicValue ()
        {
            var ap = new IsAAttributeParser ();
            const string attIdentifier = "isa";

            var e = Assert.Catch (() => {
                ap.ParsedAttribute (attIdentifier,
                                    null,
                                    new NParsedAttributeAtomic(new NameExpression("")));
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
            var ap = new IsAAttributeParser ();
            const string attIdentifier = "isa";

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
    }
}
