using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace KAOSTools.Core
{
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
        public ISet<GoalRefinee> SubGoalIdentifiers { get; set; }
        
        [DataMember]
        public ISet<GoalRefinee> DomainPropertyIdentifiers { get; set; }
        
        [DataMember]
        public ISet<GoalRefinee> DomainHypothesisIdentifiers { get; set; }

        public bool IsEmpty {
            get {
                return SubGoalIdentifiers.Count 
                    + DomainPropertyIdentifiers.Count 
                    + DomainHypothesisIdentifiers.Count == 0;
            }
        }

        public RefinementPattern RefinementPattern { get; set; }

        public GoalRefinement (KAOSModel model) : base (model)
        {
            SubGoalIdentifiers = new HashSet<GoalRefinee> ();
            DomainPropertyIdentifiers = new HashSet<GoalRefinee> ();
            DomainHypothesisIdentifiers = new HashSet<GoalRefinee> ();

            PositiveSoftGoalsIdentifiers = new HashSet<string> ();
            NegativeSoftGoalsIdentifiers = new HashSet<string> ();

            IsComplete = false;
        }

        public GoalRefinement (KAOSModel model, Goal goal) : this (model)
        {
            SubGoalIdentifiers.Add (new GoalRefinee (goal.Identifier));
        }

        public GoalRefinement (KAOSModel model, params Goal[] goals) : this (model)
        {
            foreach (var goal in goals)
                SubGoalIdentifiers.Add (new GoalRefinee (goal.Identifier));
        }

        public void SetParentGoal (Goal element)
        {
            this.ParentGoalIdentifier = element.Identifier;
        }

        public void Add (Goal goal)
        {
            this.SubGoalIdentifiers.Add (new GoalRefinee (goal.Identifier));
        }

        public void Add (Goal goal, IRefineeParameter parameter)
        {
            this.SubGoalIdentifiers.Add (new GoalRefinee (goal.Identifier, parameter));
        }
        
        public void Add (DomainProperty domProp)
        {
            this.DomainPropertyIdentifiers.Add (new GoalRefinee (domProp.Identifier));
        }
        
        public void Add (DomainProperty domProp, IRefineeParameter parameter)
        {
            this.DomainPropertyIdentifiers.Add (new GoalRefinee (domProp.Identifier, parameter));
        }
        
        public void Add (DomainHypothesis domHyp)
        {
            this.DomainHypothesisIdentifiers.Add (new GoalRefinee (domHyp.Identifier));
        }
        
        public void Add (DomainHypothesis domHyp, IRefineeParameter parameter)
        {
            this.DomainHypothesisIdentifiers.Add (new GoalRefinee (domHyp.Identifier, parameter));
        }

        //public void Remove (Goal goal)
        //{
        //    this.SubGoalIdentifiers.Remove (goal.Identifier);
        //}

        //public void Remove (DomainProperty domProp)
        //{
        //    this.DomainPropertyIdentifiers.Remove (domProp.Identifier);
        //}

        //public void Remove (DomainHypothesis domHyp)
        //{
        //    this.DomainHypothesisIdentifiers.Remove (domHyp.Identifier);
        //}

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
                SubGoalIdentifiers = new HashSet<GoalRefinee> (SubGoalIdentifiers),
                DomainPropertyIdentifiers = new HashSet<GoalRefinee> (DomainPropertyIdentifiers),
                DomainHypothesisIdentifiers = new HashSet<GoalRefinee> (DomainHypothesisIdentifiers),
                RefinementPattern = RefinementPattern
            };
        }
    }

    public class GoalRefinee {
        public string Identifier {
            get;
            set;
        }

        public IRefineeParameter Parameters
        {
            get;
            set;
        }

        public GoalRefinee (string identifier)
        {
            Identifier = identifier;
        }

        public GoalRefinee (string identifier, IRefineeParameter parameters)
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

    public interface IRefineeParameter {}

    public class PrimitiveRefineeParameter<T> : IRefineeParameter {
        public T Value {
            get;
            set;
        }
        public PrimitiveRefineeParameter (T value)
        {
            Value = value;
        }
        public override bool Equals (object obj)
        {
            if (obj != null && obj is PrimitiveRefineeParameter<T>)
                return Value.Equals (((PrimitiveRefineeParameter<T>)obj).Value);
            else
                return false;
        }

        public override int GetHashCode ()
        {
            return Value.GetHashCode ();
        }
    }
}
