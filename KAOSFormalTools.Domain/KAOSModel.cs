using System;

namespace KAOSFormalTools.Domain
{
    public class KAOSModel
    {
        public GoalModel GoalModel {
            get;
            set;
        }

        public BehaviorModel BehaviorModel {
            get;
            set;
        }

        public KAOSModel ()
            : this (new GoalModel (), new BehaviorModel ())
        {}

        public KAOSModel (GoalModel goalModel, BehaviorModel behaviorModel)
        {
            this.GoalModel = goalModel;
            this.BehaviorModel = behaviorModel;
        }
    }
}

