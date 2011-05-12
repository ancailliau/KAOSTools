using System;
using System.Linq;
using System.Collections.Generic;

namespace Model
{
	public class GoalModel
	{
		
		public delegate void ChangedModelHandler (object sender, EventArgs e);
		public event ChangedModelHandler Changed;
		
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
			if (Changed != null) {
				Changed(this, EventArgs.Empty);
			}
		}
		
		public IModelElement Get (string id)
		{
			return Elements.Find(t => t.Id == id);
		}
		
	}
}

