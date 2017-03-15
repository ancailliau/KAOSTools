using System;
using KAOSTools.Utils;
using System.Linq;
using KAOSTools.MetaModel;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

namespace TestGenerator
{
    public class MainClass : KAOSToolCLI
    {
        public static void Main (string[] args)
        {
//            Console.WriteLine ("Hello World!");
            bool nunit = false;
            string filenameNunit = "";
            bool skipLTSA = false;
            bool skipOperationalization = false;

            options.Add ("nunit", "Generate NUnit Test Cases", v => nunit = true);
            options.Add ("f=", "Specify the filename for the nunit test cases", v => filenameNunit = v);
            options.Add ("skipLTSA", "Skip the generation of the LTSA model", v => skipLTSA = true);
            options.Add ("skipOp", "Skip the check of operationalization", v => skipOperationalization = true);

            Init (args);

//            foreach (var op in model.Elements.OfType<Operation> ()) {
//                Console.WriteLine ("Operation [" + op.Identifier + "]");
//                Console.WriteLine ("  Name " + op.Name);
//                var agents = model.Elements.OfType<OperationAgentPerformance> ().SingleOrDefault (x => x.OperationIdentifier == op.Identifier);
//                if (agents != null)
//                    Console.WriteLine ("  PerformedBy " + string.Join (",", agents.Agents ()));
//
//                Console.WriteLine ("  DomPre " + FormulaPrinter.ToString (op.DomPre));
//                Console.WriteLine ("  DomPost " + FormulaPrinter.ToString (op.DomPost));
//                foreach (var reqpre in op.ReqPre) {
//                    Console.WriteLine ("  ReqPre for " + reqpre.Goal.FriendlyName);
//                    Console.WriteLine ("    " + FormulaPrinter.ToString (reqpre.Specification));
//                }
//                foreach (var reqtrig in op.ReqTrig) {
//                    Console.WriteLine ("  ReqTrig for " + reqtrig.Goal.FriendlyName);
//                    Console.WriteLine ("    " + FormulaPrinter.ToString (reqtrig.Specification));
//                }
//                foreach (var reqpost in op.ReqPost) {
//                    Console.WriteLine ("  ReqPost for " + reqpost.Goal.FriendlyName);
//                    Console.WriteLine ("    " + FormulaPrinter.ToString (reqpost.Specification));
//                }
//                Console.WriteLine ();
//            }
//
//            Console.WriteLine ("-***-");

            var ltsa = new LTSAGenerator (model);

            if (!skipLTSA) {
                if (Directory.Exists ("/tmp/kaostools")) {
                    Directory.Delete ("/tmp/kaostools", true);
                }
            }

            if (!Directory.Exists ("/tmp/kaostools")) {
                Directory.CreateDirectory ("/tmp/kaostools");
            }

			StreamWriter file;
			Process process;
			ProcessStartInfo startInfo;

            if (!skipLTSA) {
                file = new StreamWriter ("/tmp/kaostools/model.lts");
                ltsa.PrintModel (file);
                file.Close ();
            }

            Console.Write ("Generating behaviour model (using LTSA)");
            process = new Process ();
            startInfo = new ProcessStartInfo ();
            startInfo.FileName = "java";
            startInfo.Arguments = @"-cp ""/Users/acailliau/Downloads/ltsatool 2/ltsa.jar"":. Main /tmp/kaostools/model.lts Model /tmp/kaostools/model.aut";
            startInfo.WorkingDirectory = "/Users/acailliau/Documents/workspace/LTSARunner/bin";
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo = startInfo;
            process.Start ();
            process.WaitForExit ();
            Console.WriteLine (".");

            int i = 0;
            var leafGoals = model.Goals ().Where (x => x.Refinements ().Count () == 0);
            if (!skipOperationalization) {

                Console.Write ("Generating NuSMV files for checking operationalization");
                file = new StreamWriter ("/tmp/kaostools/model-operationalization.smv");
                AUT2NUSMV.Convert (ltsa, model, "/tmp/kaostools/model.aut", file);

                file.WriteLine ();
                file.WriteLine ("-- Checking the requirements");

                var mapping = new Goal [leafGoals.Count ()];
                foreach (var req in leafGoals) {
                    if (req.FormalSpec != null) {
                        file.WriteLine ("-- " + req.FriendlyName);
                        file.WriteLine ("\tLTLSPEC NAME spec_" + req.Identifier + " := " + NuSMVPrinter.ToNuSMV (req.FormalSpec.ToAsynchronous ()));
                    }
                    mapping [i] = req;
                    i++;
                }

                file.Close ();
                Console.Write (".");

                file = new StreamWriter ("/tmp/kaostools/nusmv-script-operationalization");
                file.WriteLine ("set default_trace_plugin 4");
                file.WriteLine ("read_model -i model-operationalization.smv");
                file.WriteLine ("go");
                for (i = 0; i < mapping.Length; i++) {
                    if (mapping [i].FormalSpec != null) {
                        file.WriteLine ("check_ltlspec -P spec_{0}", mapping [i].Identifier);
                        file.WriteLine ("show_traces -o counterexample-operationalization-{0}.xml", i);
                    }
                }
                file.WriteLine ("quit");

                file.Close ();
                Console.WriteLine (".");

                process = new Process ();
                startInfo = new ProcessStartInfo ();
                startInfo.FileName = "NuSMV";
                startInfo.Arguments = @"-source /tmp/kaostools/nusmv-script-operationalization";
                startInfo.WorkingDirectory = @"/tmp/kaostools/";
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                process.StartInfo = startInfo;
                process.Start ();
                process.WaitForExit ();

                for (i = 0; i < leafGoals.Count (); i++) {
                    if (mapping [i].FormalSpec != null) {
                        var filename = string.Format ("/tmp/kaostools/counterexample-operationalization-{0}.xml", i);
                        if (File.Exists (filename)) {
                            Console.WriteLine ("  '{0}' is not guaranteed by the operationalization", mapping [i].FriendlyName);
                            Console.WriteLine ("  See {0} for a counterexample.", filename);
                        } else {
                            Console.WriteLine ("  '{0}' is guaranteed by the operationalization", mapping [i].FriendlyName);
                        }
                    }
                }
            }

            Console.Write ("Generating NuSMV file for generating test cases");
            file = new StreamWriter ("/tmp/kaostools/model-testcases.smv");
            AUT2NUSMV.Convert (ltsa, model, "/tmp/kaostools/model.aut", file);

            file.WriteLine ();
            file.WriteLine ("-- Trap properties for generating test cases");
            int nb_ufc = 0;
            var mapping2 = new Dictionary<Goal, List<int>> ();
            var mapping_obstacles = new Dictionary<Obstacle, List<int>> ();
            var mapping3 = new Dictionary<int, Formula> ();
            i = 0;
            foreach (var req in model.Goals ().Where (x => x.Refinements ().Count () == 0)) {
                if (req.FormalSpec != null) {
                    mapping2.Add (req, new List<int> ());
                    file.WriteLine ("-- " + req.FriendlyName);
                    var ufcPos = req.FormalSpec.UFCpos ();
                    foreach (var u in ufcPos) {
                        var not = new Not (u.ToAsynchronous ());
                        file.WriteLine ("\tLTLSPEC NAME trap_" + i + " := " + NuSMVPrinter.ToNuSMV (not));
                        mapping2 [req].Add (i);
                        mapping3.Add (i, u);
                        nb_ufc++;
                        i++;
                    }
                    // Trying with negative UFC
                    //var ufcNeg = req.FormalSpec.UFCneg ();
                    //foreach (var u in ufcNeg) {
                    //    var not = new Not (u.ToAsynchronous ());
                    //    file.WriteLine ("\tLTLSPEC NAME trap_" + i + " := " + NuSMVPrinter.ToNuSMV (not));
                    //    mapping2 [req].Add (i);
                    //    mapping3.Add (i, u);
                    //    nb_ufc++;
                    //    i++;
                    //}
                } else {
                    //Console.Error.WriteLine ("WARNING: Goal '{0}' is not formalized", req.FriendlyName);
                }
            }

            foreach (var obstacle in model.LeafObstacles ()) {
                if (obstacle.FormalSpec != null) {
                    mapping_obstacles.Add (obstacle, new List<int> ());
                    file.WriteLine ("-- " + obstacle.FriendlyName);
                    var ufcPos = obstacle.FormalSpec.UFCpos ();
                    foreach (var u in ufcPos) {
                        var not = new Not (u.ToAsynchronous ());
                        file.WriteLine ("\tLTLSPEC NAME trap_" + i + " := " + NuSMVPrinter.ToNuSMV (not));
                        mapping_obstacles [obstacle].Add (i);
                        mapping3.Add (i, u);
                        nb_ufc++;
                        i++;
                    }
                }
            }

            file.Close ();
            Console.Write (".");

            file = new StreamWriter ("/tmp/kaostools/nusmv-script-testcases");
            file.WriteLine ("set default_trace_plugin 4");
            file.WriteLine ("read_model -i /tmp/kaostools/model-testcases.smv");
            file.WriteLine ("go_bmc");
            for (i = 0; i < nb_ufc; i++) {
                if (mapping3.ContainsKey (i)) {
                    file.WriteLine ("check_ltlspec_bmc -P trap_{0}", i);
                    file.WriteLine ("show_traces -o counterexample-testcases-{0}.xml", i);
                }
            }
            file.WriteLine ("quit");

            file.Close ();
            Console.WriteLine (".");

            process = new Process ();
            startInfo = new ProcessStartInfo ();
            startInfo.FileName = "NuSMV";
            startInfo.Arguments = @"-source /tmp/kaostools/nusmv-script-testcases";
            startInfo.WorkingDirectory = @"/tmp/kaostools/";
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;

            process.StartInfo = startInfo;
            process.Start ();
            process.WaitForExit ();

            foreach (var r in leafGoals) {
                if (r.FormalSpec != null) {
                    Console.WriteLine ("  UFC for '{0}' ({1})", r.FriendlyName, NuSMVPrinter.ToNuSMV (r.FormalSpec));
                    foreach (var u in mapping2 [r]) {
                        var filename = string.Format ("/tmp/kaostools/counterexample-testcases-{0}.xml", u);
                        if (File.Exists (filename)) {
                            Console.WriteLine ("  - {0} generate a counter-example:", NuSMVPrinter.ToNuSMV (new Not (mapping3 [u])));
                            if (nunit) {
                                var cx = NuSMVCounterExampleParser.Parse (filename);
                                var test = NUnitGenerator.Generate (string.Format ("TestCase{0}_for_{1}", u, r.Identifier), cx);

                                if (!string.IsNullOrEmpty (filenameNunit)) {
                                    File.AppendAllText (filenameNunit, test);

                                } else {
                                    Console.WriteLine ();
                                    Console.WriteLine (test);
                                    Console.WriteLine ();
                                }

                            } else {
                                Console.WriteLine ();
                                var cx = NuSMVCounterExampleParser.Parse (filename);
                                foreach (var a in cx.Items) {
                                    if (a is CXInput) {
                                        var value = a.Variables.Single (x => x.Name == "Label").Value;
                                        if (value == "tick") {
                                            Console.WriteLine ("     (tick)");
                                        } else {
                                            Console.WriteLine ("      " + value);
                                        }
                                    }
                                }
                                Console.WriteLine ();
                            }
                        } else {
                            Console.WriteLine ("  - {0} is satisfied", NuSMVPrinter.ToNuSMV (new Not (mapping3 [u])));
                        }
                    }
                }
            }

            foreach (var r in model.LeafObstacles ()) {
                if (r.FormalSpec != null) {
                    Console.WriteLine ("  UFC for '{0}' ({1})", r.FriendlyName, NuSMVPrinter.ToNuSMV (r.FormalSpec));
                    foreach (var u in mapping_obstacles [r]) {
                        var filename = string.Format ("/tmp/kaostools/counterexample-testcases-{0}.xml", u);
                        if (File.Exists (filename)) {
                            Console.WriteLine ("  - {0} generate a counter-example:", NuSMVPrinter.ToNuSMV (new Not (mapping3 [u])));
                            if (nunit) {
                                var cx = NuSMVCounterExampleParser.Parse (filename);
                                var test = NUnitGenerator.Generate (string.Format ("TestCase{0}_for_{1}", u, r.Identifier), cx);

                                if (!string.IsNullOrEmpty (filenameNunit)) {
                                    File.AppendAllText (filenameNunit, test);

                                } else {
                                    Console.WriteLine ();
                                    Console.WriteLine (test);
                                    Console.WriteLine ();
                                }

                            } else {
                                Console.WriteLine ();
                                var cx = NuSMVCounterExampleParser.Parse (filename);
                                foreach (var a in cx.Items) {
                                    if (a is CXInput) {
                                        var value = a.Variables.Single (x => x.Name == "Label").Value;
                                        if (value == "tick") {
                                            Console.WriteLine ("     (tick)");
                                        } else {
                                            Console.WriteLine ("      " + value);
                                        }
                                    }
                                }
                                Console.WriteLine ();
                            }
                        } else {
                            Console.WriteLine ("  - {0} is satisfied", NuSMVPrinter.ToNuSMV (new Not (mapping3 [u])));
                        }
                    }
                }
            }

            Console.WriteLine ("Bye!");

        }
    }
}
