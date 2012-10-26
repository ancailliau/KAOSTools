using System;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using NDesk.Options;
using KAOSFormalTools.Domain;
using System.Collections;
    
namespace KAOSFormalTools.LatexExporter
{
    class MainClass
    {
        public static void Main (string[] args)
        {
            bool show_help = false;
            bool show_header = true;
            int  level     = 0;
                
            var p = new OptionSet () {
                    { "h|help",  "show this message and exit", 
                        v => show_help = true },
                    { "p|partial",  "Do not output LaTeX headers", 
                        v => show_header = false },
                    { "l=|level=",  "Determines the depth to use for definition title. (0 for section, 1 for subsection, etc.)", 
                        v => level = Int16.Parse (v) },
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
                Console.WriteLine ("\\begin{document}");
            }

            model = BuildModel (arguments [0]);
            foreach (var goal in model.Goals.OrderBy ( g => g.GetDepth (model) ))
                DisplayGoal (goal);

            if (show_header) {
                Console.WriteLine ("\\end{document}");
            }
        }
            
        private static GoalModel model;
        private static string section = "section";

        static void DisplayGoal (Goal g)
        {
            Console.WriteLine ("\\{0}{{{1}}}", section, g.Name);

            if (!string.IsNullOrEmpty (g.Definition)) {
                Console.WriteLine ("\\paragraph{Definition :}");
                Console.WriteLine (g.Definition);
            } else {
                Console.WriteLine ("\\textit{Missing definition}");
            }

            int alternative = 0;
            foreach (var refinement in g.Refinements) {
                alternative++;

                if (g.Refinements.Count > 1) {
                    Console.WriteLine ("\\paragraph{{Alternative refinement {0}: }}", alternative);
                } else {
                    Console.WriteLine ("\\paragraph{Refinement : }");
                }

                Console.WriteLine ("\\begin{itemize}");
                
                foreach (var goal in refinement.Children)
                    Console.WriteLine ("\\item {0}", goal.Name);
                
                Console.WriteLine ("\\end{itemize}");
            }

            Console.WriteLine ();
        }
            
        static GoalModel BuildModel (string filename)
        {
            var parser = new KAOSFormalTools.Parsing.Parser ();
            return parser.Parse (File.ReadAllText (filename));
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
    }
}
