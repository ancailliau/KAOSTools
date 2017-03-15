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

    public class Constraint : KAOSCoreElement
    {
        public string Name { get; set; }
        public string Definition { get; set; }
        public List<string> Conflict  { get; set; }
        public List<string> Or  { get; set; }

        public Constraint  (KAOSModel model) : base (model)
        {
            Conflict = new List<string> ();
            Or = new List<string> ();
        }

        public override KAOSCoreElement Copy ()
        {
			return new Constraint(model) {
				Identifier = Identifier,
				Implicit = Implicit,
				InSystems = InSystems,
				CustomData = CustomData,
				Name = Name,
				Definition = Definition,
				Conflict = new List<string>(Conflict),
				Or = new List<string>(Or)
			};
        }
    }
    
}
