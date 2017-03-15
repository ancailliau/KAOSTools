using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace KAOSTools.Core
{

    #region Goal Model

    #region Meta entities

    #endregion

    #region Assignements

    #endregion

    #region Refinements

    #endregion

    #region Obstructions and resolutions

    #endregion

    #region Exceptions and assumptions

    #endregion

    #endregion

    #region Object Model

    #endregion

    public class PERTDistribution : UncertaintyDistribution {
        public float Min;
        public float Max;
        public float Mode;

        public override double Sample (Random r) {
            var mean = (Min + 4 * Mode + Max) / 6;
            var alpha = 6 * ((mean - Min) / (Max - Min));
            var beta = 6 * ((Max - mean) / (Max - Min));
            // Console.WriteLine ("beta={0}, alpha={1}, mean={2}", beta, alpha, mean);
            
            var s = r.NextDouble ();
            return (alglib.invincompletebeta (alpha, beta, s) * (Max - Min)) + Min;
        }
    }
    
}
