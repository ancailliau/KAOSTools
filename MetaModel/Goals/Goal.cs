using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace KAOSTools.Core
{

    #region Goal Model

    #region Meta entities

    [DataContract]
    public class Goal : KAOSCoreElement
    {
        [DataMember]
        public string Name { get; set; }

        public override string FriendlyName {
            get {
                return string.IsNullOrEmpty(Name) ? Identifier : Name;
            }
        }

        [DataMember]
        public string Definition { get; set; }

        public Formula FormalSpec { get; set; }

        public double CPS {
            get {
                throw new Exception ();
            }
            set {
                throw new Exception ();
            }
        }

        public double RDS { get; set; }

        public IDictionary<CostVariable, double> Costs {
            get;
            set;
        }

        public Goal (KAOSModel model) : base(model)
        {
            Costs = new Dictionary<CostVariable, double> ();
		}

        public Goal(KAOSModel model, string identifier) : base(model, identifier)
		{
			Costs = new Dictionary<CostVariable, double>();
		}

        public override KAOSCoreElement Copy ()
        {
            return new Goal (null) {
                Identifier = Identifier,
                Implicit = Implicit,
                Name = Name,
                Definition = Definition,
                FormalSpec = FormalSpec,
                CPS = CPS,
                RDS = RDS,
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
