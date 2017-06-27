using System;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Monitoring;

namespace UCLouvain.KAOSTools.Utils.Monitor
{
	class MainClass : KAOSToolCLI
	{
		public static void Main(string[] args)
		{
            Console.WriteLine ("*** This is Monitor from KAOSTools. ***");
            Console.WriteLine ("*** For more information on KAOSTools see <https://github.com/ancailliau/KAOSTools> ***");
            Console.WriteLine ("*** Please report bugs to <https://github.com/ancailliau/KAOSTools/issues> ***");
            Console.WriteLine ();
            Console.WriteLine ("*** Copyright (c) 2017, Université catholique de Louvain ***");
            Console.WriteLine ("");

            string rootname = "root";
            options.Add ("root=", "Specify the root goal for which to compute the satisfaction rate. (Default: root)", v => rootname = v);

			Init (args);
            
            var root = model.Goal (rootname);
            if (root == null) {
                PrintError ("The goal '"+rootname+"' was not found");
            }

            try {

				var modelMonitor = new ModelMonitor(model);
				
				Console.WriteLine($"Obstacles monitored: {modelMonitor.MonitoredObstacleCount}");
				
				Console.Write(">");
				Console.ReadLine();

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
