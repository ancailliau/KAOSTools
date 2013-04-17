using System;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using NDesk.Options;
using KAOSTools.MetaModel;
using System.Collections;
using LtlSharp.Utils;
    
namespace KAOSTools.ReportGenerator
{
    class MainClass
    {
        public static void Main (string[] args)
        {
            bool show_help = false;
            bool show_header = true;
            int  level     = 0;
            string title   = "Untitled";
            string authors = "John Doe";
                
            var p = new OptionSet () {
                    { "h|help",  "show this message and exit", 
                        v => show_help = true },
                    { "p|partial",  "Do not output LaTeX headers", 
                        v => show_header = false },
                    { "l=|level=",  "Determines the depth to use for definition title. (0 for section, 1 for subsection, etc.)", 
                        v => level = Int16.Parse (v) },
                    { "a=|authors=",  "Specify the authors of the document", 
                        v => authors = v },
                    { "t=|title=",  "Specify the title of the document", 
                        v => title = v },
                };
                
            List<string> arguments;
            try {
                arguments = p.Parse (args);
                    
            } catch (OptionException e) {
                PrintError (e.Message);
                return;
            }
                
            if (show_help) {
                ShowHelp (p);
                return;
            }
                
            if (arguments.Count == 0) {
                PrintError ("Please provide a file");
                return;
            }
                
            if (arguments.Count > 1) {
                PrintError ("Please provide only one file");
                return;
            }
                
            if (!File.Exists (arguments [0])) {
                PrintError ("File `" + arguments [0] + "` does not exists");
                return;
            }

            section = String.Concat (Enumerable.Repeat("sub", level)) + "section";

            if (show_header) {
                Console.WriteLine ("\\documentclass{article}");
                Console.WriteLine ("\\usepackage{latexsym}");

                Console.WriteLine ("\\declare{document}");

                Console.WriteLine ("\\title{" + title + "}");
                Console.WriteLine ("\\author{" + authors + "}");
                Console.WriteLine ("\\date{" + DateTime.Now.ToString ("d MMMM yyyy") + "}");
                Console.WriteLine ("\\maketitle");
                Console.WriteLine ("\\tableofcontents");
            }

            model = BuildModel (arguments [0]);

            Console.WriteLine ("\\section{Goals}");
            foreach (var goal in model.Goals.OrderBy ( g => g.GetDepth (model) ))
                Display (goal);
            
            Console.WriteLine ("\\section{Obstacles}");
            foreach (var obstacle in model.Obstacles.OrderBy ( o => o.GetDepth (model) ))
                Display (obstacle);
            
            Console.WriteLine ("\\section{Domain properties}");
            foreach (var domprop in model.DomainProperties.OrderBy ( o => o.Name ))
                Display (domprop);
            
            Console.WriteLine ("\\section{Agents}");
            foreach (var agent in model.Agents)
                Display (agent);

            if (show_header) {
                Console.WriteLine ("\\end{document}");
            }
        }
            
        private static GoalModel model;
        private static string section = "section";

        static void Display (Goal g)
        {
            Console.WriteLine ("\\sub{0}{{{1}}}", section, g.Name);

            if (!string.IsNullOrEmpty (g.Definition)) {
                Console.WriteLine ("\\paragraph{Definition :}");
                Console.WriteLine (g.Definition);
            } else {
                Console.WriteLine ("\\textit{Missing definition}");
            }

            if (g.FormalSpec != null) {
                Console.WriteLine ("\\paragraph{Formal specification :}");
                Console.WriteLine ("$$" + new TexToString (g.FormalSpec).String + "$$");
            }

            int alternative = 0;
            foreach (var refinement in g.Refinements) {
                alternative++;

                if (g.Refinements.Count > 1) {
                    Console.WriteLine ("\\paragraph{{Alternative refinement {0}: }}", alternative);
                } else {
                    Console.WriteLine ("\\paragraph{Refinement : }");
                }

                Console.WriteLine ("\\declare{itemize}");
                
                foreach (var goal in refinement.Children)
                    Console.WriteLine ("\\item {0}", goal.Name);
                
                Console.WriteLine ("\\end{itemize}");
            }

            if (g.Obstruction.Count > 0) {
                Console.WriteLine ("\\paragraph{Obstructed by : }");
                Console.WriteLine ("\\declare{itemize}");
                foreach (var obstacle in g.Obstruction) {
                    Console.WriteLine ("\\item {0}", obstacle.Name);
                }
                Console.WriteLine ("\\end{itemize}");
            }
                        
            if (g.AssignedAgents.Count > 0) {
                Console.WriteLine ("\\paragraph{Assigned to : }");
                foreach (var assignment in g.AssignedAgents) {
                    Console.WriteLine ("\\declare{itemize}");
                    foreach (var agent in assignment.Agents)
                        Console.WriteLine ("\\item {0}", agent.Name);
                    Console.WriteLine ("\\end{itemize}");
                }
            }


            Console.WriteLine ();
        }

        static void Display (Obstacle o)
        {
            Console.WriteLine ("\\sub{0}{{{1}}}", section, o.Name);
            
            if (!string.IsNullOrEmpty (o.Definition)) {
                Console.WriteLine ("\\paragraph{Definition :}");
                Console.WriteLine (o.Definition);
            } else {
                Console.WriteLine ("\\textit{Missing definition}");
            }
            
            if (o.FormalSpec != null) {
                Console.WriteLine ("\\paragraph{Formal specification :}");
                Console.WriteLine ("$$" + new TexToString (o.FormalSpec).String + "$$");
            }
                        
            int alternative = 0;
            foreach (var refinement in o.Refinements) {
                alternative++;
                
                if (o.Refinements.Count > 1) {
                    Console.WriteLine ("\\paragraph{{Alternative refinement {0}: }}", alternative);
                } else {
                    Console.WriteLine ("\\paragraph{Refinement : }");
                }
                
                Console.WriteLine ("\\declare{itemize}");
                
                foreach (var goal in refinement.Children)
                    Console.WriteLine ("\\item {0}", goal.Name);
                
                Console.WriteLine ("\\end{itemize}");
            }

            if (o.Resolutions.Count > 0) {
                Console.WriteLine ("\\paragraph{Resolved by : }");
                Console.WriteLine ("\\declare{itemize}");
                foreach (var goal in o.Resolutions) {
                    Console.WriteLine ("\\item {0}", goal.Name);
                }
                Console.WriteLine ("\\end{itemize}");
            }

            Console.WriteLine ();
        }

        static void Display (DomainProperty d)
        {
            Console.WriteLine ("\\sub{0}{{{1}}}", section, d.Name);
            
            if (!string.IsNullOrEmpty (d.Definition)) {
                Console.WriteLine ("\\paragraph{Definition :}");
                Console.WriteLine (d.Definition);
            } else {
                Console.WriteLine ("\\textit{Missing definition}");
            }
           
            if (d.FormalSpec != null) {
                Console.WriteLine ("\\paragraph{Formal specification :}");
                Console.WriteLine ("$$" + new TexToString (d.FormalSpec).String + "$$");
            }

            Console.WriteLine ();
        }

        static void Display (Agent a)
        {
            Console.WriteLine ("\\sub{0}{{{1}" + (a.Type == AgentType.Software ? " (software)" : "") + "}}", section, a.Name);

            if (!string.IsNullOrEmpty (a.Description)) {
                Console.WriteLine ("\\paragraph{Description :}");
                Console.WriteLine (a.Description);
            } else {
                Console.WriteLine ("\\textit{Missing description}");
            }
            
            Console.WriteLine ("\\paragraph{Responsibilities :}");
            Console.WriteLine ("\\declare{itemize}");            
            foreach (var goal in model.Goals.Where (g => g.AssignedAgents.SelectMany(x => x.Agents).Where (a2 => a2.Name == a.Name).Count() > 0))
                Console.WriteLine ("\\item {0}", goal.Name);
            Console.WriteLine ("\\end{itemize}");

            Console.WriteLine ();
        }

        static GoalModel BuildModel (string filename)
        {
            var parser = new KAOSTools.Parsing.Parser ();
            return parser.Parse (File.ReadAllText (filename)).GoalModel;
        }
            
        static void ShowHelp (OptionSet p)
        {
            Console.WriteLine ("Usage: " + System.AppDomain.CurrentDomain.FriendlyName + " model");
            Console.WriteLine ();
            Console.WriteLine ("Options:");
            p.WriteOptionDescriptions (Console.Out);
        }
            
        static void PrintError (string error)
        {  
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.Write (System.AppDomain.CurrentDomain.FriendlyName + " : ");
            Console.Error.WriteLine (error);
            Console.Error.WriteLine ("Try `" + System.AppDomain.CurrentDomain.FriendlyName + " --help' for more information.");
            Console.ResetColor ();
        }
    }

    public static class GaolExtensions {
        public static int GetDepth (this Goal goal, GoalModel model)
        {
            return (from parentGoals in model.Goals
                    where parentGoals.Refinements.SelectMany ( g => g.Children ).Contains (goal)
                    select parentGoals.GetDepth (model)).OrderBy (x => x).FirstOrDefault();
        }

        public static int GetDepth (this Obstacle obstacle, GoalModel model)
        {
            return (from parentObstacle in model.Obstacles
                    where parentObstacle.Refinements.SelectMany ( g => g.Children ).Contains (obstacle)
                    select parentObstacle.GetDepth (model)).OrderBy (x => x).FirstOrDefault();
        }
    }
}
