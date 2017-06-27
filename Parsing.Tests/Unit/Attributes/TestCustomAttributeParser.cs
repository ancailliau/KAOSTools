using System;
using UCLouvain.KAOSTools.Parsing.Parsers;
using UCLouvain.KAOSTools.Parsing.Parsers.Attributes;
using NUnit.Framework;
using UCLouvain.KAOSTools.Parsing.Parsers.Exceptions;
using System.Linq;

namespace UCLouvain.KAOSTools.Parsing.Tests.Unit.Attributes
{
    [TestFixture]
    public class TestCustomAttributeParser
    {
        [Test ()]
        public void TestValidValue ()
        {
            var ap = new CustomAttributeParser ();
            const string attIdentifier = "$custom";

            var e = ap.ParsedAttribute (attIdentifier, 
                                        null,
                                        new NParsedAttributeAtomic (new ParsedString ("test")));
            
            Assert.IsInstanceOf (typeof (ParsedCustomAttribute), e);
            var pa = (ParsedCustomAttribute)e;
            StringAssert.AreEqualIgnoringCase ("$custom", pa.Key);
            StringAssert.AreEqualIgnoringCase ("test", pa.Value);
        }

        [Test ()]
        public void TestNotIdentifierValue ()
        {
            var ap = new CustomAttributeParser ();
            const string attIdentifier = "$custom";

            var e = Assert.Catch (() => {
                ap.ParsedAttribute (attIdentifier,
                                   null,
                                   new NParsedAttributeAtomic (new ParsedFloat ()));
            });

            Assert.IsInstanceOf (typeof (InvalidAttributeValueException), e);
            StringAssert.AreEqualIgnoringCase (
                string.Format (InvalidAttributeValueException.STRING, attIdentifier),
                e.Message
            );
        }

        [Test]
        public void TestNullParameter ()
        {
            var ap = new CustomAttributeParser ();
            const string attIdentifier = "$custom";

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
        public void TestColonValue ()
        {
            TestNotColonValue (new NParsedAttributeColon ());
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
            var ap = new CustomAttributeParser ();
            const string attIdentifier = "$custom";

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
