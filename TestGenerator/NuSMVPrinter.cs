using System;
using KAOSTools.MetaModel;

namespace TestGenerator
{
	public class NuSMVPrinter
	{
		public NuSMVPrinter ()
		{
		}

        public static string ToNuSMV (Formula f)
		{
            if (f is StrongImply) {
                var f2 = (StrongImply)f;
                return string.Format ("G({0} -> {1})", ToNuSMV (f2.Left), ToNuSMV (f2.Right));

            } else if (f is Imply) {
				return ToNuSMV ((Imply)f);
			} else if (f is And) {
				return ToNuSMV ((And)f);
			} else if (f is Or) {
				return ToNuSMV ((Or)f);
			} else if (f is Not) {
                return ToNuSMV ((Not)f);
            } else if (f is PredicateReference) {
                return ToNuSMV ((PredicateReference)f);
			} else if (f is Globally) {
				return ToNuSMV ((Globally)f);
            } else if (f is Eventually) {
                return ToNuSMV ((Eventually)f);
			} else if (f is Next) {
				return ToNuSMV ((Next)f);
			} else if (f is Until) {
				return ToNuSMV ((Until)f);
            } else if (f is Tick) {
                return "(Label = tick)";
            }

			Console.WriteLine ("-" + f + "-");
			throw new NotImplementedException (f.GetType () + " is not supported");
		}

		public static string ToNuSMV (Imply f)
		{
			return string.Format ("({0} -> {1})", ToNuSMV (f.Left), ToNuSMV (f.Right));
		}

		public static string ToNuSMV (And f)
		{
			return string.Format ("({0} & {1})", ToNuSMV (f.Left), ToNuSMV (f.Right));
		}

		public static string ToNuSMV (Or f)
		{
			return string.Format ("({0} | {1})", ToNuSMV (f.Left), ToNuSMV (f.Right));
		}

		public static string ToNuSMV (Not f)
		{
//            if (f.Enclosed is PredicateReference) {
//                var v = (Predicate)f.Enclosed;
//				if (v.Equals ("tick")) {
//					return "Label != tick";
//				}
//			}

			return string.Format ("!{0}", ToNuSMV (f.Enclosed));
		}

        public static string ToNuSMV (PredicateReference f)
		{
//			if (f.Name.Equals ("tick")) {
//				return "(Label = tick)";

//			} else {
            return string.Format ("{0}", f.Predicate.FriendlyName);
//			}
		}

		public static string ToNuSMV (Globally f)
		{
            return string.Format ("G {0}", ToNuSMV (f.Enclosed));
		}

        public static string ToNuSMV (Eventually f)
		{
            return string.Format ("F {0}", ToNuSMV (f.Enclosed));
		}

		public static string ToNuSMV (Next f)
		{
            return string.Format ("X {0}", ToNuSMV (f.Enclosed));
		}

		public static string ToNuSMV (Until f)
		{
            return string.Format ("({0} U {1})", ToNuSMV (f.Left), ToNuSMV (f.Right));
		}
	}
}

