using System;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using NDesk.Options;
using KAOSTools.MetaModel;
using KAOSTools.Utils;
using KAOSTools.OmnigraffleExport.Omnigraffle;
using System.Text;

namespace KAOSTools.OmnigraffleExport
{
    class MainClass : KAOSToolCLI
    {
        static Dictionary<Omnigraffle.Sheet, Dictionary<string, Omnigraffle.ShapedGraphic>> mapping;
        static ExportOptions exportOptions;

        private static int _i = 1;
        private static int NextId {
            get {
                return _i++;
            }
        }

        public static void Main (string[] args)
        {
            string filename = "";
            bool experimental = false;
            
            exportOptions = new ExportOptions ();

            options.Add ("o|output=", "Export in specified filename", v => filename = v);
            options.Add ("experimental", "Export experimental diagrams", v => experimental = true);
            options.Add ("i|identifier", "Display identifier in diagrams", v => exportOptions.DisplayIdentifiers = true);

            try {
            Init (args);
            if (model == null) 
                return;

            mapping = new Dictionary<Omnigraffle.Sheet, Dictionary<string, Omnigraffle.ShapedGraphic>> ();

            var document   = new Omnigraffle.Document ();

            var canvas = new Omnigraffle.Sheet (1, string.Format ("Model"));
            var shapes = new Dictionary<string, IList<Graphic>> ();

            var u = new GoalModelGenerator (canvas, shapes);
                u.Render (model);
                // var u2 = new ObstacleDiagramGenerator (canvas, shapes);
                // u2.Render (model);

            document.Canvas.Add (canvas);

                int i = 0;
                foreach (var o in model.Obstructions ().Select (x => x.Obstacle ())) {
                    var s = new Omnigraffle.Sheet (1, string.Format ("Obstacle diagram " + (i++)));
                    var u3 = new ObstacleDiagramGenerator (s, new Dictionary<string, IList<Graphic>> ());
                    u3.Render (o, model);
                    document.Canvas.Add (s);
                }

                i = 0;
                foreach (var goalWithException in model.Exceptions ().Select (x => x.AnchorGoal ())
                    .Union (model.Replacements ().Select (x => x.ResolvingGoal ()))
                    .Union (model.ObstacleAssumptions ().Select (x => x.Anchor ()))
                    .Distinct ()) {
                    var s = new Omnigraffle.Sheet (1, string.Format ("Exception diagram " + (i++)));
                    var u3 = new ExceptionDiagramGenerator (s, new Dictionary<string, IList<Graphic>> ());
                    u3.Render (goalWithException, model);
                    document.Canvas.Add (s);
                }
                /*
                Console.WriteLine ("<pre>");

                var lala0 = model.Obstacle (x => x.Name == "O");
                Console.WriteLine (string.Join(";", lala0.Refinements ().SelectMany (x => x.SubObstacles()).Select (x => x.FriendlyName)));
                Console.WriteLine (string.Join(";", lala0.Resolutions().Select (x => x.ResolvingGoal()).Select (x => x.FriendlyName)));
                Console.WriteLine (string.Join(",", lala0.Obstacles ().Select (x => x.FriendlyName)));

                Console.WriteLine ("</pre>");

                Console.WriteLine ("<pre>");
                foreach (var g in model.Goals().Where (x => x.Exceptions ().Count () > 0).OrderByDescending (x => x.Exceptions ().Count ()).ToArray()) {
                    Console.WriteLine (g.FriendlyName + " : Exceptions = " + g.Exceptions().Count () );
                }
                Console.WriteLine ("----");

                foreach (var g in model.Goals().Where (x => x.Provided ().Count () > 0).OrderByDescending (x => x.Provided ().Count ()).ToArray()) {
                    Console.WriteLine (g.FriendlyName + " : Provided = " + g.Provided().Count () );
                }
                Console.WriteLine ("</pre>");
*/
            if (string.IsNullOrEmpty (filename)) 
                OmniGraffleGenerator.Export (document, Console.Out);
            else 
                OmniGraffleGenerator.Export (document, filename);
            } catch (Exception e) {
                Console.WriteLine ("<html>");
                Console.WriteLine ("<body>");

                Console.WriteLine ("<h1>" + e.Message + "</h1>");
                Console.WriteLine ("<pre>" + e.StackTrace + "</pre>");

                Console.WriteLine ("</body>");
                Console.WriteLine ("</html>");
            }
        }
    }
}
