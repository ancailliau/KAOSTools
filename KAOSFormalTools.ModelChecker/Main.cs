using System;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using NDesk.Options;
using KAOSFormalTools.Domain;

namespace KAOSFormalTools.ModelChecker
{
    class MainClass
    {
        public static void Main (string[] args)
        {
            bool show_help = false;
            bool show_formal = false;

            var p = new OptionSet () {
                { "f|formal",  "Check formal attributes", 
                    v => show_formal = true },
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

            var model =  BuildModel (r[0]);
            DisplayUnassignedLeafGoals (model);

            if (show_formal)
                DisplayMissingFormalSpec (model);
        }

        private static void DisplayMissingFormalSpec (GoalModel model) 
        {
            var goals = from g in model.Goals where g.FormalSpec == null select g;
            foreach (var goal in goals) {
                Console.WriteLine ("[WARN] Goal '{0}' is missing formal specification", goal);
            }

            var domprops = from d in model.DomainProperties where d.FormalSpec == null select d;
            foreach (var domprop in domprops) {
                Console.WriteLine ("[WARN] Domain property '{0}' is missing formal specification", domprop);
            }

            var obstacles = from o in model.Obstacles where o.FormalSpec == null select o;
            foreach (var obstacle in obstacles) {
                Console.WriteLine ("[WARN] Obstacle '{0}' is missing formal specification", obstacle);
            }
        }

        private static void DisplayUnassignedLeafGoals (GoalModel model) 
        {
            var unassignedLeafGoals = from g in model.Goals 
                where g.AssignedAgents.Count == 0 & g.Refinements.Count == 0 select g;

            if (unassignedLeafGoals.Count() > 0) {
                Console.WriteLine ("[ KO ] Unassigned leaf goals");
                
                foreach (var item in unassignedLeafGoals) {
                    Console.WriteLine ("       - {0}", item.Name);
                }
            } else {
                Console.WriteLine ("[ OK ] All leaf goals are assigned");
            }
        }

        static GoalModel BuildModel (string filename)
        {
            var parser = new KAOSFormalTools.Parsing.Parser ();
            return parser.Parse (File.ReadAllText (filename));
        }

        static void ShowHelp (OptionSet p)
        {
            Console.WriteLine ("Usage: KAOSFormalTools.ModelChecker model");
            Console.WriteLine ();
            Console.WriteLine ("Options:");
            p.WriteOptionDescriptions (Console.Out);
        }
        
        static void PrintError (string error)
        {  
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.Write ("KAOSFormalTools.ModelChecker: ");
            Console.Error.WriteLine (error);
            Console.Error.WriteLine ("Try `KAOSFormalTools.ModelChecker --help' for more information.");
            Console.ResetColor ();
        }
    }
}
