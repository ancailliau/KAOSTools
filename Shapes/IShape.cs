using System;
using Cairo;
using Gtk;
using Model;
using Editor;
using Editor.Model;

namespace Shapes
{
	public interface IShape
	{
		
		PointD Position { get ; set ; }
		
		void Display (Context context, View view);
		
		bool InBoundingBox (double x, double y, out PointD delta);
		
		PointD GetAnchor(PointD point);
		
		IModelElement RepresentedElement {
			get;
			set;
		}
		
		int Depth { get ; set ; }
		
		bool Selected { get ; set ; }
	}
}

