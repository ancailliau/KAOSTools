using System;
using System.Collections.Generic;
using System.Linq;
using KAOSTools.Core;
namespace UCLouvain.KAOSTools.Integrator
{
    public class ResolutionIntegrator
    {
        KAOSModel _model;
    
        public ResolutionIntegrator (KAOSModel model)
        {
            _model = model;
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
            throw new NotImplementedException ();
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
