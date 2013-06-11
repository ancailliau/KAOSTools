using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using ModelWebBrowser.Models;
using KAOSTools.Parsing;
using System.IO;
using KAOSTools.MetaModel;

namespace ModelWebBrowser.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index (string example = "las.kaos")
        {
            if (!System.IO.File.Exists(Path.Combine("Examples", example))) {
                throw new FileNotFoundException ();
            }

            var code = System.IO.File.ReadAllText (Path.Combine("Examples", example));
            var parser = new ModelBuilder ();

            var model = parser.Parse (code);
            model.GoalModel.IntegrateResolutions ();

            return View (new IndexModel {
                Code = code,
                Model = model
            });
        }
    }
}

