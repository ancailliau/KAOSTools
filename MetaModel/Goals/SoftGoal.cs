using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace KAOSTools.Core
{

    #region Goal Model

    #region Meta entities

    public class SoftGoal : KAOSCoreElement
    {
        public string Name { get; set; }

        public override string FriendlyName {
            get {
                return string.IsNullOrEmpty(Name) ? Identifier : Name;
            }
        }

        public string Definition { get; set; }

        public SoftGoal (KAOSModel model) : base(model)
        {
            }

        public override KAOSCoreElement Copy ()
        {
            return new SoftGoal (null) {
                Identifier = Identifier,
                Implicit = Implicit,
                Name = Name,
                Definition = Definition
            };
        }
    }

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
    
}
