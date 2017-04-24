using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.Serialization;
using UCLouvain.KAOSTools.Core.SatisfactionRates;

namespace KAOSTools.Core
{

    #region Goal Model

    #region Meta entities

    [DataContract]
    public class Obstacle : KAOSCoreElement
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

        public double EPS {
            get {
                throw new Exception ();
            }
            set {
                throw new Exception ();
            }
        }

        public double CPS {
            get {
                throw new Exception ();
            }
            set {
                throw new Exception ();
            }
        }


        public UncertainSatisfactionRate SatisfactionUncertainty { get; set; }

        public Dictionary<Expert, QuantileList> ExpertEstimates { get; set; }

        public Obstacle (KAOSModel model) : base(model)
        {
            ExpertEstimates = new Dictionary<Expert, QuantileList> ();
        }

        public Obstacle(KAOSModel model, string identifier) : base(model, identifier)
		{
		}

		public override KAOSCoreElement Copy ()
        {
            return new Obstacle (null) {
                Identifier = Identifier,
                Implicit = Implicit,
                Name = Name,
                Definition = Definition,
                FormalSpec = FormalSpec,
                CPS = CPS,
                EPS = EPS
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
