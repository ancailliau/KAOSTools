using System;
using KAOSTools.Parsing.Parsers;
using KAOSTools.Parsing.Parsers.Attributes;
using NUnit.Framework;
using UCLouvain.KAOSTools.Parsing.Parsers.Exceptions;
using System.Linq;

namespace UCLouvain.KAOSTools.Parsing.Tests.Unit.Attributes
{
    [TestFixture]
    public class TestFormalSpecAttributeParser
    {
        [Test ()]
        public void TestValidValue ()
        {
            var ap = new FormalSpecAttributeParser ();
            const string attIdentifier = "formalspec";

            var e = ap.ParsedAttribute (attIdentifier, 
                                        null,
                                        new NParsedAttributeAtomic (new ParsedBool (true)));
            
            Assert.IsInstanceOf (typeof (ParsedFormalSpecAttribute), e);
            var pa = (ParsedFormalSpecAttribute)e;
            Assert.IsInstanceOf (typeof (ParsedBool), pa.Value);
            var b = (ParsedBool)pa.Value;
            Assert.AreEqual (true, b.Value);
        }

        [Test]
        public void TestNullParameter ()
        {
            var ap = new FormalSpecAttributeParser ();
            const string attIdentifier = "formalspec";

            var e = Assert.Catch (() => {
                ap.ParsedAttribute (attIdentifier,
                                        new NParsedAttributeColon(),
                                            new NParsedAttributeAtomic (new ParsedBool (true)));
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
            var ap = new FormalSpecAttributeParser ();
            const string attIdentifier = "formalspec";

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
