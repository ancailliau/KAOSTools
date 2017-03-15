using System;
using KAOSTools.Core;
using System.IO;
using VizuSharp.DataProviders;
using VizuSharp.Visualisations;
using VizuSharp;
using VizuSharp.Surfaces;
using VizuSharp.Axes;
using System.Linq;
using System.Collections.Generic;

namespace UncertaintySimulation.Outputs
{
    public class SatisfactionUncertaintyPNG : OutputGenerator
    {
        Goal root;
        string filename;

        public SatisfactionUncertaintyPNG (Goal root, string filename)
        {
            this.root = root;
            this.filename = filename;
        }

        public void Output (UncertaintyComputation c, TextWriter stream)
        {
            var probabilityVectors = c.Simulate (root)[root];

            var data = new DataSet <double> ();
            for (int i = 0; i < probabilityVectors.Length; i++) {
                data.Add (probabilityVectors[i]);
            }

            var hist = new Histogram<double> (100, 0, 1, x => x, false, data);
            var hist2 = new Histogram<double> (100, 0, 1, x => x, false, new Filter<double> (x => x < root.RDS, data));

            var line = new LinePlot<Bin> (x => ((x.UpperBound + x.LowerBound) / 2.0d), x => x.Count, hist);

            var area = new AreaPlot<Bin> (x => ((x.UpperBound + x.LowerBound) / 2.0d), x => x.Count, hist2);
            area.AreaColor = new RGBColor (218/255.0,147/255.0,149/255.0, .5);

            line.VerticalAxis.Visible = false;
            line.HorizontalAxis.SetTicks (new [] { new Tick (0), new Tick (1), new Tick (root.RDS, "RDS = " + root.RDS) });
            line.HorizontalAxis.SetDomain (0, 1);

            area.VerticalAxis.Visible = false;
            area.VerticalAxis = line.VerticalAxis;
            area.HorizontalAxis.Visible = false;

            var plots = new List<Visualisation> ();
            plots.Add (line);
            plots.Add (area);

            var composite = new SuperposedPlot (plots);

            var surface = new CairoSurface (640, 480);
            composite.Plot (surface);

            surface.WriteToPng (filename);
        }
    }
}

