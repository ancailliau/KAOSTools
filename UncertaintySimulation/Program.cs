using System;
using KAOSTools.Utils;
using KAOSTools.MetaModel;
using BDDSharp;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using ExpertOpinionSharp.Frameworks;
using UncertaintySimulation.Outputs;

namespace UncertaintySimulation
{
    public class MainClass : KAOSToolCLI
    {
        enum OUTPUT_MODE
        {
            CSV, REPORT, DOT, SINGLE
        }

        public enum COMBINATION_METHOD
        {
            COOK, MS
        }

        public static void Main (string[] args)
        {
            string generatorId = "";
            string output_file = "";
            var generator_options = new GeneratorOptions ();

            int n_sample = 100000;
            int seed = (int) DateTime.Now.Ticks & 0x0000FFFF;
            var overshoot_factor = .1d;
            var combination_method = COMBINATION_METHOD.COOK;

            var single_name = "";

            options.Add ("t=", "Specify the output type.", 
                v => {
                    generatorId = v;
                });

            options.Add ("f=", "Specify the output file, otherwise printed at console.", 
                v => {
                    output_file = v;
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

            generator_options.RootGoal = root;
            generator_options.Filename = output_file;

            TextWriter stream = Console.Out;
            if (!string.IsNullOrEmpty (output_file)) {
                stream = new StreamWriter (output_file);
            }

            var ucomputation = new UncertaintyComputation (model, root, combination_method) {
                NSamples = n_sample,
                OverShoot = overshoot_factor,
                RandomGenerator = r
            };

            var generator = GeneratorFactory.Build (generatorId, generator_options);
            generator.Output (ucomputation, stream);

            // var probabilityVectors = ucomputation.Simulate (root);



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
