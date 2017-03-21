using System;
using KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;
using System.Collections.Generic;

namespace UCLouvain.KAOSTools.Core.Repositories.Memory
{
    public class AgentRepository : IAgentRepository
    {
		IDictionary<string, Agent> Agents;
		IDictionary<string, GoalAgentAssignment> GoalAgentAssignments;

        public AgentRepository()
        {
            Agents = new Dictionary<string, Agent>();
            GoalAgentAssignments = new Dictionary<string, GoalAgentAssignment>();
        }

        public void Add(GoalAgentAssignment assignment)
		{
			if (Agents.ContainsKey(assignment.Identifier))
			{
				throw new ArgumentException(string.Format("Agent assignment identifier already exist: {0}", assignment.Identifier));
			}

			GoalAgentAssignments.Add(assignment.Identifier, assignment);
        }

        public void Add(Agent agent)
        {
            if (Agents.ContainsKey(agent.Identifier))
            {
                throw new ArgumentException(string.Format("Agent identifier already exist: {0}", agent.Identifier));
            }

            Agents.Add(agent.Identifier, agent);
        }

        public bool Exists(string identifier)
        {
            return Agents.ContainsKey(identifier);
        }
    }
}
