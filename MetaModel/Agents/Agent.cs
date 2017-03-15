using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace KAOSTools.Core
{

    #region Goal Model

    #region Meta entities

    [DataContract]
    public class Agent : KAOSCoreElement
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

        [DataMember]
        public AgentType Type { get; set; }

        public Agent (KAOSModel model) : base(model)
        {
            Type = AgentType.None;
        }

        public override KAOSCoreElement Copy ()
        {
            return new Agent(null) {
                Identifier = Identifier,
                Implicit = Implicit,
                Name = Name,
                Definition = Definition,
                Type = Type
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
