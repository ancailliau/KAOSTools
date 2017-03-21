using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.Serialization;
using UCLouvain.KAOSTools.Core.Agents;

namespace KAOSTools.Core
{

    [DataContract]
    public class GoalAgentAssignment : KAOSCoreElement {

        [DataMember]
        public string GoalIdentifier { get; set ; }

		[DataMember]
		public IList<string> AgentIdentifiers { get; set; }

		public bool IsEmpty
		{
			get
			{
				return AgentIdentifiers.Count == 0;
			}
		}

		public GoalAgentAssignment(KAOSModel model) : base (model)
        {
			AgentIdentifiers = new List<string>();
		}

		public void Add(Agent agent)
		{
			this.AgentIdentifiers.Add(agent.Identifier);
		}

        public override KAOSCoreElement Copy ()
        {
            return new GoalAgentAssignment (null) {
                Identifier = Identifier,
                Implicit = Implicit,
                GoalIdentifier = GoalIdentifier,
                AgentIdentifiers = new List<string> (AgentIdentifiers)
            };
        }
    }
    
}
