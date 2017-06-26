using System;
using System.Linq;
using System.Text.RegularExpressions;
using KAOSTools.Core;
using KAOSTools.Utils;
using UCLouvain.KAOSTools.Core.SatisfactionRates;
using UCLouvain.KAOSTools.Integrators;
using UCLouvain.KAOSTools.Propagators;
using UCLouvain.KAOSTools.Propagators.BDD;
using System.IO;
using UCLouvain.KAOSTools.Utils.FileExporter;
using UCLouvain.KAOSTools.Optimizer;
using System.Diagnostics;

namespace UCLouvain.KAOSTools.Utils.CMSelector
{
    class MainClass : KAOSToolCLI
    {
        public static void Main (string [] args)
        {
            Console.WriteLine ("*** This is CMSelector from KAOSTools. ***");
            Console.WriteLine ("*** For more information on KAOSTools see <https://github.com/ancailliau/KAOSTools> ***");
            Console.WriteLine ("*** Please report bugs to <https://github.com/ancailliau/KAOSTools/issues> ***");
            Console.WriteLine ();
            Console.WriteLine ("*** Copyright (c) 2017, Université catholique de Louvain ***");
            Console.WriteLine ("");

            string rootname = "root";
            options.Add ("root=", "Specify the root goal for which the selection shall be optimized. (Default: root)", v => rootname = v);

            Init (args);
            
            var root = model.Goal (rootname);
            if (root == null) {
                PrintError ("The goal '"+rootname+"' was not found");
            }

            try {
                var optimizer = new NaiveCountermeasureSelectionOptimizer (model);
                var propagator = new BDDBasedResolutionPropagator (model);
                
                var sr = (DoubleSatisfactionRate) propagator.GetESR (root);
                Console.WriteLine ("Satisfaction Rate without countermeasures: " + sr.SatisfactionRate);
                Console.WriteLine ("Required Satisfaction Rate: " + root.RDS);
            
                var timer_minimal_cost = Stopwatch.StartNew ();
                var minimalCost = optimizer.GetMinimalCost (root, propagator);
                timer_minimal_cost.Stop ();
                Console.WriteLine ("Minimal cost to guarantee RSR: " + minimalCost);

                Stopwatch timer_optimization = null;
                if (minimalCost >= 0) {
                    timer_optimization = Stopwatch.StartNew ();
                    var optimalSelections = optimizer.GetOptimalSelections (minimalCost, root, propagator);
                    timer_optimization.Stop ();
                    
                    if (optimalSelections.Count () == 0) {
                        Console.WriteLine ("Optimal selections: No countermeasure to select.");
                        Console.WriteLine ();
                    } else {
                        Console.WriteLine ("Optimal selections:");
                        foreach (var o in optimalSelections) {
                            Console.WriteLine ("* " + o);
                        }
                    }
                }
                
                Console.WriteLine ();
                Console.WriteLine ("--- Statistics ---");
                Console.WriteLine ("Number of resolutions: " + model.Resolutions ().Count ());
                Console.WriteLine ("Number of resolution combination: " + Math.Pow (2, model.Resolutions ().Count ()));
                Console.WriteLine ("Time to compute minimal cost: " + timer_minimal_cost.Elapsed);
                if (timer_optimization != null)
                    Console.WriteLine ("Time to compute optimal selections: " + timer_optimization.Elapsed);
              
            } catch (Exception e) {
                PrintError ("An error occured during the computation. ("+e.Message+").\n"
                            +"Please report this error to <https://github.com/ancailliau/KAOSTools/issues>.\n"
                            +"----------------------------\n"
                            +e.StackTrace
                            +"\n----------------------------\n");
                
            }
            
        }
    }
}
