using System;
using System.Collections.Generic;
using System.Linq;

namespace KAOSTools.MetaModel
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

        public static IEnumerable<AntiGoal> AntiGoals (this KAOSModel model) {
                return model.Elements.Where (x => x is AntiGoal).Cast<AntiGoal>();
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

        public static IEnumerable<AlternativeSystem> AlternativeSystems (this KAOSModel model) {
                return model.Elements.Where (x => x is AlternativeSystem).Cast<AlternativeSystem>();
        }

        // Relations

        public static IEnumerable<GoalRefinement> GoalRefinements (this KAOSModel model) {
                return model.Elements.Where (x => x is GoalRefinement).Cast<GoalRefinement>();
        }

        public static IEnumerable<AntiGoalRefinement> AntiGoalRefinements (this KAOSModel model) {
                return model.Elements.Where (x => x is AntiGoalRefinement).Cast<AntiGoalRefinement>();
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

        public static IEnumerable<AntiGoalAgentAssignment> AntiGoalAgentAssignments (this KAOSModel model) {
                return model.Elements.Where (x => x is AntiGoalAgentAssignment).Cast<AntiGoalAgentAssignment>();
        }

        public static IEnumerable<ObstacleAgentAssignment> ObstacleAgentAssignments (this KAOSModel model) {
                return model.Elements.Where (x => x is ObstacleAgentAssignment).Cast<ObstacleAgentAssignment>();
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
        
        public static IEnumerable<Attribute> Attributes (this KAOSModel model) {
            return model.Elements.Where (x => x is Attribute).Cast<Attribute>();
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

        public static IEnumerable<AntiGoal> RootAntiGoals (this KAOSModel model) {
            var rootAntiGoals = new HashSet<string> (model.AntiGoals().Select (x => x.Identifier));
            foreach (var goal in model.AntiGoals())
                foreach (var refinement in goal.Refinements()) 
                    foreach (var child in refinement.SubAntiGoalIdentifiers)
                        rootAntiGoals.Remove (child);
            return rootAntiGoals.Select (x => model.AntiGoals().SingleOrDefault (y => y.Identifier == x));
        }

        public static ISet<AlternativeSystem> RootSystems (this KAOSModel model) {
            var systems = new HashSet<AlternativeSystem> (model.AlternativeSystems());
            foreach (var system in model.AlternativeSystems())
                foreach (var alternative in system.Alternatives) 
                    systems.Remove (alternative);

            return systems;
        }

        public static IEnumerable<Goal> ObstructedGoals (this KAOSModel model) {
                return from g in model.Goals() where g.Obstructions().Count() > 0 select g;
        }

        public static IEnumerable<Obstacle> ResolvedObstacles (this KAOSModel model) {
            return from o in model.Obstacles() where o.Resolutions().Count() > 0 select o;
        }
    }
}

