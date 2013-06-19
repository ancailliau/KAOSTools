using System;
using System.Collections.Generic;
using System.Linq;

namespace KAOSTools.MetaModel
{
    public static class KAOSModelHelpers
    {
        public static ISet<Goal> RootGoals (this KAOSModel model) {
                var goals = new HashSet<Goal> (model.Goals);
                foreach (var goal in model.Goals)
                    foreach (var refinement in goal.Refinements()) 
                        foreach (var child in refinement.Subgoals)
                            goals.Remove (child);
                foreach (var obstacle in model.Obstacles)
                    foreach (var resolution in obstacle.Resolutions())
                        goals.Remove (resolution.ResolvingGoal);
                return goals;
        }

        public static ISet<AntiGoal> RootAntiGoals (this KAOSModel model) {
            var rootAntiGoals = new HashSet<AntiGoal> (model.AntiGoals);
            foreach (var goal in model.AntiGoals)
                foreach (var refinement in goal.Refinements()) 
                    foreach (var child in refinement.SubAntiGoals)
                        rootAntiGoals.Remove (child);
            return rootAntiGoals;
        }

        public static ISet<AlternativeSystem> RootSystems (this KAOSModel model) {
            var systems = new HashSet<AlternativeSystem> (model.AlternativeSystems);
            foreach (var system in model.AlternativeSystems)
                foreach (var alternative in system.Alternatives) 
                    systems.Remove (alternative);

            return systems;
        }

        public static IEnumerable<Goal> ObstructedGoals (this KAOSModel model) {
                return from g in model.Goals where g.Obstructions().Count() > 0 select g;
        }

        public static IEnumerable<Obstacle> ResolvedObstacles (this KAOSModel model) {
            return from o in model.Obstacles where o.Resolutions().Count() > 0 select o;
        }
    }
}

