using System;
using KAOSTools.Parsing.Parsers;
using KAOSTools.Parsing.Parsers.Attributes;
using NUnit.Framework;
using UCLouvain.KAOSTools.Parsing.Parsers.Exceptions;
using System.Linq;

namespace UCLouvain.KAOSTools.Parsing.Tests.Unit.Attributes
{
    [TestFixture()]
    public class TestResolvedByAttributeParser
    {
        [Test()]
        public void TestColonParameter()
        {
            TestNotAtomicParameter(new NParsedAttributeColon());
        }

        [Test()]
        public void TestListParameter()
        {
            TestNotAtomicParameter(new NParsedAttributeList(Enumerable.Empty<ParsedElement>()));
        }

        [Test()]
        public void TestBracketParameter()
        {
            TestNotAtomicParameter(new NParsedAttributeBracket());
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

        [Test()]
        public void TestNotIdentifierParameter()
        {
            var ap = new ResolvedByAttributeParser();
            const string attIdentifier = "resolvedBy";

            var e = Assert.Catch(() => {
                ap.ParsedAttribute(attIdentifier,
                                   new NParsedAttributeAtomic(new ParsedFloat()), 
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

        public void TestNotAtomicParameter(NParsedAttributeValue v)
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
                string.Format(InvalidParameterAttributeException.ATOMIC_ONLY, attIdentifier),
                e.Message
            );
        }
    }
}
