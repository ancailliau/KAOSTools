using System;
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
	}
    
}
