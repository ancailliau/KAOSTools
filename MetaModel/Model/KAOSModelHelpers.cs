using System;
using System.Collections.Generic;
using System.Linq;
using UCLouvain.KAOSTools.Core.Agents;

namespace KAOSTools.Core
{
    public static class KAOSModelHelpers
    {
        #region Goal model

        public static IEnumerable<Goal> Goals (this KAOSModel model) {
                return model.Elements.Where (x => x is Goal).Cast<Goal>();
        }

        public static IEnumerable<Goal> Goals (this KAOSModel model, Predicate<Goal> pred) {
            return model.Elements.Where (x => x is Goal && pred (x as Goal)).Cast<Goal>();
        }

        public static Goal Goal (this KAOSModel model) {
            return (Goal) model.Elements.SingleOrDefault (x => x is Goal);
        }

        public static Goal Goal (this KAOSModel model, Predicate<Goal> pred) {
            return (Goal) model.Elements.SingleOrDefault (x => x is Goal && pred (x as Goal));
        }

        public static IEnumerable<Obstacle> Obstacles (this KAOSModel model) {
                return model.Elements.Where (x => x is Obstacle).Cast<Obstacle>();
        }
        public static IEnumerable<Obstacle> Obstacles (this KAOSModel model, Predicate<Obstacle> pred) {
            return model.Elements.Where (x => x is Obstacle && pred (x as Obstacle)).Cast<Obstacle>();
        }

        public static Obstacle Obstacle (this KAOSModel model) {
            return (Obstacle) model.Elements.SingleOrDefault (x => x is Obstacle);
        }

        public static Obstacle Obstacle (this KAOSModel model, Predicate<Obstacle> pred) {
            return (Obstacle) model.Elements.SingleOrDefault (x => x is Obstacle && pred (x as Obstacle));
        }


        public static IEnumerable<DomainProperty> DomainProperties (this KAOSModel model) {
                return model.Elements.Where (x => x is DomainProperty).Cast<DomainProperty>();
        }

        public static IEnumerable<DomainHypothesis> DomainHypotheses (this KAOSModel model) {
                return model.Elements.Where (x => x is DomainHypothesis).Cast<DomainHypothesis>();
        }

        // Relations

        public static IEnumerable<GoalRefinement> GoalRefinements (this KAOSModel model) {
			return model.Elements.OfType<GoalRefinement> ();
        }


        public static IEnumerable<ObstacleRefinement> ObstacleRefinements (this KAOSModel model) {
                return model.Elements.Where (x => x is ObstacleRefinement).Cast<ObstacleRefinement>();
        }

        public static IEnumerable<Obstruction> Obstructions (this KAOSModel model) {
                return model.Elements.Where (x => x is Obstruction).Cast<Obstruction>();
        }

        public static IEnumerable<Resolution> Resolutions (this KAOSModel model) {
                return model.Elements.Where (x => x is Resolution).Cast<Resolution>();
        }

        public static IEnumerable<GoalAgentAssignment> GoalAgentAssignments (this KAOSModel model) {
                return model.Elements.Where (x => x is GoalAgentAssignment).Cast<GoalAgentAssignment>();
        }

        public static IEnumerable<GoalException> Exceptions (this KAOSModel model) {
            return model.Elements.Where (x => x is GoalException).Cast<GoalException>();
        }

        public static IEnumerable<GoalReplacement> Replacements (this KAOSModel model) {
            return model.Elements.Where (x => x is GoalReplacement).Cast<GoalReplacement>();
        }

        public static IEnumerable<ObstacleAssumption> ObstacleAssumptions (this KAOSModel model) {
            return model.Elements.Where (x => x is ObstacleAssumption).Cast<ObstacleAssumption>();
        }

        #endregion

        #region Agent model

        public static IEnumerable<Agent> Agents (this KAOSModel model) {
                return model.Elements.Where (x => x is Agent).Cast<Agent>();
        }

        #endregion

        #region Object model

        public static IEnumerable<Predicate> Predicates (this KAOSModel model) {
                return model.Elements.Where (x => x is Predicate).Cast<Predicate>();
        }
        
        public static IEnumerable<EntityAttribute> Attributes (this KAOSModel model) {
            return model.Elements.Where (x => x is EntityAttribute).Cast<EntityAttribute>();
        }

        public static IEnumerable<Entity> Entities (this KAOSModel model) {
                return model.Elements.Where (x => x is Entity & !(x is Relation)).Cast<Entity>();
        }

        public static IEnumerable<Entity> Entities (this KAOSModel model, Predicate<Entity> pred) {
            return model.Elements.Where (x => (x is Entity & !(x is Relation)) && pred(x as Entity)).Cast<Entity>();
        }

        public static Entity Entity (this KAOSModel model, Predicate<Entity> pred) {
            return model.Elements.SingleOrDefault (x => (x is Entity & !(x is Relation)) && pred(x as Entity)) as Entity;
        }

        public static IEnumerable<Relation> Relations (this KAOSModel model) {
                return model.Elements.Where (x => x is Relation).Cast<Relation>();
        }

        public static IEnumerable<GivenType> GivenTypes (this KAOSModel model) {
            return model.Elements.Where (x => x is GivenType).Cast<GivenType>();
        }

        public static IEnumerable<GivenType> GivenTypes (this KAOSModel model, Predicate<GivenType> pred) {
                return model.Elements.Where (x => x is GivenType && pred(x as GivenType)).Cast<GivenType>();
        }
        
        public static GivenType GivenType (this KAOSModel model, Predicate<GivenType> pred) {
            return model.Elements.SingleOrDefault (x => x is GivenType && pred(x as GivenType)) as GivenType;
        }


        public static IEnumerable<Calibration> CalibrationVariables (this KAOSModel model) {
            return model.Elements.Where (x => x is Calibration).Cast<Calibration>();
        }

        public static IEnumerable<Calibration> CalibrationVariables (this KAOSModel model, Predicate<Calibration> pred) {
            return model.Elements.Where (x => x is Calibration && pred(x as Calibration)).Cast<Calibration>();
        }

        public static IEnumerable<Expert> Experts (this KAOSModel model) {
            return model.Elements.Where (x => x is Expert).Cast<Expert>();
        }

        public static IEnumerable<Expert> Experts (this KAOSModel model, Predicate<Expert> pred) {
            return model.Elements.Where (x => x is Expert && pred(x as Expert)).Cast<Expert>();
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

