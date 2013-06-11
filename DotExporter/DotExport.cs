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
            bool assignedToSoftwareAgents = (from a in g.AgentAssignments.SelectMany (x => x.Agents) select a.Type == AgentType.Software ).Count () > 0;
            var name = new StringBuilder (g.FriendlyName);
            if (name.Length > 30) {
                var midspace = g.Name.IndexOf (' ', (g.Name.Length / 3) - 1);
                if (midspace > 0)
                    name.Replace (" ", @"\n", midspace, 1);
            }
            writer.WriteLine (@"""{0}"" [shape=polygon,skew=.1,label=""{1}"",style=filled,fillcolor=""{2}"",penwidth={3},fontname=""Arial"",fontsize=10,margin=""-.2,0""];", 
                              g.Identifier, name, 
                              assignedToSoftwareAgents ? "#fff9c1" : "#d8ebfd", 
                              g.AgentAssignments.Count > 0 ? 2 : 1);
        }
        
        public void ExportDomainProperty (DomainProperty domprop)
        {
            var name = new StringBuilder (domprop.Name);
            if (name.Length > 30) {
                var midspace = domprop.Name.IndexOf (' ', (domprop.Name.Length / 2) - 1);
                name.Replace (" ", @"\n", midspace, 1);
            }
            writer.WriteLine (@"""{0}"" [shape=trapezium,label=""{1}"",style=filled,fillcolor=""{2}"",fontname=""Arial"",fontsize=10,margin=""-.2,0""];", 
                              domprop.Identifier, name, "#e8fdcb");
        }

        public void ExportDomainHypothesis (DomainHypothesis domhyp)
        {
            var name = new StringBuilder (domhyp.Name);
            if (name.Length > 30) {
                var midspace = domhyp.Name.IndexOf (' ', (domhyp.Name.Length / 2) - 1);
                name.Replace (" ", @"\n", midspace, 1);
            }
            writer.WriteLine (@"""{0}"" [shape=trapezium,label=""{1}"",style=filled,fillcolor=""{2}"",fontname=""Arial"",fontsize=10,margin=""-.2,0""];", 
                              domhyp.Identifier, name, "#FFEFEF");
        }

        public void ExportObstacle (Obstacle o)
        {
            var name = new StringBuilder (o.Name);
            if (name.Length > 30) {
                var midspace = o.Name.IndexOf (' ', (o.Name.Length / 2) - 1);
                name.Replace (" ", @"\n", midspace, 1);
            }
            writer.WriteLine (@"""{0}"" [shape=polygon,skew=-.1,label=""{1}"",style=filled,fillcolor=""#ffa9ad"",penwidth={2},fontname=""Arial"",fontsize=10,margin=""-.2,0""];", 
                              o.Identifier, name, o.Refinements.Count == 0 ? 2 : 1);
        }

        public void ExportResponsibility (Goal g, AgentAssignment assignement)
        {
            var tempGUID = Guid.NewGuid().ToString();
            writer.WriteLine (@"""{0}""[shape=circle,width=.1,fixedsize=true,label=""""];", tempGUID);
            writer.WriteLine (@"""{0}"" -> ""{1}"" [arrowtail=onormal, label=""  {2}""];", 
                              g.Identifier,
                              tempGUID,
                              g.InSystems.SetEquals(model.RootSystems) ? "" : string.Join (", ", assignement.InSystems.Select (x => x.FriendlyName)));


            foreach (var agent in assignement.Agents) {
                var tempGUID2 = Guid.NewGuid ().ToString ();
                // agent shape
                writer.WriteLine (@"""{0}"" [shape=hexagon,label=""{1}"",style=filled,fillcolor=""#dcbdfa"",fontname=""Arial"",fontsize=10,margin=""0.1,-0.1""];", tempGUID2, agent.Name);
                writer.WriteLine (@"""{0}"" -> ""{1}"" [arrowtail=none];", 
                                  tempGUID, 
                                  tempGUID2);

            }
        }

        public void ExportObstruction (Goal g, Obstacle o)
        {
            writer.WriteLine (@"""{0}"" -> ""{1}"" [arrowtail=onormaltee];", 
                              g.Identifier, 
                              o.Identifier);
        }
        
        public void ExportResolution (Obstacle o, Goal g)
        {
            writer.WriteLine (@"""{0}"" -> ""{1}"" [arrowtail=onormaltee];", 
                              o.Identifier, 
                              g.Identifier);
        }

        public void ExportRefinement (Goal parent, GoalRefinement refinement) {
            var tempGUID = Guid.NewGuid().ToString();
            writer.WriteLine (@"""{0}""[shape=circle,width=.1,fixedsize=true,label=""""];", tempGUID);
            writer.WriteLine (@"""{0}"" -> ""{1}"" [arrowtail=onormal,label=""  {2}""];", 
                              parent.Identifier,
                              tempGUID,
                              refinement.SystemReference != null ? refinement.SystemReference.FriendlyName : "");

            foreach (var child in refinement.Subgoals) {
                writer.WriteLine (@"""{0}"" -> ""{1}"" [arrowtail=none];", 
                                  tempGUID, 
                                  child.Identifier);
            }

            foreach (var domprop in refinement.DomainProperties) {
                writer.WriteLine (@"""{0}"" -> ""{1}"" [arrowtail=none];", 
                                  tempGUID, 
                                  domprop.Identifier);
            }

            foreach (var domhyp in refinement.DomainHypotheses) {
                writer.WriteLine (@"""{0}"" -> ""{1}"" [arrowtail=none];", 
                                  tempGUID, 
                                  domhyp.Identifier);
            }
        }

        public void ExportRefinement (Obstacle parent, ObstacleRefinement refinement) {
            var tempGUID = Guid.NewGuid().ToString();
            writer.WriteLine (@"""{0}""[shape=circle,width=.1,fixedsize=true,label=""""];", tempGUID);
            writer.WriteLine (@"""{0}"" -> ""{1}"" [arrowtail=onormal];", parent.Identifier, tempGUID);

            foreach (var child in refinement.Subobstacles) {
                writer.WriteLine (@"""{0}"" -> ""{1}"" [arrowtail=none];", tempGUID, child.Identifier);
            }
            
            foreach (var child in refinement.DomainHypotheses) {
                writer.WriteLine (@"""{0}"" -> ""{1}"" [arrowtail=none];", tempGUID, child.Identifier);
            }

            foreach (var child in refinement.DomainProperties) {
                writer.WriteLine (@"""{0}"" -> ""{1}"" [arrowtail=none];", tempGUID, child.Identifier);
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
                
                foreach (var child in r.Subobstacles) {
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
                
                foreach (var child in r.Subgoals) {
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
                if (g.AgentAssignments.Count > 0) {
                    foreach (var assignment in g.AgentAssignments) {
                        foreach (var agent in assignment.Agents) {
                            ExportResponsibility (g, assignment);
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
                foreach (var o in g.Obstructions) {
                    ExportObstruction (g, o);
                }
            }
            
            
            writer.WriteLine ();
            writer.WriteLine ("## OBSTACLE REFINEMENTS");

            foreach (var g in model.ObstructedGoals) {
                foreach (var o in g.Obstructions) {
                    ExportRefinementRecursively (o);
                }
            }

            writer.WriteLine ();
            writer.WriteLine ("## RESOLUTIONS");
            
            foreach (var o in model.ResolvedObstacles) {
                foreach (var resolution in o.Resolutions) {
                    ExportResolution (o, resolution.ResolvingGoal);
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

