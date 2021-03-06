﻿using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace UCLouvain.KAOSTools.Core
{

    [DataContract]
    public class Resolution : KAOSCoreElement {
        
        [DataMember]
        public string ObstacleIdentifier { get; set; }
        
        [DataMember]
        public string ResolvingGoalIdentifier { get; set; }

        [DataMember]
        public ResolutionPattern ResolutionPattern { get; set; }
        
        public string Name {
            get;
            set;
        }
   
        public override string FriendlyName {
            get {
                return Name ?? Identifier;
            }
        }
     
        public string AnchorIdentifier { get; set; }

        public Resolution (KAOSModel model) : base (model)
        {
            ResolutionPattern = ResolutionPattern.None;
        }

        public void SetResolvingGoal (Goal goal)
        {
            this.ResolvingGoalIdentifier = goal.Identifier;
        }

        public void SetObstacle (Obstacle obstacle)
        {
            this.ObstacleIdentifier = obstacle.Identifier;
        }

        public override KAOSCoreElement Copy ()
        {
            return new Resolution (null) {
                Identifier = Identifier,
                Implicit = Implicit,
                ResolvingGoalIdentifier = ResolvingGoalIdentifier,
                ObstacleIdentifier = ObstacleIdentifier,
                ResolutionPattern = ResolutionPattern
            };
        }
    }
}
