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
    public class AssumptionDiagramGenerator : AbstractDiagramGenerator
	{
        public AssumptionDiagramGenerator (Sheet sheet, IDictionary<string, IList<Graphic>> shapes)
            : base (sheet, shapes)
        {
            sheet.LayoutInfo.LayoutEngine = LayoutEngine.Circo;
        }

        public void Render (Goal g, KAOSModel model) 
        {
            Render (g);
            foreach (var e in g.Provided ()) {
                Render (e.Obstacle ());
                Render (e);
            }
        }
	}
}
