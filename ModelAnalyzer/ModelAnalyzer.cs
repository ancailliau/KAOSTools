using System;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using NDesk.Options;
using KAOSTools.MetaModel;
using KAOSTools.Utils;

namespace KAOSTools.ModelAnalyzer
{
    class ModelAnalyzer : KAOSToolCLI
    {
        private static bool show_formal = false;
        private static bool verbose = false;
        private static int levensteinThreshold = 0;

        public static void Main (string[] args)
        {
            options.Add ("f|formal",  "Check formal attributes", v => show_formal = true);
            options.Add ("v|verbose",  "Display more details about checks", v => verbose = true);
            options.Add ("l|levenstein=",  "Set threshold for duplicate detection", 
                    v => { 
                        int.TryParse (v, out levensteinThreshold);
            });

            Init (args);

            CheckImplicit (model.GoalModel);
            CheckUnassignedLeafGoals (model.GoalModel);
            CheckGoalWithSimilarNames (model.GoalModel, levensteinThreshold);
            CheckMissingDefinition (model.GoalModel);
            CheckIncompatibleSystems (model.GoalModel);

            if (show_formal)
                DisplayMissingFormalSpec (model.GoalModel);
        }

        private static void CheckImplicit (GoalModel model) 
        {
            foreach (var g in model.Goals) {
                if (g.Implicit) {
                    WriteWarning (GetGoalReferenceString (g) + " is implicitely declared.");
                }
            }
        }

        private static void CheckIncompatibleSystems (GoalModel model)
        {
            foreach (var g in model.Goals) {
                if (g.InSystems.Count() == 0) {
                    WriteKO (GetGoalReferenceString(g) + " is no alternative system.");
                }
            }

            foreach (var g in model.Goals.Where (x => x.AgentAssignments.Count() > 0)) {
                foreach (var ag in g.AgentAssignments) {
                    if (ag.InSystems.Count() == 0) {
                        WriteKO (GetGoalReferenceString(g) + " has a an incompatible agent assignment with '" + string.Join (",", ag.Agents.Select(x => x.Name)) + "'.");
                    }
                }
            }
        }

        private static void CheckMissingDefinition (GoalModel model)
        {
            var goals = model.Goals.Where (g => string.IsNullOrWhiteSpace (g.Definition));
            if (goals.Count () == 0) {
                WriteOK ("All goals have definition");

            } else {
                foreach (var goal in goals) {
                    WriteKO (GetGoalReferenceString(goal) + " is missing definition");
                }
            }

            var obstacles = model.Obstacles.Where (o => string.IsNullOrWhiteSpace (o.Definition));
            if (obstacles.Count () == 0) {
                WriteOK ("All obstacles have definition");
                
            } else {
                foreach (var obstacle in obstacles) 
                    WriteKO (string.Format ("Obstacle '{0}' is missing definition", obstacle.Name));
            }

            var domprops = model.DomainProperties.Where (d => string.IsNullOrWhiteSpace (d.Definition));
            if (domprops.Count () == 0) {
                WriteOK ("All domain properties have definition");
                    
            } else {
                foreach (var domprop in domprops) 
                    WriteKO (string.Format ("Domain property '{0}' is missing definition", domprop.Name));
            }

            var agents = model.Agents.Where (a => string.IsNullOrWhiteSpace (a.Description));
            if (agents.Count () == 0) {
                WriteOK ("All agents have description");
                        
            } else {
                foreach (var agent in agents) 
                    WriteKO (string.Format ("Agent '{0}' is missing description", agent.Name));
            }
        }

        private static void DisplayMissingFormalSpec (GoalModel model)
        {
            var goals = from g in model.Goals where g.FormalSpec == null select g;
            foreach (var goal in goals) {
                Console.WriteLine (string.Format ("Goal '{0}' is missing formal specification", goal.Name));
            }

            var domprops = from d in model.DomainProperties where d.FormalSpec == null select d;
            foreach (var domprop in domprops) {
                Console.WriteLine (string.Format ("Domain property '{0}' is missing formal specification", domprop.Name));
            }

            var obstacles = from o in model.Obstacles where o.FormalSpec == null select o;
            foreach (var obstacle in obstacles) {
                Console.WriteLine (string.Format ("Obstacle '{0}' is missing formal specification", obstacle.Name));
            }
        }

        private static void CheckUnassignedLeafGoals (GoalModel model)
        {
            var unassignedLeafGoals = from g in model.Goals 
                where g.AgentAssignments.Count == 0 & g.Refinements.Count == 0 select g;

            if (unassignedLeafGoals.Count () > 0) {
                WriteKO ("Unassigned leaf goals");
                
                foreach (var item in unassignedLeafGoals) {
                    Console.WriteLine ("       - {0}", GetGoalReferenceString(item));
                }
            } else {
                WriteOK ("All leaf goals are assigned");
            }
        }
        
        private static void CheckGoalWithSimilarNames (GoalModel model, int levensteinThreshold)
        {
            var duplicateGoals = from g1 in model.Goals 
                where (from g2 in model.Goals where g2 != g1 && g2.Name == g1.Name select g2).Count () > 0 
                select g1;

            if (duplicateGoals.Count () > 0) {
                WriteKO ("Potential duplicated goals exists");

                foreach (var item in duplicateGoals) {
                    var declaration = declarations[item].First();
                    Console.WriteLine ("       - {0} ({1}:{2},{3})", item.Name, declaration.Filename, declaration.Line, declaration.Col);
                }
            } else {
                WriteOK ("No potential duplicated goals found");
            }

            var duplicateObstacle = from o1 in model.Obstacles 
                where (from o2 in model.Obstacles where o2 != o1 && o2.Name.LevenshteinDistance (o1.Name) < levensteinThreshold select o2).Count () > 0 
                select o1;

            var displayedDuplicates = new List<Obstacle> ();

            if (duplicateObstacle.Count () > 0) {
                WriteKO ("Potential duplicated obstacles exists");

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
                WriteOK ("No potential duplicated obstacles found");
            }
        }

        private static void WriteOK (string message) {
            if (verbose) {
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.DarkGreen;
                Console.Write ("[ OK ]");
                Console.ResetColor ();
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine (" {0}", message);
                Console.ResetColor ();
            }
        }

        private static void WriteKO (string message) {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Red;
            Console.Write ("[ KO ]");
            Console.ResetColor ();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine (" {0}", message);
            Console.ResetColor ();
        }

        private static void WriteWarning (string message) {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.Write ("[WARN]");
            Console.ResetColor ();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine (" {0}", message);
            Console.ResetColor ();
        }

        private static string GetGoalReferenceString (Goal goal) 
        {
            var declaration = declarations[goal].First();
            var currentDirectory = new Uri(Directory.GetCurrentDirectory ()+"/");
            var uri = new Uri(Path.GetFullPath (Path.Combine (Path.GetDirectoryName (filename), declaration.Filename)));

            return (string.Format ("Goal '{0}' ({1}:{2},{3})", 
                                    string.IsNullOrEmpty(goal.Name) ? goal.Identifier : goal.Name, 
                                    currentDirectory.MakeRelativeUri (uri), 
                                    declaration.Line, 
                                    declaration.Col));
        }

    }
}
