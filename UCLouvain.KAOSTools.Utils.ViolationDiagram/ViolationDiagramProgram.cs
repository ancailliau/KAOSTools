using System;
using UCLouvain.KAOSTools.ExpertCombination;
using UCLouvain.KAOSTools.CriticalObstacles;
using UCLouvain.KAOSTools.Core;
using System.IO;
using System.Linq;
using System.Collections;

namespace UCLouvain.KAOSTools.Utils.ViolationDiagram
{
	class ViolationDiagramProgram : KAOSToolCLI
	{
		public static void Main(string[] args)
		{
			Console.WriteLine ("*** This is ViolationDiagram from KAOSTools. ***");
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
			
			Init(args);
			
			try {
			
				Goal root = model.Goal (rootname);
	            if (root == null) {
					PrintError("The goal or obstacle '" + rootname + "' was not found.");
	            }

				if (doCombine) {
					var expert_combination = new ExpertCombinator (model, combine);
					expert_combination.Combine();
				}

				var criticalobstacle = new CriticalUncertainObstacles(model, root);
				var scores = criticalobstacle.GetObstacleScores();

				foreach (var item in scores) {
					Console.WriteLine("{0}: us = {1}, vu = {2}",
									  string.Join(",", item.Key.Select(x => x.FriendlyName)),
									  item.Value.UncertaintySpread,
									  item.Value.ViolationUncertainty);
				}

				if (!string.IsNullOrEmpty(outfile)) {
					var out_filename = Environment.ExpandEnvironmentVariables(outfile);

					if (out_filename.StartsWith("~", StringComparison.Ordinal)) {
						var homepath = Environment.GetEnvironmentVariable("HOME");
						out_filename = out_filename.Replace("~", homepath);
					}
					
					using (var f = File.CreateText(out_filename)) {
						f.WriteLine("combination,uncertainty_spread,violation_uncertainty");
						foreach (var item in scores) {
							f.WriteLine("{0},{1},{2}",
											  string.Join(":", item.Key.Select(x => x.Name)),
											  item.Value.UncertaintySpread,
											  item.Value.ViolationUncertainty);
						}
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
