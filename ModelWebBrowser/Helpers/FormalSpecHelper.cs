using System;
using KAOSTools.Core;
using System.Web.Mvc;
using System.Text;
using System.Linq;

namespace ModelWebBrowser.Helpers
{
    public static class FormalSpecHelper
    {
        public static MvcHtmlString ToHtmlString (this Formula expression, bool link = true)
        {
            if (expression == null)
                return null;

            dynamic d = expression;
            return ToHtmlString(d, link);
        }

        public static MvcHtmlString ToHtmlString (ComparisonPredicate expression, bool link = true)
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

            return MvcHtmlString.Create (string.Format("{0} {1} {2}", 
                                                       Embed(expression, expression.Left, expression.Left.ToHtmlString(link)),
                                                       comparator,
                                                       Embed(expression, expression.Right, expression.Right.ToHtmlString(link))
                                                       ));
        }

        public static MvcHtmlString ToHtmlString ( Not expression, bool link = true)
        {
            return MvcHtmlString.Create (string.Format("&not; {0}", 
                                                       Embed(expression, expression.Enclosed, expression.Enclosed.ToHtmlString(link))
                                                       ));
        }

        public static MvcHtmlString ToHtmlString ( Next expression, bool link = true)
        {
            return MvcHtmlString.Create (string.Format("&cir; {0}", 
                                                       Embed(expression, expression.Enclosed, expression.Enclosed.ToHtmlString(link))
                                                       ));
        }

        public static MvcHtmlString ToHtmlString ( Unless expression, bool link = true)
        {
            return MvcHtmlString.Create (string.Format("{0} W {1}", 
                                                       Embed(expression, expression.Left, expression.Left.ToHtmlString(link)),
                                                       Embed(expression, expression.Right, expression.Right.ToHtmlString(link))
                                                       ));
        }

        public static MvcHtmlString ToHtmlString ( Until expression, bool link = true)
        {
            return MvcHtmlString.Create (string.Format("{0} U {1}", 
                                                       Embed(expression, expression.Left, expression.Left.ToHtmlString(link)),
                                                       Embed(expression, expression.Right, expression.Right.ToHtmlString(link))
                                                       ));
        }

        public static MvcHtmlString ToHtmlString ( Release expression, bool link = true)
        {
            return MvcHtmlString.Create (string.Format("{0} R {1}", 
                                                       Embed(expression, expression.Left, expression.Left.ToHtmlString(link)),
                                                       Embed(expression, expression.Right, expression.Right.ToHtmlString(link))
                                                       ));
        }

        public static MvcHtmlString ToHtmlString ( And expression, bool link = true)
        {
            return MvcHtmlString.Create (string.Format("{0} &and; {1}", 
                                                       Embed(expression, expression.Left, expression.Left.ToHtmlString(link)),
                                                       Embed(expression, expression.Right, expression.Right.ToHtmlString(link))
                                                       ));
        }
        
        public static MvcHtmlString ToHtmlString ( Or expression, bool link = true)
        {
            return MvcHtmlString.Create (string.Format("{0} &or; {1}", 
                                                       Embed(expression, expression.Left, expression.Left.ToHtmlString(link)),
                                                       Embed(expression, expression.Right, expression.Right.ToHtmlString(link))
                                                       ));
        }

        public static MvcHtmlString ToHtmlString ( Equivalence expression, bool link = true)
        {
            return MvcHtmlString.Create (string.Format("{0} &leftrightarrow; {1}", 
                                                       Embed(expression, expression.Left, expression.Left.ToHtmlString(link)),
                                                       Embed(expression, expression.Right, expression.Right.ToHtmlString(link))
                                                       ));
        }

        public static MvcHtmlString ToHtmlString ( Imply expression, bool link = true)
        {
            return MvcHtmlString.Create (string.Format("{0} &rightarrow; {1}", 
                                                       Embed(expression, expression.Left, expression.Left.ToHtmlString(link)),
                                                       Embed(expression, expression.Right, expression.Right.ToHtmlString(link))
                                                       ));
        }


        public static MvcHtmlString ToHtmlString ( Forall expression, bool link = true)
        {
            if (link)
            return MvcHtmlString.Create (string.Format("&forall; {0} &middot; {1}", 
                                                       string.Join(", ", expression.Declarations.Select (variable => string.Format ("{0}:<a href=#object-{2}>{1}</a>", variable.Name, variable.Type.Identifier, variable.Type.Identifier))),
                                                           Embed(expression, expression.Enclosed, expression.Enclosed.ToHtmlString(link))
                                                       ));

            return MvcHtmlString.Create (string.Format("&forall; {0} &middot; {1}", 
                                                       string.Join(", ", expression.Declarations.Select (variable => string.Format ("{0}:{1}", variable.Name, variable.Type.Identifier))),
                                                       Embed(expression, expression.Enclosed, expression.Enclosed.ToHtmlString(link))
                                                       ));
        }
        
        public static MvcHtmlString ToHtmlString ( Exists expression, bool link = true)
        {
            if (link)
            return MvcHtmlString.Create (string.Format("&exist; {0} &middot; {1}", 
                                                           string.Join(", ", expression.Declarations.Select (variable => string.Format ("{0}:<a href=#object-{2}>{1}</a>", variable.Name, variable.Type.Identifier, variable.Type.Identifier))),
                                                           Embed(expression, expression.Enclosed, expression.Enclosed.ToHtmlString(link))
                                                           ));

            return MvcHtmlString.Create (string.Format("&exist; {0} &middot; {1}", 
                                                       string.Join(", ", expression.Declarations.Select (variable => string.Format ("{0}:{1}", variable.Name, variable.Type.Identifier))),
                                                       Embed(expression, expression.Enclosed, expression.Enclosed.ToHtmlString(link))
                                                       ));
        }

        public static MvcHtmlString ToHtmlString ( StrongImply expression, bool link = true)
        {
            return MvcHtmlString.Create (string.Format("{0} &#8658; {1}", 
                                                       Embed(expression, expression.Left, expression.Left.ToHtmlString(link)),
                                                       Embed(expression, expression.Right, expression.Right.ToHtmlString(link))
            ));
        }

        public static MvcHtmlString ToHtmlString ( PredicateReference expression, bool link = true)
        {
            if (link)
                return MvcHtmlString.Create (string.Format("<a href=#predicate-{2}>{0}</a>({1})", 
                                                       expression.Predicate.Identifier,
                                                       string.Join(", ", expression.ActualArguments),
                                                       expression.Predicate.Identifier
                                                       ));

            return MvcHtmlString.Create (string.Format("{0}({1})", 
                                                           expression.Predicate.Identifier,
                                                           string.Join(", ", expression.ActualArguments)));
        }

        public static MvcHtmlString ToHtmlString ( VariableReference expression, bool link = true)
        {
            return MvcHtmlString.Create (string.Format("{0}", expression.Name));
        }


        public static MvcHtmlString ToHtmlString ( AttributeReference expression, bool link = true)
        {
            if (link)
            return MvcHtmlString.Create (string.Format("{1}.<a href=#attribute-{2}>{0}</a>", 
                                                       expression.Attribute.FriendlyName,
                                                       expression.Variable,
                                                       expression.Attribute.Identifier
                                                       ));

            return MvcHtmlString.Create (string.Format("{1}.{0}", 
                                                       expression.Attribute.FriendlyName,
                                                       expression.Variable
                                                       ));
        }

        public static MvcHtmlString ToHtmlString ( RelationReference expression, bool link = true)
        {
            if (link)
                return MvcHtmlString.Create (string.Format("({1}) &isin; <a href=#association-{2}>{0}</a>", 
                                                       expression.Relation.Identifier,
                                                       string.Join(", ", expression.ActualArguments),
                                                       expression.Relation.Identifier
                                                       ));

            return MvcHtmlString.Create (string.Format("({1}) &isin; {0}", 
                                                       expression.Relation.Identifier,
                                                       string.Join(", ", expression.ActualArguments)
                                                       ));
        }

        public static MvcHtmlString ToHtmlString ( EventuallyBefore expression, bool link = true)
        {
            var bound = GetTimeBound (expression.TimeBound);
            return MvcHtmlString.Create (string.Format("&not; {1} W{2} ({0} &and; &not; {1})", 
                                                       Embed(expression, expression.Left, expression.Left.ToHtmlString(link)),
                                                       Embed(expression, expression.Right, expression.Right.ToHtmlString(link)),
                                                       bound
                                                       ));
        }

        private static string GetInvertedTimeBound (TimeBound expression)
        {
            string bound = "";
            if (expression != null) {
                bound = "<sub>";
                if (expression.Comparator == TimeComparator.equal)
                    bound += "&ne;";
                if (expression.Comparator == TimeComparator.less)
                    bound += ">";
                if (expression.Comparator == TimeComparator.strictly_less)
                    bound += "&ge;";
                if (expression.Comparator == TimeComparator.greater)
                    bound += "<";
                if (expression.Comparator == TimeComparator.strictly_greater)
                    bound += "&le;";
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

        public static MvcHtmlString ToHtmlString ( Eventually expression, bool link = true)
        {
            var bound = GetTimeBound (expression.TimeBound);
            return MvcHtmlString.Create (string.Format("&loz;{0} {1}", 
                                                       bound,
                                                       expression.Enclosed.ToHtmlString(link)
                                                       ));
        }

        public static MvcHtmlString ToHtmlString ( Globally expression, bool link = true)
        {
            var bound = GetTimeBound (expression.TimeBound);
            return MvcHtmlString.Create (string.Format("&square;{0} {1}", 
                                                       bound,
                                                       expression.Enclosed.ToHtmlString(link)
                                                       ));
        }

        public static MvcHtmlString ToHtmlString ( StringConstant expression, bool link = true)
        {
            return MvcHtmlString.Create (string.Format("\"{0}\"", expression.Value));
        }
        public static MvcHtmlString ToHtmlString ( BoolConstant expression, bool link = true)
        {
            return MvcHtmlString.Create (string.Format("{0}", expression.Value ? "<strong>true</strong>" : "<strong>false</strong>"));
        }
        public static MvcHtmlString ToHtmlString ( NumericConstant expression, bool link = true)
        {
            return MvcHtmlString.Create (string.Format("{0}", expression.Value));
        }


        public static MvcHtmlString Embed (Formula fout, Formula enclosed, MvcHtmlString str) {
            if (fout is ComparisonCriteria && !(enclosed is BoolConstant 
                                               | enclosed is NumericConstant 
                                               | enclosed is StringConstant 
                                               | enclosed is AttributeReference 
                                               | enclosed is VariableReference
                                               | enclosed is RelationReference
                                               | enclosed is PredicateReference)) {
                return MvcHtmlString.Create ("(" + str + ")");
            }

            if ((fout is Globally 
                 | fout is Eventually 
                 | fout is Next 
                 | fout is Not
                 && !( enclosed is ComparisonCriteria 
                      | enclosed is BoolConstant 
                      | enclosed is NumericConstant 
                      | enclosed is StringConstant 
                      | enclosed is AttributeReference 
                      | enclosed is VariableReference
                      | enclosed is RelationReference
                      | enclosed is PredicateReference))) {
                return MvcHtmlString.Create ("(" + str + ")");
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
                return MvcHtmlString.Create ("(" + str + ")");
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
                return MvcHtmlString.Create ("(" + str + ")");
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
                return MvcHtmlString.Create ("(" + str + ")");
            }

            return str;
        }


    }
}

