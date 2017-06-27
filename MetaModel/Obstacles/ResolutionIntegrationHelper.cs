using System;
using System.Linq;
using NLog;

namespace UCLouvain.KAOSTools.Core
{
    public static class ResolutionIntegrationHelper
    {
		static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public static void IntegrateResolutions (this KAOSModel model) {
            foreach (var goal in model.ObstructedGoals().ToArray ()) {
                foreach (var obstacle in goal.Obstructions().ToArray ()) {
                    RecursiveIntegration (goal, obstacle.Obstacle());
                }
            }
        }

        static void RecursiveIntegration (Goal obstructedGoal, Obstacle obstacle) {
            foreach (var resolution in obstacle.Resolutions().ToArray ()) {
                Integrate (resolution);
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

        public static void Integrate (Resolution resolution) {
   //         if (resolution.Parameters.Count == 0) {
			//	logger.Info("Wants to integrate " + resolution.ResolvingGoal().FriendlyName);
			//	logger.Info(resolution.Obstacle().FriendlyName);
			//	throw new NotImplementedException("No parameters to resolvedby");
   //         }
			//var anchor = resolution.Parameters[0];
    //        // anchor = FinalAnchor (anchor);

    //        if (resolution.ResolutionPattern == ResolutionPattern.GoalSubstitution
    //            | resolution.ResolutionPattern == ResolutionPattern.GoalWeakening) {

    //            var replacement = resolution.model.Replacements()
				//							.Where(x => x.ResolvedObstacleIdentifier == resolution.ObstacleIdentifier
				//								   & x.ResolvingGoalIdentifier == resolution.ResolvingGoalIdentifier
				//								   & x.AnchorGoalIdentifier == anchor.Identifier).ToList();

				//var goalReplacement = new GoalReplacement (resolution.model) {
    //                Implicit = true
    //            };
				//goalReplacement.SetObstacle (resolution.Obstacle ());
    //            goalReplacement.SetResolvingGoal (resolution.ResolvingGoal ());
    //            goalReplacement.SetAnchorGoal (anchor);

				//resolution.model.Add (goalReplacement);
				////logger.Info("Add replacement " + resolution.ResolvingGoal().FriendlyName + " to " + anchor.FriendlyName);

    //            // Replace in refinements
    //            //var resolving_goal = resolution.ResolvingGoal ();
    //            //foreach (var r in anchor.ParentRefinements ().ToArray ()) {
    //            //    r.Remove (anchor);
    //            //    r.Add (resolving_goal);
    //            //}

    //            // Replace children refinements
    //            //foreach (var r in anchor.Refinements ().ToArray ()) {
    //            //    anchor.model.Remove (r);
    //            //    var r2 = (GoalRefinement) r.Copy ();
    //            //    r2.Identifier = Guid.NewGuid ().ToString ();
    //            //    r2.SetParentGoal (resolving_goal);
    //            //    resolution.model.Add (r2);
    //            //}

    //            //// Replace in exceptions
    //            //foreach (var r in anchor.Exceptions ().ToArray ()) {
    //            //    r.SetAnchorGoal (resolving_goal);
    //            //}

    //            //// Replace in provided
    //            //foreach (var r in anchor.Provided ().ToArray ()) {
    //            //    r.SetAnchorGoal (resolving_goal);
    //            //}

    //            // Replace in agent assignements
    //            //foreach (var r in anchor.AgentAssignments ().ToArray ()) {
    //            //    r.GoalIdentifier = resolving_goal.Identifier;
    //            //}

    //        } else {

				//var goalException = new GoalException (resolution.model) {
    //                Implicit = true
    //            };
				//goalException.SetObstacle (resolution.Obstacle ());
    //            goalException.SetResolvingGoal (resolution.ResolvingGoal ());
    //            goalException.SetAnchorGoal (anchor);

				//resolution.model.Add (goalException);

            //    /*
            //    var obstacleAssumption = new ObstacleAssumption (resolution.model);
            //    obstacleAssumption.SetAnchorGoal (anchor);
            //    obstacleAssumption.SetObstacle (obstacle);

            //    if (anchor.Identifier != obstructedGoal.Identifier) {
            //    Console.WriteLine ("DownPropagate " + obstacle.FriendlyName + " ("+obstructedGoal.FriendlyName +") on " + anchor.FriendlyName );
            //    }
            //    */

            //    // DownPropagate (obstacleAssumption, anchor);
            //}
        }

		public static void Desintegrate(Resolution resolution)
		{
			//if (resolution.Parameters.Count == 0) {
			//	throw new NotImplementedException();
			//}
			//var anchor = (Goal) resolution.Parameters[0];
			//// anchor = FinalAnchor (anchor);

			//if (resolution.ResolutionPattern == ResolutionPattern.GoalSubstitution
			//	| resolution.ResolutionPattern == ResolutionPattern.GoalWeakening) {
				
   //             var replacement = resolution.model.Replacements()
			//	                            .Where(x => x.ResolvedObstacleIdentifier == resolution.ObstacleIdentifier
			//	                                   & x.ResolvingGoalIdentifier == resolution.ResolvingGoalIdentifier
			//	                                   & x.AnchorGoalIdentifier == anchor.Identifier).ToList ();

			//	foreach (var r in replacement) {
   //                 throw new NotImplementedException();
			//		// resolution.model.Remove(r);
			//	}
			//	//logger.Info("Remove replacement " + resolution.ResolvingGoal().FriendlyName);

			//} else {

   //             var exception = resolution.model.Exceptions()
			//								.Where(x => x.ResolvedObstacleIdentifier == resolution.ObstacleIdentifier
			//									   & x.ResolvingGoalIdentifier == resolution.ResolvingGoalIdentifier
			//									   & x.AnchorGoalIdentifier == anchor.Identifier).ToList();

			//	foreach (var r in exception)
			//	{
			//		throw new NotImplementedException();
			//		// resolution.model.Remove(r);
			//	}


			//}
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

                //Console.WriteLine ("<pre> -- " + children.FriendlyName + " -- " + assumption.Obstacle().FriendlyName);

                //Console.WriteLine (string.Join ("\n", children.Obstacles ().Select (x => x.Identifier)));

                //Console.WriteLine ("</pre><hr />");

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

