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

    public class CostVariable : KAOSCoreElement
    {
        public string Name { get; set; }

        public CostVariable  (KAOSModel model) : base (model)
        {
        }

        public override KAOSCoreElement Copy ()
        {
            throw new NotImplementedException ();
        }
    }
    
}
