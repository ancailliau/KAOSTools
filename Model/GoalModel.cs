using System;
using System.Linq;
using System.Collections.Generic;

namespace Model
{
	public class GoalModel
	{
		
		private List<IModelElement> Elements;
		
		public List<Goal> Goals {
			get { return Elements.FindAll(e => e is Goal).ConvertAll<Goal>(t => t as Goal); }
		}
		
		public GoalModel ()
		{
			Elements = new List<IModelElement>();
		}
		
		public void Add (IModelElement element) 
		{
			Elements.Add(element);
		}
		
		public IModelElement Get (string id)
		{
			return Elements.Find(t => t.Id == id);
		}
		
	}
}

