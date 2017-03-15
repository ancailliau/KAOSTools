using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace KAOSTools.Core
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

    public class ObstacleAssumption : KAOSCoreElement {
        public ObstacleAssumption (KAOSModel model) : base (model)
        {
        }
        public string AnchorGoalIdentifier { get; set; }
        public string ResolvedObstacleIdentifier { get; set; }

        public void SetAnchorGoal (Goal goal)
        {
            AnchorGoalIdentifier = goal.Identifier;
        }

        public void SetObstacle (Obstacle obstacle)
        {
            ResolvedObstacleIdentifier = obstacle.Identifier;
        }

        public override KAOSCoreElement Copy ()
        {
            return new ObstacleAssumption (model) {
                AnchorGoalIdentifier = AnchorGoalIdentifier,
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
