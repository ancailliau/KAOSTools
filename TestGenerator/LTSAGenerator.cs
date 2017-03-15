using System;
using KAOSTools.MetaModel;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace TestGenerator
{
    public class LTSAFluent {
        public string Name;
        public List<string> IEvent;
        public List<string> EEvent;
        public bool Software;
        public bool DefaultValue = false;
        public LTSAFluent (string name)
        {
            this.Name = name;
            this.Software = false;
            IEvent = new List<string> ();
            EEvent = new List<string> ();
        }
    }

    public class LTSAGenerator
    {
        private KAOSModel model;

        public Dictionary<Predicate, LTSAFluent> fluents;

        public LTSAGenerator (KAOSModel model)
        {
            this.model = model;
            fluents = new Dictionary<Predicate, LTSAFluent> ();

            GenerateFluents ();
        }

        public void PrintModel (StreamWriter stream)
        {
            stream.WriteLine ("// Step 2. Defining fluents");
            stream.WriteLine ("// ========================");
            stream.WriteLine ();
            stream.WriteLine ("// Theses are derived automatically form the operation DOMAIN postconditions");
            foreach (var fl in fluents.Values) {
                stream.WriteLine ("fluent {0} = <{{{1}}},{{{2}}}> initially {3}", 
                    fl.Name, string.Join (",", fl.IEvent), string.Join (",", fl.EEvent), fl.DefaultValue ? "1" : "0"
                );
            }

            stream.WriteLine ();
            stream.WriteLine ("// Step 3. Translating domain preconditions");
            stream.WriteLine ("// ========================================");
            stream.WriteLine ();
            foreach (var op in model.Operations ()) {
                stream.WriteLine ("constraint DomPre_{0} = [] ( ( tick && ! ({1}) ) -> X (!{2} W tick ) ) ", op.Identifier,
                    LTSAFormulaPrinter.ToString (op.DomPre),
                    op.Identifier.ToLower ()
                );
            }

            stream.WriteLine ();
            stream.WriteLine ("// Constraints for interleaving of events");
            foreach (var fl in fluents.Values) {
                //set PumpOn_Events = {startPump, stopPump}
                // InterleavingCstr_for_PumpOn = (tick -> S0),
                // S0 = (tick -> S0 | {PumpOn_Events} -> tick -> S0).
                stream.WriteLine ("set {0}_Events = {{{1}}}", fl.Name, string.Join (",", fl.IEvent.Union (fl.EEvent)));
                stream.WriteLine ("InterleavingCstr_for_{0} = (tick -> S0),", fl.Name);
                stream.WriteLine ("S0 = (tick -> S0 | {{{0}_Events}} -> tick -> S0).", fl.Name);
            }


            stream.WriteLine ();
            stream.WriteLine ("// Software Model");
            stream.WriteLine ("minimal || SoftwareModel = (");
            bool first = true;
            var ops = model.Elements.OfType<OperationAgentPerformance> ()
                .Where (operation => operation.Agents ().All (y => y.Type == AgentType.Software))
                .Select (x => model.Elements.Single (y => y.Identifier == x.OperationIdentifier));
            foreach (var op in ops) {
                stream.WriteLine ("  {0}DomPre_{1}", first ? "" : "|| ", op.Identifier);
                first = false;
            }
            foreach (var fl in fluents.Values.Where (x => x.Software)) {
                stream.WriteLine ("  {0}InterleavingCstr_for_{1}", first ? "" : "|| ", fl.Name);
                first = false;
            }
            stream.WriteLine (").");

            stream.WriteLine ("// Domain properties");
            foreach (var domprop in model.DomainProperties ()) {
                stream.WriteLine ("constraint DomInvar_{0} = {1}", domprop.Identifier, LTSAFormulaPrinter.ToString (domprop.FormalSpec.ToAsynchronous ()));
            }

            stream.WriteLine ();
            stream.WriteLine ("// Environment Model");
            stream.WriteLine ("minimal || EnvironmentModel = (");
            first = true;
            var opsE = model.Elements.OfType<OperationAgentPerformance> ()
                .Where (operation => operation.Agents ().All (y => y.Type == AgentType.Environment | y.Type == AgentType.None))
                .Select (x => model.Elements.Single (y => y.Identifier == x.OperationIdentifier));
            foreach (var op in opsE) {
                stream.WriteLine ("  {0}DomPre_{1}", first ? "" : "|| ", op.Identifier);
                first = false;
            }
            foreach (var fl in fluents.Values.Where (x => !x.Software)) {
                stream.WriteLine ("  {0}InterleavingCstr_for_{1}", first ? "" : "|| ", fl.Name);
                first = false;
            }
            foreach (var domprop in model.DomainProperties ()) {
                stream.WriteLine ("  {0}DomInvar_{1}", first ? "" : "|| ", domprop.Identifier);
            }
            stream.WriteLine ("  ).");

            stream.WriteLine ();
            stream.WriteLine ("// Step 4. Translating operational requirements");
            stream.WriteLine ("// ============================================");
            stream.WriteLine ();
            foreach (var op in model.Operations ()) {
                bool print = false;
                // constraint for ReqTrig: [](tick && ReqTrig && DomPre -> X(! tick W operation))
                foreach (var rt in op.ReqTrig) {
                    stream.WriteLine ("constraint ReqTRIG_On_{0}_For_{1} = \n" +
                                      "  [](tick && ({3}) && ({4}) -> X(! tick W {2}))", op.FriendlyName, rt.Goal.LTSAName(), op.Identifier.ToLower (),
                        LTSAFormulaPrinter.ToString (rt.Specification), LTSAFormulaPrinter.ToString (op.DomPre));
                    print = true;
                }
                // constraint for ReqPre:  ([]((tick && ! ReqPRE) -> X(! action W tick)))
                foreach (var rp in op.ReqPre) {
                    stream.WriteLine ("constraint ReqPRE_On_{0}_For_{1} = \n" +
                                      "  []((tick && ! {3}) -> X(! {2} W tick))", op.FriendlyName, rp.Goal.LTSAName(), op.Identifier.ToLower (),
                        LTSAFormulaPrinter.ToString (rp.Specification));
                    print = true;
                }
                // constraint for ReqPost: [](action -> (! tick W ReqPost))
                foreach (var rp in op.ReqPost) {
                    stream.WriteLine ("constraint ReqPOST_On_{0}_For_{1} = \n" +
                                      "  []({2} -> (! tick W {3}))", op.FriendlyName, rp.Goal.LTSAName(), op.Identifier.ToLower (),
                        LTSAFormulaPrinter.ToString (rp.Specification));
                    print = true;
                }
                if (print)
                    stream.WriteLine ();
            }

            stream.WriteLine ("minimal || OperationalizedGoals = (");
            // software more
            // req/pre/trig/post above
            stream.WriteLine ("  SoftwareModel");
            foreach (var op in model.Operations ()) {
                foreach (var rt in op.ReqTrig) {
                    stream.WriteLine ("  || ReqTRIG_On_{0}_For_{1}", op.FriendlyName, rt.Goal.LTSAName());
                }
                foreach (var rp in op.ReqPre) {
                    stream.WriteLine ("  || ReqPRE_On_{0}_For_{1}", op.FriendlyName, rp.Goal.LTSAName());
                }
                foreach (var rp in op.ReqPost) {
                    stream.WriteLine ("  || ReqPOST_On_{0}_For_{1}", op.FriendlyName, rp.Goal.LTSAName());
                }
            }
            stream.WriteLine (").");
            stream.WriteLine ();
            stream.WriteLine ("|| Model = (OperationalizedGoals || EnvironmentModel ).");


        }

        public void GenerateFluents ()
        {
            foreach (var p in model.Predicates ()) {
                var f = new LTSAFluent (p.FriendlyName);
                f.DefaultValue = p.DefaultValue;
                fluents.Add (p, f);
            }

            foreach (var op in model.Operations ()) {
                AddEvent (op, op.DomPost);
            }
        }

        private void AddEvent (Operation op, Formula f)
        {
            if (f is And) {
                var a = (And)f;
                AddEvent (op, a.Left);
                AddEvent (op, a.Right);
                return;
            }

            if (f is PredicateReference) {
                var pr = (PredicateReference)f;
                var fl = fluents [pr.Predicate];
                fl.IEvent.Add (op.Identifier.ToLower ());
                var performingAgent = op.PerformedBy ().SingleOrDefault ()?.Agents ().Single ();
                if (performingAgent != null &&performingAgent.Type == AgentType.Software) {
                    fl.Software = true;
                }
                return;
            }

            if (f is Not) {
                var n = (Not)f;
                if (n.Enclosed is PredicateReference) {
                    var pr = (PredicateReference)n.Enclosed;
                    var fl = fluents [pr.Predicate];
                    fl.EEvent.Add (op.Identifier.ToLower ());
                    var performingAgent = op.PerformedBy ().SingleOrDefault ()?.Agents ().Single ();
                    if (performingAgent != null && performingAgent.Type == AgentType.Software) {
                        fl.Software = true;
                    }
                    return;
                }
            }

            throw new NotSupportedException ();
        }
    }
}

