using System;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using NDesk.Options;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Utils;
using UCLouvain.KAOSTools.OmnigraffleExport.Omnigraffle;
using System.Text;

namespace UCLouvain.KAOSTools.OmnigraffleExport
{
	public class OmnigraffleMainClass : KAOSToolCLI
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

            try
			{
				Init(args);
				if (model == null)
					return;

				Document document = ExportModel(model);

				if (string.IsNullOrEmpty(filename))
					OmniGraffleGenerator.Export(document, Console.Out);
				else
					OmniGraffleGenerator.Export(document, filename);
			}
			catch (Exception e)
			{
				Console.WriteLine("<html>");
				Console.WriteLine("<body>");

				Console.WriteLine("<h1>" + e.Message + "</h1>");
				Console.WriteLine("<pre>" + e.StackTrace + "</pre>");

				Console.WriteLine("</body>");
				Console.WriteLine("</html>");
			}
		}

		public static Document ExportModel(KAOSModel _model)
		{
			mapping = new Dictionary<Omnigraffle.Sheet, Dictionary<string, Omnigraffle.ShapedGraphic>>();

			var document = new Omnigraffle.Document();

			var canvas = new Omnigraffle.Sheet(1, string.Format("Model"));
			var shapes = new Dictionary<string, IList<Graphic>>();

			var u = new GoalModelGenerator(canvas, shapes);
			u.Render(_model);
			document.Canvas.Add(canvas);

			var s2 = new Omnigraffle.Sheet(1, "Goal and Obstacle Model");
			var u2 = new GoalAndObstacleModelGenerator(s2, new Dictionary<string, IList<Graphic>>());
			u2.Render(_model);

			document.Canvas.Add(s2);


			int i = 0;
			foreach (var o in _model.Obstructions().Select(x => x.Obstacle()))
			{
				i++;
				var s = new Omnigraffle.Sheet(1, string.Format($"Obstacle diagram for '{o.FriendlyName}'"));
				var u3 = new ObstacleDiagramGenerator(s, new Dictionary<string, IList<Graphic>>());
				u3.Render(o, _model);
				document.Canvas.Add(s);
			}

			i = 0;
			foreach (var goalWithException in _model.Exceptions().Select(x => x.AnchorGoal())
				.Union(_model.Replacements().Select(x => x.ResolvingGoal()))
				.Union(_model.ObstacleAssumptions().Select(x => x.Anchor()))
				.Distinct())
			{
				i++;
				var s = new Omnigraffle.Sheet(1, string.Format($"Exception diagram for '{goalWithException.FriendlyName}'"));
				var u3 = new ExceptionDiagramGenerator(s, new Dictionary<string, IList<Graphic>>());
				u3.Render(goalWithException, _model);
				document.Canvas.Add(s);
			}

			return document;
		}
	}
}
