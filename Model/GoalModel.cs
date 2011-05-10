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
		
		public void Add (IModelElement element) 
		{
			if (element is Goal) {
				Goals.Add(element as Goal);
			}
		}
		
	}
}

