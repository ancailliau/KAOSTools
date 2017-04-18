using System;
using KAOSTools.Parsing.Parsers;
using KAOSTools.Parsing.Parsers.Attributes;
using NUnit.Framework;
using UCLouvain.KAOSTools.Parsing.Parsers.Exceptions;
using System.Linq;

namespace UCLouvain.KAOSTools.Parsing.Tests.Unit.Attributes
{
    [TestFixture]
    public class TestRSRAttributeParser
    {
        [TestCase (0.00, 0)]
        [TestCase (0.1, .1)]
        [TestCase (1, 1)]
        [TestCase (.99, .99)]
        public void TestFloat (double i, double e)
        {
            var ap = new RSRAttributeParser ();
            const string attIdentifier = "rsr";


            var v = ap.ParsedAttribute (attIdentifier,
                                        null,
                                        new NParsedAttributeAtomic (new ParsedFloat (i)));

            Assert.IsInstanceOf (typeof (ParsedRDSAttribute), v);
            var rsr = (ParsedRDSAttribute)v;
            Assert.AreEqual (e, rsr.Value);
        }

        [TestCase (110)]
        [TestCase (10000)]
        [TestCase (100)]
        [TestCase (2)]
        [TestCase (-2)]
        [TestCase (Double.MinValue)]
        [TestCase (Double.MaxValue)]
        public void TestInvalidFloat (double p)
        {
            var ap = new RSRAttributeParser ();
            const string attIdentifier = "rsr";

            var e = Assert.Catch (() => {
                ap.ParsedAttribute (attIdentifier,
                                       null,
                                    new NParsedAttributeAtomic (new ParsedFloat (p)));
            });

            Assert.IsInstanceOf (typeof (InvalidAttributeValueException), e);
            StringAssert.AreEqualIgnoringCase (
                string.Format (InvalidAttributeValueException.PROBABILITY_EXPECTED, attIdentifier),
                e.Message
            );
        }

        [TestCase (0, 0)]
        [TestCase (1, 1)]
        public void TestInteger (int i, double e)
        {
            var ap = new RSRAttributeParser ();
            const string attIdentifier = "rsr";


            var v = ap.ParsedAttribute (attIdentifier,
                                        null,
                                        new NParsedAttributeAtomic (new ParsedInteger (i)));

            Assert.IsInstanceOf (typeof (ParsedRDSAttribute), v);
            var rsr = (ParsedRDSAttribute)v;
            Assert.AreEqual (e, rsr.Value);
        }

        [TestCase (110)]
        [TestCase (10000)]
        [TestCase (100)]
        [TestCase (2)]
        [TestCase (-2)]
        [TestCase (Int32.MinValue)]
        [TestCase (Int32.MaxValue)]
        public void TestInvalidInteger (int p)
        {
            var ap = new RSRAttributeParser ();
            const string attIdentifier = "rsr";

            var e = Assert.Catch (() => {
                ap.ParsedAttribute (attIdentifier,
                                       null,
                                    new NParsedAttributeAtomic (new ParsedInteger (p)));
            });

            Assert.IsInstanceOf (typeof (InvalidAttributeValueException), e);
            StringAssert.AreEqualIgnoringCase (
                string.Format (InvalidAttributeValueException.PROBABILITY_EXPECTED, attIdentifier),
                e.Message
            );
        }

        [TestCase (90, .9)]
        [TestCase (0, 0)]
        [TestCase (100, 1)]
        [TestCase (50.5, .505)]
        public void TestPercentage (double p, double e)
        {
            var ap = new RSRAttributeParser ();
            const string attIdentifier = "rsr";


            var v = ap.ParsedAttribute (attIdentifier,
                                        null,
                                        new NParsedAttributeAtomic (new ParsedPercentage (p)));

            Assert.IsInstanceOf (typeof (ParsedRDSAttribute), v);
            var rsr = (ParsedRDSAttribute)v;
            Assert.AreEqual (e, rsr.Value);
        }

        [TestCase(110)]
        [TestCase (10000)]
        [TestCase (100.1)]
        [TestCase (-0.1)]
        [TestCase (-100)]
        [TestCase (Double.MinValue)]
        [TestCase (Double.MaxValue)]
        public void TestInvalidPercentage (double p)
        {
            var ap = new RSRAttributeParser ();
            const string attIdentifier = "rsr";

            var e = Assert.Catch(() => {
                 ap.ParsedAttribute (attIdentifier,
                                        null,
                                        new NParsedAttributeAtomic (new ParsedPercentage (p)));
            });

            Assert.IsInstanceOf (typeof (InvalidAttributeValueException), e);
            StringAssert.AreEqualIgnoringCase (
                string.Format (InvalidAttributeValueException.PROBABILITY_EXPECTED, attIdentifier),
                e.Message
            );
        }

        [Test]
        public void TestNullParameter ()
        {
            var ap = new RSRAttributeParser();
            const string attIdentifier = "rsr";

            var e = Assert.Catch(() => {
                ap.ParsedAttribute(attIdentifier,
                                   new NParsedAttributeAtomic(new ParsedFloat()), 
                                   new NParsedAttributeAtomic(new ParsedFloat(.9)));
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
            var ap = new RSRAttributeParser ();
            const string attIdentifier = "rsr";

            var e = Assert.Catch (() => {
                ap.ParsedAttribute (attIdentifier,
                                   null,
                                    new NParsedAttributeAtomic(new NameExpression("")));
            });

            Assert.IsInstanceOf (typeof (InvalidAttributeValueException), e);
            StringAssert.AreEqualIgnoringCase (
                string.Format (InvalidAttributeValueException.FLOAT_INTEGER_PERCENTAGE_ONLY, attIdentifier),
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
            var ap = new RSRAttributeParser ();
            const string attIdentifier = "rsr";

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
