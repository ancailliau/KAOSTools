using System;
using System.Collections.Generic;
using System.Linq;

namespace KAOSTools.MetaModel
{
    public class KAOSModel
    {
        private Dictionary<string, KAOSMetaModelElement> _elements;

        public IEnumerable<KAOSMetaModelElement> Elements {
            get {
                return _elements.Values;
            }
        }

        #region Goal model

        public IEnumerable<Goal> Goals {
            get {
                return Elements.Where (x => x is Goal).Cast<Goal>();
            }
        }

        public IEnumerable<AntiGoal> AntiGoals {
            get {
                return Elements.Where (x => x is AntiGoal).Cast<AntiGoal>();
            }
        }

        public IEnumerable<Obstacle> Obstacles {
            get {
                return Elements.Where (x => x is Obstacle).Cast<Obstacle>();
            }
        }

        public IEnumerable<DomainProperty> DomainProperties {
            get {
                return Elements.Where (x => x is DomainProperty).Cast<DomainProperty>();
            }
        }

        public IEnumerable<DomainHypothesis> DomainHypotheses {
            get {
                return Elements.Where (x => x is DomainHypothesis).Cast<DomainHypothesis>();;
            }
        }

        public IEnumerable<AlternativeSystem> AlternativeSystems {
            get {
                return Elements.Where (x => x is AlternativeSystem).Cast<AlternativeSystem>();;
            }
        }

        // Relations
        
        public IEnumerable<GoalRefinement> GoalRefinements {
            get {
                return Elements.Where (x => x is GoalRefinement).Cast<GoalRefinement>();
            }
        }
        
        public IEnumerable<AntiGoalRefinement> AntiGoalRefinements {
            get {
                return Elements.Where (x => x is AntiGoalRefinement).Cast<AntiGoalRefinement>();
            }
        }

        public IEnumerable<ObstacleRefinement> ObstacleRefinements {
            get {
                return Elements.Where (x => x is ObstacleRefinement).Cast<ObstacleRefinement>();
            }
        }
        
        public IEnumerable<Obstruction> Obstructions {
            get {
                return Elements.Where (x => x is Obstruction).Cast<Obstruction>();
            }
        }

        public IEnumerable<Resolution> Resolutions {
            get {
                return Elements.Where (x => x is Resolution).Cast<Resolution>();
            }
        }

        public IEnumerable<GoalAgentAssignment> GoalAgentAssignments {
            get {
                return Elements.Where (x => x is GoalAgentAssignment).Cast<GoalAgentAssignment>();
            }
        }
        
        public IEnumerable<AntiGoalAgentAssignment> AntiGoalAgentAssignments {
            get {
                return Elements.Where (x => x is AntiGoalAgentAssignment).Cast<AntiGoalAgentAssignment>();
            }
        }

        public IEnumerable<ObstacleAgentAssignment> ObstacleAgentAssignments {
            get {
                return Elements.Where (x => x is ObstacleAgentAssignment).Cast<ObstacleAgentAssignment>();
            }
        }

        #endregion

        #region Agent model

        public IEnumerable<Agent> Agents {
            get {
                return Elements.Where (x => x is Agent).Cast<Agent>();;
            }
        }

        #endregion

        #region Object model

        public IEnumerable<Predicate> Predicates {
            get {
                return Elements.Where (x => x is Predicate).Cast<Predicate>();;
            }
        }

        public IEnumerable<Entity> Entities {
            get {
                return Elements.Where (x => x is Entity & !(x is Relation)).Cast<Entity>();;
            }
        }

        public IEnumerable<Relation> Relations {
            get {
                return Elements.Where (x => x is Relation).Cast<Relation>();;
            }
        }

        public IEnumerable<GivenType> GivenTypes {
            get {
                return Elements.Where (x => x is GivenType).Cast<GivenType>();;
            }
        }

        #endregion

        public KAOSModel ()
        {
            this._elements = new Dictionary<string, KAOSMetaModelElement> ();
        }

        public void Add (KAOSMetaModelElement element)
        {
            if (element.model == null)
                element.model = this;

            if (element.model != this)
                throw new InvalidOperationException ("Cannot add element referencing a model in another model");

            if (this._elements.ContainsKey(element.Identifier)) {
                throw new InvalidOperationException ("Duplicated ID " + element.Identifier);
            }

            this._elements.Add (element.Identifier, element);
        }
    }
}

