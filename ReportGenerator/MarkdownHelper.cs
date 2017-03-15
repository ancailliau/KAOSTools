using System;
using KAOSTools.Core;
using System.Text;
using System.Linq;
using anrControls;
using KAOSTools.DotExporter;
using System.IO;
using System.Diagnostics;
using System.Web.Script.Serialization;
using RazorEngine.Text;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;

namespace ReportGenerator.Helpers
{
    public static class MarkdownHelper
    {
        private static Markdown mk = new Markdown();

        public static string CompileMarkdown (string str)
        {
            return mk.Transform (str);
        }
    }
    
    [DataContract]
    public class PartialModelView {
        
        [DataMember]
        public ISet<Goal> Goals;
        
        [DataMember]
        public ISet<GoalRefinement> Refinements;
        
        [DataMember]
        public ISet<Obstacle> Obstacles;
        
        [DataMember]
        public ISet<Obstruction> Obstructions;
        
        [DataMember]
        public ISet<Resolution> Resolutions;
        
        [DataMember]
        public ISet<ObstacleRefinement> ObstacleRefinements;
        
        [DataMember]
        public ISet<Agent> Agents;
        
        [DataMember]
        public ISet<GoalAgentAssignment> Assignments;
        
        [DataMember]
        public ISet<GoalException> GoalExceptions;
        
        [DataMember]
        public ISet<DomainProperty> DomainProperties;
        
        [DataMember]
        public ISet<DomainHypothesis> DomainHypotheses;
        
        public PartialModelView ()
        {
            Goals = new HashSet<Goal> ();
            Refinements = new HashSet<GoalRefinement> ();
            Obstacles = new HashSet<Obstacle> ();
            DomainProperties = new HashSet<DomainProperty> ();
            DomainHypotheses = new HashSet<DomainHypothesis> ();
            Obstructions = new HashSet<Obstruction> ();
            ObstacleRefinements = new HashSet<ObstacleRefinement> ();
            Agents = new HashSet<Agent> ();
            Assignments = new HashSet<GoalAgentAssignment> ();
            Resolutions = new HashSet<Resolution> ();
            GoalExceptions = new HashSet<GoalException> ();
        }
        
        public void Add (Agent agent) {
            Agents.Add (agent);
        }
        
        public void Add (Goal goal) {
            Goals.Add (goal);
        }
        
        public void Add (GoalRefinement refinement) {
            Refinements.Add (refinement);
        }
        
        public void Add (Obstacle obstacle) {
            Obstacles.Add (obstacle);
        }
        
        public void Add (Obstruction obstruction) {
            Obstructions.Add (obstruction);
        }
        
        public void Add (Resolution resolution) {
            Resolutions.Add (resolution);
        }
        
        public void Add (GoalAgentAssignment assignment) {
            Assignments.Add (assignment);
        }
        
        public void Add (ObstacleRefinement refinement) {
            ObstacleRefinements.Add (refinement);
        }
        
        public void Add (GoalException exception) {
            GoalExceptions.Add (exception);
        }
        
        public void Add (DomainProperty agent) {
            DomainProperties.Add (agent);
        }
        
        public void Add (DomainHypothesis agent) {
            DomainHypotheses.Add (agent);
        }
    }

    public static class JSONHelper
    {
        public static IEncodedString GetPartialGoalDiagram (Goal g)
        {
            var view = new PartialModelView ();
            view.Add (g);
            
            foreach (var r in g.ParentRefinements ()) {
                view.Add (r);
                view.Add (r.ParentGoal ());
            }
            
            foreach (var r in g.Refinements()) {
                view.Add (r);
                foreach (var gg in r.SubGoals ()) {
                    view.Add (gg);    
                }
                foreach (var gg in r.DomainProperties ()) {
                    view.Add (gg);    
                }
                foreach (var gg in r.DomainHypotheses ()) {
                    view.Add (gg);    
                }
            }
            
            foreach (var o in g.Obstructions ()) {
                view.Add (o);
                view.Add (o.Obstacle ());
            }
            
            foreach (var a in g.AgentAssignments ()) {
                view.Add (a);
                foreach (var agent in a.Agents ()) {
                    view.Add (agent);    
                }
            }
            
            foreach (var r in g.Resolutions ()) {
                view.Add (r);
                view.Add (r.Obstacle ());
            }
            /*
            foreach (var r in g.Exceptions ()) {
                view.Add (r);
                view.Add (r.Obstacle ());
                view.Add (r.ResolvingGoal ());
            }
            */
            var stream1 = new MemoryStream();
            var ser = new DataContractJsonSerializer(typeof(PartialModelView));
            ser.WriteObject(stream1, view);
            stream1.Position = 0;
            StreamReader sr = new StreamReader(stream1);
            return new RawString (sr.ReadToEnd());
        }
        
        public static IEncodedString GetPartialAgentDiagram (Agent a)
        {
            var view = new PartialModelView ();
            view.Add (a);
            
            foreach (var g in a.AssignedGoals ()) {
                view.Add (g);
                view.Add (g.Goal ());
            }
            
            var stream1 = new MemoryStream();
            var ser = new DataContractJsonSerializer(typeof(PartialModelView));
            ser.WriteObject(stream1, view);
            stream1.Position = 0;
            StreamReader sr = new StreamReader(stream1);
            return new RawString (sr.ReadToEnd());
        }
        
        public static IEncodedString GetPartialObstacleDiagram (Obstacle o)
        {
            var view = new PartialModelView ();
            view.Add (o);
            
            foreach (var r in o.ParentRefinements ()) {
                view.Add (r);
                view.Add (r.ParentObstacle ());
            }
            
            foreach (var r in o.Refinements()) {
                view.Add (r);
                foreach (var gg in r.SubObstacles ()) {
                    view.Add (gg);    
                }
                foreach (var gg in r.DomainProperties ()) {
                    view.Add (gg);    
                }
                foreach (var gg in r.DomainHypotheses ()) {
                    view.Add (gg);    
                }
            }
            
            foreach (var r in o.Obstructions ()) {
                view.Add (r);
                view.Add (r.ObstructedGoal ());
            }
            
            foreach (var r in o.Resolutions ()) {
                view.Add (r);
                view.Add (r.ResolvingGoal ());
            }
            
            var stream1 = new MemoryStream();
            var ser = new DataContractJsonSerializer(typeof(PartialModelView));
            ser.WriteObject(stream1, view);
            stream1.Position = 0;
            StreamReader sr = new StreamReader(stream1);
            return new RawString (sr.ReadToEnd());
        }
        
        public static IEncodedString GetPartialDomPropDiagram (DomainProperty domprop)
        {
            var view = new PartialModelView ();
            view.Add (domprop);
            
            foreach (var r in domprop.GoalRefinements ()) {
                view.Add (r);
                view.Add (r.ParentGoal ());
                foreach (var gg in r.SubGoals ()) {
                    view.Add (gg);    
                }
                foreach (var gg in r.DomainProperties ()) {
                    view.Add (gg);    
                }
                foreach (var gg in r.DomainHypotheses ()) {
                    view.Add (gg);    
                }
            }
            
            foreach (var r in domprop.ObstacleRefinements ()) {
                view.Add (r);
                view.Add (r.ParentObstacle ());
                foreach (var gg in r.SubObstacles ()) {
                    view.Add (gg);    
                }
            }
            
            var stream1 = new MemoryStream();
            var ser = new DataContractJsonSerializer(typeof(PartialModelView));
            ser.WriteObject(stream1, view);
            stream1.Position = 0;
            StreamReader sr = new StreamReader(stream1);
            return new RawString (sr.ReadToEnd());
        }
        
        public static IEncodedString GetPartialDomHypDiagram (DomainHypothesis domhyp)
        {
            var view = new PartialModelView ();
            view.Add (domhyp);
            
            foreach (var r in domhyp.GoalRefinements ()) {
                view.Add (r);
                view.Add (r.ParentGoal ());
                foreach (var gg in r.SubGoals ()) {
                    view.Add (gg);    
                }
                foreach (var gg in r.DomainProperties ()) {
                    view.Add (gg);    
                }
                foreach (var gg in r.DomainHypotheses ()) {
                    view.Add (gg);    
                }
            }
            
            foreach (var r in domhyp.ObstacleRefinements ()) {
                view.Add (r);
                view.Add (r.ParentObstacle ());
                foreach (var gg in r.SubObstacles ()) {
                    view.Add (gg);    
                }
            }
            
            var stream1 = new MemoryStream();
            var ser = new DataContractJsonSerializer(typeof(PartialModelView));
            ser.WriteObject(stream1, view);
            stream1.Position = 0;
            StreamReader sr = new StreamReader(stream1);
            return new RawString (sr.ReadToEnd());
        }
    }

    public static class DotHelper
    {
        public static string GetPartialGoalDiagram (Goal g)
        {
            var sw = new StringWriter ();
            var exporter = new DotExport (g.model, sw);
            exporter.ExportGoal (g, true);

            foreach (var r in g.ParentRefinements ()) {
                foreach (var g2 in r.SubGoals ().Where (c => c != g)) {
                    exporter.ExportGoal (g2);
                }
                exporter.ExportGoal (r.ParentGoal ());
                exporter.ExportRefinement (r.ParentGoal (), r);
            }

            foreach (var r in g.Refinements()) {
                foreach (var g2 in r.SubGoals ()) {
                    exporter.ExportGoal (g2);
                }
                exporter.ExportRefinement (g, r);
            }

            foreach (var r in g.AgentAssignments()) {
                foreach (var g2 in r.Agents ()) {
                    exporter.ExportAgent (g2);
                }
                exporter.ExportAssignment (g, r);
            }

            foreach (var r in g.Obstructions()) {
                exporter.ExportObstacle (r.Obstacle ());
                exporter.ExportObstruction (g, r);
            }

            foreach (var r in g.Resolutions ()) {
                exporter.ExportObstacle (r.Obstacle ());
                exporter.ExportResolution (r.Obstacle (), r);
            }

            exporter.Close ();

            return GetImage (sw.ToString (), LayoutAlgorithm.Dot);
        }

        public static string GetExceptionDiagramSource (Goal g)
        {
            var sw = new StringWriter ();
            var exporter = new DotExport (g.model, sw, ranksep:2.5f, nodesep:2.5f, margin:0);
            exporter.ExportGoal (g, true);

            foreach (var r in g.Exceptions ()) {
                exporter.ExportGoal (r.ResolvingGoal());
                exporter.ExportException (r);
            }

            foreach (var r in g.model.Exceptions().Where (x => x.ResolvingGoalIdentifier == g.Identifier)) {
                exporter.ExportGoal (r.ResolvingGoal());
                exporter.ExportException (r);
            }

            exporter.Close ();

            return sw.ToString ();
        }

        public static string GetExceptionDiagram (Goal g)
        {
            return GetImage (GetExceptionDiagramSource (g), LayoutAlgorithm.Twopi);
        }

        enum LayoutAlgorithm {
            Dot, Twopi, Neato
        }

        static string GetImage (string graph, LayoutAlgorithm a)
        {
            int ExitCode = -1;
            Process Process = new Process ();
            ;
            //Defining the filename of the app
            if (a == LayoutAlgorithm.Dot) 
                Process.StartInfo.FileName = "dot";
            else if (a == LayoutAlgorithm.Twopi) 
                Process.StartInfo.FileName = "twopi";
            else if (a == LayoutAlgorithm.Neato) 
                Process.StartInfo.FileName = "neato";

            //Assigning the args to the filename
            Process.StartInfo.Arguments = @"-Tpng";
            Process.StartInfo.UseShellExecute = false;
            //Set output of program to be written to process output stream
            Process.StartInfo.RedirectStandardOutput = true;
            Process.StartInfo.RedirectStandardInput = true;

            string strOutput = "";
            byte[] result;
            try {
                //Starting the process
                Process.Start ();

                Process.StandardInput.Write (graph);
                Process.StandardInput.Close ();

                // strOutput = Process.StandardOutput.ReadToEnd();
                using (var streamReader = new MemoryStream ()) {
                    Process.StandardOutput.BaseStream.CopyTo (streamReader);
                    result = streamReader.ToArray ();
                }
                //Waiting for the process to exit
                Process.WaitForExit ();
                //Grabbing the exit code
                ExitCode = Process.ExitCode;
                //Close the process
                Process.Close ();
            }
            catch (Exception ex) {
                throw new Exception (ex.Message);
            }
            byte[] bytes = new byte[strOutput.Length * sizeof(char)];
            System.Buffer.BlockCopy (strOutput.ToCharArray (), 0, bytes, 0, bytes.Length);
            return string.Format ("data:image/jpg;base64,{0}", Convert.ToBase64String (result));
        }
    }
}

