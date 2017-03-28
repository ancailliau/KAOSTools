using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace KAOSTools.Core
{
    public class UniformSatisfactionRate : UncertainSatisfactionRate {
        public double LowerBound;
        public double UpperBound;

        public UniformSatisfactionRate(double lowerBound, double upperBound)
        {
            LowerBound = lowerBound;
            UpperBound = upperBound;
        }

        public override double Sample (Random r) {
            return MathNet.Numerics.Distributions.ContinuousUniform.Sample (r, LowerBound, UpperBound);
        }
    }
    
}
