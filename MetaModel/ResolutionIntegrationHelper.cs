using System;
using System.Linq;

namespace KAOSTools.MetaModel
{
    public static class ResolutionIntegrationHelper
    {
        public static void IntegrateResolutions (this KAOSModel model) {
            foreach (var goal in model.ObstructedGoals().ToArray ()) {
                foreach (var obstacle in goal.Obstructions().ToArray ()) {
                    RecursiveIntegration (goal, obstacle.Obstacle());
                }
            }
        }

        static void RecursiveIntegration (Goal obstructedGoal, Obstacle obstacle) {
            foreach (var resolution in obstacle.Resolutions().ToArray ()) {
                Integrate (obstructedGoal, obstacle, resolution);
            }

            foreach (var subobstacle in obstacle.Refinements().SelectMany (x => x.SubObstacles()).ToArray ()) {
                RecursiveIntegration (obstructedGoal, subobstacle);
            }
        }

        static Goal FinalAnchor (Goal goal)
        {
            if (goal.Replacements ().Any ()) { // todo shall be single
                return FinalAnchor (goal.Replacements ().First ().AnchorGoal ());
            }
            return goal;
        }

        static void Integrate (Goal obstructedGoal, Obstacle obstacle, Resolution resolution) {
            var anchor = obstructedGoal;
            if (resolution.Parameters.Count > 0) {
                anchor = resolution.Parameters[0];
            }
            anchor = FinalAnchor (anchor);

            if (resolution.ResolutionPattern == ResolutionPattern.GoalSubstitution
                | resolution.ResolutionPattern == ResolutionPattern.GoalWeakening) {

                var goalReplacement = new GoalReplacement (obstructedGoal.model) {
                    Implicit = true
                };
                goalReplacement.SetObstacle (obstacle);
                goalReplacement.SetResolvingGoal (resolution.ResolvingGoal ());
                goalReplacement.SetAnchorGoal (anchor);

                obstructedGoal.model.Add (goalReplacement);

                // Replace in refinements
                var resolving_goal = resolution.ResolvingGoal ();
                foreach (var r in anchor.ParentRefinements ().ToArray ()) {
                    r.Remove (anchor);
                    r.Add (resolving_goal);
                }

                // Replace children refinements
                foreach (var r in anchor.Refinements ().ToArray ()) {
                    anchor.model.Remove (r);
                    var r2 = (GoalRefinement) r.Copy ();
                    r2.Identifier = Guid.NewGuid ().ToString ();
                    r2.SetParentGoal (resolving_goal);
                    resolution.model.Add (r2);
                }


                // Replace in exceptions
                foreach (var r in anchor.Exceptions ().ToArray ()) {
                    r.SetAnchorGoal (resolving_goal);
                }

                // Replace in provided
                foreach (var r in anchor.Provided ().ToArray ()) {
                    r.SetAnchorGoal (resolving_goal);
                }

                // Replace in agent assignements
                foreach (var r in anchor.AgentAssignments ().ToArray ()) {
                    r.GoalIdentifier = resolving_goal.Identifier;
                }

            } else {

                var goalException = new GoalException (obstructedGoal.model) {
                    Implicit = true
                };
                goalException.SetObstacle (obstacle);
                goalException.SetResolvingGoal (resolution.ResolvingGoal ());
                goalException.SetAnchorGoal (anchor);

                obstructedGoal.model.Add (goalException);

                /*
                var obstacleAssumption = new ObstacleAssumption (resolution.model);
                obstacleAssumption.SetAnchorGoal (anchor);
                obstacleAssumption.SetObstacle (obstacle);

                if (anchor.Identifier != obstructedGoal.Identifier) {
                Console.WriteLine ("DownPropagate " + obstacle.FriendlyName + " ("+obstructedGoal.FriendlyName +") on " + anchor.FriendlyName );
                }
                */

                // DownPropagate (obstacleAssumption, anchor);
            }
        }


        /*

                foreach (var subgoal in anchor.Refinements().SelectMany(x => x.SubGoals())) {
                    var assumption = new ObstacleNegativeAssumption {
                        Assumed = obstacle,
                        Implicit = true
                    };
                    Propagate (assumption, subgoal);
                }

*/

        static void DownPropagate (ObstacleAssumption assumption, Goal goal) {

            foreach (var children in goal.Refinements().SelectMany (x => x.SubGoals()).ToArray()) {

                Console.WriteLine ("<pre> -- " + children.FriendlyName + " -- " + assumption.Obstacle().FriendlyName);

                Console.WriteLine (string.Join ("\n", children.Obstacles ().Select (x => x.Identifier)));

                Console.WriteLine ("</pre><hr />");

                if (children.Obstacles ().Select (x => x.Identifier).Contains (assumption.ResolvedObstacleIdentifier)) {

                var obstacleAssumption = (ObstacleAssumption) assumption.Copy ();
                obstacleAssumption.Identifier = Guid.NewGuid().ToString ();
                obstacleAssumption.SetAnchorGoal (children);
                children.model.Add (obstacleAssumption);

                DownPropagate (obstacleAssumption, children);
                
                }
            }
            /*
            foreach (var obstacle in goal.Obstructions()) {
                var obstacleAssumption = assumption.Copy ();
                obstacleAssumption.SetAnchorGoal (children);
                DownPropagate (obstacleAssumption, obstacle.Obstacle());
            }
            */
        }
        /*
        static void DownPropagate (ObstacleAssumption assumption, Obstacle obstacle) {
            obstacle.model.Add (obstacleAssumption);

            if (assumption is ObstacleNegativeAssumption
                && assumption.Assumed == obstacle) {
                return;
            }

            foreach (var children in obstacle.Refinements().SelectMany (x => x.SubObstacles())) {
                DownPropagate (assumption, children);
            }

            foreach (var resolution in obstacle.Resolutions()) {
                DownPropagate (assumption, resolution.ResolvingGoal());
            }
        }
*/
    }
}

