using System;
using Cairo;
using Model;
using Editor;

namespace Shapes
{
	public abstract class Shape : IShape
	{
		public string Label {
			get;
			set;
		}
		
		public PointD Position {
			get;
			set;
		}
				
		public Color BorderColor {
			get;
			set;
		}
		
		public Color BackgroundColor {
			get;
			set;
		}
		
		public double XPadding {
			get;
			set;
		}
		
		public double YPadding {
			get;
			set;
		}
		
		public IModelElement RepresentedElement {
			get;
			set;
		}
		
		public int Depth {
			get;
			set;
		}
		
		public Shape ()
		{
			Label = "";
			Position = new PointD(0,0);
			BorderColor = new Color(0,0,0);
			BackgroundColor = new Color(1,1,1);
		}

		public abstract void Display (Context context, View view);
		public abstract bool InBoundingBox (double x, double y, out PointD delta);
		public abstract PointD GetAnchor (PointD point);
	}
}

