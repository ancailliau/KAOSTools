using System;
using NDesk.Options;
using KAOSTools.MetaModel;
using System.Collections.Generic;
using System.IO;

namespace KAOSTools.Utils
{
    public abstract class KAOSFormalToolsCLI
    {
        protected static OptionSet options = new OptionSet ();
        protected static string input;
        protected static string filename;
        protected static GoalModel model;

        protected static void Init (string[] args)
        {
            bool show_help = false;
            options.Add ("h|help", "Show this message and exit", v => show_help = true);
            
            List<string> reminderArgs;
            try {
                reminderArgs = options.Parse (args);
                
            } catch (OptionException e) {
                PrintError (e.Message);
                return;
            }
            
            if (show_help) {
                ShowHelp (options);
                return;
            }

            if (reminderArgs.Count == 0) {
                filename = ".";
                input = Console.In.ReadToEnd ();

            } else {
                if (reminderArgs.Count > 1) {
                    PrintError ("Please provide only one file");
                    return;
                }

                if (!File.Exists (reminderArgs[0])) {
                    PrintError ("File `" + reminderArgs[0] + "` does not exists");
                    return;
                } else {
                    filename = reminderArgs[0];
                    input = File.ReadAllText (filename);
                }
            }

            model = BuildModel ();
            var h = new AlternativeHelpers();
            h.ComputeInAlternatives (model);
        }
        
        protected static GoalModel BuildModel ()
        {
            var parser = new KAOSTools.Parsing.Parser ();
            return parser.Parse (input, filename).GoalModel;
        }
        
        protected static void ShowHelp (OptionSet p)
        {
            Console.WriteLine ("Usage: KAOSTools.OmnigraffleExport model");
            Console.WriteLine ();
            Console.WriteLine ("Options:");
            p.WriteOptionDescriptions (Console.Out);
        }
        
        protected static void PrintError (string error)
        {  
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.Write ("KAOSTools.OmnigraffleExport: ");
            Console.Error.WriteLine (error);
            Console.Error.WriteLine ("Try `KAOSTools.OmnigraffleExport --help' for more information.");
            Console.ResetColor ();
        }
    }
}
