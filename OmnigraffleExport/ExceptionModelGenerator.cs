using System;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.OmnigraffleExport.Omnigraffle;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace UCLouvain.KAOSTools.OmnigraffleExport
{
    public class ExceptionModelGenerator : AbstractDiagramGenerator
    {
        public ExceptionModelGenerator (Sheet sheet, IDictionary<string, IList<Graphic>> shapes)
            : base (sheet, shapes)
        {
            sheet.LayoutInfo.HierarchicalOrientation = HierarchicalOrientation.BottomTop;
        }

        public void Render (KAOSModel model) 
        {
			var goals = model.Exceptions().SelectMany(x => new[] { x.AnchorGoal(), x.ResolvingGoal() });
			goals = goals.Union(model.ObstacleAssumptions().SelectMany(x => new[] { x.Anchor() })).Distinct();
            
			var obstacles = model.ObstacleAssumptions().SelectMany(x => new[] { x.Obstacle() });
			
            foreach (var g in goals) {
               Render (g);
            }
            
            foreach (var o in obstacles) {
               Render (o);
            }

			foreach (var e in model.Exceptions())
			{
				Render(e);
			}

			foreach (var e in model.ObstacleAssumptions())
			{
				Render(e);
			}
        }
    }
}

