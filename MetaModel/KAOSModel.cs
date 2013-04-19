using System;
using System.Collections.Generic;

namespace KAOSTools.MetaModel
{
    public class KAOSModel
    {
        public GoalModel GoalModel {
            get;
            set;
        }

        public Dictionary<string, Predicate> Predicates { get; set; }

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
            this.Predicates = new Dictionary<string, Predicate> ();
        }
    }
}

