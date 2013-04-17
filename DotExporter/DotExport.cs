using System;
using System.Linq;
using KAOSTools.MetaModel;
using System.IO;
using System.Text;

namespace KAOSTools.DotExporter
{
    public class DotExport
    {
        private readonly GoalModel model;
        private readonly TextWriter writer;

        public DotExport (GoalModel model, TextWriter writer)
        {
            this.model = model;
            this.writer = writer;

            writer.WriteLine ("digraph model {\n");
            writer.WriteLine ("graph[margin=0.2, nodesep=0.1, ranksep=0.3];");
            writer.WriteLine ("edge[dir=back];");
            writer.WriteLine ();
        }

        public void ExportGoal (Goal g)
        {
            bool assignedToSoftwareAgents = (from a in g.AssignedAgents.SelectMany (x => x.Agents) select a.Type == AgentType.Software ).Count () > 0;
            var name = new StringBuilder (g.Name);
            if (name.Length > 30) {
                var midspace = g.Name.IndexOf (' ', (g.Name.Length / 2) - 1);
                name.Replace (" ", @"\n", midspace, 1);
            }
            writer.WriteLine (@"""{0}"" [shape=polygon,skew=.1,label=""{1}"",style=filled,fillcolor=""{2}"",penwidth={3},fontname=""Arial"",fontsize=10,margin=""-.2,0""];", string.IsNullOrEmpty (g.Identifier) ? g.Name : g.Identifier, name, assignedToSoftwareAgents ? "#fff9c1" : "#d8ebfd", g.AssignedAgents.Count > 0 ? 2 : 1);
        }
        
        public void ExportDomainProperty (DomainProperty domprop)
        {
            var name = new StringBuilder (domprop.Name);
            if (name.Length > 30) {
                var midspace = domprop.Name.IndexOf (' ', (domprop.Name.Length / 2) - 1);
                name.Replace (" ", @"\n", midspace, 1);
            }
            writer.WriteLine (@"""{0}"" [shape=trapezium,label=""{1}"",style=filled,fillcolor=""{2}"",fontname=""Arial"",fontsize=10,margin=""-.2,0""];", string.IsNullOrEmpty (domprop.Identifier) ? domprop.Name : domprop.Identifier, name, "#e8fdcb");
        }

        public void ExportObstacle (Obstacle o)
        {
            var name = new StringBuilder (o.Name);
            if (name.Length > 30) {
                var midspace = o.Name.IndexOf (' ', (o.Name.Length / 2) - 1);
                name.Replace (" ", @"\n", midspace, 1);
            }
            writer.WriteLine (@"""{0}"" [shape=polygon,skew=-.1,label=""{1}"",style=filled,fillcolor=""#ffa9ad"",penwidth={2},fontname=""Arial"",fontsize=10,margin=""-.2,0""];", string.IsNullOrEmpty (o.Identifier) ? o.Name : o.Identifier, name, o.Refinements.Count == 0 ? 2 : 1);
        }

        public void ExportResponsibility (Agent agent, Goal g)
        {
            var tempGUID = Guid.NewGuid ().ToString ();
            writer.WriteLine (@"""{0}"" [shape=hexagon,label=""{1}"",style=filled,fillcolor=""#dcbdfa"",fontname=""Arial"",fontsize=10,margin=""0.2,0""];", tempGUID, agent.Name);
            writer.WriteLine (@"""{0}"" -> ""{1}"";", string.IsNullOrEmpty (g.Identifier) ? g.Name : g.Identifier, tempGUID);
        }

        public void ExportObstruction (Goal g, Obstacle o)
        {
            writer.WriteLine (@"""{0}"" -> ""{1}"" [arrowtail=onormaltee];", 
                              string.IsNullOrEmpty (g.Identifier) ? g.Name : g.Identifier, 
                              string.IsNullOrEmpty (o.Identifier) ? o.Name : o.Identifier);
        }
        
        public void ExportResolution (Obstacle o, Goal g)
        {
            writer.WriteLine (@"""{0}"" -> ""{1}"" [arrowtail=onormaltee];", 
                              string.IsNullOrEmpty (o.Identifier) ? o.Name : o.Identifier, 
                              string.IsNullOrEmpty (g.Identifier) ? g.Name : g.Identifier);
        }

        public void ExportRefinement (Goal parent, GoalRefinement refinement) {
            var tempGUID = Guid.NewGuid().ToString();
            writer.WriteLine (@"""{0}""[shape=circle,width=.1,fixedsize=true,label=""""];", tempGUID);
            writer.WriteLine (@"""{0}"" -> ""{1}"" [arrowtail=onormal];", 
                              string.IsNullOrEmpty (parent.Identifier) ? parent.Name : parent.Identifier,
                              tempGUID);

            foreach (var child in refinement.Children) {
                writer.WriteLine (@"""{0}"" -> ""{1}"" [arrowtail=none];", 
                                  tempGUID, 
                                  string.IsNullOrEmpty (child.Identifier) ? child.Name : child.Identifier);
            }

            foreach (var domprop in refinement.DomainProperties) {
                writer.WriteLine (@"""{0}"" -> ""{1}"" [arrowtail=none];", 
                                  tempGUID, 
                                  string.IsNullOrEmpty (domprop.Identifier) ? domprop.Name : domprop.Identifier);
            }
        }

        public void ExportRefinement (Obstacle parent, ObstacleRefinement refinement) {
            var tempGUID = Guid.NewGuid().ToString();
            writer.WriteLine (@"""{0}""[shape=circle,width=.1,fixedsize=true,label=""""];", tempGUID);
            writer.WriteLine (@"""{0}"" -> ""{1}"" [arrowtail=onormal];", 
                              string.IsNullOrEmpty (parent.Identifier) ? parent.Name : parent.Identifier,
                              tempGUID);

            foreach (var child in refinement.Children) {
                writer.WriteLine (@"""{0}"" -> ""{1}"" [arrowtail=none];", 
                                  tempGUID, 
                                  string.IsNullOrEmpty (child.Identifier) ? child.Name : child.Identifier);
            }
        }

        public void ExportRefinementRecursively (Obstacle o, bool exportObstacle = false) {

            if (o.Refinements.Count > 0) {
                writer.WriteLine ();
                writer.WriteLine ("# Refinement for obstacle '{0}'", o.Name);
                writer.WriteLine ();
            }

            if (exportObstacle)
                ExportObstacle (o);
            foreach (var r in o.Refinements) {
                ExportRefinement (o, r);
                
                foreach (var child in r.Children) {
                    ExportRefinementRecursively (child, exportObstacle);
                }
            }
        }

        public void ExportRefinementRecursively (Goal g) {

            if (g.Refinements.Count > 0) {
                writer.WriteLine ();
                writer.WriteLine ("# Refinement for goal '{0}'", g.Name);
                writer.WriteLine ();
            }

            foreach (var r in g.Refinements) {
                ExportRefinement (g, r);
                
                foreach (var child in r.Children) {
                    ExportRefinementRecursively (child);
                }
            }
        }

        public void ExportModel ()
        {
            writer.WriteLine ("## GOALS");
            writer.WriteLine ();

            foreach (var g in model.Goals) {
                ExportGoal (g);
            }

            
            writer.WriteLine ();
            writer.WriteLine ("## OBSTACLES");
            writer.WriteLine ();

            foreach (var o in model.Obstacles) {
                ExportObstacle (o);
            }

            
            writer.WriteLine ();
            writer.WriteLine ("## DOMAIN PROPERTIES");
            writer.WriteLine ();
            
            foreach (var d in model.DomainProperties) {
                ExportDomainProperty (d);
            }

            
            writer.WriteLine ();
            writer.WriteLine ("## RESPONSIBILITY");
            writer.WriteLine ();

            foreach (var g in model.Goals) {
                if (g.AssignedAgents.Count > 0) {
                    foreach (var assignment in g.AssignedAgents) {
                        foreach (var agent in assignment.Agents) {
                            ExportResponsibility (agent, g);
                        }
                    }
                }
            }
            
            
            writer.WriteLine ();
            writer.WriteLine ("## REFINEMENTS");

            foreach (var g in model.RootGoals) {
                ExportRefinementRecursively (g);
            }
            
            
            writer.WriteLine ();
            writer.WriteLine ("## OBSTRUCTIONS");

            foreach (var g in model.ObstructedGoals) {
                foreach (var o in g.Obstruction) {
                    ExportObstruction (g, o);
                }
            }
            
            
            writer.WriteLine ();
            writer.WriteLine ("## OBSTACLE REFINEMENTS");

            foreach (var g in model.ObstructedGoals) {
                foreach (var o in g.Obstruction) {
                    ExportRefinementRecursively (o);
                }
            }

            writer.WriteLine ();
            writer.WriteLine ("## RESOLUTIONS");
            
            foreach (var o in model.ResolvedObstacles) {
                foreach (var g in o.Resolutions) {
                    ExportResolution (o, g);
                }
            }

            writer.WriteLine ("## END");
            writer.WriteLine ();
        }

        public void Close () {
            writer.WriteLine ();
            writer.WriteLine ("}");

            writer.Close ();
        }
    }
}

