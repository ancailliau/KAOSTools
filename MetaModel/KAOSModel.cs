using System;
using System.Collections.Generic;
using System.Linq;

namespace KAOSTools.MetaModel
{
    public class KAOSModel
    {
        public GoalModel GoalModel {
            get;
            set;
        }

        public ISet<Predicate> Predicates { get; set; }

        public BehaviorModel BehaviorModel {
            get;
            set;
        }

        public ISet<Entity> Entities { get; set; }
        public ISet<GivenType> GivenTypes { get; set; }
        public IEnumerable<Relation> Relations { get {
                return Entities.Where (x => x.GetType () == typeof(Relation)).Cast<Relation> ();
            }
        }

        public KAOSModel ()
            : this (new GoalModel (), new BehaviorModel ())
        {}

        public KAOSModel (GoalModel goalModel, BehaviorModel behaviorModel)
        {
            this.GoalModel = goalModel;
            this.BehaviorModel = behaviorModel;
            this.Predicates = new HashSet<Predicate> ();
            this.Entities = new HashSet<Entity> ();
            this.GivenTypes = new HashSet<GivenType> ();
        }
    }
}

