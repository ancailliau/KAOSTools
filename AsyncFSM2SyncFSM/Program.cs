using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Collections.Generic;

namespace AsyncFSM2SyncFSM
{
    class MainClass
    {
        enum Mode
        {
            State, Transition, Other
        }

        public static void Main (string [] args)
        {
            Console.WriteLine ("Hello World!");

            var fsm = new FSM ();
            var mode = Mode.Other;

            State currentState = null;
            Transition currentTransition = null;

            var xmlString = File.ReadAllText (args [0]);

            using (XmlReader reader = XmlReader.Create (new StringReader (xmlString))) {
                while (reader.Read ()) {
                    switch (reader.NodeType) {
                    case XmlNodeType.Element:
                        if (reader.Name == "state") {
                            mode = Mode.State;
                            currentState = new State (reader.GetAttribute ("id"), reader.GetAttribute ("initial") == "1");

                        } else if (reader.Name == "transition") {
                            mode = Mode.Transition;
                            currentTransition = new Transition ("", reader.GetAttribute ("source"), reader.GetAttribute ("target"));

                        } else if (reader.Name == "value") {
                            if (mode == Mode.State) {
                                currentState.SetValue (reader.GetAttribute ("variable"), reader.ReadElementContentAsString ());
                            } else if (mode == Mode.Transition) {
                                if (reader.GetAttribute ("variable") == "Label") {
                                    currentTransition.label = reader.ReadElementContentAsString ();
                                } else {
                                    throw new NotImplementedException ();
                                }
                            }
                        }
                        break;
                    case XmlNodeType.EndElement:
                        if (mode == Mode.State) {
                            fsm.AddState (currentState);
                            currentState = null;
                        } else if (mode == Mode.Transition) {
                            fsm.AddTransition (currentTransition);
                            currentTransition = null;
                        }
                        mode = Mode.Other;
                        break;
                    }
                }
            }

            Console.WriteLine ("Number of states: {0}", fsm.Ids.Keys.Count);
            Console.WriteLine ("Number of transitions: {0}", fsm.Transitions.Sum (x => x.Value.Count));

            var fsm2 = new AFSM ();
            var initialNode = fsm.Ids.Values.Single (x => x.init);
            BuildAsync (initialNode, fsm, fsm2);

            Console.WriteLine ("Number of states: {0}", fsm2.Ids.Keys.Count);
            Console.WriteLine ("Number of transitions: {0}", fsm2.Transitions.Count ());

            Console.WriteLine ("---");

            using (StreamWriter file = new StreamWriter (args [1])) {
                file.WriteLine ("digraph G {");
                foreach (var n in fsm2.Ids.Values) {
                    file.WriteLine ("\t{0}[label=\"{1}\"]", n.Id, "["+n.Id+"]" + "\n" + string.Join ("\\n", n.Values.Select (x => x.Key + " = " + x.Value)));
                }
                foreach (var n in fsm2.Transitions) {
                    file.WriteLine ("\t{0} -> {1} [label=\"{{{2}}}\"]", n.source, n.target, string.Join (",", n.label));
                }
                file.WriteLine ("}");
            }


            Console.WriteLine ("----");
            var array = new Dictionary<State, int> ();
            int i = 0;
            foreach (var s in fsm2.Ids.Values) {
                Console.WriteLine ("// {0} {1}", s.Id, i);
                array.Add (s, i++);
            }

            var highWater = new HashSet<State> ();
            var noMethane = new HashSet<State> ();
            var pumpOn = new HashSet<State> ();
            foreach (var s in fsm2.Ids.Values) {
                if (bool.Parse(s.Values ["HighWater"])) {
                    highWater.Add (s);
                }
                //if (!s.Values ["Methane"]) {
                //    noMethane.Add (s);
                //}
                if (bool.Parse (s.Values ["PumpOn"])) {
                    pumpOn.Add (s);
                }
            }

            Console.WriteLine ("StateFactory factory = new State.StateFactory ();");
            Console.WriteLine ("State [] states = factory.newStates ({0}).toArray (new State [{0}]);", fsm2.Ids.Keys.Count);

            Console.WriteLine ("DMRM.Builder builder = new DMRM.Builder ();");
            foreach (var initial in highWater) {
                Console.WriteLine ("//-------------------------------------------------------");
                Console.WriteLine ("builder.setInitialState (states [{0}]);", array [initial]);
                var succs = new HashSet<string> (fsm2.Transitions.Where (x => x.source == initial.Id).Select (x => x.target));

                foreach (var s in fsm2.Ids.Values) {
                    Console.WriteLine ("\n// Transitions from {0}", s.Id);

                    var transitions = fsm2.Transitions.Where (x => x.source == s.Id);
                    foreach (var t in transitions) {
                        //if (succs.Contains (t.source)) {
                        //    // transition is out from a successor of the initial node.
                        //    Console.WriteLine ("builder.setTransition (states [{0}], states [{0}], new RationalParameter (1,1));",
                        //                           array [fsm2.Ids [t.source]], array [fsm2.Ids [t.target]]);
                        //    break;

                        //} else {
                            var v = string.Join ("*", t.label);
                            if (string.IsNullOrEmpty (v)) {
                                if (transitions.Count () == 1) {
                                    Console.WriteLine ("builder.setTransition (states [{0}], states [{1}], new RationalParameter (1,1));",
                                                   array [fsm2.Ids [t.source]], array [fsm2.Ids [t.target]]);
                                } else {
                                    v = string.Join ("+", transitions.Where (x => x.label.Count > 0).Select (x => string.Join ("*", x.label)));
                                    Console.WriteLine ("builder.setTransition (states [{0}], states [{1}], new SymbolicParameter (\"1-({2})\"));",
                                                       array [fsm2.Ids [t.source]], array [fsm2.Ids [t.target]], v);
                                }
                            } else {
                                Console.WriteLine ("builder.setTransition (states [{0}], states [{1}], new SymbolicParameter (\"{2}\"));",
                                                   array [fsm2.Ids [t.source]], array [fsm2.Ids [t.target]], v);
                            }
                        //}
                    }
                }

                Console.WriteLine ("//-------------------------------------------------------");


            }

            //Console.WriteLine ("// HighWater: " + string.Join (", ", highWater.Select (w => w.Id)));
            //Console.WriteLine ("// NoMethane: " + string.Join (", ", noMethane.Select (w => w.Id)));
            //Console.WriteLine ("// PumpOn: " + string.Join (", ", pumpOn.Select (w => w.Id)));
            //Console.WriteLine ("builder.setInitialState (states [{0}]);", array [initialNode]);


            //builder.setTransition (states [0], states [1], new SymbolicParameter ("(1-y)*0.3"));


        }

        static void BuildAsync (State initial, FSM fsm, AFSM fsm2)
        {
            Stack<State> s = new Stack<State> (new [] { initial } );
            HashSet<string> visited = new HashSet<string> ();

            int i = 0;
            int total = fsm.Ids.Count;
            while (s.Count > 0) {
                i++;
                var current = s.Pop ();
                Console.WriteLine ("----- {0}/{1}", i, total);
                visited.Add (current.Id);
                BuildRecursive (current, fsm, fsm2, s, visited);
            }
        }

        class MyTuple
        {
            public State Source;
            public State Target;
            public HashSet<string> Labels;

            public MyTuple (State source, State target, HashSet<string> labels)
            {
                Source = source;
                Target = target;
                Labels = labels;
            }

            public override bool Equals (object obj)
            {
                if (obj == null)
                    return false;
                if (ReferenceEquals (this, obj))
                    return true;
                if (obj.GetType () != typeof (MyTuple))
                    return false;
                MyTuple other = (MyTuple)obj;
                return Source == other.Source & Target == other.Target & Labels.IsSubsetOf (other.Labels) & other.Labels.IsSubsetOf (Labels);
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

                    foreach (var element in Labels) {
                        curHash = element.GetHashCode ();
                        if (valueCounts2.TryGetValue (element, out bitOffset))
                            valueCounts2 [element] = bitOffset + 1;
                        else
                            valueCounts2.Add (element, bitOffset);
                        hash = hash + ((curHash << bitOffset) |
                            (curHash >> (32 - bitOffset))) * 37;
                    }

                    return 17 + hash + 23 * (Source.GetHashCode () + 23 * Target.GetHashCode ());
                }
            }
        }

        static void BuildRecursive (State initial, 
                                    FSM fsm, AFSM fsm2, 
                                    Stack<State> nodesToVisit, 
                                    HashSet<string> visited)
        {
            // source, target, set of labels
            var t = new Stack<MyTuple> ();
            var t2 = new HashSet<MyTuple> ();

            foreach (var succ in fsm.Transitions [initial.Id]) {
                if (succ.label == "tick") {
                    fsm2.AddState (initial);
                    fsm2.AddTransition (new ATransition (new string [] { }, initial.Id, succ.target));

                    if (!visited.Contains (succ.target)) {
                        if (!nodesToVisit.Contains (fsm.Ids [succ.target])) {
                            nodesToVisit.Push (fsm.Ids [succ.target]);
                        }
                    }
                    

                } else {
                    var hashSet = new HashSet<string> (new [] { succ.label });
                    var nt = new MyTuple (initial, fsm.Ids [succ.target], hashSet);
                    if (!t2.Contains (nt)) {
                        t.Push (nt);
                        t2.Add (nt);
                    }
                }
            }

            int i = 0;
            while (t.Count > 0) {
                if (i % 1000 == 0) {
                    i = 0;
                    Console.WriteLine (t.Count);
                }
                i++;

                var currentT = t.Pop ();
                var source = currentT.Source;
                var current = currentT.Target;
                var labels = currentT.Labels;

                Console.WriteLine ("<{0}, {1}, {2}>",
                                   string.Format ("{0}", source.Id),
                                   string.Format ("{0}", current.Id),
                                   string.Format ("{{{0}}}", string.Join (",", labels)));

                foreach (var succ in fsm.Transitions [current.Id]) {
                    Console.WriteLine ("to {0} : {1}", current.Id, succ.label);
                    if (succ.label == "tick") {
                        fsm2.AddState (source);
                        fsm2.AddTransition (new ATransition (labels, source.Id, succ.target));

                        if (!visited.Contains (succ.target)) {
                            if (!nodesToVisit.Contains (fsm.Ids [succ.target])) {
                                nodesToVisit.Push (fsm.Ids [succ.target]);
                            }
                        }

                    } else {
                        var hashSet = new HashSet<string> (labels);
                        hashSet.Add (succ.label);

                        var nt = new MyTuple (source, fsm.Ids [succ.target], hashSet);
                        if (!t2.Contains (nt)) {
                            t.Push (nt);
                            t2.Add (nt);
                        }
                        //BuildRecursive (fsm.Ids [succ.target], source, hashSet, fsm, fsm2, s, visited);
                    }
                }
            }
        }
    }
}
