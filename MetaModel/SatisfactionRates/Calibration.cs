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

    public class Calibration : KAOSCoreElement
    {
        public string Name { get; set; }

        public double EPS { get; set; }

        public Dictionary<Expert, QuantileList> ExpertEstimates { get; set; }

        public override string FriendlyName {
            get {
                return string.IsNullOrEmpty(Name) ? Identifier : Name;
            }
        }

        public Calibration  (KAOSModel model) : base (model)
        {
        }

        public override KAOSCoreElement Copy ()
        {
            throw new NotImplementedException ();
        }
    }
    
}
