using System;
using NDesk.Options;
using UCLouvain.KAOSTools.Core;
using System.Collections.Generic;
using System.IO;
using UCLouvain.KAOSTools.Parsing;

namespace UCLouvain.KAOSTools.Utils
{
    public abstract class KAOSToolCLI
    {
        protected static OptionSet options = new OptionSet ();
        protected static string input;
        protected static string filename;
        protected static KAOSModel model;
        protected static List<string> reminderArgs;

        protected static void Init (string[] args)
        {
            bool show_help = false;
            options.Add ("h|help", "Show this message and exit", v => show_help = true);
            
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
                if (!File.Exists (reminderArgs[0])) {
                    PrintError ("File `" + reminderArgs[0] + "` does not exists");
                    return;
                } else {
                    filename = reminderArgs[0];
                    input = File.ReadAllText (filename);
                }
            }

            model = BuildModel ();

            //if (model != null) {
            //    var h = new AlternativeHelpers();
            //    h.ComputeInAlternatives (model);

            //    model.IntegrateResolutions ();
            //}
        }
        
        protected static KAOSModel BuildModel ()
        {
            // try {
                var parser = new KAOSTools.Parsing.ModelBuilder ();
                var m = parser.Parse (input, filename);
                return m;

            // } catch (Exception e) {
            //    Console.WriteLine (e.Message);
            //    return null;
            // }
        }
        
        protected static void ShowHelp (OptionSet p)
        {
            Console.WriteLine ("Usage: " + AppDomain.CurrentDomain.FriendlyName + " model");
            Console.WriteLine ();
            Console.WriteLine ("Options:");
            p.WriteOptionDescriptions (Console.Out);
        }
        
        protected static void PrintError (string error)
        {  
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.Write (AppDomain.CurrentDomain.FriendlyName + ": ");
            Console.Error.WriteLine (error);
            Console.Error.WriteLine ("Try `" + AppDomain.CurrentDomain.FriendlyName + " --help' for more information.");
            Console.ResetColor ();
        }
    }
}
