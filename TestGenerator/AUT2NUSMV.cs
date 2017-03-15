using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using KAOSTools.MetaModel;

namespace TestGenerator
{
    public class AUT2NUSMV
    {
        class State
        {
            public string Name;
            public State (string name)
            {
                this.Name = name;
            }
        }

        class Transition
        {
            public int Source;
            public int Target;
            public string Label;
            public Transition (int source, string label, int target)
            {
                this.Source = source;
                this.Target = target;
                this.Label = label;
            }
        }

        public static void Convert (LTSAGenerator ltsa, KAOSModel model, string filename, StreamWriter output)
        {
            var content = File.ReadAllLines (filename);

            var nTransitions = -1;
            var nStates = -1;
            var initState = -1;

            var firstLine = content.First ();
            Regex regex = new Regex(@"des\(([0-9]+),([0-9]+),([0-9]+)\)");
            Match match = regex.Match(firstLine);
            if (match.Success) {
                initState = int.Parse (match.Groups [1].Value);
                nTransitions = int.Parse (match.Groups [2].Value);
                nStates = int.Parse (match.Groups [3].Value);
            }

            var states = new State[nStates];
            for (int i = 0; i < nStates; i++) {
                states[i] = new State ("s"+i);
            }
            var transitions = new Transition [nTransitions];

            int current = 0;
            foreach (var line in content.Skip (1)) {
                regex = new Regex(@"\(([0-9]+),([a-zA-Z0-9]+),([0-9]+)\)");
                match = regex.Match(line);
                if (match.Success)
                {
                    var source = int.Parse (match.Groups [1].Value);
                    var target = int.Parse (match.Groups [3].Value);
                    var label = match.Groups [2].Value;
                    // output.WriteLine ("{0} - {1} -> {2}", source, label, target);
                    transitions [current] = new Transition (source, label, target);
                }
                current++;
            }

            var labels = transitions.Select (x => x.Label).Distinct ();

            output.WriteLine ("MODULE main");
            output.WriteLine ();

            output.WriteLine ("VAR");
            output.WriteLine ("\tState : {0} .. {1};", 0, nStates);
            foreach (var fl in ltsa.fluents.Values) {
                output.WriteLine ("\t{0} : boolean;", fl.Name);
            }
            output.WriteLine ();

            output.WriteLine ("IVAR");
            output.WriteLine ("\tLabel : {{ {0} }};", string.Join (",", labels));
            output.WriteLine ();

            output.WriteLine ("INIT");
            output.WriteLine ("\tState = {0}", initState);
            foreach (var fl in ltsa.fluents.Values) {
                output.WriteLine ("\t & {1}{0}", fl.Name, fl.DefaultValue ? "" : "!");
            }

            output.WriteLine ();
            foreach (var fl in ltsa.fluents.Values) {
                // TRANS (Label = startPump) -> next (PumpOn);
                // TRANS (Label = stopPump) -> !next (PumpOn);
                // TRANS ( ! (Label in {startPump, stopPump}) ) -> PumpOn = next (PumpOn);
                output.WriteLine ("-- Fluent {0}", fl.Name);
                output.WriteLine ("\tTRANS (Label in {{{0}}}) -> next ({1});", string.Join (",", fl.IEvent), fl.Name);
                output.WriteLine ("\tTRANS (Label in {{{0}}}) -> !next ({1});", string.Join (",", fl.EEvent), fl.Name);
                output.WriteLine ("\tTRANS ( ! (Label in {{{0}}}) ) -> {1} = next ({1});", string.Join (",", fl.IEvent.Union (fl.EEvent)), fl.Name);
                output.WriteLine ();
            }

            //foreach (var domprop in model.DomainProperties ()) {
            //    if (domprop.FormalSpec is Globally) {
            //        var g = (Globally)domprop.FormalSpec;
            //        output.WriteLine ("\tINVAR {0}", NuSMVPrinter.ToNuSMV (g.Enclosed));
            //    } else if (domprop.FormalSpec is StrongImply) {
            //        var left = ((StrongImply)domprop.FormalSpec).Left;
            //        var right = ((StrongImply)domprop.FormalSpec).Right;
            //        output.WriteLine ("\tINVAR {0} -> {1}", NuSMVPrinter.ToNuSMV (left), NuSMVPrinter.ToNuSMV (right));

            //    } else {
            //        Console.WriteLine ("We only support domain properties with [] (simple_expr) structure");
            //    }
            //}

            output.WriteLine ();
            foreach (var t in transitions) {
                output.WriteLine ("\tTRANS (( State = {0} & Label = {1} ) -> next(State) = {2});",
                    t.Source, t.Label, t.Target);
            }

            foreach (var t in transitions.GroupBy (x => x.Source)) {
                output.WriteLine ("\tTRANS ( State = {0} ) -> ( Label in {{ {1} }} );", t.Key, string.Join (",",  t.Select (a => a.Label)));
            }

            for (int i = 0; i < nStates; i++) {
                if (transitions.All (t => t.Source != i)) {
                    output.WriteLine ("\tTRANS ( State = {0} ) -> ( ! ( Label in {{ {1} }} ) );", i, string.Join (",", labels));
                }
            }

        }
    }
}

