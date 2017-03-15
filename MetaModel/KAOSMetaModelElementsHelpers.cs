using System;
using System.Collections.Generic;
using System.Linq;

namespace KAOSTools.Core
{
    public static class KAOSCoreElementsHelpers
    {
        #region Goal

        public static IEnumerable<GoalRefinement> Refinements (this Goal goal) {
            return from refinement in goal.model.GoalRefinements()
                where refinement.ParentGoalIdentifier == goal.Identifier
                    select refinement;
        }

        public static IEnumerable<Obstruction> Obstructions (this Goal goal) {
            return from obstruction in goal.model.Obstructions()
                where obstruction.ObstructedGoalIdentifier == goal.Identifier
                    select obstruction;
        }

        public static IEnumerable<Resolution> Resolutions (this Goal goal) {
            return from resolution in goal.model.Resolutions()
                    where resolution.ResolvingGoalIdentifier == goal.Identifier
                select resolution;
        }

        public static IEnumerable<GoalAgentAssignment> AgentAssignments (this Goal goal) {
            return from assignement in goal.model.GoalAgentAssignments()
                where assignement.GoalIdentifier == goal.Identifier
                    select assignement;
        }

        public static IEnumerable<GoalRefinement> ParentRefinements (this Goal goal) {
            return from refinement in goal.model.GoalRefinements()
                where refinement.SubGoalIdentifiers.Contains(goal.Identifier)
                    select refinement;
        }


        public static IEnumerable<GoalException> Exceptions (this Goal goal) {
            return from e in goal.model.Exceptions ()
                    where e.AnchorGoalIdentifier == goal.Identifier
                   select e;
        }

        public static IEnumerable<GoalReplacement> Replacements (this Goal goal) {
            return from e in goal.model.Replacements ()
					                 where e.AnchorGoalIdentifier == goal.Identifier
                select e;
        }

        public static IEnumerable<ObstacleAssumption> Provided (this Goal goal) {
            return from e in goal.model.ObstacleAssumptions ()
                    where e.AnchorGoalIdentifier == goal.Identifier
                select e;
        }

        #endregion

        #region Obstacles

        public static IEnumerable<ObstacleRefinement> Refinements (this Obstacle obstacle) {
            return from refinement in obstacle.model.ObstacleRefinements()
                where refinement.ParentObstacleIdentifier == obstacle.Identifier
                    select refinement;
        }
        
        public static IEnumerable<ObstacleRefinement> ParentRefinements (this Obstacle obstacle) {
            return from refinement in obstacle.model.ObstacleRefinements()
                where refinement.SubobstacleIdentifiers.Contains(obstacle.Identifier)
                    select refinement;
        }

        public static IEnumerable<Resolution> Resolutions (this Obstacle obstacle) {
            return from resolution in obstacle.model.Resolutions()
                where resolution.ObstacleIdentifier == obstacle.Identifier
                    select resolution;
        }

        public static IEnumerable<ObstacleAgentAssignment> AgentAssignments (this Obstacle obstacle) {
            return from assignement in obstacle.model.ObstacleAgentAssignments()
                where assignement.ObstacleIdentifier == obstacle.Identifier
                    select assignement;
        }
        
        public static IEnumerable<Obstruction> Obstructions (this Obstacle obstacle) {
            return from obstruction in obstacle.model.Obstructions()
                where obstruction.ObstacleIdentifier == obstacle.Identifier
                    select obstruction;
        }

        #endregion

        #region Domain Properties

        public static IEnumerable<ObstacleRefinement> ObstacleRefinements (this DomainProperty domProp) {
            return from refinement in domProp.model.ObstacleRefinements()
                where refinement.DomainPropertyIdentifiers.Contains (domProp.Identifier)
                    select refinement;
        }

        public static IEnumerable<GoalRefinement> GoalRefinements (this DomainProperty domProp) {
            return from refinement in domProp.model.GoalRefinements()
                where refinement.DomainPropertyIdentifiers.Contains (domProp.Identifier)
                    select refinement;
        }

        #endregion

        #region Domain Hypothesis

        public static IEnumerable<ObstacleRefinement> ObstacleRefinements (this DomainHypothesis domHyp) {
            return from refinement in domHyp.model.ObstacleRefinements()
                where refinement.DomainHypothesisIdentifiers.Contains (domHyp.Identifier)
                    select refinement;
        }

        public static IEnumerable<GoalRefinement> GoalRefinements (this DomainHypothesis domHyp) {
            return from refinement in domHyp.model.GoalRefinements()
                where refinement.DomainHypothesisIdentifiers.Contains (domHyp.Identifier)
                    select refinement;
        }

        #endregion
        
        #region Agents

        public static IEnumerable<GoalAgentAssignment> AssignedGoals (this Agent agent) {
            return from assignements in agent.model.GoalAgentAssignments()
                where assignements.Agents().Contains (agent)
                    select assignements;
        }
        
        public static IEnumerable<ObstacleAgentAssignment> AssignedObstacles (this Agent agent) {
            return from assignements in agent.model.ObstacleAgentAssignments()
                where assignements.Agents().Contains (agent)
                    select assignements;
        }
        
        public static IEnumerable<AntiGoalAgentAssignment> AssignedAntiGoals (this Agent agent) {
            return from assignements in agent.model.AntiGoalAgentAssignments()
                where assignements.Agents().Contains (agent)
                    select assignements;
        }

        #endregion

        #region Agent assignments

        public static IEnumerable<Agent> Agents (this AgentAssignment agentAssignement) {
            return agentAssignement.AgentIdentifiers
                .Select(x => agentAssignement.model.Agents().SingleOrDefault(y => y.Identifier == x));
        }
        
        public static Goal Goal (this GoalAgentAssignment goalAA) {
            return goalAA.model.Goals().SingleOrDefault(y => y.Identifier == goalAA.GoalIdentifier);
        }

        public static Obstacle Obstacle (this ObstacleAgentAssignment obstacleAA) {
            return obstacleAA.model.Obstacles().SingleOrDefault(y => y.Identifier == obstacleAA.ObstacleIdentifier);
        }

        #endregion

        #region Goal refinement

        public static IEnumerable<Goal> SubGoals (this GoalRefinement refinement) {
            return refinement.SubGoalIdentifiers
                .Select(x => refinement.model.Goals().SingleOrDefault(y => y.Identifier == x));
        }
        
        public static IEnumerable<DomainProperty> DomainProperties (this GoalRefinement refinement) {
            return refinement.DomainPropertyIdentifiers
                .Select(x => refinement.model.DomainProperties().SingleOrDefault(y => y.Identifier == x));
        }

        public static IEnumerable<DomainHypothesis> DomainHypotheses (this GoalRefinement refinement) {
            return refinement.DomainHypothesisIdentifiers
                .Select(x => refinement.model.DomainHypotheses().SingleOrDefault(y => y.Identifier == x));
        }

        public static Goal ParentGoal (this GoalRefinement refinement) {
            return refinement.model.Goals().SingleOrDefault(y => y.Identifier == refinement.ParentGoalIdentifier);
        }
        

        #endregion

                
        #region Obstacle refinement

        public static IEnumerable<Obstacle> SubObstacles (this ObstacleRefinement refinement) {
            return refinement.SubobstacleIdentifiers
                .Select(x => refinement.model.Obstacles().SingleOrDefault(y => y.Identifier == x));
        }

        public static IEnumerable<DomainProperty> DomainProperties (this ObstacleRefinement refinement) {
            return refinement.DomainPropertyIdentifiers
                .Select(x => refinement.model.DomainProperties().SingleOrDefault(y => y.Identifier == x));
        }

        public static IEnumerable<DomainHypothesis> DomainHypotheses (this ObstacleRefinement refinement) {
            return refinement.DomainHypothesisIdentifiers
                .Select(x => refinement.model.DomainHypotheses().SingleOrDefault(y => y.Identifier == x));
        }

        public static Obstacle ParentObstacle (this ObstacleRefinement refinement) {
            return refinement.model.Obstacles().SingleOrDefault(y => y.Identifier == refinement.ParentObstacleIdentifier);
        }

        #endregion
    
        #region Resolution
                
        public static Goal ResolvingGoal (this Resolution resolution) {
            return resolution.model.Goals().SingleOrDefault(y => y.Identifier == resolution.ResolvingGoalIdentifier);
        }
        
        public static Obstacle Obstacle (this Resolution resolution) {
            return resolution.model.Obstacles().SingleOrDefault(y => y.Identifier == resolution.ObstacleIdentifier);
        }

        #endregion

        #region Obstruction
        
        public static Goal ObstructedGoal (this Obstruction obstruction) {
            return obstruction.model.Goals().SingleOrDefault(y => y.Identifier == obstruction.ObstructedGoalIdentifier);
        }

        public static Obstacle Obstacle (this Obstruction obstruction) {
            return obstruction.model.Obstacles().SingleOrDefault(y => y.Identifier == obstruction.ObstacleIdentifier);
        }

        #endregion

        #region Exception

        public static Goal AnchorGoal (this GoalException exception) {
            return exception.model.Goals().SingleOrDefault(y => y.Identifier == exception.AnchorGoalIdentifier);
        }

        public static Goal ResolvingGoal (this GoalException exception) {
            return exception.model.Goals().SingleOrDefault(y => y.Identifier == exception.ResolvingGoalIdentifier);
        }

        public static Obstacle Obstacle (this GoalException exception) {
            return exception.model.Obstacles().SingleOrDefault(y => y.Identifier == exception.ResolvedObstacleIdentifier);
        }

        public static Goal AnchorGoal (this GoalReplacement exception) {
            return exception.model.Goals().SingleOrDefault(y => y.Identifier == exception.AnchorGoalIdentifier);
        }

        public static Goal ResolvingGoal (this GoalReplacement exception) {
            return exception.model.Goals().SingleOrDefault(y => y.Identifier == exception.ResolvingGoalIdentifier);
        }

        public static Obstacle Obstacle (this GoalReplacement exception) {
            return exception.model.Obstacles().SingleOrDefault(y => y.Identifier == exception.ResolvedObstacleIdentifier);
        }

        public static Goal Anchor (this ObstacleAssumption exception) {
            return exception.model.Goals().SingleOrDefault(y => y.Identifier == exception.AnchorGoalIdentifier);
        }

        public static Obstacle Obstacle (this ObstacleAssumption exception) {
            return exception.model.Obstacles().SingleOrDefault(y => y.Identifier == exception.ResolvedObstacleIdentifier);
        }
        #endregion

        #region Entity
        
        public static IEnumerable<Entity> Parents (this Entity entity) {
            return entity.ParentIdentifiers.Select 
                (x => entity.model.Entities().SingleOrDefault(y => y.Identifier == x));
        }
        
        public static IEnumerable<Attribute> Attributes (this Entity entity) {
            return entity.model.Attributes().Where(x => x.EntityIdentifier == entity.Identifier);
        }

        public static ISet<Entity> Ancestors (this Entity entity) {
            var ancestors = new HashSet<Entity>();
            ancestors.Add(entity.model.Entities().SingleOrDefault (x => x.Identifier == entity.Identifier));
            foreach (var parent in entity.Parents()) {
                foreach (var a in parent.Ancestors()) {
                    ancestors.Add (a);
                }
            }
            return ancestors;
        }

        #endregion

        #region Entity

        public static GivenType Type (this Attribute attribute) {
            return attribute.model.GivenTypes().SingleOrDefault(y => y.Identifier == attribute.TypeIdentifier);
        }

        #endregion
    }
}

