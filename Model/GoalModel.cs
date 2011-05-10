using System;
using System.Collections.Generic;

namespace Model
{
	public class GoalModel
	{
		public List<IModelElement> Elements {
			get;
			set;
		}
		
		public GoalModel ()
		{
			Elements = new List<IModelElement>();
		}
		
	}
}

