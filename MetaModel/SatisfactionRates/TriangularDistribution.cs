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

    public class TriangularDistribution : UncertaintyDistribution {
        public float Min;
        public float Max;
        public float Mode;

        public override double Sample (Random r) {
            return MathNet.Numerics.Distributions.Triangular.Sample (r, Min, Max, Mode);
        }
    }
    
}
