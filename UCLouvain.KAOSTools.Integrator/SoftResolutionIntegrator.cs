using System;
using System.Collections.Generic;
using System.Linq;
using KAOSTools.Core;
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

            Obstacle obstacle = resolution.Obstacle ();
            obstacle.Resolved = true;
        }

        void RemoveException (Resolution resolution)
        {
            var e = _model.Exceptions ().Where (x => x.AnchorGoalIdentifier == resolution.AnchorIdentifier
            & x.ResolvedObstacleIdentifier == resolution.ObstacleIdentifier
            & x.ResolvingGoalIdentifier == resolution.ResolvingGoalIdentifier);            

            _model.goalRepository.Remove (e);

            Obstacle obstacle = resolution.Obstacle ();
            obstacle.Resolved = false;
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
