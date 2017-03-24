using System;
using KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;
using System.Collections.Generic;
using System.Linq;

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

        public Agent GetAgent(Predicate<Agent> predicate)
        {
            return Agents.Values.SingleOrDefault(x => predicate(x));
        }

        public Agent GetAgent(string identifier)
        {
            return Agents.ContainsKey(identifier) ? Agents[identifier] : null;
        }

        public GoalAgentAssignment GetAgentAssignment(Predicate<GoalAgentAssignment> predicate)
        {
            return GoalAgentAssignments.Values.SingleOrDefault(x => predicate(x));
        }

        public IEnumerable<GoalAgentAssignment> GetAgentAssignments()
		{
            return GoalAgentAssignments.Values;
        }

        public IEnumerable<GoalAgentAssignment> GetAgentAssignments(Predicate<GoalAgentAssignment> predicate)
		{
			return GoalAgentAssignments.Values.Where(x => predicate(x));
        }

        public IEnumerable<Agent> GetAgents()
        {
            return Agents.Values;
        }

        public IEnumerable<Agent> GetAgents(Predicate<Agent> predicate)
        {
            return Agents.Values.Where(x => predicate(x));
        }
    }
}
