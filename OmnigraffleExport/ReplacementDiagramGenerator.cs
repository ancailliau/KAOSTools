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
    public class ReplacementDiagramGenerator : AbstractDiagramGenerator
    {
        public ReplacementDiagramGenerator (Sheet sheet, IDictionary<string, IList<Graphic>> shapes)
            : base (sheet, shapes)
        {
            sheet.LayoutInfo.HierarchicalOrientation = HierarchicalOrientation.BottomTop;
        }

        public void Render (Goal g, KAOSModel model) 
        {
            Render (g);
            foreach (var e in g.Replacements ()) {
                Render (e.AnchorGoal ());
                Render (e);
            }

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
        }
	}
}
