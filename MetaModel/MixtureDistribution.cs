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

    public class MixtureDistribution : UncertaintyDistribution
    {
        readonly double[] cummulativeWeight;
        readonly QuantileDistribution[] distributions;

        public MixtureDistribution (double[] cummulativeWeight, QuantileDistribution[] distributions)
        {
            this.cummulativeWeight = cummulativeWeight;
            this.distributions = distributions;
        }

        public override double Sample (Random _random)
        {
            double s = _random.NextDouble ();

            int i;
            for (i = 1; i < cummulativeWeight.Length - 1; i++) {
                if (s < cummulativeWeight [i]) {
                    break;
                }
            }
            return distributions[i-1].Sample (_random);
        }

        public double LowerBound {
            get {
                return distributions.Min (x => x.LowerBound);
            }
        }

        public double UpperBound {
            get {
                return distributions.Max (x => x.UpperBound);
            }
        }
    }
    
}
