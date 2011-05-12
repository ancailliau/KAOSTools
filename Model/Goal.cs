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
		
		public override bool Equals (object obj)
		{
			if (obj == null)
				return false;
			if (ReferenceEquals (this, obj))
				return true;
			if (obj.GetType () != typeof(Goal))
				return false;
			Model.Goal other = (Model.Goal)obj;
			return Id == other.Id;
		}


		public override int GetHashCode ()
		{
			unchecked {
				return (Id != null ? Id.GetHashCode () : 0);
			}
		}

	}
}

