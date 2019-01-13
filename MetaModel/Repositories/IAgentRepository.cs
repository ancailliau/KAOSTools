using System;
using System.Collections.Generic;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;

namespace UCLouvain.KAOSTools.Core.Repositories
{

	public interface IAgentRepository
	{
		void Add(Agent goal);
		void Add(GoalAgentAssignment goal);
		void Add(AgentMonitoringLink link);
		void Add(AgentControlLink link);

		bool Exists(string identifier);

        Agent GetAgent(string identifier);
		Agent GetAgent(Predicate<Agent> predicate);
		
		GoalAgentAssignment GetAgentAssignment(Predicate<GoalAgentAssignment> predicate);
		AgentMonitoringLink GetMonitoringLink(Predicate<AgentMonitoringLink> predicate);
		AgentControlLink GetControlLink(Predicate<AgentControlLink> predicate);

		IEnumerable<Agent> GetAgents();
		IEnumerable<GoalAgentAssignment> GetAgentAssignments();
		IEnumerable<AgentMonitoringLink> GetAgentMonitoringLinks();
		IEnumerable<AgentControlLink> GetAgentControlLinks();

		IEnumerable<Agent> GetAgents(Predicate<Agent> predicate);
		IEnumerable<GoalAgentAssignment> GetAgentAssignments(Predicate<GoalAgentAssignment> predicate);
		IEnumerable<AgentMonitoringLink> GetAgentMonitoringLinks(Predicate<AgentMonitoringLink> predicate);
		IEnumerable<AgentControlLink> GetAgentControlLinks(Predicate<AgentControlLink> predicate);
	}
    
}
