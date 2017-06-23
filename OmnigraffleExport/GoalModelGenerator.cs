using System;
using KAOSTools.Core;
using KAOSTools.OmnigraffleExport.Omnigraffle;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace KAOSTools.OmnigraffleExport
{
    public class GoalModelGenerator : AbstractDiagramGenerator
    {
        public GoalModelGenerator (Sheet sheet, IDictionary<string, IList<Graphic>> shapes)
            : base (sheet, shapes)
        {
            sheet.LayoutInfo.HierarchicalOrientation = HierarchicalOrientation.BottomTop;
        }

        public void Render (KAOSModel model) 
        {
            //var goalsInRefinements = model.GoalRefinements ().SelectMany (x => x.SubGoals ().Union (new[] {
            //    x.ParentGoal ()
            //})).Distinct ();

            foreach (var g in model.Goals ()) {
               Render (g);
            }

            foreach (var d in model.GoalRefinements ().SelectMany (x => x.DomainProperties()).Distinct ()) {
                Render (d);
            }
            
            foreach (var r in model.GoalRefinements ()) {
                Render (r);
            }

            foreach (var r in model.GoalAgentAssignments ()) {
                Render (r, true);
            }
        }
    }
}

