using System;
using System.Linq;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra.Double;

namespace KAOSTools.MetaModel
{

    public class Transition {

        public string Label       { get; set; }
        public double Probability { get; set; }

        public State  From        { get; set; }
        public State  To          { get; set; }

        public Transition (State from, State to)
            : this ("", 1, from, to)
        {}

        public Transition (double probability, State from, State to)
            : this ("", probability, from, to)
        {}

        public Transition (string label, State from, State to)
            : this (label, 1, from, to)
        {}

        public Transition (string label, double probability, State from, State to)
        {
            this.Label = label;
            this.Probability = probability;
            this.To = to;
            this.From = from;
        }

        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(Transition))
                return false;
            Transition other = (Transition)obj;
            return To.Equals (other.To) 
                && From.Equals (other.From) 
                && Probability == other.Probability 
                && Label == other.Label;
        }

        public override int GetHashCode ()
        {
            unchecked {
                return (To != null ? To.GetHashCode () : 0) 
                    ^ (From != null ? From.GetHashCode () : 0) 
                    ^ Probability.GetHashCode () 
                    ^ (Label != null ? Label.GetHashCode () : 0);
            }
        }
    }

    public class State {
        public string            Identifier        { get; set; }
        public bool              Initial           { get; set; }
        public ISet<Predicate> ValidPropositions { get; set; }

        public State ()
            : this ("")
        {}

        public State (string identifier)
            : this (identifier, false)
        {}

        public State (string identifier, bool initial)
        {
            Identifier        = identifier;
            Initial           = initial;
            ValidPropositions = new HashSet<Predicate> ();
        }

        public State (State copy)
        {
            Identifier        = copy.Identifier;
            Initial           = copy.Initial;
            ValidPropositions = copy.ValidPropositions;
        }

        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(State))
                return false;
            State other = (State)obj;
            return Identifier == other.Identifier;
        }

        public override int GetHashCode ()
        {
            unchecked {
                return (Identifier != null ? Identifier.GetHashCode () : 0);
            }
        }
    }

    public class MarkovChain
    {
        private ISet<State> _states;
        public  ISet<State> States { 
            get {
                return _states;
            }
        }

        private ISet<Transition> _transitions;
        public  ISet<Transition> Transitions { 
            get {
                return _transitions;
            }
        }

        public IEnumerable<State> InitialStates {
            get {
                return States.Where (x => x.Initial == true);
            }
        }

        public MarkovChain ()
        {
            _states      = new HashSet<State> ();
            _transitions = new HashSet<Transition> ();
        }

        public State Add (State state)
        {
            _states.Add (state);
            return state;
        }

        public Transition Add (Transition transition)
        {
            _transitions.Add (transition);

            if (!_states.Contains (transition.From))
                _states.Add (transition.From);
            
            if (!_states.Contains (transition.To))
                _states.Add (transition.To);

            return transition;
        }
    }

    public class BehaviorModel
    {
        public IList<MarkovChain> StateMachines { get; set; }

        public BehaviorModel ()
        {
            StateMachines = new List<MarkovChain> ();
        }
    }
}

