using NUnit.Framework;
using System;
using System.IO;
using System.Collections.Generic;

namespace BoolSimplify.Tests
{
    [TestFixture()]
    public class Test
    {
        [Test()]
        public void T1 ()
        {
            var str = @"000
010
011
110
101
111";
            Formula f = FormulaFromString (new StringReader (str));
            f.ReduceToPrimeImplicants ();
            f.ReducePrimeImplicantsToSubset ();

            Assert.AreEqual (3, f.Terms.Count);
            Assert.AreEqual (3, f.Terms[0].NumVars);
        }

        [Test()]
        public void T2 ()
        {
            var str = @"0000
0001
0010
0011
0101
0111
1000
1010
1100
1101
1111";

            Formula f = FormulaFromString (new StringReader (str));
            f.ReduceToPrimeImplicants ();
            f.ReducePrimeImplicantsToSubset ();

            Assert.AreEqual (4, f.Terms.Count);
            Assert.AreEqual (4, f.Terms[0].NumVars);
        }

        [Test()]
        public void T3 ()
        {
            var str = @"0110
1110
1111
0111
1001
1101
1011
1111";
            Formula f = FormulaFromString (new StringReader (str));
            f.ReduceToPrimeImplicants ();
            f.ReducePrimeImplicantsToSubset ();

            Assert.AreEqual (2, f.Terms.Count);
            Assert.AreEqual (4, f.Terms[0].NumVars);
        }

        [Test()]
        public void T4 ()
        {
            var str = @"1010
1110
0110
0010
1000
0000
1111";
            Formula f = FormulaFromString (new StringReader (str));
            f.ReduceToPrimeImplicants ();
            f.ReducePrimeImplicantsToSubset ();

            Assert.AreEqual (3, f.Terms.Count);
            Assert.AreEqual (4, f.Terms[0].NumVars);
        }

        static Term readTerm (StringReader reader)
        {
            int c = '\0';
            List<byte> t = new List<byte> ();
            while (c != '\n' && c != -1) {
                c = reader.Read ();
                if (c == '0') {
                    t.Add ((byte)0);
                } else if (c == '1') {
                    t.Add ((byte)1);
                }
            }
            if (t.Count > 0) {
                byte[] resultBytes = new byte[t.Count];
                for (int i=0; i<t.Count; i++) {
                    resultBytes [i] = (byte)t [i];
                }
                return new Term (resultBytes);
            } else {
                return null;
            }
        }

        static Formula FormulaFromString (StringReader reader)
        {
            List<Term> terms = new List<Term> ();
            Term term;
            while ((term = readTerm(reader)) != null) {
                terms.Add (term);
            }
            return new Formula (terms);
        }
    }
}

