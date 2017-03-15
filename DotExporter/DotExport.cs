using System;
using System.Linq;
using KAOSTools.Core;
using System.IO;
using System.Text;

namespace KAOSTools.DotExporter
{
    public class DotExport
    {
        private readonly KAOSModel model;
        private readonly TextWriter writer;

        public DotExport (KAOSModel model, TextWriter writer, 
            float margin = 0.2f, float nodesep = 0.1f, float ranksep = 0.3f)
        {
            this.model = model;
            this.writer = writer;

            writer.WriteLine ("digraph model {\n");
            writer.WriteLine (string.Format ("graph[margin={0}, nodesep={1}, ranksep={2}];", 
                              margin, nodesep, ranksep));
            writer.WriteLine ("edge[dir=back];");
            writer.WriteLine ();
        }

        public void ExportGoal (Goal g, bool bold = false)
        {
            bool assignedToSoftwareAgents = (from assignments in g.AgentAssignments()
                                             from agent in assignments.Agents()
                                             select agent.Type == AgentType.Software).Count () > 0;
            var name = new StringBuilder (g.FriendlyName);
            if (name.Length > 30) {
                var midspace = g.Name.IndexOf (' ', (g.Name.Length / 3) - 1);
                if (midspace > 0) {
                    if (bold) {
                        name.Replace (" ", "<BR/>", midspace, 1);
                    } else {
                        name.Replace (" ", @"\n", midspace, 1);
                    }
                }
            }
            writer.WriteLine (@"""{0}"" [shape=polygon,skew=.05,label={1},style=filled,fillcolor=""{2}"",penwidth={3},fontname=""HelveticaNeue"",fontsize=9,margin=""0,0""];", 
                g.Identifier, bold ? "<<FONT COLOR=\"red\"><B>" + name + "</B></FONT>>" : "\"" + name + "\"", 
                              assignedToSoftwareAgents ? "#fff9c1" : "#d8ebfd", 
                              g.AgentAssignments().Count() > 0 ? 2 : 1);
        }
        
        public void ExportDomainProperty (DomainProperty domprop)
        {
            var name = new StringBuilder (domprop.Name);
            if (name.Length > 30) {
                var midspace = domprop.Name.IndexOf (' ', (domprop.Name.Length / 2) - 1);
                name.Replace (" ", @"\n", midspace, 1);
            }
            writer.WriteLine (@"""{0}"" [shape=trapezium,label=""{1}"",style=filled,fillcolor=""{2}"",fontname=""HelveticaNeue"",fontsize=9,margin=""-.2,0""];", 
                              domprop.Identifier, name, "#e8fdcb");
        }

        public void ExportDomainHypothesis (DomainHypothesis domhyp)
        {
            var name = new StringBuilder (domhyp.Name);
            if (name.Length > 30) {
                var midspace = domhyp.Name.IndexOf (' ', (domhyp.Name.Length / 2) - 1);
                name.Replace (" ", @"\n", midspace, 1);
            }
            writer.WriteLine (@"""{0}"" [shape=trapezium,label=""{1}"",style=filled,fillcolor=""{2}"",fontname=""HelveticaNeue"",fontsize=9,margin=""-.2,0""];", 
                              domhyp.Identifier, name, "#FFEFEF");
        }

        public void ExportObstacle (Obstacle o)
        {
            var name = new StringBuilder (o.Name);
            if (name.Length > 30) {
                var midspace = o.Name.IndexOf (' ', (o.Name.Length / 2) - 1);
                name.Replace (" ", @"\n", midspace, 1);
            }
            writer.WriteLine (@"""{0}"" [shape=polygon,skew=-.1,label=""{1}"",style=filled,fillcolor=""#ffa9ad"",penwidth={2},fontname=""HelveticaNeue"",fontsize=9,margin=""-.2,0""];", 
                              o.Identifier, name, o.Refinements().Count() == 0 ? 2 : 1);
        }

        public void ExportAgent (Agent g)
        {
            writer.WriteLine (@"""{0}"" [shape=hexagon,label=""{1}"",style=filled,fillcolor=""#dcbdfa"",fontname=""HelveticaNeue"",fontsize=9,margin=""0.1,-0.1""];", g.Identifier, g.Name);
        }

        public void ExportAssignment (Goal g, AgentAssignment assignement)
        {
            var tempGUID = Guid.NewGuid().ToString();
            writer.WriteLine (@"""{0}""[shape=circle,width=.1,fixedsize=true,label=""""];", tempGUID);
            writer.WriteLine (@"""{0}"" -> ""{1}"" [arrowtail=onormal, label=""  {2}""];", 
                              g.Identifier,
                              tempGUID,
                              "");


            foreach (var agent in assignement.Agents()) {
                writer.WriteLine (@"""{0}"" -> ""{1}"" [arrowtail=none];", 
                                  tempGUID, 
                    agent.Identifier);

            }
        }

        public void ExportException (GoalException r)
        {
            writer.WriteLine (@"""{0}"" -> ""{1}"" [arrowtail=onormal,label=<<B>Except</B> {2}>,fontname=""HelveticaNeue"",fontsize=7,margin=""0.1,-0.1"",center=true,fillcolor=""#ffffff""];", 
                r.ResolvingGoal().Identifier, 
                r.AnchorGoal().Identifier,
                r.Obstacle().FriendlyName);
        }

        public void ExportObstruction (Goal g, Obstruction o)
        {
            writer.WriteLine (@"""{0}"" -> ""{1}"" [arrowtail=onormaltee];", 
                              g.Identifier, 
                              o.Obstacle ().Identifier);
        }
        
        public void ExportResolution (Obstacle o, Resolution g)
        {
            writer.WriteLine (@"""{0}"" -> ""{1}"" [arrowtail=onormaltee];", 
                              o.Identifier, 
                              g.ResolvingGoal().Identifier);
        }

        public void ExportRefinement (Goal parent, GoalRefinement refinement) {
            var tempGUID = Guid.NewGuid().ToString();
            writer.WriteLine (@"""{0}""[shape=circle,width=.1,fixedsize=true,label=""""];", tempGUID);
            writer.WriteLine (@"""{0}"" -> ""{1}"" [arrowtail=onormal,label=""  {2}""];", 
                              parent.Identifier,
                              tempGUID,
                string.Empty);

            foreach (var child in refinement.SubGoals()) {
                writer.WriteLine (@"""{0}"" -> ""{1}"" [arrowtail=none];", 
                                  tempGUID, 
                                  child.Identifier);
            }

            foreach (var domprop in refinement.DomainProperties()) {
                writer.WriteLine (@"""{0}"" -> ""{1}"" [arrowtail=none];", 
                                  tempGUID, 
                                  domprop.Identifier);
            }

            foreach (var domhyp in refinement.DomainHypotheses()) {
                writer.WriteLine (@"""{0}"" -> ""{1}"" [arrowtail=none];", 
                                  tempGUID, 
                                  domhyp.Identifier);
            }
        }

        public void ExportRefinement (Obstacle parent, ObstacleRefinement refinement) {
            var tempGUID = Guid.NewGuid().ToString();
            writer.WriteLine (@"""{0}""[shape=circle,width=.1,fixedsize=true,label=""""];", tempGUID);
            writer.WriteLine (@"""{0}"" -> ""{1}"" [arrowtail=onormal];", parent.Identifier, tempGUID);

            foreach (var child in refinement.SubObstacles()) {
                writer.WriteLine (@"""{0}"" -> ""{1}"" [arrowtail=none];", tempGUID, child.Identifier);
            }
            
            foreach (var child in refinement.DomainHypotheses()) {
                writer.WriteLine (@"""{0}"" -> ""{1}"" [arrowtail=none];", tempGUID, child.Identifier);
            }

            foreach (var child in refinement.DomainProperties()) {
                writer.WriteLine (@"""{0}"" -> ""{1}"" [arrowtail=none];", tempGUID, child.Identifier);
            }
        }

        public void ExportRefinementRecursively (Obstacle o, bool exportObstacle = false) {

            if (o.Refinements().Count() > 0) {
                writer.WriteLine ();
                writer.WriteLine ("# Refinement for obstacle '{0}'", o.Name);
                writer.WriteLine ();
            }

            if (exportObstacle)
                ExportObstacle (o);
            foreach (var r in o.Refinements()) {
                ExportRefinement (o, r);
                
                foreach (var child in r.SubObstacles()) {
                    ExportRefinementRecursively (child, exportObstacle);
                }
            }
        }

        public void ExportRefinementRecursively (Goal g) {

            if (g.Refinements().Count() > 0) {
                writer.WriteLine ();
                writer.WriteLine ("# Refinement for goal '{0}'", g.Name);
                writer.WriteLine ();
            }

            foreach (var r in g.Refinements()) {
                ExportRefinement (g, r);
                
                foreach (var child in r.SubGoals()) {
                    ExportRefinementRecursively (child);
                }
            }
        }

        public void ExportModel ()
        {
            writer.WriteLine ("## GOALS");
            writer.WriteLine ();

            foreach (var g in model.Goals()) {
                ExportGoal (g);
            }

            
            writer.WriteLine ();
            writer.WriteLine ("## OBSTACLES");
            writer.WriteLine ();

            foreach (var o in model.Obstacles()) {
                ExportObstacle (o);
            }

            
            writer.WriteLine ();
            writer.WriteLine ("## DOMAIN PROPERTIES");
            writer.WriteLine ();
            
            foreach (var d in model.DomainProperties()) {
                ExportDomainProperty (d);
            }

            
            writer.WriteLine ();
            writer.WriteLine ("## RESPONSIBILITY");
            writer.WriteLine ();

            /*
            foreach (var g in model.Goals()) {
                if (g.AgentAssignments().Count() > 0) {
                    foreach (var assignment in g.AgentAssignments()) {
                        foreach (var agent in assignment.Agents()) {
                            ExportResponsibility (g, assignment);
                        }
                    }
                }
            }*/
            
            
            writer.WriteLine ();
            writer.WriteLine ("## REFINEMENTS");

            foreach (var g in model.RootGoals()) {
                ExportRefinementRecursively (g);
            }
            
            
            writer.WriteLine ();
            writer.WriteLine ("## OBSTRUCTIONS");
            /*
            foreach (var g in model.ObstructedGoals()) {
                foreach (var o in g.Obstructions()) {
                    ExportObstruction (g, o.Obstacle());
                }
            }
            */
            
            
            writer.WriteLine ();
            writer.WriteLine ("## OBSTACLE REFINEMENTS");

            foreach (var g in model.ObstructedGoals()) {
                foreach (var o in g.Obstructions()) {
                    ExportRefinementRecursively (o.Obstacle());
                }
            }

            writer.WriteLine ();
            writer.WriteLine ("## RESOLUTIONS");
            
            foreach (var o in model.ResolvedObstacles()) {
                foreach (var resolution in o.Resolutions()) {
                    ExportResolution (o, resolution);
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

