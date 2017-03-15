using System;
using System.Linq;
using KAOSTools.MetaModel;
using System.Collections.Generic;

namespace TestGenerator
{
    public static class NotExtensions 
    {
        public static Formula Negate (this Formula f) {
            if (f is Imply) {
                return new And () {
                    Left = ((Imply)f).Left,
                    Right = ((Imply)f).Right.Negate (),
                };
            }

            if (f is And) {
                return new Or () {
                    Left = ((And)f).Left.Negate (),
                    Right = ((And)f).Right.Negate (),
                };
            }

            if (f is Or) {
                return new And () {
                    Left = ((Or)f).Left.Negate (),
                    Right = ((Or)f).Right.Negate (),
                };
            }

            if (f is PredicateReference) {
                return new Not () {
                    Enclosed = f
                };
            }

            if (f is Not) {
                return ((Not)f).Enclosed;
            }

            if (f is Next) {
                return new Next { Enclosed = (((Next)f).Enclosed.Negate ()) };
            }


            throw new NotImplementedException (f.GetType ().ToString ());
        }
    }

    public static class UFCExtensions 
    {
        public static ISet<Formula> UFCpos (this Formula formula) {
            
            if (formula is Imply) {
                var f = (Imply)formula;
                return new Or () { Left = f.Left.Negate (), Right = f.Right}.UFCpos();
            }

            if (formula is Or) {
                var f = (Or)formula;
                var hashSet = new HashSet<Formula> ();
                foreach (var a in f.Left.UFCpos()) {
                    hashSet.Add (new And (a, f.Right.Negate()));
                }
                foreach (var b in f.Right.UFCpos()) {
                    hashSet.Add (new And (f.Left.Negate(), b));
                }
                return hashSet;
            }

            if (formula is And) {
                var f = (And)formula;
                var hashSet = new HashSet<Formula> ();
                foreach (var a in f.Left.UFCpos()) {
                    hashSet.Add (new And (a, f.Right));
                }
                foreach (var b in f.Right.UFCpos()) {
                    hashSet.Add (new And (f.Left, b));
                }
                return hashSet;
            }

            if (formula is StrongImply) {
                var f = (StrongImply)formula;
                return new Globally () { 
                    Enclosed = new Imply () {
                        Left = f.Left, 
                        Right = f.Right
                    }
                }.UFCpos();
            }

            if (formula is Globally) {
                var f = (Globally)formula;
                var hashSet = new HashSet<Formula> ();
                foreach (var a in f.Enclosed.UFCpos ()) {
                    hashSet.Add (new Until (f.Enclosed, new And(a, new Globally (f.Enclosed))));
                }
                return hashSet;
            }

            if (formula is PredicateReference) {
                return new HashSet<Formula> (new [] { formula });
            }

            if (formula is Not) {
                var f = (Not)formula;
                return f.Enclosed.UFCneg ();
            }

            if (formula is Next) {
                var f = (Next)formula;
                var hashSet = new HashSet<Formula> ();
                foreach (var a in f.Enclosed.UFCpos()) {
                    hashSet.Add (new Next(a));
                }
                return hashSet;
            }


            if (formula is Eventually) {
                var f = (Eventually)formula;
                var hashSet = new HashSet<Formula> ();
				var Aneg = f.Enclosed.Negate ();
                foreach (var a in f.Enclosed.UFCpos ()) {
                    hashSet.Add (new Until (Aneg, a));
                }
                return hashSet;
            }

            throw new NotImplementedException (formula.GetType ().ToString ());
        }
        public static ISet<Formula> UFCneg (this Formula  formula) {
            if (formula is Imply) {
                var f = (Imply)formula;
                return new Or () { Left = f.Left.Negate (), Right = f.Right }.UFCneg();
            }


            if (formula is Or) {
                var f = (Or)formula;
                var hashSet = new HashSet<Formula> ();
                foreach (var a in f.Left.UFCneg()) {
                    hashSet.Add (new And (a, f.Right.Negate()));
                }
                foreach (var b in f.Right.UFCneg()) {
                    hashSet.Add (new And (f.Left.Negate(), b));
                }
                return hashSet;
            }

            if (formula is And) {
                var f = (And)formula;
                var hashSet = new HashSet<Formula> ();
                foreach (var a in f.Left.UFCneg()) {
                    hashSet.Add (new And (a, f.Right));
                }
                foreach (var b in f.Right.UFCneg()) {
                    hashSet.Add (new And (f.Left, b));
                }
                return hashSet;
            }

            if (formula is PredicateReference) {
                return new HashSet<Formula> (new [] { formula.Negate() });
            }

            if (formula is Not) {
                var f = (Not)formula;
                return f.Enclosed.UFCpos ();
            }

            if (formula is Next) {
                var f = (Next)formula;
                var hashSet = new HashSet<Formula> ();
                foreach (var a in f.Enclosed.UFCneg()) {
                    hashSet.Add (new Next(a));
                }
                return hashSet;
            }

            if (formula is StrongImply) {
                var f = (StrongImply)formula;
                return new Globally () {
                    Enclosed = new Imply () {
                        Left = f.Left,
                        Right = f.Right
                    }
                }.UFCneg ();
            }

            if (formula is Globally) {
                var f = (Globally)formula;
                var hashSet = new HashSet<Formula> ();
                foreach (var a in f.Enclosed.UFCneg ()) {
                    hashSet.Add (new Until (f.Enclosed, a));
                }
                return hashSet;
            }

            throw new NotImplementedException (formula.GetType ().ToString ());
        }
    }

	public class UniqueFirstCause
	{
        public static Formula[] GeneratePositiveObligations(Formula f)
		{
            return f.UFCpos ().ToArray ();
		}
        public static Formula[] GenerateNegativeObligations(Formula f)
		{
            return f.UFCneg ().ToArray ();
		}
	}
}

