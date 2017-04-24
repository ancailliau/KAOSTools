using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace KAOSTools.Core
{

    #region Goal Model

    #region Meta entities

    [DataContract]
    public class DomainProperty : KAOSCoreElement
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

        public double EPS { get {
                throw new Exception ();
            }
            set {
                throw new Exception (); 
            } 
        }

		public DomainProperty(KAOSModel model) : base(model) { }
        public DomainProperty(KAOSModel model, string identifier) : base(model, identifier) { }

        public override KAOSCoreElement Copy ()
        {
            return new DomainProperty (null) {
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
