using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace UCLouvain.KAOSTools.Core
{
    public class ObstacleRefinement : KAOSCoreElement
    {
        public string ParentObstacleIdentifier { get; set; }

        public ISet<ObstacleRefinee> SubobstacleIdentifiers { get; set; }
        public ISet<ObstacleRefinee> DomainPropertyIdentifiers { get; set; }
        public ISet<ObstacleRefinee> DomainHypothesisIdentifiers { get; set; }

        public bool IsEmpty {
            get {
                return SubobstacleIdentifiers.Count + DomainPropertyIdentifiers.Count + DomainHypothesisIdentifiers.Count == 0;
            }
        }
        public ObstacleRefinement (KAOSModel model) : base (model)
        {
            SubobstacleIdentifiers = new HashSet<ObstacleRefinee> ();
            DomainPropertyIdentifiers = new HashSet<ObstacleRefinee> ();
            DomainHypothesisIdentifiers = new HashSet<ObstacleRefinee> ();
        }

        public ObstacleRefinement (KAOSModel model, Obstacle obstacle) : this (model)
        {
            SubobstacleIdentifiers.Add (new ObstacleRefinee (obstacle.Identifier));
        }

        public ObstacleRefinement (KAOSModel model, params Obstacle[] obstacles) : this (model)
        {
            foreach (var obstacle in obstacles)
                SubobstacleIdentifiers.Add (new ObstacleRefinee (obstacle.Identifier));
        }

        public void SetParentObstacle (Obstacle element)
        {
            this.ParentObstacleIdentifier = element.Identifier;
        }

        public void Add (Obstacle obstacle)
        {
            this.SubobstacleIdentifiers.Add (new ObstacleRefinee (obstacle.Identifier));
        }

        public void Add (Obstacle obstacle, IRefineeParameter parameter)
        {
            this.SubobstacleIdentifiers.Add (new ObstacleRefinee (obstacle.Identifier, parameter));
        }

        public void Add (DomainProperty domProp)
        {
            this.DomainPropertyIdentifiers.Add (new ObstacleRefinee (domProp.Identifier));
        }

        public void Add (DomainProperty domProp, IRefineeParameter parameter)
        {
            this.DomainPropertyIdentifiers.Add (new ObstacleRefinee (domProp.Identifier, parameter));
        }

        public void Add (DomainHypothesis domHyp)
        {
            this.DomainHypothesisIdentifiers.Add (new ObstacleRefinee (domHyp.Identifier));
        }

        public void Add (DomainHypothesis domHyp, IRefineeParameter parameter)
        {
            this.DomainHypothesisIdentifiers.Add (new ObstacleRefinee (domHyp.Identifier, parameter));
        }

        public override KAOSCoreElement Copy ()
        {
            return new ObstacleRefinement (null) {
                Identifier = Identifier,
                Implicit = Implicit,
                ParentObstacleIdentifier = ParentObstacleIdentifier,
                SubobstacleIdentifiers = new HashSet<ObstacleRefinee> (SubobstacleIdentifiers),
                DomainPropertyIdentifiers = new HashSet<ObstacleRefinee> (DomainPropertyIdentifiers),
                DomainHypothesisIdentifiers = new HashSet<ObstacleRefinee> (DomainHypothesisIdentifiers)
            };
        }
    }

    public class ObstacleRefinee {
        public string Identifier {
            get;
            set;
        }

        public IRefineeParameter Parameters
        {
            get;
            set;
        }

        public ObstacleRefinee (string identifier)
        {
            Identifier = identifier;
        }

        public ObstacleRefinee (string identifier, IRefineeParameter parameters)
        {
            Identifier = identifier;
            Parameters = parameters;
        }

        public override bool Equals (object obj)
        {
            if (obj != null && obj is GoalRefinee)
                return Identifier.Equals (((GoalRefinee)obj).Identifier);
            else
                return false;
        }

        public override int GetHashCode ()
        {
            return 17 + 23 * (Identifier.GetHashCode () + 23 * Parameters?.GetHashCode () ?? 0);
        }
    }
}
