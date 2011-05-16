using System;
using System.Collections.Generic;

namespace Model
{
	public class Agent : IModelElement
	{
		public string Id {
			get;
			set;
		}
		
		public string Name {
			get;
			set;
		}
		
		public string TypeName { get { return "agent"; } }
		
		
		public Agent ()
		{
			Id = Guid.NewGuid().ToString();
		}
		
		public Agent (string name) 
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
			if (obj.GetType () != typeof(Agent))
				return false;
			Model.Agent other = (Model.Agent)obj;
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

