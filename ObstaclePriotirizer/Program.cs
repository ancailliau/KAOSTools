using System;
using System.Collections.Generic;
using System.Linq;
using KAOSTools.MetaModel;
using UncertaintySimulation;

namespace ObstaclePriotirizer
{
    class MainClass : KAOSTools.Utils.KAOSToolCLI
    {

        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Init(args);

            var cp = new CPSCalculator(model);
            cp.ComputeCPS();

            Console.WriteLine("---");

            var ec = new EffectCalculator(model);
            foreach (var root in model.RootGoals()) {
                Console.WriteLine("Effect for " + root.FriendlyName);
                var e = ec.GetEffects(root);
                foreach (var ee in e) {
                    Console.WriteLine("{0} => {1}", string.Join(",", ee.Key.Select (x=>x.FriendlyName)), ee.Value);
                }
            }
        }

    }
}
