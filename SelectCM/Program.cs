using System;
using KAOSTools.Utils;
using System.Linq;
using KAOSTools.Core;
using UncertaintySimulation;

namespace SelectCM
{
    class MainClass : KAOSToolCLI
    {
        public static void Main (string[] args)
        {
            Console.WriteLine ("Hello World!");

            Init (args);

            foreach (var g in model.Resolutions ().Select (resolution => resolution.ResolvingGoal ())) {
                Console.WriteLine (g.FriendlyName + " *******");
                foreach (var c in g.Costs) {
                    Console.WriteLine (c.Key.FriendlyName + " >> " + c.Value);
                }
            }

            int n_sample = 100000;
            var overshoot_factor = .1d;
            int seed = 0; //(int) DateTime.Now.Ticks & 0x0000FFFF;
            var r = new Random (seed);
            var combination_method = UncertaintySimulation.MainClass.COMBINATION_METHOD.COOK;

            foreach (var root in model.RootGoals ()) {

                var ucomputation = new UncertaintyComputation (model, root, combination_method) {
                    NSamples = n_sample,
                    OverShoot = overshoot_factor,
                    RandomGenerator = r
                };

                var probabilityVectors = ucomputation.Simulate (root);
                Console.WriteLine (Math.Round (root.EURS (probabilityVectors[root], ucomputation.NSamples), 4) * 100);
            }

            Console.WriteLine ("Terminate.");
        }
    }
}
