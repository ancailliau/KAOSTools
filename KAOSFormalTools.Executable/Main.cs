using System;
using NDesk.Options;
using KAOSFormalTools.Domain;
using System.Collections.Generic;
using System.IO;

namespace KAOSFormalTools.Executable
{
    public abstract class KAOSFormalToolsCLI
    {
        protected static OptionSet options = new OptionSet ();
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
                PrintError ("Please provide a file");
                return;
            }
            
            if (reminderArgs.Count > 1) {
                PrintError ("Please provide only one file");
                return;
            }
            
            if (!File.Exists (reminderArgs[0])) {
                PrintError ("File `" + reminderArgs[0] + "` does not exists");
                return;
            }

            filename = reminderArgs[0];
            model = BuildModel ();
        }
        
        protected static GoalModel BuildModel ()
        {
            var parser = new KAOSFormalTools.Parsing.Parser ();
            return parser.Parse (File.ReadAllText (filename), filename);
        }
        
        protected static void ShowHelp (OptionSet p)
        {
            Console.WriteLine ("Usage: KAOSFormalTools.OmnigraffleExport model");
            Console.WriteLine ();
            Console.WriteLine ("Options:");
            p.WriteOptionDescriptions (Console.Out);
        }
        
        protected static void PrintError (string error)
        {  
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.Write ("KAOSFormalTools.OmnigraffleExport: ");
            Console.Error.WriteLine (error);
            Console.Error.WriteLine ("Try `KAOSFormalTools.OmnigraffleExport --help' for more information.");
            Console.ResetColor ();
        }
    }
}
