using System;
using System.Collections.Generic;
namespace AsyncFSM2SyncFSM
{
    public class State
    {
        public Dictionary<string, string> Values {
            get;
            set;
        }

        public string Id {
            get {
                return id;
            }
        }

        string id;
        public bool init;

        public State (string id, bool init)
        {
            this.id = id;
            this.init = init;
            Values = new Dictionary<string, string> ();
        }

        public State (string id, bool init, Dictionary<string, string> values)
        {
            this.id = id;
            this.init = init;
            Values = new Dictionary<string, string> (values);
        }

        public void SetValue (string k, string v)
        {
            if (Values.ContainsKey (k)) {
                Values [k] = v;
            } else {
                Values.Add (k, v);
            }
        }

        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof (Transition))
                return false;
            State other = (State)obj;
            return id == other.id;
        }

        public override int GetHashCode ()
        {
            unchecked {
                return id.GetHashCode ();
            }
        }
    }

    public class Transition
    {
        public string source { get; set; }

        public string target { get; set; }

        public string label {
            get;
            set;
        }

        public Transition (string label, string source, string target)
        {
            this.label = label;
            this.source = source;
            this.target = target;
        }

        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof (Transition))
                return false;
            Transition other = (Transition)obj;
            return source == other.source & target == other.target & label == other.label;
        }

        public override int GetHashCode ()
        {
            unchecked {
                return 17 * label.GetHashCode () + 23 * (source.GetHashCode () + 23 * target.GetHashCode ());
            }
        }
    }


    public class ATransition
    {
        public string source;

        public string target;

        public HashSet<string> label {
            get;
            private set;
        }

        public ATransition (IEnumerable<string> label, string source, string target)
        {
            this.label = new HashSet<string> (label);
            this.source = source;
            this.target = target;
        }

        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof (ATransition))
                return false;
            ATransition other = (ATransition)obj;
            return source == other.source & target == other.target & label.IsSubsetOf (other.label) & other.label.IsSubsetOf (label);
        }

        public override int GetHashCode ()
        {
            unchecked {
                // See http://stackoverflow.com/questions/670063/getting-hash-of-a-list-of-strings-regardless-of-order
                // for more details
                var hash = 0;
                int curHash = 0;
                int bitOffset = 0;
                // Stores number of occurences so far of each value.
                var valueCounts2 = new Dictionary<string, int> ();

                foreach (var element in label) {
                    curHash = element.GetHashCode ();
                    if (valueCounts2.TryGetValue (element, out bitOffset))
                        valueCounts2 [element] = bitOffset + 1;
                    else
                        valueCounts2.Add (element, bitOffset);
                    hash = hash + ((curHash << bitOffset) |
                        (curHash >> (32 - bitOffset))) * 37;
                }

                return 17 + hash + 23 * (source.GetHashCode () + 23 * target.GetHashCode ());
            }
        }
    }

    public class FSM
    {
        public Dictionary<string, State> Ids { get; private set; }
        public Dictionary<string, HashSet<Transition>> Transitions { get; private set; }

        public FSM ()
        {
            Ids = new Dictionary<string, State> ();
            Transitions = new Dictionary<string, HashSet<Transition>> ();
        }

        public void AddState (State s)
        {
            Ids.Add (s.Id, s);
        }

        public void AddTransition (Transition t)
        {
            if (Transitions.ContainsKey (t.source)) {
                Transitions [t.source].Add (t);
            } else {
                Transitions.Add (t.source, new HashSet<Transition> (new [] { t }));
            }
        }
    }

    public class AFSM
    {
        public Dictionary<string, State> Ids { get; private set; }
        public HashSet<ATransition> Transitions { get; private set; }

        public AFSM ()
        {
            Ids = new Dictionary<string, State> ();
            Transitions = new HashSet<ATransition> ();
        }

        public void AddState (State s)
        {
            if (Ids.ContainsKey (s.Id)) {
            } else {
                Ids.Add (s.Id, s);
            }
        }

        public void AddTransition (ATransition t)
        {
            Transitions.Add (t);
        }
    }
}

