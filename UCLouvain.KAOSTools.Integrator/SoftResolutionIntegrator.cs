using System;
using System.Collections.Generic;
using System.Linq;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Core.SatisfactionRates;

namespace UCLouvain.KAOSTools.Integrators
{
    public class SoftResolutionIntegrator
    {
        KAOSModel _model;
        ChangePropagator propagator;
    
        public SoftResolutionIntegrator (KAOSModel model)
        {
            _model = model;
            propagator = new ChangePropagator (_model);
        }
        
        public void Integrate (Resolution resolution)
        {
            if (resolution.ResolutionPattern == ResolutionPattern.GoalSubstitution
            || resolution.ResolutionPattern == ResolutionPattern.GoalWeakening)
            {
                RemoveObstructedGoal (resolution);
                
            } else if (resolution.ResolutionPattern == ResolutionPattern.ObstaclePrevention
            || resolution.ResolutionPattern == ResolutionPattern.ObstacleMitigation
            || resolution.ResolutionPattern == ResolutionPattern.ObstacleWeakMitigation
            || resolution.ResolutionPattern == ResolutionPattern.ObstacleStrongMitigation
            || resolution.ResolutionPattern == ResolutionPattern.ObstacleReduction
            || resolution.ResolutionPattern == ResolutionPattern.GoalRestoration)
            {
                KeepObstructedGoal (resolution);
                
            } else if (resolution.ResolutionPattern == ResolutionPattern.None) {
                throw new IntegrationException (IntegrationException.NO_PATTERN);
                
            } else {
                throw new IntegrationException (string.Format (IntegrationException.PATTERN_NOT_KNOWN, resolution.ResolutionPattern));
            }
        }
        
        public void Remove (Resolution resolution)
        {
            if (resolution.ResolutionPattern == ResolutionPattern.GoalSubstitution
            || resolution.ResolutionPattern == ResolutionPattern.GoalWeakening)
            {
                RemoveReplacement (resolution);
                
            } else if (resolution.ResolutionPattern == ResolutionPattern.ObstaclePrevention
            || resolution.ResolutionPattern == ResolutionPattern.ObstacleMitigation
            || resolution.ResolutionPattern == ResolutionPattern.ObstacleWeakMitigation
            || resolution.ResolutionPattern == ResolutionPattern.ObstacleStrongMitigation
            || resolution.ResolutionPattern == ResolutionPattern.ObstacleReduction
            || resolution.ResolutionPattern == ResolutionPattern.GoalRestoration)
            {
                RemoveException (resolution);
                
            } else if (resolution.ResolutionPattern == ResolutionPattern.None) {
                throw new IntegrationException (IntegrationException.NO_PATTERN);
                
            } else {
                throw new IntegrationException (string.Format (IntegrationException.PATTERN_NOT_KNOWN, resolution.ResolutionPattern));
            }
        }

        void KeepObstructedGoal (Resolution resolution)
        {
            // Create the exception
            var exception = new GoalException (_model);
            exception.AnchorGoalIdentifier = resolution.AnchorIdentifier;
            exception.ResolvedObstacleIdentifier = resolution.ObstacleIdentifier;
            exception.ResolvingGoalIdentifier = resolution.ResolvingGoalIdentifier;
            
            _model.Add (exception);

			var anchor = _model.Goal(resolution.AnchorIdentifier);
			Propagate (anchor, resolution.Obstacle());

            Obstacle obstacle = resolution.Obstacle ();
            obstacle.Resolved = true;
        }
        
        IEnumerable<Goal> GetAncestors (Goal g)
        {
			return g.ParentRefinements().SelectMany(x => GetAncestors (x.ParentGoal())).Union(new[] { g });
        }
        
        IEnumerable<Goal> GetDescendants (Goal g)
        {
			return g.Refinements().SelectMany(x => x.SubGoals().SelectMany(y => GetDescendants(y))).Union(new[] { g });
        }
        
        IEnumerable<Goal> GetObstructedGoals (Obstacle o)
        {
			return o.ParentRefinements().SelectMany(x => GetObstructedGoals (x.ParentObstacle()))
				   .Union(o.model.Obstructions(x => x.ObstacleIdentifier == o.Identifier).Select(x => x.ObstructedGoal()));
        }

		IEnumerable<Goal> GetGoalsToPropagate(Goal anchor_goal, Obstacle obstacle)
		{
			var obstructed_goals = GetObstructedGoals(obstacle);
			var ancestors = obstructed_goals.SelectMany(GetAncestors).Distinct();
			var descendants = GetDescendants(anchor_goal);
			var goals = ancestors.Intersect(descendants);
			return goals;
		}
        
        void Propagate (Goal anchor_goal, Obstacle obstacle)
		{
			IEnumerable<Goal> goals = GetGoalsToPropagate(anchor_goal, obstacle);
			foreach (var goal in goals)
			{
				var assumption = new ObstacleAssumption(_model);
				assumption.AnchorGoalIdentifier = goal.Identifier;
				assumption.SetObstacle(obstacle);
				_model.Add(assumption);
			}
		}

		void RemoveException (Resolution resolution)
        {
            var e = _model.Exceptions ().Where (x => x.AnchorGoalIdentifier == resolution.AnchorIdentifier
            & x.ResolvedObstacleIdentifier == resolution.ObstacleIdentifier
            & x.ResolvingGoalIdentifier == resolution.ResolvingGoalIdentifier);            

            _model.goalRepository.Remove (e);

			var anchor = _model.Goal(resolution.AnchorIdentifier);
			PropagateRemove(anchor, resolution.Obstacle());

            Obstacle obstacle = resolution.Obstacle ();
            obstacle.Resolved = false;
        }
        
        void PropagateRemove (Goal anchor_goal, Obstacle obstacle)
        {
			IEnumerable<Goal> goals = GetGoalsToPropagate(anchor_goal, obstacle);
			foreach (var goal in goals)
			{
				var assumptions = _model.ObstacleAssumptions().Where(x => x.AnchorGoalIdentifier == goal.Identifier
														& x.ResolvedObstacleIdentifier == obstacle.Identifier).ToArray();
				foreach (var a in assumptions)
					_model.obstacleRepository.Remove(a);
			}
        }

        void RemoveObstructedGoal (Resolution resolution)
        {
            // Get the obstructed goals
            var obstructed = resolution.GetObstructedGoalIdentifiers ();

            // Get the obstructed goals in the refinment of the anchor goal
            var anchorRefinements = _model.GoalRefinements (x => x.ParentGoalIdentifier == resolution.AnchorIdentifier);
            if (anchorRefinements.Count () > 1) {
                throw new IntegrationException (IntegrationException.SINGLE_REFINEMENT_ONLY);
            }
            if (anchorRefinements.Count () == 0) {
                throw new IntegrationException (IntegrationException.ANCHOR_NO_REFINEMENT);
            }
            var anchorRefinement = anchorRefinements.Single ();
            var obstructedChildren = anchorRefinement.SubGoalIdentifiers.Select (x => x.Identifier)
            .Intersect (obstructed);

            // Remove these
            anchorRefinement.SubGoalIdentifiers 
                = new HashSet<GoalRefinee> (
                    anchorRefinement.SubGoalIdentifiers.Where (
                        x => !obstructedChildren.Contains (x.Identifier)));

            // Adds the countermeasure goal to the refinement
            anchorRefinement.Add (resolution.ResolvingGoalIdentifier);

            // Add replacement annotation
            var replacement = new GoalReplacement (_model);
            replacement.AnchorGoalIdentifier = resolution.AnchorIdentifier;
            replacement.ResolvedObstacleIdentifier = resolution.ObstacleIdentifier;
            replacement.ResolvingGoalIdentifier = resolution.ResolvingGoalIdentifier;
            replacement.ReplacedGoals = obstructedChildren.ToList ();
            _model.Add (replacement);
        }
        
        void RemoveReplacement (Resolution resolution)
        {
            var e = _model.Replacements ().Where (x => x.AnchorGoalIdentifier == resolution.AnchorIdentifier
            & x.ResolvedObstacleIdentifier == resolution.ObstacleIdentifier
            & x.ResolvingGoalIdentifier == resolution.ResolvingGoalIdentifier);            

            _model.goalRepository.Remove (e);

            foreach (var ee in e) {
                var anchorRefinements = _model.GoalRefinements (x => x.ParentGoalIdentifier == resolution.AnchorIdentifier);
                if (anchorRefinements.Count () > 1) {
                    throw new IntegrationException (IntegrationException.SINGLE_REFINEMENT_ONLY);
                }
                if (anchorRefinements.Count () == 0) {
                    throw new IntegrationException (IntegrationException.ANCHOR_NO_REFINEMENT);
                }
                var anchorRefinement = anchorRefinements.Single ();

                // Remove replacing goal
                anchorRefinement.SubGoalIdentifiers
                    = new HashSet<GoalRefinee> (
                        anchorRefinement.SubGoalIdentifiers.Where (
                            x => x.Identifier != resolution.ResolvingGoalIdentifier));

                // Add all replaced goals
                foreach (var item in ee.ReplacedGoals) {
                    anchorRefinement.Add (item);
                }
            }
        }
    }
}
