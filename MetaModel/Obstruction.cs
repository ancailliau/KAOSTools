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

    [DataContract]
    public class Obstruction : KAOSCoreElement {
        
        [DataMember]
        public string ObstructedGoalIdentifier { get; set; }
        
        [DataMember]
        public string ObstacleIdentifier { get; set; }

        public Obstruction (KAOSModel model) : base (model) {}

        public void SetObstructedGoal (Goal goal)
        {
            this.ObstructedGoalIdentifier = goal.Identifier;
        }

        public void SetObstacle (Obstacle obstacle)
        {
            this.ObstacleIdentifier = obstacle.Identifier;
        }

        public override KAOSCoreElement Copy ()
        {
            return new Obstruction (null) {
                Identifier = Identifier,
                Implicit = Implicit,
                ObstructedGoalIdentifier = ObstructedGoalIdentifier,
                ObstacleIdentifier = ObstacleIdentifier
            };
        }
    }

    #endregion

    #region Exceptions and assumptions

    #endregion

    #endregion

    #region Object Model

    #endregion
    
}
