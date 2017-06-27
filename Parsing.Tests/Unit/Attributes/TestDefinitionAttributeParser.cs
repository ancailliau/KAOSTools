using System;
using UCLouvain.KAOSTools.Parsing.Parsers;
using UCLouvain.KAOSTools.Parsing.Parsers.Attributes;
using NUnit.Framework;
using UCLouvain.KAOSTools.Parsing.Parsers.Exceptions;
using System.Linq;

namespace UCLouvain.KAOSTools.Parsing.Tests.Unit.Attributes
{
    [TestFixture]
    public class TestDefinitionAttributeParser
    {
        [TestCase ("")]
        [TestCase ("Description")]
        [TestCase ("   ")]
        public void TestValidValue (string r)
        {
            var ap = new DefinitionAttributeParser ();
            const string attIdentifier = "definition";

            var e = ap.ParsedAttribute (attIdentifier, 
                                        null,
                                        new NParsedAttributeAtomic (new ParsedString (r)));
            
            Assert.IsInstanceOf (typeof (ParsedDefinitionAttribute), e);
            var pa = (ParsedDefinitionAttribute)e;
            Assert.AreEqual (r, pa.Value.Value);
        }

        [Test ()]
        public void TestNotStringValue ()
        {
            var ap = new DefinitionAttributeParser ();
            const string attIdentifier = "definition";

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
            var ap = new DefinitionAttributeParser ();
            const string attIdentifier = "definition";

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
            var ap = new DefinitionAttributeParser ();
            const string attIdentifier = "definition";

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
