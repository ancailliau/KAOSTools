using System;
using System.Collections.Generic;
using System.Linq;
using BDDSharp;
using KAOSTools.Core;

namespace UncertaintySimulation
{
    public static class BDDProbabilityHelpers {

        public static double GetProbability (this BDDNode node, Dictionary<int, double> samplingVector) 
        {
            if (node.IsOne) return 1.0;
            if (node.IsZero) return 0.0;

            var v = samplingVector.ContainsKey(node.Index) ? samplingVector[node.Index] : 0;
            return node.Low.GetProbability(samplingVector) * (1 - v)
                + node.High.GetProbability(samplingVector) * v;
        }

        public static double ESSRS (this Goal goal, double[] samples, int n_samples)
        {
            var d = goal.ESVRS(samples, n_samples);
            return Math.Sqrt(d);
        }

        public static double ESVRS (this Goal goal, double[] samples, int n_samples)
        {
            var ss = samples.Where(x => x < goal.RDS);
            int k = ss.Count ();
            if (k == 0)
                return 0;

            return ss.Select (x => Math.Pow(x - goal.RDS, 2)).Sum () / k;
        }

        public static double InvSpread (this Goal goal, double[] samples, int n_samples)
        {
            var ss = samples.Where(x => x >= goal.RDS);
            int k = ss.Count ();
            if (k == 0)
                return 0;

            return Math.Sqrt (ss.Select (x => Math.Pow(x - goal.RDS, 2)).Sum () / k);
        }

        public static double EURS (this Goal goal, double[] samples, int n_samples)
        {
            return goal.EUS (goal.RDS, samples, n_samples);
        }

        public static double EUS (this Goal goal, double threshold, double[] samples, int n_samples)
        {
            return samples.Where (x => x < threshold).Count () / (double) n_samples;
        }
    }
}

