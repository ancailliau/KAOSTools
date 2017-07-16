using System;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Core.SatisfactionRates;
using UCLouvain.KAOSTools.Propagators;
using UCLouvain.KAOSTools.Propagators.BDD;
using System.IO;
using UCLouvain.KAOSTools.ExpertCombination;
using UCLouvain.ExpertOpinionSharp.Statistics;

namespace UCLouvain.KAOSTools.Utils.UncertaintyPropagator
{
	class UncertaintyPropagatorProgram : KAOSToolCLI
	{
		private static ExpertCombinator expert_combination;

		public static void Main(string[] args)
		{
			Console.WriteLine ("*** This is UncertaintyPropagator from KAOSTools. ***");
            Console.WriteLine ("*** For more information on KAOSTools see <https://github.com/ancailliau/KAOSTools> ***");
            Console.WriteLine ("*** Please report bugs to <https://github.com/ancailliau/KAOSTools/issues> ***");
            Console.WriteLine ();
            Console.WriteLine ("*** Copyright (c) 2017, Université catholique de Louvain ***");
            Console.WriteLine ("");

            string rootname = "root";
            options.Add ("root=", "Specify the root goal for which to compute the satisfaction rate. (Default: root)", v => rootname = v);

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
            
            KAOSCoreElement root = model.Goal (rootname);
            if (root == null) {
				root = model.Obstacle(rootname);
				if (root == null) {
					PrintError("The goal or obstacle '" + rootname + "' was not found.");
				}
            }

            try {
				if (doCombine) {
					expert_combination = new ExpertCombinator (model, combine);
					expert_combination.Combine();
				}
            
                IPropagator _propagator;
				_propagator = new BDDBasedUncertaintyPropagator(model, n_samples);

				SimulatedSatisfactionRate esr = null;
				if (root is Goal g)
					esr = (SimulatedSatisfactionRate) _propagator.GetESR (g);
				else if (root is Obstacle o)
					esr = (SimulatedSatisfactionRate) _propagator.GetESR (o);
            
                Console.WriteLine (root.FriendlyName + ":");
                Console.WriteLine("  Mean: {0:P}", esr.Mean);
				if (root is Goal g2)
				{
					Console.WriteLine("  Required Satisfaction Rate: {0:P}", g2.RDS);
					Console.WriteLine("  Violation Uncertainty: {0:P}", esr.ViolationUncertainty(g2.RDS));
					Console.WriteLine("  Uncertainty Spread: {0}", esr.UncertaintySpread(g2.RDS));
				}

				if (doCombine && combine == ExpertCombinationMethod.Cook) {
					var stats = (CookFrameworkStatistics) expert_combination.GetStatistics ();
					Console.WriteLine();
					Console.WriteLine("-- Information score (Cook):");
					foreach (var item in stats.InformationScores) {
						Console.WriteLine($"  {item.Key} : {item.Value}");
					}
					Console.WriteLine("-- Calibration score (Cook):");
					foreach (var item in stats.CalibrationScores) {
						Console.WriteLine($"  {item.Key} : {item.Value}");
					}
					Console.WriteLine("-- Combination weights (Cook):");
					foreach (var item in stats.Weights) {
						Console.WriteLine($"  {item.Key.Name} : {item.Value}");
					}
				}

				if (!string.IsNullOrEmpty(outfile)) {
					var out_filename = Environment.ExpandEnvironmentVariables(outfile);
					using (var f = File.CreateText(out_filename)) {
						f.WriteLine(root.Identifier);
						f.Write(string.Join("\n", esr.Values));
					}
				}
            
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
