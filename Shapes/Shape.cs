using System;
using Cairo;
using Model;
using Editor;

namespace Shapes
{
	public abstract class Shape : IShape
	{
		
		public PointD Position {
			get;
			set;
		}
				
		protected Color BorderColor {
			get;
			set;
		}
		
		protected Color BackgroundColor {
			get;
			set;
		}
		
		protected double XPadding {
			get;
			set;
		}
		
		protected double YPadding {
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
		
		public string Id {
			get;
			set;
		}
		
		public bool Selected { get ; set ; }
		
		public Shape (string id)
		{
			Id = id;
			Position = new PointD(0,0);
			BorderColor = new Color(0,0,0);
			BackgroundColor = new Color(1,1,1);
		}

		public abstract void Display (Context context, View view);
		public abstract bool InBoundingBox (double x, double y, out PointD delta);
		public abstract PointD GetAnchor (PointD point);
	}
}

