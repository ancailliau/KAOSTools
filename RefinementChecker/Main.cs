using System;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using NDesk.Options;
using KAOSTools.MetaModel;
using LtlSharp;
using KAOSTools.RefinementChecker;
using KAOSTools.Utils;

namespace KAOSTools.RefinementChecker
{
    class MainClass : KAOSToolCLI
    {
        public static void Main (string[] args)
        {
            bool show_help = false;
            string nusmvModel = "";
            string nusmvOutput = "";
            string obstacles = null;
            bool verbose = false;

            options.Add ("w|write=", "Write a NuSMV model in specified file",
                         v => nusmvOutput = v);
            options.Add ("r|read=", "Read a NuSMV output and display a report",
                         v => nusmvModel = v);
            options.Add ("o|obstacles=", "List of obstacles",
                         v => obstacles = v);
            options.Add ("v|verbose", "Verbose mode",
                         v => verbose = true);
            options.Add ("h|help", "show this message and exit", 
                         v => show_help = true);
            Init (args);

            try {
                ProofObligationGenerator generator;

                if (obstacles != null) {
                    var g = new List<Goal> ();
                    var obstructedGoalNames = obstacles.Split (',');
                    foreach (var untrimmedName in obstructedGoalNames) {
                        var name = untrimmedName.Trim ();
                        var goals = model.Goals.Where (x => x.Name == name);
                        g.AddRange (goals);
                    }

                    generator = new ProofObligationGenerator (model, null, g);
                } else {
                    generator = new ProofObligationGenerator (model);
                }

                if (!string.IsNullOrEmpty (nusmvModel)) {
                    model.InterpretNuSMVOutput (nusmvModel, generator, verbose);

                } else {
                    model.WriteNuSMVModel (nusmvOutput, generator);
                }
            } catch (Exception e) {
                PrintError (e.Message);
            }
        }
    }
}
