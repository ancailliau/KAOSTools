using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.Serialization;
using KAOSTools.Core;

namespace UCLouvain.KAOSTools.Core.Agents
{
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
    
}
