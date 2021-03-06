﻿using System;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.OmnigraffleExport.Omnigraffle;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace UCLouvain.KAOSTools.OmnigraffleExport
{
    public class GoalAndObstacleModelGenerator : AbstractDiagramGenerator
    {
        public GoalAndObstacleModelGenerator (Sheet sheet, IDictionary<string, IList<Graphic>> shapes)
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
            
            foreach (var o in model.Obstacles ()) {
                Render (o);
            }
            
            foreach (var o in model.ObstacleRefinements ()) {
                Render (o);
            }

            foreach (var o in model.Obstructions ()) {
                Render (o);
            }
            
            foreach (var o in model.Resolutions ()) {
                Render (o);
            }

            foreach (var r in model.GoalAgentAssignments ()) {
                Render (r, true);
            }
            
            foreach (var o in model.Resolutions ()) {
                RenderAnchorArrow (o);
            }
        }
    }
}

