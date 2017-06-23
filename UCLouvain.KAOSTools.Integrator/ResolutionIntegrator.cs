using System;
using System.Collections.Generic;
using System.Linq;
using KAOSTools.Core;

namespace UCLouvain.KAOSTools.Integrators
{
    public class ResolutionIntegrator
    {
        KAOSModel _model;
        ChangePropagator propagator;
    
        public ResolutionIntegrator (KAOSModel model)
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
            || resolution.ResolutionPattern == ResolutionPattern.ObstacleReduction)
            {
                KeepObstructedGoal (resolution);
            }
        }

        void KeepObstructedGoal (Resolution resolution)
        {   
            // Get the anchor goal
            var anchor = resolution.AnchorIdentifier;
            var anchor_goal = _model.Goal (anchor);

            // Get the refinements in which the anchor goal is involved
            var refinements = _model.GoalRefinements (x => x.SubGoalIdentifiers.Any (y => y.Identifier == anchor));

            // Create a new goal
            var new_anchor = new Goal (_model);
            new_anchor.Name = anchor_goal.Name;
            new_anchor.FormalSpec = anchor_goal.FormalSpec;
            _model.Add (new_anchor);

            // Replace anchor goal by new goal in refinements
            foreach (var r in refinements) {
                r.SubGoalIdentifiers = new HashSet<GoalRefinee> (r.SubGoalIdentifiers.Where (x => x.Identifier != anchor));
                r.Add (new_anchor);
            }

            // Create a new refinement with anchor goal and cm goal
            var new_refinement = new GoalRefinement (_model);
            new_refinement.ParentGoalIdentifier = new_anchor.Identifier;
            new_refinement.Add (anchor);
            new_refinement.Add (resolution.ResolvingGoalIdentifier);
            _model.Add (new_refinement);

            // Propagate conditions
            if (resolution.Obstacle ().FormalSpec != null)
                propagator.DownPropagateAddConjunct (anchor_goal, new Not (resolution.Obstacle ().FormalSpec));
        }

        void RemoveObstructedGoal (Resolution resolution)
        {
            // Get the obstructed goals
            var obstructed = GetObstructedGoals (resolution.ObstacleIdentifier);

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
        }
        
        IEnumerable<string> GetObstructedGoals (string obstacleIdentifier)
        {
            var obstructedGoals = _model.Obstructions (
                (obj) => obj.ObstacleIdentifier == obstacleIdentifier)
                .Select (x => x.ObstructedGoalIdentifier);
            return GetAncestors (obstructedGoals);
        }
        
        IEnumerable<string> GetAncestors (IEnumerable<string> goals) 
        {
            var fixpoint = new HashSet<string> (goals);
            var goalsToProcessSet = new HashSet<string> (goals);
            var goalsToProcess = new Queue<string>(goals);
            while (goalsToProcess.Count > 0)
            {
                var current = goalsToProcess.Dequeue ();
                goalsToProcessSet.Remove (current);
                var refinements = _model.GoalRefinements (x => x.SubGoalIdentifiers.Any (y => y.Identifier == current));
                foreach (var r in refinements) {
                    fixpoint.Add (r.ParentGoalIdentifier);
                    if (!goalsToProcessSet.Contains (r.ParentGoalIdentifier)) {
                        goalsToProcessSet.Add (r.ParentGoalIdentifier);
                        goalsToProcess.Enqueue (r.ParentGoalIdentifier);
                    }
                }
            }

            return fixpoint;
        }
    }
}
