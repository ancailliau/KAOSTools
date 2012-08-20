using System;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using NDesk.Options;
using KAOSFormalTools.Domain;
using LtlSharp;
using KAOSFormalTools.RefinementChecker;

namespace Beaver.CLI
{
    class MainClass
    {
        public static void Main (string[] args)
        {
            bool show_help = false;
            string nusmvModel      = "";
            string nusmvOutput     = "";
    
            var p = new OptionSet () {
                { "w|write=", "Write a NuSMV model in specified file",
                    v => nusmvOutput = v },
                { "r|read=", "Read a NuSMV output and display a report",
                    v => nusmvModel = v },
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

            var model =  BuildModel (r[0]);

            if (!string.IsNullOrEmpty (nusmvModel)) {
                model.InterpretNuSMVOutput (nusmvModel);

            } else {
                model.WriteNuSMVModel (nusmvOutput);
            }
        }

        static GoalModel BuildModel (string filename)
        {
            var parser = new KAOSFormalTools.Parsing.Parser ();
            return parser.Parse (File.ReadAllText (filename));
        }

        static void ShowHelp (OptionSet p)
        {
            Console.WriteLine ("Usage: ltlsharp [OPTIONS]+ formula");
            Console.WriteLine ();
            Console.WriteLine ("Options:");
            p.WriteOptionDescriptions (Console.Out);
        }
        
        static void PrintError (string error)
        {  
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.Write ("ltlsharp: ");
            Console.Error.WriteLine (error);
            Console.Error.WriteLine ("Try `ltlsharp --help' for more information.");
            Console.ResetColor ();
        }
    }
}
