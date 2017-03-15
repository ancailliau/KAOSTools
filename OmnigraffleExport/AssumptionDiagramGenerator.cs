using System;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using NDesk.Options;
using KAOSTools.Core;
using KAOSTools.Utils;
using KAOSTools.OmnigraffleExport.Omnigraffle;
using System.Text;

namespace KAOSTools.OmnigraffleExport
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
