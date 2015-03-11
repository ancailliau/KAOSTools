using System;
using KAOSTools.Utils;
using KAOSTools.MetaModel;
using BDDSharp;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using ExpertOpinionSharp.Frameworks;

namespace UncertaintySimulation
{
    class MainClass : KAOSToolCLI
    {
        enum OUTPUT_MODE
        {
            CSV, REPORT, DOT, SINGLE
        }

        enum COMBINATION_METHOD
        {
            COOK, MS
        }

        public static void Main (string[] args)
        {
            var output_mode = OUTPUT_MODE.REPORT;
            string output_file = "";
            int n_sample = 100000;
            int seed = (int) DateTime.Now.Ticks & 0x0000FFFF;

            var hist_width = 60;
            var hist_height = 20;

            var overshoot_factor = .1d;
            var combination_method = COMBINATION_METHOD.COOK;

            var single_name = "";

            options.Add ("report", "Print the report", v => output_mode = OUTPUT_MODE.REPORT);
            options.Add ("csv:", "Export data to the specified CSV file or print it", 
                v => { 
                    output_mode = OUTPUT_MODE.CSV; 
                    output_file = v;
                });
            options.Add ("dot:", "Export the BDD to the specified DOT file or print it", v => {
                output_mode = OUTPUT_MODE.DOT; 
                output_file = v;
            });
            options.Add ("single:", "Experimental", v => {
                output_mode = OUTPUT_MODE.SINGLE; 
                single_name = v;
            });
            options.Add ("n=|sample=", "Specify the number of sample to use for the simulation (Default: " + n_sample + ")", 
                v => {
                    int res;
                    if (int.TryParse (v, out res)) {
                        n_sample = res;
                    } else {
                        Console.Error.WriteLine ("Unable to parse '" + v + "' as a number. Keeping default value '" + n_sample + "'");   
                    }
                });
            options.Add ("s=|seed=", "Specify the initial seed for the random number generator (Default is time-dependent)", 
                v => {
                    int res;
                    if (int.TryParse (v, out res)) {
                        seed = res;
                    } else {
                        Console.Error.WriteLine ("Unable to parse '" + v + "' as a number. Keeping default value '" + seed + "'");   
                    }
                });
            options.Add ("width=", "Specify the width of the ASCII chart (Default: " + hist_width + ")", 
                v => {
                    int res;
                    if (int.TryParse (v, out res)) {
                        hist_width = res;
                    } else {
                        Console.Error.WriteLine ("Unable to parse '" + v + "' as a number. Keeping default value '" + hist_width + "'");   
                    }
                });
            options.Add ("height=", "Specify the height of the ASCII chart (Default: " + hist_height + ")", 
                v => {
                    int res;
                    if (int.TryParse (v, out res)) {
                        hist_height = res;
                    } else {
                        Console.Error.WriteLine ("Unable to parse '" + v + "' as a number. Keeping default value '" + hist_height + "'");   
                    }
                });
            options.Add ("overshoot=", "Specify the overshoot factor to use on the expert estimates (Default: " + overshoot_factor + ")", 
                v => {
                    double res;
                    if (double.TryParse (v, out res)) {
                        overshoot_factor = res;
                    } else {
                        Console.Error.WriteLine ("Unable to parse '" + v + "' as a number. Keeping default value '" + overshoot_factor + "'");   
                    }
                });
            options.Add ("cook", "Use the Cook framework to combine experts opinions", 
                v => { 
                    combination_method = COMBINATION_METHOD.COOK;
                });
            options.Add ("ms", "Use the Mendel-Sheridan framework to combine experts opinions", 
                v => { 
                    combination_method = COMBINATION_METHOD.MS;
                });

            Init (args);
            string id = reminderArgs.Count > 1 ? reminderArgs[1] : null;

            var r = new Random (seed);
            if (string.IsNullOrWhiteSpace (id)) {
                Console.Error.WriteLine ("Please specify a goal.");
                return;
            }

            // Compute the obstruction set for root goal
            var root = model.Goal (x => x.Identifier == id);
            if (root == null) {
                Console.Error.WriteLine ("Goal '" + id + "' was not found in the model.");
                return;
            }

            var manager = new BDDManager (0);

            var mapping = new Dictionary<KAOSMetaModelElement, int> ();
            var reverse_mapping = new Dictionary<int, KAOSMetaModelElement> ();
            var obstructionSet = root.GetObstructionSet(manager, mapping, reverse_mapping);
            obstructionSet = manager.Sifting (obstructionSet);

            TextWriter stream;
            if (string.IsNullOrWhiteSpace (output_file)) {
                stream = Console.Out;
            } else {
                stream = new StreamWriter (output_file);
            }

            if (output_mode == OUTPUT_MODE.DOT) {
                stream.WriteLine(manager.ToDot (obstructionSet, (x) => reverse_mapping[x.Index].FriendlyName));
                return;
            }

            var p = new Dictionary <KAOSMetaModelElement, double[]> ();
            foreach (var n in reverse_mapping) {
                p.Add (n.Value, new double[n_sample]);
            }
            p.Add (root, new double[n_sample]);

            /*
            Console.WriteLine ("--- Parameters");
            Console.WriteLine (model.Parameters["experts.quantiles"]);

            Console.WriteLine ();
            Console.WriteLine ("--- Calibration variables");
            foreach (var o in model.CalibrationVariables ()) {
                Console.WriteLine (o.FriendlyName);
            }

            Console.WriteLine ();
            Console.WriteLine ("--- Leaf obstacles ");
            foreach (var o in model.LeafObstacles ()) {
                Console.WriteLine (o.FriendlyName);
            }
            */

            var quantiles = model.Parameters["experts.quantiles"]
                .Remove (model.Parameters["experts.quantiles"].Length - 1)
                .Substring (1)
                .Split(',').Select (x => {
                var y = x.Trim ();
                if (y.EndsWith ("%")) {
                    return double.Parse (y.Remove (y.Length - 1));
                } else {
                    return double.Parse (y);
                }
            });
            var enumerable = quantiles.ToArray ();

            var qq = new double [enumerable.Count () + 2];
            qq [0] = 0;
            for (int i = 0; i < enumerable.Count(); i++) {
                qq [i + 1] = enumerable [i];
            }
            qq [qq.Length - 1] = 1;

            /*
            Console.WriteLine (string.Join (";", qq));
            */

            ExpertOpinionFramework ef;
            if (combination_method == COMBINATION_METHOD.MS) {
                ef = new MendelSheridanFramework (qq);
            } else if (combination_method == COMBINATION_METHOD.COOK) {
                ef = new CookFramework (qq);
            } else {
                throw new NotImplementedException ();
            }

            ef.OvershootFactor = overshoot_factor;

            foreach (var o in model.LeafObstacles ()) {
                foreach (var estimate in o.ExpertEstimates) {
                    ef.AddEstimate (estimate.Key.Identifier, o.Identifier, estimate.Value.Quantiles.ToArray ());
//                    Console.WriteLine (estimate.Key.Name);
                }
            }

            foreach (var o in model.CalibrationVariables ()) {
                foreach (var estimate in o.ExpertEstimates) {
                    ef.AddEstimate (estimate.Key.Identifier, o.Identifier, estimate.Value.Quantiles.ToArray ());
                }
                ef.SetValue (o.Identifier, o.EPS);
            }

//            Console.WriteLine ("weight" + string.Join (",", ((CookFramework) ef).GetWeights ()));
//            Console.WriteLine ("info" + string.Join (",", ((CookFramework) ef).GetInformationScores ()));
//            Console.WriteLine ("calib" + string.Join (",", ((CookFramework) ef).GetCalibrationScores ()));
//            return;

            var r2 = new Random ();

            foreach (var o in model.LeafObstacles ()) {
                try {
                    if (o.ExpertEstimates.Count > 0) {

                        var dm = ef.Fit (o.Identifier);

                        if (dm is ExpertOpinionSharp.Distributions.QuantileDistribution) {
                            var ddm = (ExpertOpinionSharp.Distributions.QuantileDistribution) dm;
                            o.UEPS = new QuantileDistribution (ddm.probabilities, ddm.quantiles);
                        } else if (dm is ExpertOpinionSharp.Distributions.MixtureDistribution) {
                            var ddm = (ExpertOpinionSharp.Distributions.MixtureDistribution) dm;
                            o.UEPS = new MixtureDistribution (ddm.cummulativeWeight, 
                                ddm.distributions.Select (x => new QuantileDistribution (x.probabilities, x.quantiles)).ToArray ()
                            );
                        }
//                        Console.WriteLine (o.FriendlyName + " >> " + o.UEPS);
//                        Console.WriteLine (o.FriendlyName + " ==> " + o.UEPS.Sample (r2));

                    } else {
                        throw new NotImplementedException ("'" + o.FriendlyName + "' is not estimated.");
                        // o.UEPS = new QuantileDistribution (new double[] { 0, 0 }, new double[] { 0, 1});
                    }

                } catch (Exception e) {
                    Console.WriteLine ("Error with " + o.FriendlyName);
                    Console.WriteLine (e.Message);
                    Console.WriteLine ("---");
                    Console.WriteLine (e);
                    return;
                }
            }

            if (output_mode == OUTPUT_MODE.SINGLE) {

                IEnumerable<Obstacle> obstacles;
                if (string.IsNullOrEmpty (single_name)) {
                    obstacles = model.LeafObstacles ();
                } else {
                    obstacles = model.Obstacles (x => x.Identifier == single_name);
                }

                foreach (var leaf in obstacles) {

                    p = new Dictionary <KAOSMetaModelElement, double[]> ();
                    foreach (var n in reverse_mapping) {
                        p.Add (n.Value, new double[n_sample]);
                    }
                    p.Add (root, new double[n_sample]);

                    for (int i = 0; i < n_sample; i++) {
                        // Fill sample vector
                        var sampleVector = new Dictionary <int, double> ();
                        foreach (var n in reverse_mapping) {
                            double sample;
                            if (n.Value is Obstacle) {
                                var obstacle = ((Obstacle)n.Value);
                                if (obstacle.Identifier.Equals (leaf.Identifier)) {
                                    sample = obstacle.UEPS.Sample (r);
                                } else {
                                    sample = 0;
                                }

                            } else if (n.Value is DomainHypothesis) {
                                var domainHypothesis = ((DomainHypothesis)n.Value);
                                if (domainHypothesis.UEPS == null) {
                                    Console.WriteLine ("Cannot sample '" + domainHypothesis.FriendlyName + "'.");
                                    return;
                                }
                                sample = domainHypothesis.UEPS.Sample (r);
                            } else {
                                throw new NotImplementedException ();
                            }
                            sampleVector.Add (n.Key, sample);
                            p [n.Value] [i] = sample;
                        }

                        p [root] [i] = 1.0 - obstructionSet.GetProbability (sampleVector);
                    }

                    if (string.IsNullOrEmpty (single_name)) {
                        Console.WriteLine (leaf.FriendlyName + ","
                        + (Math.Round (root.EURS (p [root], n_sample), 4)) + ","
                        + Math.Round (root.ESSRS (p [root], n_sample), 4));
                    } else {
                        /*
                        var essrs = root.ESSRS (p [root], n_sample);
                        foreach (var k in new [] { Math.Sqrt (20), 2, 1 }) {
                            var lower_bound = root.RDS - k * essrs;

                            var n_sample_below_RDS = ((double)p [root].Count (x => x <= root.RDS));
                            var n_sample_between_lower_and_RDS = (double)p [root].Count (x => x >= lower_bound & x <= root.RDS);
                            var theoric_percentage_above_lower_bound = (100 - Math.Round (1.0 / Math.Pow (k, 2) * 100, 2));
                            var real_percentage_above_lower_bound = Math.Round (n_sample_between_lower_and_RDS / n_sample_below_RDS * 100, 2);

                            Console.Write ("    at least " + theoric_percentage_above_lower_bound + "% in [" + Math.Round (lower_bound, 2) + ", " + root.RDS + "]");
                            Console.WriteLine (" (real: " + real_percentage_above_lower_bound + ")");
                        }
                        */


                        Console.WriteLine (model.Obstacle (x => x.Identifier == single_name).UEPS);

                        Console.WriteLine ("----");

                        PrintCSV (hist_width, root, p, n_sample, reverse_mapping, stream);
                    }

                }

                return;
            }


            for (int i = 0; i < n_sample; i++) {
                // Fill sample vector
                var sampleVector = new Dictionary <int, double> ();
                foreach (var n in reverse_mapping) {
                    double sample;
                    if (n.Value is Obstacle) {
                        sample = ((Obstacle) n.Value).UEPS.Sample(r);

                    } else if (n.Value is DomainHypothesis) {
                        var domainHypothesis = ((DomainHypothesis)n.Value);
                        if (domainHypothesis.UEPS == null) {
                            Console.WriteLine("Cannot sample '"+domainHypothesis.FriendlyName+"'.");
                            return;
                        }
                        sample = domainHypothesis.UEPS.Sample(r);
                    } else {
                        throw new NotImplementedException ();
                    }
                    sampleVector.Add(n.Key, sample);
                    p[n.Value][i] = sample;

                    //                    Console.WriteLine (n.Value.FriendlyName + "===>" + p[nachieve_incident_resolved.Value][i]);
                }

                p[root][i] = 1.0 - obstructionSet.GetProbability (sampleVector);

                // Console.WriteLine (p[root][i]);
            }


            if (output_mode == OUTPUT_MODE.CSV) {
            
                PrintCSV (hist_width, root, p, n_sample, reverse_mapping, stream);

            }

            if (output_mode == OUTPUT_MODE.REPORT) {
                Console.WriteLine("");
                Console.WriteLine("  " + root.FriendlyName + ":");
                Console.WriteLine("  " + new string ('-', root.FriendlyName.Length));
                Console.WriteLine();

                Console.WriteLine("  Samples used = " + n_sample);
                Console.WriteLine("  RDS = " + root.RDS);
                Console.WriteLine();

                var essrs = Math.Round(root.ESSRS(p[root], n_sample), 4);
                Console.WriteLine("  EURS = " + Math.Round (root.EURS (p[root], n_sample), 4) * 100 + "%");
                Console.WriteLine("  ESVRS = " + Math.Round (root.ESVRS (p[root], n_sample), 4));
                Console.WriteLine("  ESSRS = " + essrs);
                Console.WriteLine();

                Console.WriteLine("  Uncertainty intervals");

                foreach (var k in new [] { Math.Sqrt (20), 2 }) {
                    var lower_bound = root.RDS - k * essrs;

                    var n_sample_below_RDS = ((double) p[root].Count(x => x <= root.RDS));
                    var n_sample_between_lower_and_RDS = (double) p[root].Count(x => x >= lower_bound & x <= root.RDS);
                    var theoric_percentage_above_lower_bound = (100 - Math.Round(1.0 / Math.Pow(k, 2) * 100, 2));
                    var real_percentage_above_lower_bound = Math.Round(n_sample_between_lower_and_RDS / n_sample_below_RDS * 100, 2);

                    Console.Write("    at least " + theoric_percentage_above_lower_bound + "% in [" + Math.Round(lower_bound, 2) + ", " + root.RDS + "]");
                    Console.WriteLine(" (real: " + real_percentage_above_lower_bound + ")");
                }

                Console.WriteLine();

                DisplayASCIIHistogram(n_sample, p[root], root, hist_width, hist_height);
            }
        }

        static void DisplayASCIIHistogram(int n_sample, double[] p, Goal g, int n_buckets, int hist_height)
        {
            double w = 1.0 / n_buckets;
            var buckets = new double[n_buckets];
            for (int i = 0; i < n_sample; i++) {
                buckets[Math.Max (0, (int)(p[i] / w))]++;
            }
            for (int i = 0; i < n_buckets; i++) {
                buckets[i] /= n_sample;
            }

            // Histogram
            for (int j = hist_height; j >= 0; j--) {
                Console.Write(" ");
                Console.Write((j * (100 / hist_height)).ToString().PadLeft(3) + " - ");
                for (int i = 0; i < n_buckets; i++) {
                    var d = Math.Round(buckets[i] * 100, 2);
                    if (d > (j * (100 / hist_height))) {
                        Console.Write("#");
                    }
                    else {
                        Console.Write(" ");
                    }
                }
                Console.WriteLine();
            }
            var rds_position = (int)(g.RDS / w);
            var offset = (int) (((n_buckets - rds_position) < 4) ? (((n_buckets - rds_position) > 3) ? 3 : 2) : 1);

            Console.Write("      |");
            Console.WriteLine((new String (' ', rds_position) + "|").PadRight (n_buckets - 1) + "|");
            Console.Write("      0");
            Console.WriteLine((new String (' ', rds_position - offset) + "RDS").PadRight (n_buckets - 1) + "1");
            Console.WriteLine();
        }

        static int PrintCSV (int hist_width, Goal root, Dictionary<KAOSMetaModelElement, double[]> p, int n_sample,
            Dictionary<int, KAOSMetaModelElement> reverse_mapping, TextWriter stream)
        {
            double w = 1.0 / hist_width;
            var buckets = new Dictionary<KAOSMetaModelElement, double[]> ();
            foreach (var kv in reverse_mapping) {
                var obstacle = kv.Value;
                buckets [obstacle] = new double[hist_width+1];
                for (int i = 0; i < n_sample; i++) {
                    var bucket_n = Math.Max(0, Math.Min ((int)(p [obstacle] [i] / w), hist_width - 1));

                    buckets [obstacle] [bucket_n]++;
                }
                for (int i = 0; i < hist_width; i++) {
                    buckets [obstacle] [i] /= n_sample;
                }
            }
            buckets.Add (root, new double[hist_width+1]);
            for (int i = 0; i < n_sample; i++) {
                var d = p [root] [i];
                var i2 = (int)(d / w);
                buckets [root] [Math.Max(0,i2)]++;
            }
            for (int i = 0; i < hist_width; i++) {
                buckets [root] [i] /= n_sample;
            }
            // One line per bucket
            var sorted_obstacle = reverse_mapping.Values.ToList ();
            sorted_obstacle.Sort ((x, y) => string.Compare (x.Identifier, y.Identifier, StringComparison.Ordinal));
            stream.WriteLine ("x," + string.Join (",", sorted_obstacle.Select (x => x.Identifier)) + "," + root.Identifier);
            for (int i = 0; i < hist_width; i++) {
                stream.WriteLine ((i + 0.5) * w + "," + string.Join (",", sorted_obstacle.Select (x => buckets [x] [i])) + "," + buckets [root] [i]);
            }
            return n_sample;
        }
    }
}
