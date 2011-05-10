using System;

namespace Model
{
	public interface IModelElement
	{
		
		string Id {
			get;
			set;
		}
		
		string TypeName {
			get;
		}
		
	}
}

