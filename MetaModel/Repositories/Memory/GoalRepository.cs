using System;
using System.Collections.Generic;
using System.Linq;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Goals;

namespace UCLouvain.KAOSTools.Core.Repositories.Memory
{
	public class GoalRepository : IGoalRepository
	{
		IDictionary<string, Goal> Goals;
		IDictionary<string, GoalRefinement> GoalRefinements;
		IDictionary<string, SoftGoal> SoftGoals;
		IDictionary<string, GoalException> GoalExceptions;
		IDictionary<string, GoalReplacement> GoalReplacements;

		IDictionary<string, GoalProvidedNot> GoalProvidedNotAnnotations;
		IDictionary<string, GoalProvided> GoalProvidedAnnotations;
		IDictionary<string, GoalRelaxedTo> GoalRelaxedToAnnotations;

		public GoalRepository()
		{
			Goals = new Dictionary<string, Goal>();
			GoalRefinements = new Dictionary<string, GoalRefinement>();
			SoftGoals = new Dictionary<string, SoftGoal>();
			GoalExceptions = new Dictionary<string, GoalException>();
			GoalReplacements = new Dictionary<string, GoalReplacement>();
			GoalProvidedNotAnnotations = new Dictionary<string, GoalProvidedNot>();
			GoalProvidedAnnotations = new Dictionary<string, GoalProvided>();
			GoalRelaxedToAnnotations = new Dictionary<string, GoalRelaxedTo>();
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


		public void Remove(IEnumerable<GoalException> exceptions)
		{
			foreach (var e in exceptions.ToArray())
				GoalExceptions.Remove(e.Identifier);
		}

		public void Remove(IEnumerable<GoalReplacement> replacement)
		{
			foreach (var e in replacement.ToArray())
				GoalExceptions.Remove(e.Identifier);
		}

		#region ProvidedNot

		public void Add(GoalProvidedNot annotation)
		{
			if (GoalProvidedNotAnnotations.ContainsKey(annotation.Identifier))
			{
				throw new ArgumentException(string.Format("Goal ProvidedNot annotation identifier already exist: {0}", annotation.Identifier));
			}

			GoalProvidedNotAnnotations.Add(annotation.Identifier, annotation);
		}

		public bool GoalProvidedNotAnnotationExists(string identifier)
		{
			return GoalProvidedNotAnnotations.ContainsKey(identifier);
		}

		public GoalProvidedNot GetGoalProvidedNotAnnotation(string identifier)
		{
			return GoalProvidedNotAnnotations.ContainsKey(identifier) ? GoalProvidedNotAnnotations[identifier] : null;
		}

		public IEnumerable<GoalProvidedNot> GetGoalProvidedNotAnnotations()
		{
			return GoalProvidedNotAnnotations.Values;
		}
		
		public IEnumerable<GoalProvidedNot> GetGoalProvidedNotAnnotations(Predicate<GoalProvidedNot> predicate)
		{
			return GoalProvidedNotAnnotations.Values.Where(x => predicate(x));
		}
		
		public GoalProvidedNot GetGoalProvidedNotAnnotation(Predicate<GoalProvidedNot> predicate)
		{
			return GoalProvidedNotAnnotations.Values.SingleOrDefault(x => predicate(x));
		}
		
		public void Remove(IEnumerable<GoalProvidedNot> annotation)
		{
			foreach (var e in annotation.ToArray())
				GoalProvidedNotAnnotations.Remove(e.Identifier);
		}

		#endregion

		#region Provided

		public void Add(GoalProvided annotation)
		{
			if (GoalProvidedAnnotations.ContainsKey(annotation.Identifier))
			{
				throw new ArgumentException(string.Format("Goal Provided annotation identifier already exist: {0}", annotation.Identifier));
			}

			GoalProvidedAnnotations.Add(annotation.Identifier, annotation);
		}

		public bool GoalProvidedAnnotationExists(string identifier)
		{
			return GoalProvidedAnnotations.ContainsKey(identifier);
		}

		public GoalProvided GetGoalProvidedAnnotation(string identifier)
		{
			return GoalProvidedAnnotations.ContainsKey(identifier) ? GoalProvidedAnnotations[identifier] : null;
		}

		public IEnumerable<GoalProvided> GetGoalProvidedAnnotations()
		{
			return GoalProvidedAnnotations.Values;
		}
		
		public IEnumerable<GoalProvided> GetGoalProvidedAnnotations(Predicate<GoalProvided> predicate)
		{
			return GoalProvidedAnnotations.Values.Where(x => predicate(x));
		}
		
		public GoalProvided GetGoalProvidedAnnotation(Predicate<GoalProvided> predicate)
		{
			return GoalProvidedAnnotations.Values.SingleOrDefault(x => predicate(x));
		}
		
		public void Remove(IEnumerable<GoalProvided> annotation)
		{
			foreach (var e in annotation.ToArray())
				GoalProvidedAnnotations.Remove(e.Identifier);
		}

		#endregion

		#region RelaxedTo

		public void Add(GoalRelaxedTo annotation)
		{
			if (GoalRelaxedToAnnotations.ContainsKey(annotation.Identifier))
			{
				throw new ArgumentException(string.Format("Goal RelaxedTo annotation identifier already exist: {0}", annotation.Identifier));
			}

			GoalRelaxedToAnnotations.Add(annotation.Identifier, annotation);
		}

		public bool GoalRelaxedToAnnotationExists(string identifier)
		{
			return GoalRelaxedToAnnotations.ContainsKey(identifier);
		}

		public GoalRelaxedTo GetGoalRelaxedToAnnotation(string identifier)
		{
			return GoalRelaxedToAnnotations.ContainsKey(identifier) ? GoalRelaxedToAnnotations[identifier] : null;
		}

		public IEnumerable<GoalRelaxedTo> GetGoalRelaxedToAnnotations()
		{
			return GoalRelaxedToAnnotations.Values;
		}
		
		public IEnumerable<GoalRelaxedTo> GetGoalRelaxedToAnnotations(Predicate<GoalRelaxedTo> predicate)
		{
			return GoalRelaxedToAnnotations.Values.Where(x => predicate(x));
		}
		
		public GoalRelaxedTo GetGoalRelaxedToAnnotation(Predicate<GoalRelaxedTo> predicate)
		{
			return GoalRelaxedToAnnotations.Values.SingleOrDefault(x => predicate(x));
		}
		
		public void Remove(IEnumerable<GoalRelaxedTo> annotation)
		{
			foreach (var e in annotation.ToArray())
				GoalRelaxedToAnnotations.Remove(e.Identifier);
		}

		#endregion

	}
}
