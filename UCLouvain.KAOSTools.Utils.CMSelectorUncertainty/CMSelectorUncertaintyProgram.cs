using System;
using System.Linq;
using System.Text.RegularExpressions;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Utils;
using UCLouvain.KAOSTools.Core.SatisfactionRates;
using UCLouvain.KAOSTools.Integrators;
using UCLouvain.KAOSTools.Propagators;
using UCLouvain.KAOSTools.Propagators.BDD;
using System.IO;
using UCLouvain.KAOSTools.Optimizer;
using System.Diagnostics;
using System.Collections.Generic;
using UCLouvain.KAOSTools.ExpertCombination;

namespace UCLouvain.KAOSTools.Utils.CMSelectorUncertainty
{
    class CMSelectorUncertaintyProgram : KAOSToolCLI
    {
        public static void Main (string [] args)
        {
            Console.WriteLine ("*** This is CMSelectorUncertainty from KAOSTools. ***");
            Console.WriteLine ("*** For more information on KAOSTools see <https://github.com/ancailliau/KAOSTools> ***");
            Console.WriteLine ("*** Please report bugs to <https://github.com/ancailliau/KAOSTools/issues> ***");
            Console.WriteLine ();
            Console.WriteLine ("*** Copyright (c) 2017, Université catholique de Louvain ***");
            Console.WriteLine ("");

            string rootname = "root";
            options.Add ("root=", "Specify the root goal for which the selection shall be optimized. (Default: root)", v => rootname = v);

			int n_samples = 1000;
			options.Add("n_samples=",
						$"Specify the number of samples to use to perform Monte-Carlo simulation. (Default: {n_samples})", 
						v => n_samples = int.Parse(v));

			string outfile = null;
			options.Add("outfile=", "Specify the file to export the raw values, in csv.", v => outfile = v);

			bool doCombine = false;
			var combine = ExpertCombinationMethod.MendelSheridan;
			options.Add("combine=", "Combine the estimates from the experts using the specified method.", v => {
				doCombine = true;
				if (v.Equals("cook"))
					combine = ExpertCombinationMethod.Cook;
				else if (v.Equals("mendel-sheridan") | v.Equals("mendelsheridan") | v.Equals("ms"))
					combine = ExpertCombinationMethod.MendelSheridan;
				else {
					PrintError($"Combination method '{v}' is not known");
					Environment.Exit(1);
				}
			});

            Init (args);
            
            var root = model.Goal (rootname);
            if (root == null) {
                PrintError ("The goal '"+rootname+"' was not found");
            }

            try {

				if (doCombine) {
					var expert_combination = new ExpertCombinator (model, combine);
					expert_combination.Combine();
				}
				
                var optimizer = new NaiveCountermeasureSelectionOptimizerWithUncertainty (model, .05);
                var propagator = new BDDBasedUncertaintyPropagator (model, n_samples);
                
                var sr = (SimulatedSatisfactionRate) propagator.GetESR (root);
                Console.WriteLine ("Violation Uncertainty without countermeasures: " + sr.ViolationUncertainty(root.RDS));
                Console.WriteLine ("Uncertainty Spread without countermeasures: " + sr.UncertaintySpread(root.RDS));
                Console.WriteLine ("Required Satisfaction Rate: " + root.RDS);
            
                var minimalCost = optimizer.GetMinimalCost (root, propagator);
                Console.WriteLine ("Minimal cost to guarantee RSR: " + minimalCost);

                if (minimalCost >= 0) {
                    var optimalSelections = optimizer.GetOptimalSelections (minimalCost, root, propagator);
                    
                    if (optimalSelections.Count () == 0) {
                        Console.WriteLine ("Optimal selections: No countermeasure to select.");
                        Console.WriteLine ();
                    } else {
                        Console.WriteLine ($"Optimal selections ({optimalSelections.Count ()}):");
                        foreach (var o in optimalSelections.Distinct ()) {
                            Console.WriteLine ("* {0} (vu={1}, us={2})", o, o.ViolationUncertainty, o.UncertaintySpread);
                        }
                    }
                } else {
                    var optimalSelections = optimizer.GetOptimalSelections (minimalCost, root, propagator);
                        Console.WriteLine ($"The best that can be achived ({optimalSelections.Count ()}):");
                        foreach (var o in optimalSelections.Distinct ()) {
                            Console.WriteLine ("* {0} (vu={1}, us={2})", o, o.ViolationUncertainty, o.UncertaintySpread);
                        }
                }

                var stat = optimizer.GetStatistics ();
                Console.WriteLine ();
                Console.WriteLine ("--- Statistics ---");
                Console.WriteLine ("Number of countermeasure goals: " + stat.NbResolvingGoals);
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
