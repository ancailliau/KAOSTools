using System;
using Model;
using System.Collections.Generic;

namespace Model
{
	public class Refinement : IModelElement
	{
		
		public string Id {
			get;
			set;
		}		
		
		public string Name { get; set; }
		
		public IModelElement Refined {
			get;
			set;
		}
		
		public List<IModelElement> Refinees {
			get;
			set;
		}
		
		
		public Refinement ()
		{
			Refinees = new List<IModelElement>();
			Id = Guid.NewGuid().ToString();
		}
		
		public void Add (IModelElement element) 
		{
			Refinees.Add(element);
		}
		
		public override bool Equals (object obj)
		{
			if (obj == null)
				return false;
			if (ReferenceEquals (this, obj))
				return true;
			if (obj.GetType () != typeof(Refinement))
				return false;
			Model.Refinement other = (Model.Refinement)obj;
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

