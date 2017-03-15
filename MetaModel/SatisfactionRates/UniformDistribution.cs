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

    public class UniformDistribution : UncertaintyDistribution {
        public float LowerBound;
        public float UpperBound;

        public override double Sample (Random r) {
            return MathNet.Numerics.Distributions.ContinuousUniform.Sample (r, LowerBound, UpperBound);
        }
    }
    
}
