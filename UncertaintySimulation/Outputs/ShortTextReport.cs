using System;
using System.IO;
using KAOSTools.MetaModel;
using System.Linq;

namespace UncertaintySimulation.Outputs
{
    public class ShortTextReport : OutputGenerator
    {
        Goal root;

        public ShortTextReport (Goal root)
        {
            this.root = root;
        }

        public void Output (UncertaintyComputation c, TextWriter stream)
        {
            var probabilityVectors = c.Simulate (root);

            stream.WriteLine("");
            stream.WriteLine("  " + root.FriendlyName + ":");
            stream.WriteLine("  " + new string ('-', root.FriendlyName.Length));
            stream.WriteLine();

            stream.WriteLine("  Samples used = " + c.NSamples);
            stream.WriteLine("  RDS = " + root.RDS);
            stream.WriteLine();

            var essrs = Math.Round(root.ESSRS(probabilityVectors[root], c.NSamples), 4);
            stream.WriteLine("  EURS = " + Math.Round (root.EURS (probabilityVectors[root], c.NSamples), 4) * 100 + "%");
            stream.WriteLine("  ESVRS = " + Math.Round (root.ESVRS (probabilityVectors[root], c.NSamples), 4));
            stream.WriteLine("  ESSRS = " + essrs);
            stream.WriteLine ("  InvSpread = " + Math.Round (root.InvSpread (probabilityVectors [root], c.NSamples), 4));
            stream.WriteLine();

            stream.WriteLine("  Uncertainty intervals");

            foreach (var k in new [] { Math.Sqrt (20), 2 }) {
                var lower_bound = root.RDS - k * essrs;

                var n_sample_below_RDS = ((double) probabilityVectors[root].Count(x => x <= root.RDS));
                var n_sample_between_lower_and_RDS = (double) probabilityVectors[root].Count(x => x >= lower_bound & x <= root.RDS);
                var theoric_percentage_above_lower_bound = (100 - Math.Round(1.0 / Math.Pow(k, 2) * 100, 2));
                var real_percentage_above_lower_bound = Math.Round(n_sample_between_lower_and_RDS / n_sample_below_RDS * 100, 2);

                stream.Write("    at least " + theoric_percentage_above_lower_bound + "% in [" + Math.Round(lower_bound, 2) + ", " + root.RDS + "]");
                stream.WriteLine(" (real: " + real_percentage_above_lower_bound + ")");
            }

            stream.WriteLine();
        }

    }
}

