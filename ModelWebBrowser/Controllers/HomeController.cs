using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using ModelWebBrowser.Models;
using UCLouvain.KAOSTools.Parsing;
using System.IO;
using UCLouvain.KAOSTools.Core;

namespace ModelWebBrowser.Controllers
{
    public class HomeController : Controller
    {
        // private const string file = "/Users/acailliau/Dropbox/PhD/2013/Dependent obstacles/carpooling.kaos";
        // private const string file =  "../Examples/bcms/Z-whole-model.kaos";
        // private const string file =  "/Users/acailliau/Dropbox/PhD/2014/Papers/RE14/examples.kaos";
        private const string file =  "/Users/acailliau/Dropbox/PhD/2014/Papers/RE14/Models/las.kaos";

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
            model.IntegrateResolutions ();

            Console.WriteLine ("End of init");
        }

        public ActionResult Index ()
        {
            return View();
            //return RedirectToAction ("GoalModel");
        }

        public ActionResult GoalModel ()
        {
            return View (new KAOSModelPage {
                Code = code,
                Model = model
            });
        }
        /*
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
*/
    }
}

