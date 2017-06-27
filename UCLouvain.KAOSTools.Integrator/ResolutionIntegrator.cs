using System;
using System.Collections.Generic;
using System.Linq;
using KAOSTools.Core;
using UCLouvain.KAOSTools.Core.SatisfactionRates;

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

            Obstacle obstacle = resolution.Obstacle ();
            propagator.DownPropagateAddConjunct
                (resolution,
                 new Not (obstacle.FormalSpec),
                 obstacle.Name != null ? "Not " + obstacle.Name : null);

            // Set the probability of the resolved obstacle to 0
            // TODO Use a flag "resolved"
            obstacle.Resolved = true;
            //_model.satisfactionRateRepository.AddObstacleSatisfactionRate (obstacle.Identifier, new DoubleSatisfactionRate (0));

            // Remove the resolution, its appearans in refinements and obstructions
            //Delete (resolution);
        }

        private void Delete (Resolution resolution, bool others = true)
        {
            Obstacle obstacle = _model.Obstacle (resolution.ObstacleIdentifier);
            _model.obstacleRepository.Remove (resolution);

            if (!others)
                return;

            foreach (var obstruction in obstacle.Obstructions ().ToList ()) {
                _model.obstacleRepository.Remove (obstruction);
            }

            foreach (var refinement in _model.ObstacleRefinements (x => x.SubobstacleIdentifiers.Any (y => y.Identifier == obstacle.Identifier))) {
                foreach (var refinee in refinement.SubobstacleIdentifiers.Where (x => x.Identifier == obstacle.Identifier).ToList ()) {
                    refinement.SubobstacleIdentifiers.Remove (refinee);
                }
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
            
            //Delete (resolution, false);
        }
    }
}
