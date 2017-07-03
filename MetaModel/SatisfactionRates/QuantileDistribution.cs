using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace UCLouvain.KAOSTools.Core
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

    public class QuantileDistribution : UncertainSatisfactionRate
    {
        readonly double[] _probabilities;
        public readonly double[] _quantiles;

        public QuantileDistribution (double[] probabilities, double[] quantiles)
        {
            this._probabilities = probabilities;
			this._quantiles = quantiles;
		}

        public override double Sample (Random _random)
        {
            double s = _random.NextDouble ();

            int i;
            for (i = 1; i < _probabilities.Length - 1; i++) {
                if (s < _probabilities [i]) {
                    break;
                }
            }

			double p0 = _probabilities[i - 1];
			double p1 = _probabilities[i];
			
			double q0 = _quantiles[i - 1];
			double q1 = _quantiles[i];
			
			var ss = (s - p0) / (p1 - p0);
			var ss2 = ss * (q1 - q0) + q0;
            return ss2;
        }

        public double LowerBound {
            get {
                return _quantiles.Min ();
            }
        }

        public double UpperBound {
            get {
                return _quantiles.Max ();
            }
        }

        public override string ToString ()
        {
            return string.Format ("[QuantileDistribution: {0}]", 
                string.Join (" ", Enumerable.Range (0, _quantiles.Length).Select (i => _quantiles[i] + ":" + _probabilities[i]))
            );
        }

    }
    
}
