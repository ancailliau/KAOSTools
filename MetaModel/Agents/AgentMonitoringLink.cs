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
    public class AgentMonitoringLink : KAOSCoreElement
    {
		public string AgentIdentifier {
			get;
			set;
		}
		
		public string PredicateIdentifier {
			get;
			set;
		}
		
        public AgentMonitoringLink (KAOSModel model) : base(model)
        {
        }

        public AgentMonitoringLink(KAOSModel model, string identifier) : base(model, identifier)
		{
		}

		public override KAOSCoreElement Copy ()
        {
			throw new NotImplementedException();
        }
    }
    
}
