using System;
using KAOSTools.MetaModel;
using KAOSTools.OmnigraffleExport.Omnigraffle;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace KAOSTools.OmnigraffleExport
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
            var goalsInRefinements = model.GoalRefinements ().SelectMany (x => x.SubGoals ().Union (new[] {
                x.ParentGoal ()
            })).Distinct ();

            foreach (var g in goalsInRefinements) {
               Render (g);
            }

            foreach (var d in model.GoalRefinements ().SelectMany (x => x.DomainProperties()).Distinct ()) {
                Render (d);
            }
            
            foreach (var r in model.GoalRefinements ()) {
                Render (r);
            }

            var obstacles = model.Obstructions ().Select (x => x.Obstacle ());
            foreach (var o in obstacles) {
                Render (o, model);
            }

            foreach (var r in model.GoalAgentAssignments ()) {
                Render (r, true);
            }
        }

        public void Render (Obstacle o, KAOSModel model) 
        {
            Render (o);
            foreach (var obstruction in o.model.Obstructions ().Where (x => x.ObstacleIdentifier == o.Identifier)) {
                // Render (obstruction.ObstructedGoal ());
                Render (obstruction);
            }

            RenderRefinement (o);
        }

        void RenderRefinement (Obstacle o) {
            foreach (var refinement in o.Refinements()) {
                foreach (var child in refinement.SubObstacles ()) {
                    if (!shapes.ContainsKey (child.Identifier)) {
                        Render (child);
                    }
                    RenderRefinement (child);

                    foreach (var resolution in child.Resolutions ()) {
                        if (!shapes.ContainsKey (resolution.ResolvingGoalIdentifier)) {
                            Render (resolution.ResolvingGoal ());
                        }
                        if (!shapes.ContainsKey (resolution.Identifier)) {
                            Render (resolution);
                        }
                    }
                }
                foreach (var child in refinement.DomainHypotheses ()) {
                    Render (child);
                }
                Render (refinement);
            }
        }
    }
}

