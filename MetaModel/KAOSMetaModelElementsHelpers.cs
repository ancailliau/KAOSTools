using System;
using System.Collections.Generic;
using System.Linq;

namespace KAOSTools.MetaModel
{
    public static class KAOSMetaModelElementsHelpers
    {
        #region Goal

        public static IEnumerable<GoalRefinement> Refinements (this Goal goal) {
            return from refinement in goal.model.GoalRefinements()
                where refinement.ParentGoal == goal
                    select refinement;
        }

        public static IEnumerable<Obstruction> Obstructions (this Goal goal) {
            return from obstruction in goal.model.Obstructions()
                where obstruction.ObstructedGoal == goal
                    select obstruction;
        }

        public static IEnumerable<GoalAgentAssignment> AgentAssignments (this Goal goal) {
            return from assignement in goal.model.GoalAgentAssignments()
                where assignement.Goal == goal
                    select assignement;
        }

        public static IEnumerable<GoalRefinement> ParentRefinements (this Goal goal) {
            return from refinement in goal.model.GoalRefinements()
                where refinement.Subgoals.Contains(goal)
                    select refinement;
        }

        #endregion

        #region AntiGoals()

        public static IEnumerable<AntiGoalRefinement> Refinements (this AntiGoal antiGoal) {
            return from refinement in antiGoal.model.AntiGoalRefinements()
                where refinement.ParentAntiGoal == antiGoal
                    select refinement;
        }

        public static IEnumerable<AntiGoalAgentAssignment> AgentAssignments (this AntiGoal antiGoal) {
            return from assignement in antiGoal.model.AntiGoalAgentAssignments()
                where assignement.AntiGoal == antiGoal
                    select assignement;
        }

        public static IEnumerable<AntiGoalRefinement> ParentRefinements (this AntiGoal antiGoal) {
            return from refinement in antiGoal.model.AntiGoalRefinements()
                where refinement.SubAntiGoals.Contains(antiGoal)
                    select refinement;
        }

        #endregion

        #region Obstacles

        public static IEnumerable<ObstacleRefinement> Refinements (this Obstacle obstacle) {
            return from refinement in obstacle.model.ObstacleRefinements()
                where refinement.ParentObstacle == obstacle
                    select refinement;
        }
        
        public static IEnumerable<ObstacleRefinement> ParentRefinements (this Obstacle obstacle) {
            return from refinement in obstacle.model.ObstacleRefinements()
                where refinement.Subobstacles.Contains(obstacle)
                    select refinement;
        }

        public static IEnumerable<Resolution> Resolutions (this Obstacle obstacle) {
            return from resolution in obstacle.model.Resolutions()
                where resolution.Obstacle == obstacle
                    select resolution;
        }

        public static IEnumerable<ObstacleAgentAssignment> AgentAssignments (this Obstacle obstacle) {
            return from assignement in obstacle.model.ObstacleAgentAssignments()
                where assignement.Obstacle == obstacle
                    select assignement;
        }

        #endregion

        #region Domain Properties

        public static IEnumerable<ObstacleRefinement> ObstacleRefinements (this DomainProperty domProp) {
            return from refinement in domProp.model.ObstacleRefinements()
                where refinement.DomainProperties.Contains (domProp)
                    select refinement;
        }

        public static IEnumerable<GoalRefinement> GoalRefinements (this DomainProperty domProp) {
            return from refinement in domProp.model.GoalRefinements()
                where refinement.DomainProperties.Contains (domProp)
                    select refinement;
        }

        #endregion

        #region Domain Hypothesis

        public static IEnumerable<ObstacleRefinement> ObstacleRefinements (this DomainHypothesis domHyp) {
            return from refinement in domHyp.model.ObstacleRefinements()
                where refinement.DomainHypotheses.Contains (domHyp)
                    select refinement;
        }

        public static IEnumerable<GoalRefinement> GoalRefinements (this DomainHypothesis domHyp) {
            return from refinement in domHyp.model.GoalRefinements()
                where refinement.DomainHypotheses.Contains (domHyp)
                    select refinement;
        }

        #endregion
        
        #region Agents

        public static IEnumerable<GoalAgentAssignment> AssignedGoals (this Agent agent) {
            return from assignements in agent.model.GoalAgentAssignments()
                where assignements.Agents.Contains (agent)
                    select assignements;
        }
        
        public static IEnumerable<ObstacleAgentAssignment> AssignedObstacles (this Agent agent) {
            return from assignements in agent.model.ObstacleAgentAssignments()
                where assignements.Agents.Contains (agent)
                    select assignements;
        }
        
        public static IEnumerable<AntiGoalAgentAssignment> AssignedAntiGoals (this Agent agent) {
            return from assignements in agent.model.AntiGoalAgentAssignments()
                where assignements.Agents.Contains (agent)
                    select assignements;
        }

        #endregion
    }
}

