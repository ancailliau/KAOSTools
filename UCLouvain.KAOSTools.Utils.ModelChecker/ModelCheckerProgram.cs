using System;
using System.Linq;
using UCLouvain.KAOSTools.Core;

namespace UCLouvain.KAOSTools.Utils.ModelChecker
{
	class ModelCheckerProgram : KAOSToolCLI
	{
		public static void Main(string[] args)
		{
			Console.WriteLine ("*** This is ModelChecker from KAOSTools. ***");
            Console.WriteLine ("*** For more information on KAOSTools see <https://github.com/ancailliau/KAOSTools> ***");
            Console.WriteLine ("*** Please report bugs to <https://github.com/ancailliau/KAOSTools/issues> ***");
            Console.WriteLine ();
            Console.WriteLine ("*** Copyright (c) 2017, Université catholique de Louvain ***");
            Console.WriteLine ("");

            Init (args);
            
            Console.WriteLine("Goals: " + model.Goals().Count());
            Console.WriteLine("Root goals: " + model.RootGoals().Count());
            Console.WriteLine("Leaf goals: " + model.LeafGoals().Count());
            Console.WriteLine("Goal refinements: " + model.GoalRefinements().Count());
            Console.WriteLine();
			Console.WriteLine("Obstacles: " + model.Obstacles().Count());
            Console.WriteLine("Root obstacles: " + model.RootObstacles().Count());
            Console.WriteLine("Leaf obstacles: " + model.LeafObstacles().Count());
            Console.WriteLine("Obstacle refinements: " + model.ObstacleRefinements().Count());
            Console.WriteLine();
			Console.WriteLine("Resolutions: " + model.Resolutions().Count());
		}
	}
}
