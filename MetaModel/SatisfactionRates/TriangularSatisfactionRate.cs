using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace KAOSTools.Core
{
    public class TriangularSatisfactionRate : UncertainSatisfactionRate {
		public double Min;
		public double Mode;
        public double Max;

        public TriangularSatisfactionRate(double min, double mode, double max)
        {
            Min = min;
            Mode = mode;
            Max = max;
        }

        public override double Sample (Random r) {
            return MathNet.Numerics.Distributions.Triangular.Sample (r, Min, Max, Mode);
        }
    }
    
}
