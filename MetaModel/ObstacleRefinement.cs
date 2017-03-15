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

    public class ObstacleRefinement : KAOSCoreElement
    {
        public string ParentObstacleIdentifier { get; set; }

        public ISet<string> SubobstacleIdentifiers { get; set; }
        public ISet<string> DomainPropertyIdentifiers { get; set; }
        public ISet<string> DomainHypothesisIdentifiers { get; set; }

        public bool IsEmpty {
            get {
                return SubobstacleIdentifiers.Count + DomainPropertyIdentifiers.Count + DomainHypothesisIdentifiers.Count == 0;
            }
        }
        public ObstacleRefinement (KAOSModel model) : base (model)
        {
            SubobstacleIdentifiers = new HashSet<string> ();
            DomainPropertyIdentifiers = new HashSet<string> ();
            DomainHypothesisIdentifiers = new HashSet<string> ();
        }

        public ObstacleRefinement (KAOSModel model, Obstacle obstacle) : this (model)
        {
            SubobstacleIdentifiers.Add (obstacle.Identifier);
        }

        public ObstacleRefinement (KAOSModel model, params Obstacle[] obstacles) : this (model)
        {
            foreach (var obstacle in obstacles)
                SubobstacleIdentifiers.Add (obstacle.Identifier);
        }

        public void SetParentObstacle (Obstacle element)
        {
            this.ParentObstacleIdentifier = element.Identifier;
        }

        public void Add (Obstacle obstacle)
        {
            this.SubobstacleIdentifiers.Add (obstacle.Identifier);
        }

        public void Add (DomainProperty domProp)
        {
            this.DomainPropertyIdentifiers.Add (domProp.Identifier);
        }

        public void Add (DomainHypothesis domHyp)
        {
            this.DomainHypothesisIdentifiers.Add (domHyp.Identifier);
        }

        public override KAOSCoreElement Copy ()
        {
            return new ObstacleRefinement (null) {
                Identifier = Identifier,
                Implicit = Implicit,
                ParentObstacleIdentifier = ParentObstacleIdentifier,
                SubobstacleIdentifiers = new HashSet<string> (SubobstacleIdentifiers),
                DomainPropertyIdentifiers = new HashSet<string> (DomainPropertyIdentifiers),
                DomainHypothesisIdentifiers = new HashSet<string> (DomainHypothesisIdentifiers)
            };
        }
    }

    #endregion

    #region Obstructions and resolutions

    #endregion

    #region Exceptions and assumptions

    #endregion

    #endregion

    #region Object Model

    #endregion
    
}
