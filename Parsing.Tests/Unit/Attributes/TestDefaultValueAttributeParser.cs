using System;
using UCLouvain.KAOSTools.Parsing.Parsers;
using UCLouvain.KAOSTools.Parsing.Parsers.Attributes;
using NUnit.Framework;
using UCLouvain.KAOSTools.Parsing.Parsers.Exceptions;
using System.Linq;

namespace UCLouvain.KAOSTools.Parsing.Tests.Unit.Attributes
{
    [TestFixture]
    public class TestDefaultValueAttributeParser
    {
        [TestCase (true)]
        [TestCase (false)]
        public void TestValidValue (bool r)
        {
            var ap = new DefaultValueAttributeParser ();
            const string attIdentifier = "default";

            var e = ap.ParsedAttribute (attIdentifier, 
                                        null,
                                        new NParsedAttributeAtomic (new ParsedBool (r)));
            
            Assert.IsInstanceOf (typeof (DefaultValueAttribute), e);
            var pa = (DefaultValueAttribute)e;
            Assert.AreEqual (r, pa.Value);
        }

        [Test ()]
        public void TestNotBooleanValue ()
        {
            var ap = new DefaultValueAttributeParser ();
            const string attIdentifier = "default";

            var e = Assert.Catch (() => {
                ap.ParsedAttribute (attIdentifier,
                                   null,
                                   new NParsedAttributeAtomic (new ParsedFloat ()));
            });

            Assert.IsInstanceOf (typeof (InvalidAttributeValueException), e);
            StringAssert.AreEqualIgnoringCase (
                string.Format (InvalidAttributeValueException.BOOL, attIdentifier),
                e.Message
            );
        }

        [Test]
        public void TestNullParameter ()
        {
            var ap = new DefaultValueAttributeParser ();
            const string attIdentifier = "default";

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
            var ap = new DefaultValueAttributeParser ();
            const string attIdentifier = "default";

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
