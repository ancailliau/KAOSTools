using System;
using System.Linq;

namespace KAOSTools.MetaModel
{
    public static class ResolutionIntegrationHelper
    {
        public static void IntegrateResolutions (this KAOSModel model) {
            foreach (var goal in model.ObstructedGoals()) {
                foreach (var obstacle in goal.Obstructions()) {
                    RecursiveIntegration (goal, obstacle.Obstacle);
                }
            }
        }

        static void RecursiveIntegration (Goal obstructedGoal, Obstacle obstacle) {
            foreach (var resolution in obstacle.Resolutions()) {
                Integrate (obstructedGoal, obstacle, resolution);
            }

            foreach (var subobstacle in obstacle.Refinements().SelectMany (x => x.Subobstacles)) {
                RecursiveIntegration (obstructedGoal, subobstacle);
            }
        }

        static void Integrate (Goal obstructedGoal, Obstacle obstacle, Resolution resolution) {
            if (resolution.ResolutionPattern == ResolutionPattern.GoalSubstitution) {
                obstructedGoal.Exceptions.Add (new GoalException () {
                    ResolvedObstacle = obstacle,
                    ResolvingGoal = resolution.ResolvingGoal,
                    Implicit = true
                });

            } else if (resolution.ResolutionPattern == ResolutionPattern.ObstaclePrevention) {
                obstructedGoal.Assumptions.Add (new GoalAssumption () {
                    Assumed = resolution.ResolvingGoal,
                    Implicit = true
                });

            } else if (resolution.ResolutionPattern == ResolutionPattern.ObstacleReduction) {
                obstructedGoal.Assumptions.Add (new GoalAssumption () {
                    Assumed = resolution.ResolvingGoal,
                    Implicit = true
                });

            } else if (resolution.ResolutionPattern == ResolutionPattern.GoalRestoration) {
                obstructedGoal.Exceptions.Add (new GoalException () {
                    ResolvedObstacle = obstacle,
                    ResolvingGoal = resolution.ResolvingGoal,
                    Implicit = true
                });

            } else if (resolution.ResolutionPattern == ResolutionPattern.GoalWeakening) {
                obstructedGoal.Exceptions.Add (new GoalException () {
                    ResolvedObstacle = obstacle,
                    ResolvingGoal = resolution.ResolvingGoal,
                    Implicit = true
                });

            } else if (resolution.ResolutionPattern == ResolutionPattern.ObstacleStrongMitigation) {
                obstructedGoal.Exceptions.Add (new GoalException () {
                    ResolvedObstacle = obstacle,
                    ResolvingGoal = resolution.ResolvingGoal,
                    Implicit = true
                });

            } else if (resolution.ResolutionPattern == ResolutionPattern.ObstacleWeakMitigation) {
                var anchor = resolution.Parameters.First () as Goal;

                anchor.Exceptions.Add (new GoalException () {
                    ResolvedObstacle = obstacle,
                    ResolvingGoal = resolution.ResolvingGoal,
                    Implicit = true
                });

                foreach (var subgoal in anchor.Refinements().SelectMany(x => x.Subgoals)) {
                    var assumption = new ObstacleNegativeAssumption {
                        Assumed = obstacle,
                        Implicit = true
                    };
                    Propagate (assumption, subgoal);
                }
            }
        }

        static void Propagate (Assumption assumption, Goal goal) {
            goal.Assumptions.Add (assumption);

            foreach (var children in goal.Refinements().SelectMany (x => x.Subgoals)) {
                Propagate (assumption, children);
            }

            foreach (var obstacle in goal.Obstructions()) {
                Propagate (assumption, obstacle.Obstacle);
            }
        }

        static void Propagate (Assumption assumption, Obstacle obstacle) {
            obstacle.Assumptions.Add (assumption);
            if (assumption is ObstacleNegativeAssumption
                && (assumption as ObstacleNegativeAssumption).Assumed == obstacle) {
                return;
            }

            foreach (var children in obstacle.Refinements().SelectMany (x => x.Subobstacles)) {
                Propagate (assumption, children);
            }

            foreach (var resolution in obstacle.Resolutions()) {
                Propagate (assumption, resolution.ResolvingGoal);
            }
        }
    }
}

