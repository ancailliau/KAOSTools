using System;
using BenchmarkDotNet.Running;

namespace UCLouvain.KAOSTools.Propagators.Benchmark
{
    class MainClass
    {
        public static void Main (string [] args)
        {
            Console.WriteLine ("Hello World!");
            var summary = BenchmarkRunner.Run<PatternVSBDD>();

        }
    }
}
