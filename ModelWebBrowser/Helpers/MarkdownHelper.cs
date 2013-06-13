using System;
using KAOSTools.MetaModel;
using System.Web.Mvc;
using System.Text;
using System.Linq;
using anrControls;

namespace ModelWebBrowser.Helpers
{
    public static class MarkdownHelper
    {
        private static Markdown mk = new Markdown();

        public static MvcHtmlString CompileMarkdown (string str)
        {
            return MvcHtmlString.Create (mk.Transform (str));
        }
    }
}

