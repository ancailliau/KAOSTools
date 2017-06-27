using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace UCLouvain.KAOSTools.Core
{

    #region Goal Model

    #region Meta entities

    #endregion

    #region Assignements

    #endregion

    #region Refinements

    #endregion

    #region Obstructions and resolutions

    #endregion

    #region Exceptions and assumptions

    public class GoalReplacement : KAOSCoreElement {
        public GoalReplacement (KAOSModel model) : base (model)
        {
        }
        public string AnchorGoalIdentifier { get; set; }
        public string ResolvedObstacleIdentifier { get; set; }
        public string ResolvingGoalIdentifier { get; set; }

        public IEnumerable<string> ReplacedGoals;

        public void SetAnchorGoal (Goal goal)
        {
            AnchorGoalIdentifier = goal.Identifier;
        }

		public void SetAnchorGoal(string goal)
		{
			AnchorGoalIdentifier = goal;
		}

        public void SetResolvingGoal (Goal goal)
        {
            ResolvingGoalIdentifier = goal.Identifier;
        }

        public void SetObstacle (Obstacle obstacle)
        {
            ResolvedObstacleIdentifier = obstacle.Identifier;
        }

        public override KAOSCoreElement Copy ()
        {
            return new GoalReplacement (model) {
                AnchorGoalIdentifier = AnchorGoalIdentifier,
                ResolvingGoalIdentifier = ResolvingGoalIdentifier,
                ResolvedObstacleIdentifier = ResolvedObstacleIdentifier,
                Implicit = Implicit,
                Identifier = Identifier
            };
        }

    }

    #endregion

    #endregion

    #region Object Model

    #endregion
    
}
