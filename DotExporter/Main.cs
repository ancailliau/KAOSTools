using System;
using System.Linq;
using System.IO;
using KAOSTools.Parsing;
using KAOSTools.MetaModel;
using NDesk.Options;
using System.Collections.Generic;
using KAOSTools.Utils;

namespace KAOSTools.DotExporter
{
    class MainClass : KAOSToolCLI
    {
        public static void Main (string[] args)
        {
            string modelFilename     = "";

            string refinements       = "";
            string obstructions      = "";
            string responsibility    = "";
            string resolutions       = "";

            bool   exportModel       = false;
            
            options.Add ("o|output=", "File to use to store dot model",
                         v => modelFilename = v );
            options.Add ("all", "Export all model (override refinements, obstructions, responsibilities)",
                         v => exportModel = true  );
            options.Add ("refinements=", "Refinements to output, give name or identifier for parent goal.",
                         v => refinements = v  );
            options.Add ("obstructions=", "Obstruction trees to output, give name or identifier for obstructed goal.",
                         v => obstructions = v  );
            options.Add ("responsibilities=", "Responsibilities to output, give name or identifier for goal.",
                         v => responsibility = v  );
            options.Add ("resolutions=", "Resolutions to output, give name or identifier for obstacle.",
                         v => resolutions = v  );

            Init (args);

            var exporter = new DotExport (model.GoalModel, !string.IsNullOrEmpty (modelFilename) ? new StreamWriter (modelFilename) : Console.Out);

            if (exportModel) {
                exporter.ExportModel ();
                exporter.Close ();
                return;
            }

            if (!string.IsNullOrEmpty (refinements)) {
                var fds = refinements.Split (',');
                foreach (var gname in fds) {
                    var name = gname.Trim ();
                    var goals = model.GoalModel.Goals.Where (g => g.Identifier == name);

                    if (goals.Count () == 0) {
                        goals = model.GoalModel.Goals.Where (g => g.Name == name);

                        if (goals.Count () == 0) {
                            PrintError (string.Format ("Could not find goal '{0}'", refinements));
                            return;
                        }
                    }

                    foreach (var g in goals) {
                        exporter.ExportGoal (g);
                        foreach (var refinement in g.Refinements) {
                            exporter.ExportRefinement (g, refinement);
                            foreach (var child in refinement.Subgoals) {
                                exporter.ExportGoal (child);
                                foreach (var assignment in child.AgentAssignments) {
                                    foreach (var agent in assignment.Agents) {
                                        exporter.ExportResponsibility (agent, child);
                                    }
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
                    var obstacles = model.GoalModel.Obstacles.Where (g => g.Name == name);
                    
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
                    var goals = model.GoalModel.Goals.Where (g => g.Name == name);

                    if (goals.Count () == 0) {
                        PrintError (string.Format ("Could not find goal '{0}'", refinements));
                        return;
                    }

                    foreach (var g in goals) {
                        exporter.ExportGoal (g);
                        foreach (var obstruction in g.Obstructions) {
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
                    var goals = model.GoalModel.Goals.Where (g => g.Name == name);

                    if (goals.Count () == 0) {
                        PrintError (string.Format ("Could not find goal '{0}'", refinements));
                        return;
                    }

                    foreach (var g in goals) {
                        exporter.ExportGoal (g);
                        foreach (var assignment in g.AgentAssignments) {
                            foreach (var agent in assignment.Agents) {
                                exporter.ExportResponsibility (agent, g);
                            }
                        }
                    }
                }
            }

            exporter.Close ();
        }
    }
}
