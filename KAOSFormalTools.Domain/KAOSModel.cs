using System;
using System.Collections.Generic;

namespace KAOSFormalTools.Domain
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

    public class Predicate
    {
        public string Name {
            get;
            set;
        }

        public string Definition {
            get;
            set;
        }
        
        public string Signature {
            get;
            set;
        }

        public string FormalSpec {
            get;
            set;
        }
    }

}

