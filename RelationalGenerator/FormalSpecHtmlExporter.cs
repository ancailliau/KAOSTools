using System;
using KAOSTools.MetaModel;
using System.Text;
using System.Linq;

namespace KAOSTools.ReportGenerator
{
    public class FormalSpecHtmlExporter
    {
        public static string ToHtmlString (Formula expression)
        {
            if (expression == null)
                return null;

            dynamic d = expression;
            return ToHtmlString(d);
        }

        private static string ToHtmlString (ComparisonPredicate expression)
        {
            var comparator = "";
            if (expression.Criteria == ComparisonCriteria.Equals)
                comparator = "=";
            
            if (expression.Criteria == ComparisonCriteria.BiggerThan)
                comparator = ">";
            
            if (expression.Criteria == ComparisonCriteria.BiggerThanOrEquals)
                comparator = "&ge;";
            
            if (expression.Criteria == ComparisonCriteria.LessThan)
                comparator = "<";
            
            if (expression.Criteria == ComparisonCriteria.LessThanOrEquals)
                comparator = "&le;";
            
            if (expression.Criteria == ComparisonCriteria.NotEquals)
                comparator = "&ne;";

            return string.Format ("{0} {1} {2}", 
                                 Parenthesize (expression, expression.Left, ToHtmlString (expression.Left)),
                                 comparator,
                                 Parenthesize (expression, expression.Right, ToHtmlString (expression.Right))
                                 );
        }

        private static string ToHtmlString (Not expression)
        {
            return string.Format ("&not; {0}", 
                                 Parenthesize (expression, expression.Enclosed, ToHtmlString (expression.Enclosed))
                                 );
        }

        private static string ToHtmlString (Next expression)
        {
            return string.Format("&cir; {0}",
                                 Parenthesize(expression, expression.Enclosed, ToHtmlString(expression.Enclosed))
                                 );
        }

        private static string ToHtmlString (Unless expression)
        {
            return string.Format("{0} W {1}", 
                                 Parenthesize(expression, expression.Left, ToHtmlString(expression.Left)),
                                 Parenthesize(expression, expression.Right, ToHtmlString(expression.Right))
                                 );
        }

        private static string ToHtmlString (Until expression)
        {
            return string.Format("{0} U {1}", 
                                 Parenthesize(expression, expression.Left, ToHtmlString(expression.Left)),
                                 Parenthesize(expression, expression.Right, ToHtmlString(expression.Right))
                                 );
        }

        private static string ToHtmlString (Release expression)
        {
            return string.Format("{0} R {1}", 
                                 Parenthesize(expression, expression.Left, ToHtmlString(expression.Left)),
                                 Parenthesize(expression, expression.Right, ToHtmlString(expression.Right))
                                 );
        }

        private static string ToHtmlString (And expression)
        {
            return string.Format("{0} &and; {1}", 
                                 Parenthesize(expression, expression.Left, ToHtmlString(expression.Left)),
                                 Parenthesize(expression, expression.Right, ToHtmlString(expression.Right))
                                 );
        }
        
        private static string ToHtmlString (Or expression)
        {
            return string.Format("{0} &or; {1}", 
                                 Parenthesize(expression, expression.Left, ToHtmlString(expression.Left)),
                                 Parenthesize(expression, expression.Right, ToHtmlString(expression.Right))
                                 );
        }

        private static string ToHtmlString (Equivalence expression)
        {
            return string.Format("{0} &leftrightarrow; {1}", 
                                 Parenthesize(expression, expression.Left, ToHtmlString(expression.Left)),
                                 Parenthesize(expression, expression.Right, ToHtmlString(expression.Right))
                                 );
        }

        private static string ToHtmlString (Imply expression)
        {
            return string.Format("{0} &rightarrow; {1}", 
                                 Parenthesize(expression, expression.Left, ToHtmlString(expression.Left)),
                                 Parenthesize(expression, expression.Right, ToHtmlString(expression.Right))
                                 );
        }


        private static string ToHtmlString (Forall expression)
        {
            return string.Format("&forall; {0} &middot; {1}", 
                                 string.Join(", ", expression.Declarations.Select (variable => string.Format ("{0}:{1}", variable.Name, variable.Type.Identifier))),
                                 Parenthesize(expression, expression.Enclosed, ToHtmlString(expression.Enclosed))
                                 );
        }
        
        private static string ToHtmlString (Exists expression)
        {
            return string.Format("&exist; {0} &middot; {1}", 
                                                       string.Join(", ", expression.Declarations.Select (variable => string.Format ("{0}:{1}", variable.Name, variable.Type.Identifier))),
                                                       Parenthesize(expression, expression.Enclosed, ToHtmlString(expression.Enclosed))
                                                       );
        }

        private static string ToHtmlString (StrongImply expression)
        {
            return string.Format("{0} &#8658; {1}", 
                                 Parenthesize(expression, expression.Left, ToHtmlString(expression.Left)),
                                 Parenthesize(expression, expression.Right, ToHtmlString(expression.Right))
            );
        }

        private static string ToHtmlString (PredicateReference expression)
        {
            return string.Format("{0}({1})",
                                 expression.Predicate.Identifier,
                                 string.Join(", ", expression.ActualArguments));
        }

        private static string ToHtmlString (VariableReference expression)
        {
            return string.Format("{0}", expression.Name);
        }


        private static string ToHtmlString (AttributeReference expression)
        {
            return string.Format("{1}.{0}", 
                                 expression.Attribute.FriendlyName,
                                 expression.Variable
                                 );
        }

        private static string ToHtmlString (RelationReference expression)
        {
            return string.Format("({1}) &isin; {0}", 
                                 expression.Relation.Identifier,
                                 string.Join(", ", expression.ActualArguments)
                                 );
        }

        private static string ToHtmlString (EventuallyBefore expression)
        {
            var bound = GetTimeBound (expression.TimeBound);
            return string.Format("&not; {1} W{2} ({0} &and; &not; {1})", 
                                 Parenthesize(expression, expression.Left, ToHtmlString(expression.Left)),
                                 Parenthesize(expression, expression.Right, ToHtmlString(expression.Right)),
                                 bound
                                 );
        }

        private static string GetTimeBound (TimeBound expression)
        {
            string bound = "";
            if (expression != null) {
                bound = "<sub>";
                if (expression.Comparator == TimeComparator.equal)
                    bound += "=";
                if (expression.Comparator == TimeComparator.less)
                    bound += "&le;";
                if (expression.Comparator == TimeComparator.strictly_less)
                    bound += "<";
                if (expression.Comparator == TimeComparator.greater)
                    bound += "&ge;";
                if (expression.Comparator == TimeComparator.strictly_greater)
                    bound += ">";
                string format = "";
                if (expression.Bound.Days >= 1)
                    format += "d'd '";
                if (expression.Bound.Hours >= 1)
                    format += "h'h '";
                if (expression.Bound.Minutes >= 1)
                    format += "m'm '";
                if (expression.Bound.Seconds >= 1)
                    format += "s's '";
                if (expression.Bound.Milliseconds >= 1)
                    format += "fffffff'ms '";
                bound += expression.Bound.ToString (format);
                bound += "</sub>";
            }
            return bound;
        }

        private static string ToHtmlString (Eventually expression)
        {
            var bound = GetTimeBound (expression.TimeBound);
            return string.Format("&loz;{0} {1}", 
                                 bound,
                                 ToHtmlString(expression.Enclosed)
                                 );
        }

        private static string ToHtmlString (Globally expression)
        {
            var bound = GetTimeBound (expression.TimeBound);
            return string.Format("&square;{0} {1}", 
                                 bound,
                                 ToHtmlString(expression.Enclosed)
                                 );
        }

        private static string ToHtmlString (StringConstant expression)
        {
            return string.Format("\"{0}\"", expression.Value);
        }

        private static string ToHtmlString (BoolConstant expression)
        {
            return string.Format("{0}", expression.Value ? "<strong>true</strong>" : "<strong>false</strong>");
        }

        private static string ToHtmlString (NumericConstant expression)
        {
            return string.Format("{0}", expression.Value);
        }

        private static string Parenthesize (Formula fout, Formula enclosed, string str) {
            if (fout is ComparisonCriteria && !(enclosed is BoolConstant 
                                               | enclosed is NumericConstant 
                                               | enclosed is StringConstant 
                                               | enclosed is AttributeReference 
                                               | enclosed is VariableReference
                                               | enclosed is RelationReference
                                               | enclosed is PredicateReference)) {
                return string.Format("({0})", str);
            }

            if ((fout is Globally 
                 | fout is Eventually 
                 | fout is Next 
                 | fout is Not
                 && !(enclosed is ComparisonCriteria 
                      | enclosed is BoolConstant 
                      | enclosed is NumericConstant 
                      | enclosed is StringConstant 
                      | enclosed is AttributeReference 
                      | enclosed is VariableReference
                      | enclosed is RelationReference
                      | enclosed is PredicateReference))) {
                return string.Format("({0})", str);
            }

            if ((fout is EventuallyBefore 
                 | fout is And 
                 | fout is Or 
                 | fout is Unless
                 | fout is Release
                 | fout is Until
                 && !(enclosed is Globally 
                 | enclosed is Eventually 
                 | enclosed is Next 
                 | enclosed is Not
                 | enclosed is ComparisonCriteria 
                 | enclosed is BoolConstant 
                 | enclosed is NumericConstant 
                 | enclosed is StringConstant 
                 | enclosed is AttributeReference 
                 | enclosed is VariableReference
                 | enclosed is RelationReference
                 | enclosed is PredicateReference))) {
                return string.Format("({0})", str);
            }

            if ((fout is Equivalence 
                 | fout is Imply 
                 | fout is StrongImply
                 && !(enclosed is EventuallyBefore 
                 | enclosed is And 
                 | enclosed is Or 
                 | enclosed is Unless
                 | enclosed is Release
                 | enclosed is Until
                 | enclosed is Globally 
                 | enclosed is Eventually 
                 | enclosed is Next 
                 | enclosed is Not
                 | enclosed is ComparisonCriteria 
                 | enclosed is BoolConstant 
                 | enclosed is NumericConstant 
                 | enclosed is StringConstant 
                 | enclosed is AttributeReference 
                 | enclosed is VariableReference
                 | enclosed is RelationReference
                 | enclosed is PredicateReference))) {
                return string.Format("({0})", str);
            }
            
            if ((fout is Forall 
                 | fout is Exists
                 && !(enclosed is Equivalence 
                 | enclosed is Imply 
                 | enclosed is StrongImply
                 | enclosed is EventuallyBefore 
                 | enclosed is And 
                 | enclosed is Or 
                 | enclosed is Unless
                 | enclosed is Release
                 | enclosed is Until
                 | enclosed is Globally 
                 | enclosed is Eventually 
                 | enclosed is Next 
                 | enclosed is Not
                 | enclosed is ComparisonCriteria 
                 | enclosed is BoolConstant 
                 | enclosed is NumericConstant 
                 | enclosed is StringConstant 
                 | enclosed is AttributeReference 
                 | enclosed is VariableReference
                 | enclosed is RelationReference
                 | enclosed is PredicateReference))) {
                return string.Format("({0})", str);
            }

            return str;
        }
    }
}

