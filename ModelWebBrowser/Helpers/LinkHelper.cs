using System;
using System.Web.Mvc;
using KAOSTools.MetaModel;
using System.Web.Mvc.Html;

namespace ModelWebBrowser.Helpers
{
    public static class LinkHelper
    {
        public static MvcHtmlString GetLink (this HtmlHelper helper, Goal goal)
        {
            return helper.ActionLink (goal.FriendlyName, "GoalModel");
        }

        public static MvcHtmlString GetLink (this HtmlHelper helper, AntiGoal antigoal)
        {
            return helper.ActionLink (antigoal.FriendlyName, "GoalModel");
        }

        public static MvcHtmlString GetLink (this HtmlHelper helper, Obstacle obstacle)
        {
            return helper.ActionLink (obstacle.FriendlyName, "GoalModel");
        }

        public static MvcHtmlString GetLink (this HtmlHelper helper, DomainProperty domprop)
        {
            return helper.ActionLink (domprop.FriendlyName, "GoalModel");
        }

        public static MvcHtmlString GetLink (this HtmlHelper helper, DomainHypothesis domhyp)
        {
            return helper.ActionLink (domhyp.FriendlyName, "GoalModel");
        }

        public static MvcHtmlString GetLink (this HtmlHelper helper, Agent agent)
        {
            return helper.ActionLink (agent.FriendlyName, "GoalModel");
        }
        
        public static MvcHtmlString GetLink (this HtmlHelper helper, Entity entity)
        {
            return helper.ActionLink (entity.FriendlyName, "GoalModel");
        }
        
        public static MvcHtmlString GetLink (this HtmlHelper helper, Relation relation)
        {
            return helper.ActionLink (relation.FriendlyName, "GoalModel");
        }
        
        public static MvcHtmlString GetLink (this HtmlHelper helper, GivenType type)
        {
            return helper.ActionLink (type.FriendlyName, "GoalModel");
        }

        public static MvcHtmlString GetLink (this HtmlHelper helper, Predicate predicate)
        {
            return helper.ActionLink (predicate.FriendlyName, "GoalModel");
        }
    }
}

