using System;
using KAOSTools.Parsing.Parsers;
using KAOSTools.Parsing.Parsers.Attributes;
using NUnit.Framework;
using UCLouvain.KAOSTools.Parsing.Parsers.Exceptions;
using System.Linq;

namespace UCLouvain.KAOSTools.Parsing.Tests.Unit.Attributes
{
    [TestFixture]
    public class TestAssignedToAttributeParser
    {
        [Test ()]
        public void TestNotAtomicList ()
        {
            var ap = new AssignedToAttributeParser ();
            const string attIdentifier = "assignedTo";

            var e = Assert.Catch (() => {
                ap.ParsedAttribute (attIdentifier,
                                   null,
                                    new NParsedAttributeList (new [] { new NParsedAttributeColon () }));
            });

            Assert.IsInstanceOf (typeof (InvalidAttributeValueException), e);
            StringAssert.AreEqualIgnoringCase (
                string.Format (InvalidAttributeValueException.ATOMIC_ONLY, attIdentifier),
                e.Message
            );
        }

        [Test ()]
        public void TestNotIdentifierList ()
        {
            var ap = new AssignedToAttributeParser ();
            const string attIdentifier = "assignedTo";

            var e = Assert.Catch (() => {
                ap.ParsedAttribute (attIdentifier,
                                   null,
                                    new NParsedAttributeList (new [] { new NParsedAttributeAtomic (new ParsedFloat ()) }));
            });

            Assert.IsInstanceOf (typeof (InvalidAttributeValueException), e);
            StringAssert.AreEqualIgnoringCase (
                string.Format (InvalidAttributeValueException.IDENTIFIER, attIdentifier),
                e.Message
            );
        }

        [Test ()]
        public void TestNotIdentifierValue ()
        {
            var ap = new AssignedToAttributeParser ();
            const string attIdentifier = "assignedTo";

            var e = Assert.Catch (() => {
                ap.ParsedAttribute (attIdentifier,
                                   null,
                                   new NParsedAttributeAtomic (new ParsedFloat ()));
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
            var ap = new AssignedToAttributeParser ();
            const string attIdentifier = "assignedTo";

            var e = Assert.Catch(() => {
                ap.ParsedAttribute(attIdentifier,
                                   new NParsedAttributeAtomic(new ParsedFloat()), 
                                   new NParsedAttributeAtomic());
            });

            Assert.IsInstanceOf(typeof(InvalidParameterAttributeException), e);
            StringAssert.AreEqualIgnoringCase(
                string.Format(InvalidParameterAttributeException.NO_PARAM, attIdentifier),
                e.Message
            );
        }
 
        [Test]
        public void TestColonValue ()
        {
            TestNotColonValue (new NParsedAttributeColon ());
        }

        [Test]
        public void TestBracketValue ()
        {
            TestNotColonValue (new NParsedAttributeBracket ());
        }

        public void TestNotColonValue (NParsedAttributeValue v)
        {
            var ap = new AssignedToAttributeParser ();
            const string attIdentifier = "assignedTo";

            var e = Assert.Catch (() => {
                ap.ParsedAttribute (attIdentifier,
                                   null,
                                   v);
            });

            Assert.IsInstanceOf (typeof (InvalidAttributeValueException), e);
            StringAssert.AreEqualIgnoringCase (
                string.Format (InvalidAttributeValueException.ATOMIC_OR_LIST, attIdentifier),
                e.Message
            );
        }
    }
}
