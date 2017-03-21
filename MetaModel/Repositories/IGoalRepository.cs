using System;
using KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;

namespace UCLouvain.KAOSTools.Core.Repositories
{
    public interface IGoalRepository
    {
        void Add(Goal goal);
		void Add(GoalRefinement refinement);
        void Add(GoalException exception);
		void Add(GoalReplacement replacement);
		void Add(SoftGoal goal);

		bool GoalExists(string identifier);
		bool GoalRefinementExists(string identifier);
		bool GoalExceptionExists(string identifier);
		bool GoalReplacementExists(string identifier);
        bool SoftGoalExists(string identifier);
	}
}
