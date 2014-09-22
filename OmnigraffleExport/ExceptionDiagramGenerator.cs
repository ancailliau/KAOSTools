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
    public class ExceptionDiagramGenerator : AbstractDiagramGenerator
	{
        public ExceptionDiagramGenerator (Sheet sheet, IDictionary<string, IList<Graphic>> shapes)
            : base (sheet, shapes)
        {
            sheet.LayoutInfo.LayoutEngine    = LayoutEngine.Neato;
            sheet.LayoutInfo.NeatoSeparation = 0.55;
            sheet.LayoutInfo.NeatoLineLength = 1.69;
            sheet.LayoutInfo.NeatoOverlap    = false;

            sheet.LayoutInfo.AutoLayout = true;
        }

        public void Render (Goal g, KAOSModel model) 
        {
            Render (g);

            // Exceptions
            foreach (var e in g.Exceptions ()) {
                if (!shapes.ContainsKey (e.ResolvingGoalIdentifier))
                    Render (e.ResolvingGoal());
                Render (e);
            }

            // Replacements
            foreach (var e in g.Replacements ()) {
                if (!shapes.ContainsKey (e.AnchorGoalIdentifier))
                    Render (e.AnchorGoal ());
                Render (e);
            }

            // Provided
            foreach (var e in g.Provided ()) {
                if (!shapes.ContainsKey (e.ResolvedObstacleIdentifier))
                    Render (e.Obstacle ());
                Render (e);
            }

            // Context refinements
            /*
            foreach (var r in g.ParentRefinements ().Union (g.Refinements ())) {
                if (!shapes.ContainsKey (r.ParentGoalIdentifier)) {
                    Render (r.ParentGoal ());
                }

                foreach (var sg in r.SubGoals ()) {
                    if (!shapes.ContainsKey (sg.Identifier)) {
                        Render (sg);
                    }
                }

                Render (r);
            }
            */
        }
	}
}
