using System;
using System.Linq;
using System.Text.RegularExpressions;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Utils;
using UCLouvain.KAOSTools.Core.SatisfactionRates;
using UCLouvain.KAOSTools.Integrators;
using UCLouvain.KAOSTools.Propagators;
using UCLouvain.KAOSTools.Propagators.BDD;
using System.IO;
using UCLouvain.KAOSTools.Utils.FileExporter;
using UCLouvain.KAOSTools.OmnigraffleExport.Omnigraffle;
using UCLouvain.KAOSTools.OmnigraffleExport;

namespace UCLouvain.KAOSTools.Utils.CMSelector
{
    class CMIntegratorProgram : KAOSToolCLI
    {
        public static void Main (string [] args)
        {
            Console.WriteLine ("*** This is CMIntegrator from KAOSTools. ***");
            Console.WriteLine ("*** For more information on KAOSTools see <https://github.com/ancailliau/KAOSTools> ***");
            Console.WriteLine ("*** Please report bugs to <https://github.com/ancailliau/KAOSTools/issues> ***");
            Console.WriteLine ();
            Console.WriteLine ("*** Copyright (c) 2017, Université catholique de Louvain ***");
            Console.WriteLine ("");

            Init (args);

            bool stop = false;
            while (!stop) {
                try {

                    Console.Write ("> ");
                    var input = Console.ReadLine ().Trim ();
                    if (input.Equals ("quit") | input.Equals("exit")) {
                        stop = true;
                        continue;
                    }

                    bool success = false;
                    Regex regex = new Regex (@"resolve ([a-zA-Z][a-zA-Z0-9_-]*)");
                    Match match = regex.Match (input);
                    if (match.Success) {
                        success = true;
                        var o_identifier = match.Groups [1].Value;
						if (o_identifier.Equals("all")) {
                            var integrator = new SoftResolutionIntegrator (model);
							foreach (var resolution in model.Resolutions())
							{
								integrator.Integrate(resolution);
							}
							continue;
						}
                        
                        
                        Obstacle o;
                        if ((o = model.Obstacle (o_identifier)) != null) {
                            var resolutions = o.Resolutions ().ToArray ();
                            for (int i = 0; i < resolutions.Length; i++) {
                                Console.WriteLine ($"[{i}] " + resolutions [i].ResolvingGoal ().FriendlyName);
                            }
                            Console.Write ("Select the countermeasure goal to integrate: ");
                            int index = -1;
                            do {
                                var input_index = Console.ReadLine ();
                                if (int.TryParse (input_index, out index)) {
                                    if (index < resolutions.Length) {
                                        var integrator = new SoftResolutionIntegrator (model);
                                        integrator.Integrate (resolutions [index]);
                                    } else {
                                        index = -1;
                                    }
                                }
                            } while (index < 0);
                            continue;

                        } else {
                            Console.WriteLine ($"Obstacle '{o_identifier}' not found");
                        }
                    }

                    regex = new Regex (@"export ([a-zA-Z0-9_\.-]+)");
                    match = regex.Match (input);
                    if (match.Success) {
                        success = true;
                        if (File.Exists (match.Groups [1].Value)) {
                            Console.Write ($"Do you want to overwrite '{match.Groups [1].Value}' (yes/no)? ");
                            var input_resp = Console.ReadLine ().Trim ();
                            if (input_resp.Equals ("yes")) {
                                File.Delete (match.Groups [1].Value);
                            } else {
                                continue;
                            }
                        }

                        var e = new KAOSFileExporter (model);
                        File.WriteAllText (match.Groups [1].Value, e.Export ());
                        Console.WriteLine ("Model exported to " + match.Groups [1].Value);
                        continue;
                    }
                    
                    regex = new Regex (@"export_diagram ([a-zA-Z0-9_\.-]+)");
                    match = regex.Match (input);
                    if (match.Success) {
                        success = true;
                        if (File.Exists (match.Groups [1].Value)) {
                            Console.Write ($"Do you want to overwrite '{match.Groups [1].Value}' (yes/no)? ");
                            var input_resp = Console.ReadLine ().Trim ();
                            if (input_resp.Equals ("yes")) {
                                File.Delete (match.Groups [1].Value);
                            } else {
                                continue;
                            }
                        }

						Document document = OmnigraffleMainClass.ExportModel(model);
						OmniGraffleGenerator.Export(document, match.Groups [1].Value);
                        
                        Console.WriteLine ("Model exported to " + match.Groups [1].Value);
                        continue;
                    }

                    if (!success) {
                        Console.WriteLine ("Command not recognized.");
                    }
                } catch (Exception e) {
                    PrintError (e.Message);
                    Console.Write ("Print more? (yes/no)");
                    var input_resp = Console.ReadLine ().Trim ();
                    if (!input_resp.Equals ("yes"))
                        continue;
                    else
                        PrintError ("An error occured during the computation. (" + e.Message + ").\n"
                                    + "Please report this error to <https://github.com/ancailliau/KAOSTools/issues>.\n"
                                    + "----------------------------\n"
                                    + e.StackTrace
                                    + "\n----------------------------\n");

                }
            }

        }
    }
}
