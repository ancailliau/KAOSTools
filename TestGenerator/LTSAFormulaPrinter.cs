using System;
using KAOSTools.MetaModel;

namespace TestGenerator
{
    public class LTSAFormulaPrinter
    {
        public LTSAFormulaPrinter ()
        {
        }

        public static string ToString (Formula formula)
        {
            if (formula is Not) {
                return ToString ((Not)formula);
            }

            if (formula is PredicateReference) {
                return ToString ((PredicateReference)formula);
            }

            if (formula is And) {
                return ToString ((And)formula);
            }
            if (formula is Or) {
                return ToString ((Or)formula);
            }
            if (formula is Globally) {
                return ToString ((Globally)formula);
            }
            if (formula is Next) {
                return ToString ((Next)formula);
            }
            if (formula is BoolConstant) {
                return ToString ((BoolConstant)formula);
            }
            if (formula is StrongImply) {
                return ToString ((StrongImply) formula);
            }
            if (formula is Imply) {
                return ToString ((Imply)formula);
            }
            if (formula is Until) {
                return ToString ((Until)formula);
            }
            if (formula is Unless) {
                return ToString ((Unless)formula);
            }
            if (formula is Tick) {
                return ToString ((Tick)formula);
            }

            throw new NotImplementedException (formula.GetType ().Name);
        }

        public static string ToString (Tick formula)
        {
            return string.Format ("tick");
        }

        public static string ToString (Unless formula)
        {
            return string.Format ("(({0}) W ({1}))", ToString (formula.Left), ToString (formula.Right));
        }
        public static string ToString (Until formula)
        {
            return string.Format ("(({0}) U ({1}))", ToString (formula.Left), ToString (formula.Right));
        }

        public static string ToString (Imply formula)
        {
            return string.Format ("({0} -> {1})", ToString (formula.Left), ToString (formula.Right));
        }

        public static string ToString (StrongImply formula)
        {
            return string.Format ("[]({0} -> {1})", ToString (formula.Left), ToString (formula.Right));
        }

        public static string ToString (BoolConstant formula)
        {
            return string.Format ("{0}", formula.Value ? "rigid(1)" : "rigid(0)");
        }

        public static string ToString (Next formula)
        {
            return string.Format ("X({0})", ToString (formula.Enclosed));
        }

        public static string ToString (Globally formula)
        {
            return string.Format ("[]({0})", ToString (formula.Enclosed));
        }

        public static string ToString (Or formula)
        {
            return string.Format ("({0} || {1})", ToString (formula.Left), ToString (formula.Right));
        }

        public static string ToString (And formula)
        {
            return string.Format ("({0} && {1})", ToString (formula.Left), ToString (formula.Right));
        }

        public static string ToString (Not negation)
        {
            return string.Format ("!({0})", ToString (negation.Enclosed));
        }

        public static string ToString (PredicateReference predicate)
        {
            return string.Format (predicate.Predicate.FriendlyName);
        }
    }
}

