using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace KAOSTools.Core
{
    public class BetaSatisfactionRate : UncertainSatisfactionRate {
        public double Alpha;
        public double Beta;

		public BetaSatisfactionRate(double alpha, double beta)
        {
            Alpha = alpha;
            Beta = beta;
        }

        public override double Sample (Random r) {
            return MathNet.Numerics.Distributions.Beta.Sample(r, Alpha, Beta);
        }
    }
}
