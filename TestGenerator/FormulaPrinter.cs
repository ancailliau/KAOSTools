using System;
using KAOSTools.MetaModel;

namespace TestGenerator
{
    public class FormulaPrinter
    {
        public FormulaPrinter ()
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

            throw new NotImplementedException (formula.GetType ().Name);
        }

        public static string ToString (Or formula)
        {
            return string.Format ("{0} or {1}", ToString (formula.Left), ToString (formula.Right));
        }

        public static string ToString (And formula)
        {
            return string.Format ("{0} and {1}", ToString (formula.Left), ToString (formula.Right));
        }

        public static string ToString (Not negation)
        {
            return string.Format ("not " + ToString (negation.Enclosed));
        }

        public static string ToString (PredicateReference predicate)
        {
            return string.Format (predicate.Predicate.FriendlyName + "(" + string.Join (", ", predicate.ActualArguments) + ")");
        }
    }
}

