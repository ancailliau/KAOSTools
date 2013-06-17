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
        private const string file = "../Examples/bcms/Z-whole-model.kaos";

        private static ModelBuilder parser;
        private static string code;
        private static KAOSModel model;

        static HomeController ()
        {
            Console.WriteLine ("Init");

            if (!System.IO.File.Exists(Path.Combine("Examples", file))) {
                throw new FileNotFoundException ();
            }

            code = System.IO.File.ReadAllText (Path.Combine("Examples", file));
            parser = new ModelBuilder ();

            model = parser.Parse (code, Path.Combine("Examples", file));
            model.GoalModel.IntegrateResolutions ();

            Console.WriteLine ("End of init");
        }

        public ActionResult Index ()
        {
            return RedirectToAction ("GoalModel");
        }

        public ActionResult GoalModel ()
        {
            return View (new KAOSModelPage {
                Code = code,
                Model = model,
                Declarations = parser.Declarations
            });
        }

        public ActionResult AgentModel ()
        {
            return View (new KAOSModelPage {
                Code = code,
                Model = model,
                Declarations = parser.Declarations
            });
        }

        public ActionResult ObjectModel ()
        {
            return View (new KAOSModelPage {
                Code = code,
                Model = model,
                Declarations = parser.Declarations
            });
        }

    }
}

