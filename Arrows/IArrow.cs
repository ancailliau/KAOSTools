using System;
using Shapes;
using Editor;
using Cairo;

namespace Arrows
{
	public interface IArrow
	{
		
		IShape Start {
			get;
			set;
		}
		
		IShape End {
			get;
			set;
		}
		
		void Display (Context context, View view);
	}
}

