using System;
using System.Linq;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Integrators;

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
            Console.WriteLine();
			Console.WriteLine("Contexts: " + model.Contexts().Count());
			
			var integrator = new SoftResolutionIntegrator (model);
			foreach (var resolution in model.Resolutions())
			{
				integrator.Integrate(resolution);
			}
			
            Console.WriteLine();
			Console.WriteLine("Generated goal exceptions: " + model.Exceptions().Count());
			Console.WriteLine(" (distributed over " + model.Exceptions().Select(x => x.AnchorGoalIdentifier).Distinct().Count() + " goals)");
			Console.WriteLine("Generated provided assumption: " + model.ObstacleAssumptions().Count());
			Console.WriteLine(" (distributed over " + model.ObstacleAssumptions().Select(x => x.AnchorGoalIdentifier).Distinct().Count() + " goals)");
		}
	}
}
