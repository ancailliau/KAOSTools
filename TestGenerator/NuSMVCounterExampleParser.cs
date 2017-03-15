using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace TestGenerator
{
    public class CounterExample
    {
        public List<CXItem> Items {
            get;
            set;
        }
        public CounterExample ()
        {
            Items = new List<CXItem> ();
        }
    }

    public class CXItem {
        public ISet<CXVariable> Variables {
            get;
            set;
        }
        public CXItem ()
        {
            Variables = new HashSet<CXVariable> ();
        }
    }

    public class CXState : CXItem
    {
    }

    public class CXInput : CXItem
    {
    }

    public class CXVariable
    {
        public string Name {
            get;
            set;
        }
        public string Value {
            get;
            set;
        }
        public CXVariable (string name)
        {
            this.Name = name;
        }
        
        public CXVariable (string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }
        
    }

    public class NuSMVCounterExampleParser
    {
        public NuSMVCounterExampleParser ()
        {
            
        }

        private enum State
        {
            None, CounterExample, Node, State, Value, Input
        }

        public static CounterExample Parse (string fname)
        {
            var input = File.ReadAllText (fname);

            CounterExample cx = null;

            CXVariable currentVar = null;
            CXItem currentItem = null;

            State currentState = State.None;

            // Create an XmlReader
            using (XmlReader reader = XmlReader.Create (new StringReader (input))) {

                // Parse the file and display each of the nodes.
                while (reader.Read ()) {
                    switch (reader.NodeType) {
                    case XmlNodeType.Element:
                        //                      Console.WriteLine (reader.Name);
                        if (reader.Name == "counter-example") {
                            cx = new CounterExample ();
                            currentState = State.CounterExample;

                        } else if (reader.Name == "node") {
                            currentState = State.Node;

                        } else if (reader.Name == "state") {
                            currentState = State.State;
                            currentItem = new CXState ();

                        } else if (reader.Name == "value") {
                            if (currentState == State.State) {
                                currentVar = new CXVariable (reader.GetAttribute ("variable"));
                            } else if (currentState == State.Input) {
                                currentVar = new CXVariable (reader.GetAttribute ("variable"));
                            }

                        } else if (reader.Name == "input") {
                            currentState = State.Input;
                            currentItem = new CXInput ();

                        }
                        break;

                    case XmlNodeType.Text:
                        if (currentState == State.State) {
                            currentVar.Value = reader.Value;

                        } else if (currentState == State.Input) {
                            currentVar.Value = reader.Value;

                        }
                        //                      Console.WriteLine (reader.Value);
                        //                      writer.WriteString(reader.Value);
                        break;
                    case XmlNodeType.XmlDeclaration:
                    case XmlNodeType.ProcessingInstruction:
                        //                      writer.WriteProcessingInstruction(reader.Name, reader.Value);
                        break;
                    case XmlNodeType.Comment:
                        //                      writer.WriteComment(reader.Value);
                        break;
                    case XmlNodeType.EndElement:
                        //                      writer.WriteFullEndElement();

                        if (reader.Name == "counter-example") {
                            currentState = State.None;

                        } else if (reader.Name == "node") {
                            currentState = State.CounterExample;

                        } else if (reader.Name == "state") {
                            currentState = State.Node;
                            cx.Items.Add (currentItem);

                        } else if (reader.Name == "value") {
                            currentState = State.State;
                            currentItem.Variables.Add (currentVar);

                        } else if (reader.Name == "input") {
                            currentState = State.Node;
                            cx.Items.Add (currentItem);

                        }

                        break;
                    }
                }

            }
            return cx;
        }
    }
}

