using System;
using Cairo;

namespace Shapes
{
	public abstract class Shape : IShape
	{
		public string Label {
			get;
			set;
		}
		
		public PointD TopLeft {
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
		
		public Shape ()
		{
			Label = "";
			TopLeft = new PointD(0,0);
			BorderColor = new Color(0,0,0);
			BackgroundColor = new Color(1,1,1);
		}

		public abstract void Display (Context context, Gtk.DrawingArea drawingArea);
		public abstract bool InBoundingBox (double x, double y, out PointD delta);
	}
}

