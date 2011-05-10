using System;
using System.Collections.Generic;

namespace Model
{
	public class Goal : IModelElement
	{
		public string Id {
			get;
			set;
		}
		
		public string Name {
			get;
			set;
		}
		
		public string TypeName { get { return "goal"; } }
		
		public List<Refinement> Refinements {
			get;
			set;
		}
		
		public Goal ()
		{
			Refinements = new List<Refinement>();
		}
		
		public Goal (string name) 
			: this()
		{
			Name = name;
		}
	}
}

