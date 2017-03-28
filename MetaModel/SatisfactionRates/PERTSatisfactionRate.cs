using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace KAOSTools.Core
{
    public class PERTSatisfactionRate : UncertainSatisfactionRate {
        public double Min;
		public double Mode;
        public double Max;

		public PERTSatisfactionRate(double min, double mode, double max)
        {
            Min = min;
            Mode = mode;
            Max = max;
        }

        public override double Sample (Random r) {
            var mean = (Min + 4 * Mode + Max) / 6;
            var alpha = 6 * ((mean - Min) / (Max - Min));
            var beta = 6 * ((Max - mean) / (Max - Min));
            
            var s = r.NextDouble ();
            return (alglib.invincompletebeta (alpha, beta, s) * (Max - Min)) + Min;
        }
    }
    
}
