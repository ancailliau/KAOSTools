using System;
using System.Collections.Generic;
using KAOSTools.Core;

namespace UCLouvain.KAOSTools.Core.Repositories.Memory
{
    public class GoalRepository : IGoalRepository
	{
		IDictionary<string, Goal> Goals;
		IDictionary<string, GoalRefinement> GoalRefinements;
		IDictionary<string, SoftGoal> SoftGoals;
		IDictionary<string, GoalException> GoalExceptions;
		IDictionary<string, GoalReplacement> GoalReplacements;

        public GoalRepository()
        {
            Goals = new Dictionary<string, Goal>();
            GoalRefinements = new Dictionary<string, GoalRefinement>();
            SoftGoals = new Dictionary<string, SoftGoal>();
            GoalExceptions = new Dictionary<string, GoalException>();
            GoalReplacements = new Dictionary<string, GoalReplacement>();
        }

        public void Add(GoalReplacement replacement)
		{
			if (GoalReplacements.ContainsKey(replacement.Identifier))
			{
				throw new ArgumentException(string.Format("Goal replacement identifier already exist: {0}", replacement.Identifier));
			}

			GoalReplacements.Add(replacement.Identifier, replacement);
        }

        public void Add(SoftGoal goal)
		{
			if (SoftGoals.ContainsKey(goal.Identifier))
			{
				throw new ArgumentException(string.Format("Soft goal identifier already exist: {0}", goal.Identifier));
			}

			SoftGoals.Add(goal.Identifier, goal);
        }

        public void Add(GoalException exception)
		{
			if (GoalExceptions.ContainsKey(exception.Identifier))
			{
				throw new ArgumentException(string.Format("Goal Exception identifier already exist: {0}", exception.Identifier));
			}

			GoalExceptions.Add(exception.Identifier, exception);
        }

        public void Add(GoalRefinement refinement)
		{
            if (GoalRefinements.ContainsKey(refinement.Identifier))
			{
				throw new ArgumentException(string.Format("Goal refinement identifier already exist: {0}", refinement.Identifier));
			}

			GoalRefinements.Add(refinement.Identifier, refinement);
		}

        public void Add(Goal goal)
		{
			if (Goals.ContainsKey(goal.Identifier))
			{
				throw new ArgumentException(string.Format("Goal identifier already exist: {0}", goal.Identifier));
			}

			Goals.Add(goal.Identifier, goal);
        }

        public bool GoalExceptionExists(string identifier)
        {
            return GoalExceptions.ContainsKey(identifier);
        }

        public bool GoalExists(string identifier)
		{
			return Goals.ContainsKey(identifier);
        }

        public bool GoalRefinementExists(string identifier)
		{
            return GoalRefinements.ContainsKey(identifier);
        }

        public bool GoalReplacementExists(string identifier)
		{
            return GoalReplacements.ContainsKey(identifier);
        }

        public bool SoftGoalExists(string identifier)
		{
			return SoftGoals.ContainsKey(identifier);
        }
    }
}
