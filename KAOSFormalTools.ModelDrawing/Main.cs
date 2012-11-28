using System;
using System.Linq;
using System.IO;
using KAOSFormalTools.Parsing;
using KAOSFormalTools.Domain;
using NDesk.Options;
using System.Collections.Generic;

namespace KAOSFormalTools.ModelDrawing
{
    class MainClass
    {
        public static void Main (string[] args)
        {
            string modelFilename     = "";

            string refinements       = "";
            string obstructions      = "";
            string responsibility    = "";
            string resolutions       = "";

            bool   exportModel       = false;
            bool   show_help         = false;
            
            var p = new OptionSet () {
                { "o|output=", "File to use to store dot model",
                    v => modelFilename = v },
                { "all", "Export all model (override refinements, obstructions, responsibilities)",
                    v => exportModel = true },
                { "refinements=", "Refinements to output, give name or identifier for parent goal.",
                    v => refinements = v },
                { "obstructions=", "Obstruction trees to output, give name or identifier for obstructed goal.",
                    v => obstructions = v },
                { "responsibilities=", "Responsibilities to output, give name or identifier for goal.",
                    v => responsibility = v },
                { "resolutions=", "Resolutions to output, give name or identifier for obstacle.",
                    v => resolutions = v },
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

            if (r.Count == 0 || string.IsNullOrEmpty (r[0]) || !File.Exists (r[0])) {
                ShowHelp (p);
                return;
            }

            var model = BuildModel (r[0]);
            var exporter = new DotExporter (model, !string.IsNullOrEmpty (modelFilename) ? new StreamWriter (modelFilename) : Console.Out);

            if (exportModel) {
                exporter.ExportModel ();
                exporter.Close ();
                return;
            }

            if (!string.IsNullOrEmpty (refinements)) {
                var fds = refinements.Split (',');
                foreach (var gname in fds) {
                    var name = gname.Trim ();
                    var goals = model.Goals.Where (g => g.Identifier == name);

                    if (goals.Count () == 0) {
                        goals = model.Goals.Where (g => g.Name == name);

                        if (goals.Count () == 0) {
                            PrintError (string.Format ("Could not find goal '{0}'", refinements));
                            return;
                        }
                    }

                    foreach (var g in goals) {
                        exporter.ExportGoal (g);
                        foreach (var refinement in g.Refinements) {
                            exporter.ExportRefinement (g, refinement);
                            foreach (var child in refinement.Children) {
                                exporter.ExportGoal (child);
                                foreach (var agent in child.AssignedAgents) {
                                    exporter.ExportResponsibility (agent, child);
                                }
                            }
                        }
                    }
                }
            } 

            if (!string.IsNullOrEmpty (resolutions)) {
                var resolvedObstacles = resolutions.Split (',');
                foreach (var obstacleName in resolvedObstacles) {
                    var name = obstacleName.Trim ();
                    var obstacles = model.Obstacles.Where (g => g.Name == name);
                    
                    if (obstacles.Count () == 0) {
                        PrintError (string.Format ("Could not find obstacle '{0}'", obstacleName));
                        return;
                    }
                    
                    foreach (var obstacle in obstacles) {
                        exporter.ExportObstacle (obstacle);
                        foreach (var goal in obstacle.Resolutions) {
                            exporter.ExportGoal (goal);
                            exporter.ExportResolution (obstacle, goal);
                        }
                    }
                }
            } 

            if (!string.IsNullOrEmpty (obstructions)) {
                var fds = obstructions.Split (',');
                foreach (var gname in fds) {
                    var name = gname.Trim ();
                    var goals = model.Goals.Where (g => g.Name == name);

                    if (goals.Count () == 0) {
                        PrintError (string.Format ("Could not find goal '{0}'", refinements));
                        return;
                    }

                    foreach (var g in goals) {
                        exporter.ExportGoal (g);
                        foreach (var obstruction in g.Obstruction) {
                            exporter.ExportObstruction (g, obstruction);
                            exporter.ExportRefinementRecursively (obstruction, true);
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty (responsibility)) {
                var fds = responsibility.Split (',');
                foreach (var gname in fds) {
                    var name = gname.Trim ();
                    var goals = model.Goals.Where (g => g.Name == name);

                    if (goals.Count () == 0) {
                        PrintError (string.Format ("Could not find goal '{0}'", refinements));
                        return;
                    }

                    foreach (var g in goals) {
                        exporter.ExportGoal (g);
                        foreach (var agent in g.AssignedAgents) {
                            exporter.ExportResponsibility (agent, g);
                        }
                    }
                }
            }

            exporter.Close ();
        }

        static GoalModel BuildModel (string filename)
        {
            var parser = new KAOSFormalTools.Parsing.Parser ();
            return parser.Parse (File.ReadAllText (filename));
        }

        static void ShowHelp (OptionSet p)
        {
            Console.WriteLine ("Usage: KAOSFormalTools.ModelDrawing MODEL");
            Console.WriteLine ();
            Console.WriteLine ("Options:");
            p.WriteOptionDescriptions (Console.Out);
        }
        
        static void PrintError (string error)
        {  
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.Write ("ltlsharp: ");
            Console.Error.WriteLine (error);
            Console.Error.WriteLine ("Try `KAOSFormalTools.ModelDrawing --help' for more information.");
            Console.ResetColor ();
        }
    }
}
