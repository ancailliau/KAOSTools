using System;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using NDesk.Options;
using KAOSFormalTools.Domain;
using LtlSharp;
using KAOSFormalTools.RefinementChecker;

namespace KAOSFormalTools.RefinementChecker
{
    class MainClass
    {
        public static void Main (string[] args)
        {
            bool show_help = false;
            string nusmvModel      = "";
            string nusmvOutput     = "";
            string obstacles       = null;
            bool   verbose         = false;

            var p = new OptionSet () {
                { "w|write=", "Write a NuSMV model in specified file",
                    v => nusmvOutput = v },
                { "r|read=", "Read a NuSMV output and display a report",
                    v => nusmvModel = v },
                { "o|obstacles=", "List of obstacles",
                    v => obstacles = v },
                { "v|verbose", "Verbose mode",
                    v => verbose = true },
                { "h|help",  "show this message and exit", 
                    v => show_help = true },
            };

            List<string> r;
            try {
                r = p.Parse (args);
                
            } catch (OptionException e) {
                PrintError (e.Message);
                return;
            }

            if (show_help) {
                ShowHelp (p);
                return;
            }
    
            if (show_help) {
                ShowHelp (p);
                return;
            }

            if (string.IsNullOrEmpty (nusmvModel) & string.IsNullOrEmpty (nusmvOutput)) {
                PrintError ("`-w|--write` or `-r|--read` shall be given");
                return;
            }

            if (!string.IsNullOrEmpty (nusmvModel) & !string.IsNullOrEmpty (nusmvOutput)) {
                PrintError ("`-w|--write` or `-r|--read` are mutually exclusive");
                return;
            }

            if (r.Count == 0) {
                PrintError ("Please provide a file");
                return;
            }
                        
            if (r.Count > 1) {
                PrintError ("Please provide only one file");
                return;
            }

            if (!File.Exists (r[0])) {
                PrintError ("File `" + r[0] + "` does not exists");
                return;
            }

            try {
                var model =  BuildModel (r[0]);
                ProofObligationGenerator generator;

                if (obstacles != null) {
                    var g = new List<Goal> ();
                    var obstructedGoalNames = obstacles.Split (',');
                    foreach (var untrimmedName in obstructedGoalNames) {
                        var name = untrimmedName.Trim ();
                        var goals = model.GoalModel.Goals.Where (x => x.Name == name);
                        g.AddRange (goals);
                    }

                    generator = new ProofObligationGenerator (model.GoalModel, null, g);
                } else {
                    generator = new ProofObligationGenerator (model.GoalModel);
                }

                if (!string.IsNullOrEmpty (nusmvModel)) {
                    model.GoalModel.InterpretNuSMVOutput (nusmvModel, generator, verbose);

                } else {
                    model.GoalModel.WriteNuSMVModel (nusmvOutput, generator);
                }
            } catch (Exception e) {
                PrintError (e.Message);
            }
        }

        static KAOSModel BuildModel (string filename)
        {
            var parser = new KAOSFormalTools.Parsing.Parser ();
            return parser.Parse (File.ReadAllText (filename));
        }

        static void ShowHelp (OptionSet p)
        {
            Console.WriteLine ("Usage: " + System.AppDomain.CurrentDomain.FriendlyName + " model");
            Console.WriteLine ();
            Console.WriteLine ("Options:");
            p.WriteOptionDescriptions (Console.Out);
        }
        
        static void PrintError (string error)
        {  
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.Write ("{0}: ", System.AppDomain.CurrentDomain.FriendlyName);
            Console.Error.WriteLine (error);
            Console.Error.WriteLine ("Try `{0} --help' for more information.", System.AppDomain.CurrentDomain.FriendlyName);
            Console.ResetColor ();
        }
    }
}
