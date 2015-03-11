using System;
using KAOSTools.MetaModel;
using KAOSTools.OmnigraffleExport.Omnigraffle;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace KAOSTools.OmnigraffleExport
{
    public class ObstacleDiagramGenerator : AbstractDiagramGenerator
    {
        public ObstacleDiagramGenerator (Sheet sheet, IDictionary<string, IList<Graphic>> shapes)
            : base (sheet, shapes)
        {
            sheet.LayoutInfo.HierarchicalOrientation = HierarchicalOrientation.BottomTop;
        }

        public void Render (Obstacle o, KAOSModel model) 
        {
            Render (o);
            foreach (var obstruction in o.model.Obstructions ().Where (x => x.ObstacleIdentifier == o.Identifier)) {
                Render (obstruction.ObstructedGoal ());
                Render (obstruction);
            }

            RenderRefinement (o);
        }

        void RenderRefinement (Obstacle o) {
            foreach (var refinement in o.Refinements()) {
                foreach (var child in refinement.SubObstacles ()) {
                    Render (child);
                    RenderRefinement (child);

                    foreach (var resolution in child.Resolutions ()) {
                        Render (resolution.ResolvingGoal ());
                        Render (resolution);
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

