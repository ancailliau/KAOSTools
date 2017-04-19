using System;
using KAOSTools.Parsing.Parsers;
using KAOSTools.Parsing.Parsers.Attributes;
using NUnit.Framework;
using UCLouvain.KAOSTools.Parsing.Parsers.Exceptions;
using System.Linq;
using System.Collections.Generic;

namespace UCLouvain.KAOSTools.Parsing.Tests.Unit.Attributes
{
    [TestFixture]
    public class TestESRAttributeParser
    {
        #region distribution

        [TestCase ("")]
        [TestCase ("bla")]
        [TestCase ("UNIFORM")]
        [TestCase ("   ")]
        public void TestInvalidDistributionIdentifier (string name)
        {
            var ap = new ESRAttributeParser ();
            const string attIdentifier = "esr";

            var e = Assert.Catch (() => {
                ap.ParsedAttribute (attIdentifier,
                                            null,
                                            new NParsedAttributeBracket (
                                                new IdentifierExpression (name),
                                                new NParsedAttributeList (Enumerable.Empty<ParsedElement> ())
                                               ));
            });

            Assert.IsInstanceOf (typeof (InvalidAttributeValueException), e);
            StringAssert.AreEqualIgnoringCase (
                string.Format (InvalidAttributeValueException.INVALID_VALUE, attIdentifier),
                e.Message
            );
        }

        public void TestInvalidDistribution ()
        {
            var ap = new ESRAttributeParser ();
            const string attIdentifier = "esr";

            var e = Assert.Catch (() => {
                ap.ParsedAttribute (attIdentifier,
                                            null,
                                            new NParsedAttributeBracket (
                                                new ParsedFloat (0),
                                                new NParsedAttributeList (Enumerable.Empty<ParsedElement> ())
                                               ));
            });

            Assert.IsInstanceOf (typeof (InvalidAttributeValueException), e);
            StringAssert.AreEqualIgnoringCase (
                string.Format (InvalidAttributeValueException.IDENTIFIER, attIdentifier),
                e.Message
            );
        }

        [Test()]
        public void TestInvalidDistributionParameterColon ()
        {
            TestInvalidDistributionParameter (new NParsedAttributeColon ());
        }

        [Test ()]
        public void TestInvalidDistributionParameterBracket ()
        {
            TestInvalidDistributionParameter (new NParsedAttributeBracket ());
        }

        public void TestInvalidDistributionParameter (ParsedElement v)
        {
            var ap = new ESRAttributeParser ();
            const string attIdentifier = "esr";

            var e = Assert.Catch (() => {
                ap.ParsedAttribute (attIdentifier,
                                            null,
                                            new NParsedAttributeBracket (
                                                new IdentifierExpression ("beta"),
                                                v
                                               ));
            });

            Assert.IsInstanceOf (typeof (InvalidAttributeValueException), e);
            StringAssert.AreEqualIgnoringCase (
                string.Format (InvalidAttributeValueException.ATOMIC_OR_LIST, attIdentifier),
                e.Message
            );
        }

        #endregion

        #region Beta distribution

        [TestCase (0.1, 1)]
        [TestCase (0, 1.1)]
        [TestCase (0, 1)]
        [TestCase (5, 20)]
        [TestCase (0, Double.MaxValue)]
        [TestCase (Double.MaxValue, 0)]
        public void TestBetaFloat (double alpha, double beta)
        {
            var ap = new ESRAttributeParser ();
            const string attIdentifier = "esr";

            var v = ap.ParsedAttribute (attIdentifier,
                                        null,
                                        new NParsedAttributeBracket (
                                            new IdentifierExpression ("beta"), 
                                            new NParsedAttributeList (
                                                new[] { 
                                                    new NParsedAttributeAtomic (new ParsedFloat(alpha)), 
                                                    new NParsedAttributeAtomic (new ParsedFloat (beta))
                                                })
                                           ));

            Assert.IsInstanceOf (typeof (ParsedBetaDistribution), v);
            var esr = (ParsedBetaDistribution)v;
            Assert.AreEqual (alpha, esr.Alpha);
            Assert.AreEqual (beta, esr.Beta);
        }
 
        [TestCase (0, -1)]
        [TestCase (-1, 0)]
        [TestCase (-20, -5)]
        [TestCase (Double.MinValue, 0)]
        public void TestInvalidBetaFloat (double alpha, double beta)
        {
            var ap = new ESRAttributeParser ();
            const string attIdentifier = "esr";

            var e = Assert.Catch (() => {
                ap.ParsedAttribute (attIdentifier,
                                            null,
                                            new NParsedAttributeBracket (
                                                new IdentifierExpression ("beta"),
                                                new NParsedAttributeList (
                                                new [] {
                                                    new NParsedAttributeAtomic (new ParsedFloat(alpha)),
                                                    new NParsedAttributeAtomic (new ParsedFloat (beta))
                                                })
                                               ));
            });

            Assert.IsInstanceOf (typeof (InvalidAttributeValueException), e);
            StringAssert.AreEqualIgnoringCase (
                string.Format (InvalidAttributeValueException.INVALID_VALUE, attIdentifier),
                e.Message
            );
        }

        [TestCase (0, 1)]
        [TestCase (5, 20)]
        [TestCase (Int16.MaxValue, 0)]
        [TestCase (0, Int16.MaxValue)]
        public void TestBetaInteger (int alpha, int beta)
        {
            var ap = new ESRAttributeParser ();
            const string attIdentifier = "esr";

            var v = ap.ParsedAttribute (attIdentifier,
                                        null,
                                        new NParsedAttributeBracket (
                                            new IdentifierExpression ("beta"),
                                            new NParsedAttributeList (
                                                new [] {
                                                    new NParsedAttributeAtomic (new ParsedInteger (alpha)),
                                                    new NParsedAttributeAtomic (new ParsedInteger (beta))
                                                })
                                           ));

            Assert.IsInstanceOf (typeof (ParsedBetaDistribution), v);
            var esr = (ParsedBetaDistribution)v;
            Assert.AreEqual (alpha, esr.Alpha);
            Assert.AreEqual (beta, esr.Beta);
        }

        [TestCase (-1, 0)]
        [TestCase (0, -1)]
        [TestCase (-5, -20)]
        [TestCase (Int16.MinValue, 0)]
        public void TestInvalidBetaInteger (int alpha, int beta)
        {
            var ap = new ESRAttributeParser ();
            const string attIdentifier = "esr";

            var e = Assert.Catch (() => {
                ap.ParsedAttribute (attIdentifier,
                                            null,
                                            new NParsedAttributeBracket (
                                                new IdentifierExpression ("beta"),
                                            new NParsedAttributeList (
                                                new [] {
                                                    new NParsedAttributeAtomic (new ParsedInteger (alpha)),
                                                    new NParsedAttributeAtomic (new ParsedInteger (beta))
                                                })
                                               ));
            });

            Assert.IsInstanceOf (typeof (InvalidAttributeValueException), e);
            StringAssert.AreEqualIgnoringCase (
                string.Format (InvalidAttributeValueException.INVALID_VALUE, attIdentifier),
                e.Message
            );
        }

        [TestCase (0.1, 1)]
        [TestCase (0, 1.1)]
        [TestCase (0, 1)]
        [TestCase (5, 20)]
        [TestCase (0, Double.MaxValue)]
        [TestCase (Double.MaxValue, 0)]
        [TestCase (Int16.MaxValue, 0)]
        [TestCase (0, Int16.MaxValue)]
        public void TestBetaPercentage (double alpha, double beta)
        {
            var ap = new ESRAttributeParser ();
            const string attIdentifier = "esr";

            var v = ap.ParsedAttribute (attIdentifier,
                                        null,
                                        new NParsedAttributeBracket (
                                            new IdentifierExpression ("beta"),
                                            new NParsedAttributeList (
                                                new [] {
                                                    new NParsedAttributeAtomic (new ParsedPercentage (alpha)),
                                                    new NParsedAttributeAtomic (new ParsedPercentage (beta))
                                                })
                                           ));

            Assert.IsInstanceOf (typeof (ParsedBetaDistribution), v);
            var esr = (ParsedBetaDistribution)v;
            Assert.AreEqual (alpha / 100d, esr.Alpha);
            Assert.AreEqual (beta / 100d, esr.Beta);
        }

        [TestCase (-1, 0)]
        [TestCase (0, -1)]
        [TestCase (-5, -20)]
        [TestCase (-5.7, -20.4)]
        [TestCase (Int16.MinValue, 0)]
        public void TestInvalidBetaPercentage (double alpha, double beta)
        {
            var ap = new ESRAttributeParser ();
            const string attIdentifier = "esr";

            var e = Assert.Catch (() => {
                ap.ParsedAttribute (attIdentifier,
                                            null,
                                            new NParsedAttributeBracket (
                                                new IdentifierExpression ("beta"),
                                            new NParsedAttributeList (
                                                new [] {
                                                    new NParsedAttributeAtomic (new ParsedPercentage (alpha)),
                                                    new NParsedAttributeAtomic (new ParsedPercentage (beta))
                                                })
                                               ));
            });

            Assert.IsInstanceOf (typeof (InvalidAttributeValueException), e);
            StringAssert.AreEqualIgnoringCase (
                string.Format (InvalidAttributeValueException.INVALID_VALUE, attIdentifier),
                e.Message
            );
        }
 
        [Test()]
        public void TestInvalidBetaParameter ()
        {
            TestInvalidBetaParameterHelper (new [] {
                                                    new NParsedAttributeAtomic (new ParsedString("")),
                                                    new NParsedAttributeAtomic (new ParsedFloat (0))
                                                });

            TestInvalidBetaParameterHelper (new [] {
                                                    new NParsedAttributeAtomic (new ParsedFloat(0))
                                                });
        }

        public void TestInvalidBetaParameterHelper (IEnumerable<ParsedElement> v)
        {
            var ap = new ESRAttributeParser ();
            const string attIdentifier = "esr";

            var e = Assert.Catch (() => {
                ap.ParsedAttribute (attIdentifier,
                                            null,
                                            new NParsedAttributeBracket (
                                                new IdentifierExpression ("beta"),
                                                new NParsedAttributeList (v)
                                               ));
            });

            Assert.IsInstanceOf (typeof (InvalidAttributeValueException), e);
            StringAssert.AreEqualIgnoringCase (
                string.Format (InvalidAttributeValueException.INVALID_VALUE, attIdentifier),
                e.Message
            );
        }

        #endregion

        #region Uniform distribution

        [TestCase (0.1, 1)]
        [TestCase (0, 1)]
        [TestCase (0.5, .5)]
        [TestCase (0, .1)]
        public void TestUniformFloat (double lo, double hi)
        {
            var ap = new ESRAttributeParser ();
            const string attIdentifier = "esr";

            var v = ap.ParsedAttribute (attIdentifier,
                                        null,
                                        new NParsedAttributeBracket (
                                            new IdentifierExpression ("uniform"),
                                            new NParsedAttributeList (
                                                new [] {
                                                    new NParsedAttributeAtomic (new ParsedFloat(lo)),
                                                    new NParsedAttributeAtomic (new ParsedFloat (hi))
                                                })
                                           ));

            Assert.IsInstanceOf (typeof (ParsedUniformDistribution), v);
            var esr = (ParsedUniformDistribution)v;
            Assert.AreEqual (lo, esr.LowerBound);
            Assert.AreEqual (hi, esr.UpperBound);
        }

        [TestCase (0, 1.1)]
        [TestCase (5, 20)]
        [TestCase (0, Double.MaxValue)]
        [TestCase (Double.MaxValue, 0)]
        [TestCase (0, -1)]
        [TestCase (-1, 0)]
        [TestCase (-20, -5)]
        [TestCase (Double.MinValue, 0)]
        public void TestInvalidUniformFloat (double lo, double hi)
        {
            var ap = new ESRAttributeParser ();
            const string attIdentifier = "esr";

            var e = Assert.Catch (() => {
                ap.ParsedAttribute (attIdentifier,
                                            null,
                                            new NParsedAttributeBracket (
                                                new IdentifierExpression ("uniform"),
                                                new NParsedAttributeList (
                                                new [] {
                                                    new NParsedAttributeAtomic (new ParsedFloat(lo)),
                                                    new NParsedAttributeAtomic (new ParsedFloat (hi))
                                                })
                                               ));
            });

            Assert.IsInstanceOf (typeof (InvalidAttributeValueException), e);
            StringAssert.AreEqualIgnoringCase (
                string.Format (InvalidAttributeValueException.INVALID_VALUE, attIdentifier),
                e.Message
            );
        }

        [TestCase (0, 1)]
        public void TestUniformInteger (int lo, int hi)
        {
            var ap = new ESRAttributeParser ();
            const string attIdentifier = "esr";

            var v = ap.ParsedAttribute (attIdentifier,
                                        null,
                                        new NParsedAttributeBracket (
                                            new IdentifierExpression ("uniform"),
                                            new NParsedAttributeList (
                                                new [] {
                                                    new NParsedAttributeAtomic (new ParsedInteger (lo)),
                                                    new NParsedAttributeAtomic (new ParsedInteger (hi))
                                                })
                                           ));

            Assert.IsInstanceOf (typeof (ParsedUniformDistribution), v);
            var esr = (ParsedUniformDistribution)v;
            Assert.AreEqual (lo, esr.LowerBound);
            Assert.AreEqual (hi, esr.UpperBound);
        }

        [TestCase (-1, 0)]
        [TestCase (0, -1)]
        [TestCase (-5, -20)]
        [TestCase (Int16.MinValue, 0)]
        [TestCase (5, 20)]
        [TestCase (Int16.MaxValue, 0)]
        [TestCase (0, Int16.MaxValue)]
        public void TestInvalidUniformInteger (int lo, int hi)
        {
            var ap = new ESRAttributeParser ();
            const string attIdentifier = "esr";

            var e = Assert.Catch (() => {
                ap.ParsedAttribute (attIdentifier,
                                            null,
                                            new NParsedAttributeBracket (
                                                new IdentifierExpression ("uniform"),
                                            new NParsedAttributeList (
                                                new [] {
                                                    new NParsedAttributeAtomic (new ParsedInteger (lo)),
                                                    new NParsedAttributeAtomic (new ParsedInteger (hi))
                                                })
                                               ));
            });

            Assert.IsInstanceOf (typeof (InvalidAttributeValueException), e);
            StringAssert.AreEqualIgnoringCase (
                string.Format (InvalidAttributeValueException.INVALID_VALUE, attIdentifier),
                e.Message
            );
        }

        [TestCase (0.1, 1)]
        [TestCase (0, 1)]
        [TestCase (0.5, .5)]
        [TestCase (0, .1)]
        [TestCase (0, 1.1)]
        [TestCase (5, 20)]
        public void TestUniformPercentage (double lo, double hi)
        {
            var ap = new ESRAttributeParser ();
            const string attIdentifier = "esr";

            var v = ap.ParsedAttribute (attIdentifier,
                                        null,
                                        new NParsedAttributeBracket (
                                            new IdentifierExpression ("uniform"),
                                            new NParsedAttributeList (
                                                new [] {
                                                    new NParsedAttributeAtomic (new ParsedPercentage (lo)),
                                                    new NParsedAttributeAtomic (new ParsedPercentage (hi))
                                                })
                                           ));

            Assert.IsInstanceOf (typeof (ParsedUniformDistribution), v);
            var esr = (ParsedUniformDistribution)v;
            Assert.AreEqual (lo / 100d, esr.LowerBound);
            Assert.AreEqual (hi / 100d, esr.UpperBound);
        }

        [TestCase (-1, 0)]
        [TestCase (0, -1)]
        [TestCase (-5, -20)]
        [TestCase (-5.7, -20.4)]
        [TestCase (Int16.MinValue, 0)]
        [TestCase (0, Double.MaxValue)]
        [TestCase (Double.MaxValue, 0)]
        [TestCase (Int16.MaxValue, 0)]
        [TestCase (0, Int16.MaxValue)]
        public void TestInvalidUniformPercentage (double lo, double hi)
        {
            var ap = new ESRAttributeParser ();
            const string attIdentifier = "esr";

            var e = Assert.Catch (() => {
                ap.ParsedAttribute (attIdentifier,
                                            null,
                                            new NParsedAttributeBracket (
                                                new IdentifierExpression ("uniform"),
                                            new NParsedAttributeList (
                                                new [] {
                                                    new NParsedAttributeAtomic (new ParsedPercentage (lo)),
                                                    new NParsedAttributeAtomic (new ParsedPercentage (hi))
                                                })
                                               ));
            });

            Assert.IsInstanceOf (typeof (InvalidAttributeValueException), e);
            StringAssert.AreEqualIgnoringCase (
                string.Format (InvalidAttributeValueException.INVALID_VALUE, attIdentifier),
                e.Message
            );
        }

        [Test ()]
        public void TestInvalidUniformParameter ()
        {
            TestInvalidUniformParameterHelper (new [] {
                                                    new NParsedAttributeAtomic (new ParsedString ("")),
                                                    new NParsedAttributeAtomic (new ParsedFloat (0))
                                                });

            TestInvalidUniformParameterHelper (new [] {
                                                    new NParsedAttributeAtomic (new ParsedFloat (0)),
                                                });
        }

        public void TestInvalidUniformParameterHelper (IEnumerable<ParsedElement> v)
        {
            var ap = new ESRAttributeParser ();
            const string attIdentifier = "esr";

            var e = Assert.Catch (() => {
                ap.ParsedAttribute (attIdentifier,
                                            null,
                                            new NParsedAttributeBracket (
                                                new IdentifierExpression ("uniform"),
                                                new NParsedAttributeList (v)
                                               ));
            });

            Assert.IsInstanceOf (typeof (InvalidAttributeValueException), e);
            StringAssert.AreEqualIgnoringCase (
                string.Format (InvalidAttributeValueException.INVALID_VALUE, attIdentifier),
                e.Message
            );
        }

        #endregion

        #region Triangular distribution

        [TestCase (0.1, .4, 1)]
        [TestCase (0, .4, 1)]
        [TestCase (0.5, .5, .5)]
        [TestCase (0, .05, .1)]
        public void TestTriangularFloat (double lo, double mode, double hi)
        {
            var ap = new ESRAttributeParser ();
            const string attIdentifier = "esr";

            var v = ap.ParsedAttribute (attIdentifier,
                                        null,
                                        new NParsedAttributeBracket (
                                            new IdentifierExpression ("triangular"),
                                            new NParsedAttributeList (
                                                new [] {
                                                    new NParsedAttributeAtomic (new ParsedFloat(lo)),
                                                    new NParsedAttributeAtomic (new ParsedFloat (mode)),
                                                    new NParsedAttributeAtomic (new ParsedFloat (hi))
                                                })
                                           ));

            Assert.IsInstanceOf (typeof (ParsedTriangularDistribution), v);
            var esr = (ParsedTriangularDistribution)v;
            Assert.AreEqual (lo, esr.Min);
            Assert.AreEqual (mode, esr.Mode);
            Assert.AreEqual (hi, esr.Max);
        }

        [TestCase (0, .4, 1.1)]
        [TestCase (0, 1.3, 1.1)]
        [TestCase (5, 6, 20)]
        [TestCase (0, 1, Double.MaxValue)]
        [TestCase (Double.MaxValue, 0, 0)]
        [TestCase (0, 0, -1)]
        [TestCase (-1, 0, 0)]
        [TestCase (-20, -10, -5)]
        [TestCase (Double.MinValue, 0, 0)]
        public void TestInvalidTriangularFloat (double lo, double mode, double hi)
        {
            var ap = new ESRAttributeParser ();
            const string attIdentifier = "esr";

            var e = Assert.Catch (() => {
                ap.ParsedAttribute (attIdentifier,
                                            null,
                                            new NParsedAttributeBracket (
                                                new IdentifierExpression ("triangular"),
                                                new NParsedAttributeList (
                                                new [] {
                                                    new NParsedAttributeAtomic (new ParsedFloat(lo)),
                                                    new NParsedAttributeAtomic (new ParsedFloat (mode)),
                                                    new NParsedAttributeAtomic (new ParsedFloat (hi))
                                                })
                                               ));
            });

            Assert.IsInstanceOf (typeof (InvalidAttributeValueException), e);
            StringAssert.AreEqualIgnoringCase (
                string.Format (InvalidAttributeValueException.INVALID_VALUE, attIdentifier),
                e.Message
            );
        }

        [TestCase (0, 0, 1)]
        public void TestTriangularInteger (int lo, int mode, int hi)
        {
            var ap = new ESRAttributeParser ();
            const string attIdentifier = "esr";

            var v = ap.ParsedAttribute (attIdentifier,
                                        null,
                                        new NParsedAttributeBracket (
                                            new IdentifierExpression ("triangular"),
                                            new NParsedAttributeList (
                                                new [] {
                                                    new NParsedAttributeAtomic (new ParsedInteger (lo)),
                                                    new NParsedAttributeAtomic (new ParsedFloat (mode)),
                                                    new NParsedAttributeAtomic (new ParsedInteger (hi))
                                                })
                                           ));

            Assert.IsInstanceOf (typeof (ParsedTriangularDistribution), v);
            var esr = (ParsedTriangularDistribution)v;
            Assert.AreEqual (lo, esr.Min);
            Assert.AreEqual (mode, esr.Mode);
            Assert.AreEqual (hi, esr.Max);
        }

        [TestCase (-1, 0, 0)]
        [TestCase (0, 0, -1)]
        [TestCase (-5, -10, -20)]
        [TestCase (Int16.MinValue, 0, 0)]
        [TestCase (5, 10, 20)]
        [TestCase (Int16.MaxValue, 0, 0)]
        [TestCase (0, 0, Int16.MaxValue)]
        public void TestInvalidTriangularInteger (int lo, int mode, int hi)
        {
            var ap = new ESRAttributeParser ();
            const string attIdentifier = "esr";

            var e = Assert.Catch (() => {
                ap.ParsedAttribute (attIdentifier,
                                            null,
                                            new NParsedAttributeBracket (
                                                new IdentifierExpression ("triangular"),
                                            new NParsedAttributeList (
                                                new [] {
                                                    new NParsedAttributeAtomic (new ParsedInteger (lo)),
                                                    new NParsedAttributeAtomic (new ParsedFloat (mode)),
                                                    new NParsedAttributeAtomic (new ParsedInteger (hi))
                                                })
                                               ));
            });

            Assert.IsInstanceOf (typeof (InvalidAttributeValueException), e);
            StringAssert.AreEqualIgnoringCase (
                string.Format (InvalidAttributeValueException.INVALID_VALUE, attIdentifier),
                e.Message
            );
        }

        [TestCase (0.1, .4, 1)]
        [TestCase (0, .4, 1)]
        [TestCase (0.5, .5, .5)]
        [TestCase (0, .05, .1)]
        [TestCase (0, 0, 1)]
        [TestCase (0, 1, 1)]
        public void TestTriangularPercentage (double lo, double mode, double hi)
        {
            var ap = new ESRAttributeParser ();
            const string attIdentifier = "esr";

            var v = ap.ParsedAttribute (attIdentifier,
                                        null,
                                        new NParsedAttributeBracket (
                                            new IdentifierExpression ("triangular"),
                                            new NParsedAttributeList (
                                                new [] {
                                                    new NParsedAttributeAtomic (new ParsedPercentage (lo)),
                                                    new NParsedAttributeAtomic (new ParsedPercentage (mode)),
                                                    new NParsedAttributeAtomic (new ParsedPercentage (hi))
                                                })
                                           ));

            Assert.IsInstanceOf (typeof (ParsedTriangularDistribution), v);
            var esr = (ParsedTriangularDistribution)v;
            Assert.AreEqual (lo / 100d, esr.Min);
            Assert.AreEqual (mode / 100d, esr.Mode);
            Assert.AreEqual (hi / 100d, esr.Max);
        }

        [TestCase (-1, 0, 0)]
        [TestCase (0, 0, -1)]
        [TestCase (-5, -10, -20)]
        [TestCase (Int16.MinValue, 0, 0)]
        [TestCase (5, 10, 20)]
        [TestCase (Int16.MaxValue, 0, 0)]
        [TestCase (0, 0, Int16.MaxValue)]
        [TestCase (Double.MinValue, 0, 0)]
        [TestCase (0, 1, Double.MaxValue)]
        [TestCase (Double.MaxValue, 0, 0)]
        public void TestInvalidTriangularPercentage (double lo, double mode, double hi)
        {
            var ap = new ESRAttributeParser ();
            const string attIdentifier = "esr";

            var e = Assert.Catch (() => {
                ap.ParsedAttribute (attIdentifier,
                                            null,
                                            new NParsedAttributeBracket (
                                                new IdentifierExpression ("triangular"),
                                            new NParsedAttributeList (
                                                new [] {
                                                    new NParsedAttributeAtomic (new ParsedPercentage (lo)),
                                                    new NParsedAttributeAtomic (new ParsedFloat (mode)),
                                                    new NParsedAttributeAtomic (new ParsedPercentage (hi))
                                                })
                                               ));
            });

            Assert.IsInstanceOf (typeof (InvalidAttributeValueException), e);
            StringAssert.AreEqualIgnoringCase (
                string.Format (InvalidAttributeValueException.INVALID_VALUE, attIdentifier),
                e.Message
            );
        }

        [Test ()]
        public void TestInvalidTriangularParameter ()
        {
            TestInvalidTriangularParameterHelper (new [] {
                                                    new NParsedAttributeAtomic (new ParsedString ("")),
                                                    new NParsedAttributeAtomic (new ParsedString ("")),
                                                    new NParsedAttributeAtomic (new ParsedFloat (0))
                                                });

            TestInvalidTriangularParameterHelper (new [] {
                                                    new NParsedAttributeAtomic (new ParsedString ("")),
                                                    new NParsedAttributeAtomic (new ParsedFloat (0))
                                                });

            TestInvalidTriangularParameterHelper (new [] {
                                                    new NParsedAttributeAtomic (new ParsedFloat (0)),
                                                    new NParsedAttributeAtomic (new ParsedFloat (0))
                                                });

            TestInvalidTriangularParameterHelper (new [] {
                                                    new NParsedAttributeAtomic (new ParsedFloat (0)),
                                                });
        }

        public void TestInvalidTriangularParameterHelper (IEnumerable<ParsedElement> v)
        {
            var ap = new ESRAttributeParser ();
            const string attIdentifier = "esr";

            var e = Assert.Catch (() => {
                ap.ParsedAttribute (attIdentifier,
                                            null,
                                            new NParsedAttributeBracket (
                                                new IdentifierExpression ("triangular"),
                                                new NParsedAttributeList (v)
                                               ));
            });

            Assert.IsInstanceOf (typeof (InvalidAttributeValueException), e);
            StringAssert.AreEqualIgnoringCase (
                string.Format (InvalidAttributeValueException.INVALID_VALUE, attIdentifier),
                e.Message
            );
        }

        #endregion

        #region PERT distribution

        [TestCase (0.1, .4, 1)]
        [TestCase (0, .4, 1)]
        [TestCase (0.5, .5, .5)]
        [TestCase (0, .05, .1)]
        public void TestPERTFloat (double lo, double mode, double hi)
        {
            var ap = new ESRAttributeParser ();
            const string attIdentifier = "esr";

            var v = ap.ParsedAttribute (attIdentifier,
                                        null,
                                        new NParsedAttributeBracket (
                                            new IdentifierExpression ("pert"),
                                            new NParsedAttributeList (
                                                new [] {
                                                    new NParsedAttributeAtomic (new ParsedFloat(lo)),
                                                    new NParsedAttributeAtomic (new ParsedFloat (mode)),
                                                    new NParsedAttributeAtomic (new ParsedFloat (hi))
                                                })
                                           ));

            Assert.IsInstanceOf (typeof (ParsedPertDistribution), v);
            var esr = (ParsedPertDistribution)v;
            Assert.AreEqual (lo, esr.Min);
            Assert.AreEqual (mode, esr.Mode);
            Assert.AreEqual (hi, esr.Max);
        }

        [TestCase (0, .4, 1.1)]
        [TestCase (0, 1.3, 1.1)]
        [TestCase (5, 6, 20)]
        [TestCase (0, 1, Double.MaxValue)]
        [TestCase (Double.MaxValue, 0, 0)]
        [TestCase (0, 0, -1)]
        [TestCase (-1, 0, 0)]
        [TestCase (-20, -10, -5)]
        [TestCase (Double.MinValue, 0, 0)]
        public void TestInvalidPERTFloat (double lo, double mode, double hi)
        {
            var ap = new ESRAttributeParser ();
            const string attIdentifier = "esr";

            var e = Assert.Catch (() => {
                ap.ParsedAttribute (attIdentifier,
                                            null,
                                            new NParsedAttributeBracket (
                                                new IdentifierExpression ("pert"),
                                                new NParsedAttributeList (
                                                new [] {
                                                    new NParsedAttributeAtomic (new ParsedFloat(lo)),
                                                    new NParsedAttributeAtomic (new ParsedFloat (mode)),
                                                    new NParsedAttributeAtomic (new ParsedFloat (hi))
                                                })
                                               ));
            });

            Assert.IsInstanceOf (typeof (InvalidAttributeValueException), e);
            StringAssert.AreEqualIgnoringCase (
                string.Format (InvalidAttributeValueException.INVALID_VALUE, attIdentifier),
                e.Message
            );
        }

        [TestCase (0, 0, 1)]
        public void TestPERTInteger (int lo, int mode, int hi)
        {
            var ap = new ESRAttributeParser ();
            const string attIdentifier = "esr";

            var v = ap.ParsedAttribute (attIdentifier,
                                        null,
                                        new NParsedAttributeBracket (
                                            new IdentifierExpression ("pert"),
                                            new NParsedAttributeList (
                                                new [] {
                                                    new NParsedAttributeAtomic (new ParsedInteger (lo)),
                                                    new NParsedAttributeAtomic (new ParsedFloat (mode)),
                                                    new NParsedAttributeAtomic (new ParsedInteger (hi))
                                                })
                                           ));

            Assert.IsInstanceOf (typeof (ParsedPertDistribution), v);
            var esr = (ParsedPertDistribution)v;
            Assert.AreEqual (lo, esr.Min);
            Assert.AreEqual (mode, esr.Mode);
            Assert.AreEqual (hi, esr.Max);
        }

        [TestCase (-1, 0, 0)]
        [TestCase (0, 0, -1)]
        [TestCase (-5, -10, -20)]
        [TestCase (Int16.MinValue, 0, 0)]
        [TestCase (5, 10, 20)]
        [TestCase (Int16.MaxValue, 0, 0)]
        [TestCase (0, 0, Int16.MaxValue)]
        public void TestInvalidPERTInteger (int lo, int mode, int hi)
        {
            var ap = new ESRAttributeParser ();
            const string attIdentifier = "esr";

            var e = Assert.Catch (() => {
                ap.ParsedAttribute (attIdentifier,
                                            null,
                                            new NParsedAttributeBracket (
                                                new IdentifierExpression ("pert"),
                                            new NParsedAttributeList (
                                                new [] {
                                                    new NParsedAttributeAtomic (new ParsedInteger (lo)),
                                                    new NParsedAttributeAtomic (new ParsedFloat (mode)),
                                                    new NParsedAttributeAtomic (new ParsedInteger (hi))
                                                })
                                               ));
            });

            Assert.IsInstanceOf (typeof (InvalidAttributeValueException), e);
            StringAssert.AreEqualIgnoringCase (
                string.Format (InvalidAttributeValueException.INVALID_VALUE, attIdentifier),
                e.Message
            );
        }

        [TestCase (0.1, .4, 1)]
        [TestCase (0, .4, 1)]
        [TestCase (0.5, .5, .5)]
        [TestCase (0, .05, .1)]
        [TestCase (0, 0, 1)]
        [TestCase (0, 1, 1)]
        public void TestPERTPercentage (double lo, double mode, double hi)
        {
            var ap = new ESRAttributeParser ();
            const string attIdentifier = "esr";

            var v = ap.ParsedAttribute (attIdentifier,
                                        null,
                                        new NParsedAttributeBracket (
                                            new IdentifierExpression ("pert"),
                                            new NParsedAttributeList (
                                                new [] {
                                                    new NParsedAttributeAtomic (new ParsedPercentage (lo)),
                                                    new NParsedAttributeAtomic (new ParsedPercentage (mode)),
                                                    new NParsedAttributeAtomic (new ParsedPercentage (hi))
                                                })
                                           ));

            Assert.IsInstanceOf (typeof (ParsedPertDistribution), v);
            var esr = (ParsedPertDistribution)v;
            Assert.AreEqual (lo / 100d, esr.Min);
            Assert.AreEqual (mode / 100d, esr.Mode);
            Assert.AreEqual (hi / 100d, esr.Max);
        }

        [TestCase (-1, 0, 0)]
        [TestCase (0, 0, -1)]
        [TestCase (-5, -10, -20)]
        [TestCase (Int16.MinValue, 0, 0)]
        [TestCase (5, 10, 20)]
        [TestCase (Int16.MaxValue, 0, 0)]
        [TestCase (0, 0, Int16.MaxValue)]
        [TestCase (Double.MinValue, 0, 0)]
        [TestCase (0, 1, Double.MaxValue)]
        [TestCase (Double.MaxValue, 0, 0)]
        public void TestInvalidPERTPercentage (double lo, double mode, double hi)
        {
            var ap = new ESRAttributeParser ();
            const string attIdentifier = "esr";

            var e = Assert.Catch (() => {
                ap.ParsedAttribute (attIdentifier,
                                            null,
                                            new NParsedAttributeBracket (
                                                new IdentifierExpression ("pert"),
                                            new NParsedAttributeList (
                                                new [] {
                                                    new NParsedAttributeAtomic (new ParsedPercentage (lo)),
                                                    new NParsedAttributeAtomic (new ParsedFloat (mode)),
                                                    new NParsedAttributeAtomic (new ParsedPercentage (hi))
                                                })
                                               ));
            });

            Assert.IsInstanceOf (typeof (InvalidAttributeValueException), e);
            StringAssert.AreEqualIgnoringCase (
                string.Format (InvalidAttributeValueException.INVALID_VALUE, attIdentifier),
                e.Message
            );
        }

        [Test ()]
        public void TestInvalidPERTParameter ()
        {
            TestInvalidPERTParameterHelper (new [] {
                                                    new NParsedAttributeAtomic (new ParsedString ("")),
                                                    new NParsedAttributeAtomic (new ParsedString ("")),
                                                    new NParsedAttributeAtomic (new ParsedFloat (0))
                                                });

            TestInvalidPERTParameterHelper (new [] {
                                                    new NParsedAttributeAtomic (new ParsedString ("")),
                                                    new NParsedAttributeAtomic (new ParsedFloat (0))
                                                });

            TestInvalidPERTParameterHelper (new [] {
                                                    new NParsedAttributeAtomic (new ParsedFloat (0)),
                                                    new NParsedAttributeAtomic (new ParsedFloat (0))
                                                });

            TestInvalidPERTParameterHelper (new [] {
                                                    new NParsedAttributeAtomic (new ParsedFloat (0)),
                                                });
        }

        public void TestInvalidPERTParameterHelper (IEnumerable<ParsedElement> v)
        {
            var ap = new ESRAttributeParser ();
            const string attIdentifier = "esr";

            var e = Assert.Catch (() => {
                ap.ParsedAttribute (attIdentifier,
                                            null,
                                            new NParsedAttributeBracket (
                                                new IdentifierExpression ("pert"),
                                                new NParsedAttributeList (v)
                                               ));
            });

            Assert.IsInstanceOf (typeof (InvalidAttributeValueException), e);
            StringAssert.AreEqualIgnoringCase (
                string.Format (InvalidAttributeValueException.INVALID_VALUE, attIdentifier),
                e.Message
            );
        }

        #endregion

        #region Single-value ESR

        [TestCase (0.00, 0)]
        [TestCase (0.1, .1)]
        [TestCase (1, 1)]
        [TestCase (.99, .99)]
        public void TestFloat (double i, double e)
        {
            var ap = new ESRAttributeParser ();
            const string attIdentifier = "esr";


            var v = ap.ParsedAttribute (attIdentifier,
                                        null,
                                        new NParsedAttributeAtomic (new ParsedFloat (i)));

            Assert.IsInstanceOf (typeof (ParsedProbabilityAttribute), v);
            var rsr = (ParsedProbabilityAttribute)v;
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
            var ap = new ESRAttributeParser ();
            const string attIdentifier = "esr";

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
            var ap = new ESRAttributeParser ();
            const string attIdentifier = "esr";


            var v = ap.ParsedAttribute (attIdentifier,
                                        null,
                                        new NParsedAttributeAtomic (new ParsedInteger (i)));

            Assert.IsInstanceOf (typeof (ParsedProbabilityAttribute), v);
            var rsr = (ParsedProbabilityAttribute)v;
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
            var ap = new ESRAttributeParser ();
            const string attIdentifier = "esr";

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
            var ap = new ESRAttributeParser ();
            const string attIdentifier = "esr";


            var v = ap.ParsedAttribute (attIdentifier,
                                        null,
                                        new NParsedAttributeAtomic (new ParsedPercentage (p)));

            Assert.IsInstanceOf (typeof (ParsedProbabilityAttribute), v);
            var rsr = (ParsedProbabilityAttribute)v;
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
            var ap = new ESRAttributeParser ();
            const string attIdentifier = "esr";

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

        #endregion

        [Test]
        public void TestParameterNotIdentifier ()
        {
            var ap = new ESRAttributeParser ();
            const string attIdentifier = "esr";

            var e = Assert.Catch (() => {
                ap.ParsedAttribute (attIdentifier,
                                   new NParsedAttributeAtomic (new ParsedFloat ()),
                                   new NParsedAttributeAtomic (new ParsedFloat (.9)));
            });

            Assert.IsInstanceOf (typeof (InvalidParameterAttributeException), e);
            StringAssert.AreEqualIgnoringCase (
                string.Format (InvalidParameterAttributeException.IDENTIFIER, attIdentifier),
                e.Message
            );
        }

        [Test]
        public void TestParameterNotAtomic ()
        {
            var ap = new ESRAttributeParser ();
            const string attIdentifier = "esr";

            var e = Assert.Catch (() => {
                ap.ParsedAttribute (attIdentifier,
                                    new NParsedAttributeColon (),
                                    new NParsedAttributeAtomic (new ParsedFloat (.9)));
            });

            Assert.IsInstanceOf (typeof (InvalidParameterAttributeException), e);
            StringAssert.AreEqualIgnoringCase (
                string.Format (InvalidParameterAttributeException.ATOMIC_ONLY, attIdentifier),
                e.Message
            );
        }

        [Test]
        public void TestInvalidAtomicValue ()
        {
            var ap = new ESRAttributeParser ();
            const string attIdentifier = "esr";

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
        public void TestListValue ()
        {
            TestNotAtomicValue (new NParsedAttributeList (Enumerable.Empty<ParsedElement> ()));
        }

        public void TestNotAtomicValue (NParsedAttributeValue v)
        {
            var ap = new ESRAttributeParser ();
            const string attIdentifier = "esr";

            var e = Assert.Catch (() => {
                ap.ParsedAttribute (attIdentifier,
                                   null,
                                   v);
            });

            Assert.IsInstanceOf (typeof (InvalidAttributeValueException), e);
            StringAssert.AreEqualIgnoringCase (
                string.Format (InvalidAttributeValueException.ATOMIC_OR_BRACKET, attIdentifier),
                e.Message
            );
        }
    }
}
