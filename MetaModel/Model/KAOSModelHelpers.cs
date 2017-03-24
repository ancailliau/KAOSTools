using System;
using System.Collections.Generic;
using System.Linq;
using UCLouvain.KAOSTools.Core.Agents;

namespace KAOSTools.Core
{
    public static class KAOSModelHelpers
    {
        #region Goals

        public static IEnumerable<Goal> Goals (this KAOSModel model) {
            return model.goalRepository.GetGoals();
        }

        public static IEnumerable<Goal> Goals (this KAOSModel model, Predicate<Goal> pred) {
            return model.goalRepository.GetGoals (pred);
        }

        public static Goal Goal (this KAOSModel model, Predicate<Goal> pred) {
            return model.goalRepository.GetGoal (pred);
        }

        public static IEnumerable<GoalRefinement> GoalRefinements(this KAOSModel model)
		{
			return model.goalRepository.GetGoalRefinements();
		}

		public static IEnumerable<GoalException> Exceptions(this KAOSModel model)
		{
            return model.goalRepository.GetGoalExceptions();
		}

		public static IEnumerable<GoalReplacement> Replacements(this KAOSModel model)
		{
            return model.goalRepository.GetGoalReplacements();
		}

        #endregion

        #region Obtacles

        public static IEnumerable<Obstacle> Obstacles (this KAOSModel model) {
            return model.obstacleRepository.GetObstacles ();
        }

        public static IEnumerable<Obstacle> Obstacles (this KAOSModel model, Predicate<Obstacle> pred) {
            return model.obstacleRepository.GetObstacles (pred);
        }

        public static Obstacle Obstacle (this KAOSModel model, Predicate<Obstacle> pred) {
            return model.obstacleRepository.GetObstacle(pred);
		}
		
        public static IEnumerable<ObstacleRefinement> ObstacleRefinements(this KAOSModel model)
		{
            return model.obstacleRepository.GetObstacleRefinements();
		}

		public static IEnumerable<Obstruction> Obstructions(this KAOSModel model)
		{
			return model.obstacleRepository.GetObstructions();
		}
		
        public static IEnumerable<Resolution> Resolutions(this KAOSModel model)
		{
            return model.obstacleRepository.GetResolutions();
		}

		public static IEnumerable<ObstacleAssumption> ObstacleAssumptions(this KAOSModel model)
		{
            return model.obstacleRepository.GetObstacleAssumptions();
		}

		#endregion

		#region Domain properties

		public static IEnumerable<DomainProperty> DomainProperties (this KAOSModel model) {
            return model.domainRepository.GetDomainProperties();
        }

        public static IEnumerable<DomainProperty> DomainProperties(this KAOSModel model, Predicate<DomainProperty> predicate)
		{
			return model.domainRepository.GetDomainProperties(predicate);
		}
		
        public static DomainProperty DomainProperty(this KAOSModel model, Predicate<DomainProperty> predicate)
		{
			return model.domainRepository.GetDomainProperty(predicate);
		}

		#endregion

		#region Domain Hypothesis

		public static IEnumerable<DomainHypothesis> DomainHypotheses(this KAOSModel model)
		{
            return model.domainRepository.GetDomainHypotheses();
        }
		
        public static IEnumerable<DomainHypothesis> DomainHypotheses(this KAOSModel model, Predicate<DomainHypothesis> predicate)
		{
			return model.domainRepository.GetDomainHypotheses(predicate);
		}

		public static DomainHypothesis DomainHypothesis(this KAOSModel model, Predicate<DomainHypothesis> predicate)
		{
			return model.domainRepository.GetDomainHypothesis(predicate);
		}

		#endregion

        #region Agents

        public static IEnumerable<Agent> Agents (this KAOSModel model) {
            return model.agentRepository.GetAgents();
		}

		public static IEnumerable<GoalAgentAssignment> GoalAgentAssignments(this KAOSModel model)
		{
            return model.agentRepository.GetAgentAssignments();
		}

        #endregion

        #region Object model

        public static IEnumerable<Predicate> Predicates (this KAOSModel model) {
            return model.formalSpecRepository.GetPredicates();
        }
        
        public static IEnumerable<EntityAttribute> Attributes (this KAOSModel model) {
            return model.entityRepository.GetEntityAttributes();
        }

        public static IEnumerable<Entity> Entities (this KAOSModel model) {
            return model.entityRepository.GetEntities();
        }

        public static IEnumerable<Entity> Entities (this KAOSModel model, Predicate<Entity> pred) {
            return model.entityRepository.GetEntities(pred);
        }

		public static Entity Entity(this KAOSModel model, Predicate<Entity> pred)
		{
            return model.entityRepository.GetEntity(pred);
        }

		public static IEnumerable<Relation> Relations(this KAOSModel model)
		{
            return model.entityRepository.GetRelations();
        }

		public static IEnumerable<GivenType> GivenTypes(this KAOSModel model)
		{
            return model.entityRepository.GetGivenTypes();
        }

		public static IEnumerable<GivenType> GivenTypes(this KAOSModel model, Predicate<GivenType> pred)
		{
            return model.entityRepository.GetGivenTypes(pred);
        }
        
        public static GivenType GivenType(this KAOSModel model, Predicate<GivenType> pred)
		{
            return model.entityRepository.GetGivenType(pred);
		}

        #endregion

        #region Metamodel data

        public static IEnumerable<Calibration> CalibrationVariables(this KAOSModel model)
		{
            return model.modelMetadataRepository.GetCalibrations();
        }

        public static IEnumerable<Calibration> CalibrationVariables (this KAOSModel model, Predicate<Calibration> pred) {
            return model.modelMetadataRepository.GetCalibrations(pred);
        }

        public static IEnumerable<Expert> Experts (this KAOSModel model) {
            return model.modelMetadataRepository.GetExperts();
        }

		public static IEnumerable<Expert> Experts(this KAOSModel model, Predicate<Expert> pred)
		{
			return model.modelMetadataRepository.GetExperts(pred);
        }

        #endregion


		public static ISet<Goal> RootGoals (this KAOSModel model) {
                var goals = new HashSet<Goal> (model.Goals());
                foreach (var goal in model.Goals())
                    foreach (var refinement in goal.Refinements()) 
                        foreach (var child in refinement.SubGoals())
                            goals.Remove (child);
                foreach (var obstacle in model.Obstacles())
                    foreach (var resolution in obstacle.Resolutions())
                        goals.Remove (resolution.ResolvingGoal());
                return goals;
        }

        public static IEnumerable<Obstacle> RootObstacles (this KAOSModel model) {
            return model.Obstructions ().Select ( x => x.Obstacle() );
        }

        public static IEnumerable<Goal> ObstructedGoals (this KAOSModel model) {
            return from g in model.Obstructions() select g.ObstructedGoal ();
        }

        public static IEnumerable<Obstacle> ResolvedObstacles (this KAOSModel model) {
            return from o in model.Obstacles() where o.Resolutions().Count() > 0 select o;
        }

        public static IEnumerable<Obstacle> Obstacles (this Goal goal) {
            return goal.Refinements ().SelectMany (x => x.SubGoals ().SelectMany (y => y.Obstacles ()))
                    .Union (goal.Obstructions().SelectMany (x => x.Obstacles()));
        }

        public static IEnumerable<Obstacle> Obstacles (this Obstacle o) {
            return o.Refinements ().SelectMany (x => x.SubObstacles().SelectMany (y => y.Obstacles ()))
                    .Union (o.Resolutions().SelectMany (x => x.Obstacles()));
        }

        public static IEnumerable<Obstacle> Obstacles (this Obstruction o) {
            return o.Obstacle().Obstacles ();
        }

        public static IEnumerable<Obstacle> Obstacles (this Resolution o) {
            return o.ResolvingGoal ().Obstacles ();
        }


        public static ISet<Obstacle> LeafObstacles (this KAOSModel model) {
            var obstacles = new HashSet<Obstacle> (model.Obstacles());

            foreach (var refinement in model.ObstacleRefinements ())
                obstacles.Remove (refinement.ParentObstacle ());

            //foreach (var obstruction in model.Obstructions ())
            //    obstacles.Remove (obstruction.Obstacle ());

            return obstacles;
        }
    }
}

