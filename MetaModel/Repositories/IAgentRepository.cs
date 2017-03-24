using System;
using System.Collections.Generic;
using KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;

namespace UCLouvain.KAOSTools.Core.Repositories
{

	public interface IAgentRepository
	{
		void Add(Agent goal);
		void Add(GoalAgentAssignment goal);

		bool Exists(string identifier);

        Agent GetAgent(string identifier);

		Agent GetAgent(Predicate<Agent> predicate);
		GoalAgentAssignment GetAgentAssignment(Predicate<GoalAgentAssignment> predicate);

		IEnumerable<Agent> GetAgents();
		IEnumerable<GoalAgentAssignment> GetAgentAssignments();

		IEnumerable<Agent> GetAgents(Predicate<Agent> predicate);
		IEnumerable<GoalAgentAssignment> GetAgentAssignments(Predicate<GoalAgentAssignment> predicate);
	}
    
}
