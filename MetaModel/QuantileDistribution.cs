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

    public class QuantileDistribution : UncertaintyDistribution
    {
        readonly double[] probabilities;
        readonly double[] quantiles;

        public QuantileDistribution (double[] probabilities, double[] quantiles)
        {
            this.probabilities = probabilities;
            this.quantiles = quantiles;
        }

        public override double Sample (Random _random)
        {
            double s = _random.NextDouble ();

            int i;
            for (i = 1; i < probabilities.Length - 1; i++) {
                if (s < probabilities [i]) {
                    break;
                }
            }

            var ss = (s - probabilities [i-1]) / (probabilities [i] - probabilities [i - 1]);
            var ss2 = ss * (quantiles [i] - quantiles [i - 1]) + quantiles[i - 1];
            return ss2;
        }

        public double LowerBound {
            get {
                return quantiles.Min ();
            }
        }

        public double UpperBound {
            get {
                return quantiles.Max ();
            }
        }

        public override string ToString ()
        {
            return string.Format ("[QuantileDistribution: {0}]", 
                string.Join (" ", Enumerable.Range (0, quantiles.Length).Select (i => quantiles[i] + ":" + probabilities[i]))
            );
        }

    }
    
}
