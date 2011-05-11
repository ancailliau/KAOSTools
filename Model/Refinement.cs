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
		
		public string TypeName { get { return "refinement"; } }
		
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
		}
		
		public void Add (IModelElement element) 
		{
			Refinees.Add(element);
		}
		
		
	}
}

