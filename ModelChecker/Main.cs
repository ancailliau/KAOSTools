using System;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using NDesk.Options;
using KAOSTools.MetaModel;

namespace KAOSTools.ModelChecker
{
    class MainClass
    {
        public static void Main (string[] args)
        {
            bool show_help = false;
            bool show_formal = false;
            int levensteinThreshold = 0;

            var p = new OptionSet () {
                { "f|formal",  "Check formal attributes", 
                    v => show_formal = true },
                { "l|levenstein=",  "Set threshold for duplicate detection", 
                    v => { 
                        int.TryParse (v, out levensteinThreshold);
                    }
                },
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

            if (!File.Exists (r [0])) {
                PrintError ("File `" + r [0] + "` does not exists");
                return;
            }

            var model = BuildModel (r [0]);
            CheckUnassignedLeafGoals (model);
            CheckGoalWithSimilarNames (model, levensteinThreshold);
            CheckMissingDefinition (model);

            if (show_formal)
                DisplayMissingFormalSpec (model);
        }

        private static void CheckMissingDefinition (GoalModel model)
        {
            var goals = model.Goals.Where (g => string.IsNullOrWhiteSpace (g.Definition));
            if (goals.Count () == 0) {
                Console.WriteLine ("[ OK ] All goals have definition");

            } else {
                foreach (var goal in goals) 
                    Console.WriteLine ("[ KO ] Goal '{0}' is missing definition", goal.Name);
            }

            var obstacles = model.Obstacles.Where (o => string.IsNullOrWhiteSpace (o.Definition));
            if (obstacles.Count () == 0) {
                Console.WriteLine ("[ OK ] All obstacles have definition");
                
            } else {
                foreach (var obstacle in obstacles) 
                    Console.WriteLine ("[ KO ] Obstacle '{0}' is missing definition", obstacle.Name);
            }

            var domprops = model.DomainProperties.Where (d => string.IsNullOrWhiteSpace (d.Definition));
            if (domprops.Count () == 0) {
                Console.WriteLine ("[ OK ] All domain properties have definition");
                    
            } else {
                foreach (var domprop in domprops) 
                    Console.WriteLine ("[ KO ] Domain property '{0}' is missing definition", domprop.Name);
            }

            var agents = model.Agents.Where (a => string.IsNullOrWhiteSpace (a.Description));
            if (agents.Count () == 0) {
                Console.WriteLine ("[ OK ] All agents have description");
                        
            } else {
                foreach (var agent in agents) 
                    Console.WriteLine ("[ KO ] Agent '{0}' is missing description", agent.Name);
            }
        }

        private static void DisplayMissingFormalSpec (GoalModel model)
        {
            var goals = from g in model.Goals where g.FormalSpec == null select g;
            foreach (var goal in goals) {
                Console.WriteLine ("[WARN] Goal '{0}' is missing formal specification", goal.Name);
            }

            var domprops = from d in model.DomainProperties where d.FormalSpec == null select d;
            foreach (var domprop in domprops) {
                Console.WriteLine ("[WARN] Domain property '{0}' is missing formal specification", domprop.Name);
            }

            var obstacles = from o in model.Obstacles where o.FormalSpec == null select o;
            foreach (var obstacle in obstacles) {
                Console.WriteLine ("[WARN] Obstacle '{0}' is missing formal specification", obstacle.Name);
            }
        }

        private static void CheckUnassignedLeafGoals (GoalModel model)
        {
            var unassignedLeafGoals = from g in model.Goals 
                where g.AssignedAgents.Count == 0 & g.Refinements.Count == 0 select g;

            if (unassignedLeafGoals.Count () > 0) {
                Console.WriteLine ("[ KO ] Unassigned leaf goals");
                
                foreach (var item in unassignedLeafGoals) {
                    Console.WriteLine ("       - {0}", item.Name);
                }
            } else {
                Console.WriteLine ("[ OK ] All leaf goals are assigned");
            }
        }
        
        private static void CheckGoalWithSimilarNames (GoalModel model, int levensteinThreshold)
        {
            var duplicateGoals = from g1 in model.Goals 
                where (from g2 in model.Goals where g2 != g1 && g2.Name == g1.Name select g2).Count () > 0 
                select g1;

            if (duplicateGoals.Count () > 0) {
                Console.WriteLine ("[ KO ] Potential duplicated goals exists");

                foreach (var item in duplicateGoals) {
                    Console.WriteLine ("       - {0}", item.Name);
                }
            } else {
                Console.WriteLine ("[ OK ] No potential duplicated goals found");
            }

            var duplicateObstacle = from o1 in model.Obstacles 
                where (from o2 in model.Obstacles where o2 != o1 && o2.Name.LevenshteinDistance (o1.Name) < levensteinThreshold select o2).Count () > 0 
                select o1;

            var displayedDuplicates = new List<Obstacle> ();

            if (duplicateObstacle.Count () > 0) {
                Console.WriteLine ("[ KO ] Potential duplicated obstacles exists");

                foreach (var item in duplicateObstacle) {
                    if (!displayedDuplicates.Contains (item)) {
                        Console.WriteLine ("       - '{0}'", item.Name);
                        var duplicates = from o2 in model.Obstacles where o2 != item && o2.Name.LevenshteinDistance (item.Name) < levensteinThreshold select o2;
                        foreach (var item1 in duplicates) {
                            Console.WriteLine ("         with '{0}'", item1.Name);
                            displayedDuplicates.Add (item1);
                        }
                    }
                }
            } else {
                Console.WriteLine ("[ OK ] No potential duplicated obstacles found");
            }
        }

        static GoalModel BuildModel (string filename)
        {
            var parser = new KAOSTools.Parsing.Parser ();
            return parser.Parse (File.ReadAllText (filename)).GoalModel;
        }

        static void ShowHelp (OptionSet p)
        {
            Console.WriteLine ("Usage: KAOSTools.ModelChecker model");
            Console.WriteLine ();
            Console.WriteLine ("Options:");
            p.WriteOptionDescriptions (Console.Out);
        }
        
        static void PrintError (string error)
        {  
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.Write ("KAOSTools.ModelChecker: ");
            Console.Error.WriteLine (error);
            Console.Error.WriteLine ("Try `KAOSTools.ModelChecker --help' for more information.");
            Console.ResetColor ();
        }
    }
}
