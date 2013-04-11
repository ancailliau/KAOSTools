using System;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using NDesk.Options;
using KAOSFormalTools.Domain;

namespace KAOSFormalTools.TextExporter
{
    class MainClass
    {
        static Random random;
        
        public static void Main (string[] args)
        {
            bool show_help = false;
            
            var p = new OptionSet () {
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
            
            random = new Random();
            
            var model =  BuildModel (r[0]);
            foreach (var g in model.RootGoals)
                DisplayGoal (0, g);
        }

        static void DisplayGoal (int level, Goal g)
        {

            Console.WriteLine (new String(' ', level * 2) + (g.AssignedAgents.Count == 0 ? "{0}" : "{0}*"), g.Name);
            
            foreach (var refinement in g.Refinements)
                DisplayRefinement (level, refinement);
        }

        static void DisplayRefinement (int level, GoalRefinement r)
        {
            foreach (var goal in r.Children)
                DisplayGoal (level + 1, goal);
        }

        static GoalModel BuildModel (string filename)
        {
            var parser = new KAOSFormalTools.Parsing.Parser ();
            return parser.Parse (File.ReadAllText (filename)).GoalModel;
        }
        
        static void ShowHelp (OptionSet p)
        {
            Console.WriteLine ("Usage: KAOSFormalTools.OmnigraffleExport model");
            Console.WriteLine ();
            Console.WriteLine ("Options:");
            p.WriteOptionDescriptions (Console.Out);
        }
        
        static void PrintError (string error)
        {  
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.Write ("KAOSFormalTools.OmnigraffleExport: ");
            Console.Error.WriteLine (error);
            Console.Error.WriteLine ("Try `KAOSFormalTools.OmnigraffleExport --help' for more information.");
            Console.ResetColor ();
        }
    }
}
