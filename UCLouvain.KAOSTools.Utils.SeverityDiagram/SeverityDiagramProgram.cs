using System;
using System.IO;
using System.Linq;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.CriticalObstacles;

namespace UCLouvain.KAOSTools.Utils.SeverityDiagram
{
	class SeverityDiagramProgram : KAOSToolCLI
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

			string outfile = null;
			options.Add("outfile=", "Specify the file to export the raw values, in csv.", v => outfile = v);
			
			int combination_size = 1;
			options.Add("c|combination_size=", "Specify the size of the combination to generate.", v => combination_size = int.Parse(v));

			Init(args);
			
			try {
			
				Goal root = model.Goal (rootname);
	            if (root == null) {
					PrintError("The goal '" + rootname + "' was not found.");
	            }

				var o = new Obstacle(model);
				o.Resolutions().Select(x => x.ResolvingGoal());
				
	            
				var criticalobstacle = new SingleValueCriticalObstacles (model, root);
				var scores = criticalobstacle.GetObstacleScores(combination_size);

				foreach (var item in scores) {
					Console.WriteLine("{0}: vs = {1}",
									  string.Join(",", item.Key.Select(x => x.FriendlyName)),
									  item.Value.ViolationSeverity);
				}

				if (!string.IsNullOrEmpty(outfile)) {
					var out_filename = Environment.ExpandEnvironmentVariables(outfile);

					if (out_filename.StartsWith("~", StringComparison.Ordinal)) {
						var homepath = Environment.GetEnvironmentVariable("HOME");
						out_filename = out_filename.Replace("~", homepath);
					}
					
					using (var f = File.CreateText(out_filename)) {
						f.WriteLine("combination,combination_probability,violation_severity");
						foreach (var item in scores) {
							f.WriteLine("{0},{2},{1}",
											  string.Join(":", item.Key.Select(x => x.Name)),
											  item.Value.ViolationSeverity,
											  item.Value.CombinationProbability);
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
