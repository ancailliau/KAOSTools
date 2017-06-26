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
            
                var minimalCost = optimizer.GetMinimalCost (root, propagator);
                Console.WriteLine ("Minimal cost to guarantee RSR: " + minimalCost);

                if (minimalCost >= 0) {
                    var optimalSelections = optimizer.GetOptimalSelections (minimalCost, root, propagator);
                    
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

                var stat = optimizer.GetStatistics ();
                Console.WriteLine ();
                Console.WriteLine ("--- Statistics ---");
                Console.WriteLine ("Number of resolutions: " + stat.NbResolution);
                Console.WriteLine ("Number of possible selections: " + stat.NbSelections);
                Console.WriteLine ("Number of safe selections: " + stat.NbSafeSelections);
                Console.WriteLine ("Number of tested selections (for minimal cost): " + stat.NbTestedSelections);
                Console.WriteLine ("Number of tested selections (for optimal selection): " + stat.NbTestedSelectionsForOptimality);
                Console.WriteLine ("Maximal safe cost: " + stat.MaxSafeCost);
                Console.WriteLine ("Time to compute minimal cost: " + stat.TimeToComputeMinimalCost);
                Console.WriteLine ("Time to compute optimal selections: " + stat.TimeToComputeMinimalCost);
              
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
