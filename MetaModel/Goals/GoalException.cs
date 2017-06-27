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

    [DataContract]
    public class GoalException : KAOSCoreElement {
        public GoalException (KAOSModel model) : base (model)
        {
        }
        
        [DataMember]
        public string AnchorGoalIdentifier { get; set; }
        
        [DataMember]
        public string ResolvedObstacleIdentifier { get; set; }
        
        [DataMember]
        public string ResolvingGoalIdentifier { get; set; }

        public void SetAnchorGoal (Goal goal)
        {
            AnchorGoalIdentifier = goal.Identifier;
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
            return new GoalException (model) {
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
