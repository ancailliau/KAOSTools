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

    [DataContract]
    public class GoalRefinement : KAOSCoreElement
    {
        
        [DataMember]
        public string ParentGoalIdentifier { get; set; }

        public string SystemReferenceIdentifier { get; set; }

        public bool IsComplete {
            get;
            set;
        }

        public ISet<string> PositiveSoftGoalsIdentifiers { get; set; }
        public ISet<string> NegativeSoftGoalsIdentifiers { get; set; }

        [DataMember]
        public IList<string> SubGoalIdentifiers { get; set; }
        
        [DataMember]
        public ISet<string> DomainPropertyIdentifiers { get; set; }
        
        [DataMember]
        public ISet<string> DomainHypothesisIdentifiers { get; set; }

        public bool IsEmpty {
            get {
                return SubGoalIdentifiers.Count 
                    + DomainPropertyIdentifiers.Count 
                    + DomainHypothesisIdentifiers.Count == 0;
            }
        }

        public RefinementPattern RefinementPattern { get; set; }
        public List<dynamic> Parameters { get; set; }

        public GoalRefinement (KAOSModel model) : base (model)
        {
            SubGoalIdentifiers = new List<string> ();
            DomainPropertyIdentifiers = new HashSet<string> ();
            DomainHypothesisIdentifiers = new HashSet<string> ();

            PositiveSoftGoalsIdentifiers = new HashSet<string> ();
            NegativeSoftGoalsIdentifiers = new HashSet<string> ();

            IsComplete = false;

            Parameters = new List<dynamic> ();
        }

        public GoalRefinement (KAOSModel model, Goal goal) : this (model)
        {
            SubGoalIdentifiers.Add (goal.Identifier);
        }

        public GoalRefinement (KAOSModel model, params Goal[] goals) : this (model)
        {
            foreach (var goal in goals)
                SubGoalIdentifiers.Add (goal.Identifier);
        }

        public void SetParentGoal (Goal element)
        {
            this.ParentGoalIdentifier = element.Identifier;
        }

        public void SetSystemReference (AlternativeSystem system)
        {
            this.SystemReferenceIdentifier = system.Identifier;
        }

        public void Add (Goal goal)
        {
            this.SubGoalIdentifiers.Add (goal.Identifier);
        }
        
        public void Add (DomainProperty domProp)
        {
            this.DomainPropertyIdentifiers.Add (domProp.Identifier);
        }
        
        public void Add (DomainHypothesis domHyp)
        {
            this.DomainHypothesisIdentifiers.Add (domHyp.Identifier);
        }

        public void Remove (Goal goal)
        {
            this.SubGoalIdentifiers.Remove (goal.Identifier);
        }

        public void Remove (DomainProperty domProp)
        {
            this.DomainPropertyIdentifiers.Remove (domProp.Identifier);
        }

        public void Remove (DomainHypothesis domHyp)
        {
            this.DomainHypothesisIdentifiers.Remove (domHyp.Identifier);
        }

        public void AddNegativeSoftGoal (SoftGoal goal)
        {
            this.NegativeSoftGoalsIdentifiers.Add (goal.Identifier);
        }

        public void AddPositiveSoftGoal (SoftGoal goal)
        {
            this.PositiveSoftGoalsIdentifiers.Add (goal.Identifier);
        }

        public override KAOSCoreElement Copy ()
        {
            return new GoalRefinement (null) {
                Identifier = Identifier,
                Implicit = Implicit,
                ParentGoalIdentifier = ParentGoalIdentifier,
                SystemReferenceIdentifier = SystemReferenceIdentifier,
                SubGoalIdentifiers = new List<string> (SubGoalIdentifiers),
                DomainPropertyIdentifiers = new HashSet<string> (DomainPropertyIdentifiers),
                DomainHypothesisIdentifiers = new HashSet<string> (DomainHypothesisIdentifiers),
                RefinementPattern = RefinementPattern,
                Parameters = new List<dynamic> (Parameters)
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
