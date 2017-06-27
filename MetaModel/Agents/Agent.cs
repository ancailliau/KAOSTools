using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.Serialization;
using UCLouvain.KAOSTools.Core;

namespace UCLouvain.KAOSTools.Core.Agents
{
    /// <summary>
    /// Represents an agent
    /// </summary>
    [DataContract]
    public class Agent : KAOSCoreElement
    {

        /// <summary>
        /// Gets or sets the name of the agent.
        /// </summary>
        /// <value>The name.</value>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets the name of the agent if specified, otherwise gets the identifier.
        /// </summary>
        /// <value>The name or identifier of the agent.</value>
        public override string FriendlyName {
            get {
                return string.IsNullOrEmpty(Name) ? Identifier : Name;
            }
        }

        /// <summary>
        /// Gets or sets the definition of the agent.
        /// </summary>
        /// <value>The definition.</value>
        [DataMember]
        public string Definition { get; set; }

        /// <summary>
        /// Gets or sets the type of the agent.
        /// </summary>
        /// <value>The type.</value>
        [DataMember]
        public AgentType Type { get; set; }

        /// <summary>
        /// Initializes a new agent for the specified model.
        /// </summary>
        /// <param name="model">Model.</param>
        public Agent (KAOSModel model) : base(model)
        {
            Type = AgentType.None;
        }

        public Agent(KAOSModel model, string identifier) : base(model, identifier)
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
