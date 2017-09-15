using System;
using NUnit.Framework;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Propagators.BDD;

namespace UCLouvain.KAOSTools.Propagators.Tests
{
    [TestFixture ()]
	public class TestBDDBasedWithResolutions
	{
		[Test()]
		public void TestBDD ()
        {
			var model = new KAOSModel();
			var pg = new Goal(model, "pg");
			var sg1 = new Goal(model, "sg1");
			var sg2 = new Goal(model, "sg2");
			var cm = new Goal(model, "cm");
			
			model.Add(pg);
			model.Add(sg1);
			model.Add(sg2);
			model.Add(cm);

			var o1 = new Obstacle(model, "o1");
			var o2 = new Obstacle(model, "o2");
			var o3 = new Obstacle(model, "o3");

			model.Add(o1);
			model.Add(o2);
			model.Add(o3);

			var obs1 = new Obstruction(model) { 
				ObstacleIdentifier = "o1", 
				ObstructedGoalIdentifier = "sg1"
			};
			
			var obs2 = new Obstruction(model) { 
				ObstacleIdentifier = "o2", 
				ObstructedGoalIdentifier = "sg2"
			};
			
			var obs3 = new Obstruction(model) {
				ObstacleIdentifier = "o3",
				ObstructedGoalIdentifier = "cm"
			};
			
			model.Add(obs1);
			model.Add(obs2);
			model.Add(obs3);

			var except = new GoalException(model)
			{
				Identifier = "e_1",
				ResolvedObstacleIdentifier = o2.Identifier,
				ResolvingGoalIdentifier = cm.Identifier,
				AnchorGoalIdentifier = pg.Identifier
			};
			model.Add(except);

			var r = new GoalRefinement(model);
			r.SetParentGoal(pg);
			r.Add(sg1);
			r.Add(sg2);
			model.Add(r);
			
			var p1 = new ObstructionResolutionSuperset (pg);
			Console.WriteLine(p1.ToDot());
		}
	}
}
