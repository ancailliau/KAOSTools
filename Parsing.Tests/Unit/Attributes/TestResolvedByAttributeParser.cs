using System;
using UCLouvain.KAOSTools.Parsing.Parsers;
using UCLouvain.KAOSTools.Parsing.Parsers.Attributes;
using NUnit.Framework;
using UCLouvain.KAOSTools.Parsing.Parsers.Exceptions;
using System.Linq;

namespace UCLouvain.KAOSTools.Parsing.Tests.Unit.Attributes
{
    [TestFixture()]
    public class TestResolvedByAttributeParser
    {
        [Test()]
        public void TestAtomicParameter()
        {
            TestNotColonParameter(new NParsedAttributeAtomic());
        }

        [Test()]
        public void TestListParameter()
        {
            TestNotColonParameter(new NParsedAttributeList(Enumerable.Empty<ParsedElement>()));
        }

        [Test()]
        public void TestBracketParameter()
        {
            TestNotColonParameter(new NParsedAttributeBracket());
        }

        [Test()]
        public void TestBracketValue()
        {
            var v = new NParsedAttributeBracket();
            TestNotAtomicValue(v);
        }

        [Test()]
        public void TestColonValue()
        {
            var v = new NParsedAttributeColon();
            TestNotAtomicValue(v);
        }

        [Test()]
        public void TestListValue()
        {
            var v = new NParsedAttributeList(Enumerable.Empty<ParsedElement>());
            TestNotAtomicValue(v);
        }

        [Test ()]
        public void TestNotIdentifierParameter()
        {
            var ap = new ResolvedByAttributeParser();
            const string attIdentifier = "resolvedBy";

            var e = Assert.Catch(() => {
                ap.ParsedAttribute(attIdentifier,
                                   new NParsedAttributeColon(new ParsedFloat(),new IdentifierExpression("anchor")), 
                                   new NParsedAttributeAtomic(new IdentifierExpression(attIdentifier)));
            });

            Assert.IsInstanceOf(typeof(InvalidParameterAttributeException), e);
            StringAssert.AreEqualIgnoringCase(
                string.Format(InvalidParameterAttributeException.IDENTIFIER, attIdentifier),
                e.Message
            );
        }

        [Test()]
        public void TestNotIdentifierValue()
        {
            var ap = new ResolvedByAttributeParser();
            const string attIdentifier = "resolvedBy";

            var e = Assert.Catch(() => {
                ap.ParsedAttribute(attIdentifier,
                                   null,
                                   new NParsedAttributeAtomic(new ParsedFloat()));
            });

            Assert.IsInstanceOf(typeof(InvalidAttributeValueException), e);
            StringAssert.AreEqualIgnoringCase(
                string.Format(InvalidAttributeValueException.IDENTIFIER, attIdentifier),
                e.Message
            );
        }

        public void TestNotAtomicValue(NParsedAttributeValue v) {
            var ap = new ResolvedByAttributeParser();
            const string attIdentifier = "resolvedBy";

            var e = Assert.Catch(() => {
                ap.ParsedAttribute(attIdentifier,
                                   null,
                                   v);
            });

            Assert.IsInstanceOf(typeof(InvalidAttributeValueException), e);
            StringAssert.AreEqualIgnoringCase(
                string.Format(InvalidAttributeValueException.ATOMIC_ONLY, attIdentifier),
                e.Message
            );
        }

        public void TestNotColonParameter(NParsedAttributeValue v)
        {
            var ap = new ResolvedByAttributeParser();
            const string attIdentifier = "resolvedBy";

            var e = Assert.Catch(() => {
                ap.ParsedAttribute(attIdentifier,
                                   v,
                                   new NParsedAttributeAtomic(new IdentifierExpression(attIdentifier)));
            });

            Assert.IsInstanceOf(typeof(InvalidParameterAttributeException), e);
            StringAssert.AreEqualIgnoringCase(
                string.Format(InvalidParameterAttributeException.COLON_ONLY, attIdentifier),
                e.Message
            );
        }
    }
}
