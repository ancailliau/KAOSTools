using System;
using System.Collections.Generic;
using UCLouvain.KAOSTools.Core;
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

		Goal GetGoal(string identifier);
		SoftGoal GetSoftGoal(string identifier);
		GoalRefinement GetGoalRefinement(string identifier);
		GoalException GetGoalException(string identifier);
		GoalReplacement GetGoalReplacement(string identifier);

        Goal GetGoal(Predicate<Goal> predicate);
		SoftGoal GetSoftGoal(Predicate<SoftGoal> predicate);
		GoalRefinement GetGoalRefinement(Predicate<GoalRefinement> predicate);
		GoalException GetGoalException(Predicate<GoalException> predicate);
		GoalReplacement GetGoalReplacement(Predicate<GoalReplacement> predicate);

		IEnumerable<Goal> GetGoals();
		IEnumerable<GoalRefinement> GetGoalRefinements();
		IEnumerable<GoalException> GetGoalExceptions();
		IEnumerable<GoalReplacement> GetGoalReplacements();
		IEnumerable<SoftGoal> GetSoftGoals();

		IEnumerable<Goal> GetGoals(Predicate<Goal> predicate);
		IEnumerable<GoalRefinement> GetGoalRefinements(Predicate<GoalRefinement> predicate);
		IEnumerable<GoalException> GetGoalExceptions(Predicate<GoalException> predicate);
		IEnumerable<GoalReplacement> GetGoalReplacements(Predicate<GoalReplacement> predicate);
		IEnumerable<SoftGoal> GetSoftGoals(Predicate<SoftGoal> predicate);
        
        void Remove (IEnumerable<GoalException> e);
        void Remove (IEnumerable<GoalReplacement> e);
    }
}
