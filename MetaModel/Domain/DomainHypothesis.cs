using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace KAOSTools.Core
{

    #region Goal Model

    #region Meta entities

    [DataContract]
    public class DomainHypothesis : KAOSCoreElement
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

        public double EPS { get; set; }

        public UncertainSatisfactionRate SatisfactionUncertainty { get; set; }

        public DomainHypothesis (KAOSModel model) : base(model)
        {

		}

        public DomainHypothesis(KAOSModel model, string identifier) : base(model, identifier)
		{

		}

        public override KAOSCoreElement Copy ()
        {
            return new DomainHypothesis (null) {
                Identifier = Identifier,
                Implicit = Implicit,
                Name = Name,
                Definition = Definition,
                FormalSpec = FormalSpec,
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
