using System;
using UCLouvain.KAOSTools.Propagators.Benchmark;
using System.Diagnostics;

namespace UCLouvain.KAOSTools.Propagators.CLI
{
    class MainClass
    {
        public static void Main (string [] args)
        {
            Console.WriteLine ("Hello World!");

            int max = 10;

            var bench = new PatternVSBDD ();
            bench.NbGoals = int.Parse (args [0]);
            bench.NbObstructions = int.Parse (args [1]);
            bench.NbObstacles = int.Parse (args [2]);
            Console.Write ("Setup...");
            bench.Setup ();
            Console.WriteLine ("(done)");

            Console.WriteLine ("Pattern-based computation:");
            var watch = Stopwatch.StartNew ();
            for (int i = 0; i < max; i++) {
                bench.PatternBasedComputation ();
            }
            watch.Stop ();
            Console.WriteLine ($"    Elapsed: {watch.ElapsedMilliseconds/max}ms");
            
            Console.WriteLine ("BDD-based computation:");
            watch = Stopwatch.StartNew ();
            for (int i = 0; i < max; i++) {
                bench.BDDBasedComputation ();
            }
            watch.Stop ();
            Console.WriteLine ($"    Elapsed: {watch.ElapsedMilliseconds/max}ms");
            
            Console.WriteLine ("BDD-based computation with a pre-built:");
            watch = Stopwatch.StartNew ();
            for (int i = 0; i < max; i++) {
                bench.BDDBasedComputationPrebuilt ();
            }
            watch.Stop ();
            Console.WriteLine ($"    Elapsed: {watch.ElapsedMilliseconds/max}ms");
            
        }
    }
}
