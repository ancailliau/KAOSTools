using System;
using System.Collections.Generic;
using System.Linq;
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

        public Goal GetGoal(string identifier)
        {
            return Goals.ContainsKey(identifier) ? Goals[identifier] : null;
        }

        public SoftGoal GetSoftGoal(string identifier)
		{
			return SoftGoals.ContainsKey(identifier) ? SoftGoals[identifier] : null;
        }

        public GoalRefinement GetGoalRefinement(string identifier)
		{
			return GoalRefinements.ContainsKey(identifier) ? GoalRefinements[identifier] : null;
        }

        public GoalException GetGoalException(string identifier)
		{
			return GoalExceptions.ContainsKey(identifier) ? GoalExceptions[identifier] : null;
        }

        public GoalReplacement GetGoalReplacement(string identifier)
		{
			return GoalReplacements.ContainsKey(identifier) ? GoalReplacements[identifier] : null;
        }

        public IEnumerable<Goal> GetGoals()
        {
            return Goals.Values;
        }

        public IEnumerable<GoalRefinement> GetGoalRefinements()
        {
            return GoalRefinements.Values;
        }

        public IEnumerable<GoalException> GetGoalExceptions()
        {
            return GoalExceptions.Values;
        }

        public IEnumerable<GoalReplacement> GetGoalReplacements()
        {
            return GoalReplacements.Values;
        }

        public IEnumerable<SoftGoal> GetSoftGoals()
        {
            return SoftGoals.Values;
        }

        public IEnumerable<Goal> GetGoals(Predicate<Goal> predicate)
        {
            return Goals.Values.Where(x => predicate(x));
        }

        public IEnumerable<GoalRefinement> GetGoalRefinements(Predicate<GoalRefinement> predicate)
		{
            return GoalRefinements.Values.Where(x => predicate(x));
        }

        public IEnumerable<GoalException> GetGoalExceptions(Predicate<GoalException> predicate)
		{
            return GoalExceptions.Values.Where(x => predicate(x));
        }

        public IEnumerable<GoalReplacement> GetGoalReplacements(Predicate<GoalReplacement> predicate)
		{
            return GoalReplacements.Values.Where(x => predicate(x));
        }

        public IEnumerable<SoftGoal> GetSoftGoals(Predicate<SoftGoal> predicate)
		{
            return SoftGoals.Values.Where(x => predicate(x));
        }

        public Goal GetGoal(Predicate<Goal> predicate)
        {
            return Goals.Values.SingleOrDefault(x => predicate(x));
        }

        public SoftGoal GetSoftGoal(Predicate<SoftGoal> predicate)
		{
            return SoftGoals.Values.SingleOrDefault(x => predicate(x));
        }

        public GoalRefinement GetGoalRefinement(Predicate<GoalRefinement> predicate)
		{
            return GoalRefinements.Values.SingleOrDefault(x => predicate(x));
        }

        public GoalException GetGoalException(Predicate<GoalException> predicate)
		{
            return GoalExceptions.Values.SingleOrDefault(x => predicate(x));
        }

        public GoalReplacement GetGoalReplacement(Predicate<GoalReplacement> predicate)
		{
            return GoalReplacements.Values.SingleOrDefault(x => predicate(x));
        }

        public void Remove (IEnumerable<GoalException> exceptions)
        {
            foreach (var e in exceptions.ToArray ())
                GoalExceptions.Remove (e.Identifier);
        }

        public void Remove (IEnumerable<GoalReplacement> replacement)
        {
            foreach (var e in replacement.ToArray ())
                GoalExceptions.Remove (e.Identifier);
        }
    }
}
