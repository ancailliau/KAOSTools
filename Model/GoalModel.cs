using System;
using System.Collections.Generic;

namespace Model
{
	public class GoalModel
	{
		public List<Goal> Goals {
			get;
			set;
		}
		
		public GoalModel ()
		{
			Goals = new List<Goal>();
		}
		
		public void AddGoals (IEnumerable<Goal> goals)
		{
			Goals.AddRange(goals);
		}
	}
}

