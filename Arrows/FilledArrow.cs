using System;
using Shapes;
using Cairo;
using Gtk;
using Editor;

namespace Arrows
{
	public class FilledArrow
	{
		
		public IShape Start {
			get;
			set;
		}
		
		public IShape End {
			get;
			set;
		}
		
		public Color StrokeColor {
			get;
			set;
		}
		
		public Color FillColor {
			get;
			set;
		}
		
		private double arrowHalfAngle;
		private double arrowSideWith;
		
		public FilledArrow ()
		{
			arrowHalfAngle = Math.Atan2(5, 10);
			arrowSideWith = Math.Sqrt(125);
		}
		
		public void Display (Context context, View view)
		{
			var drawingArea = view.DrawingArea;
			var startPosition = Start.GetAnchor(End.Position);
			var endPosition = End.GetAnchor(Start.Position);
			
			var oldSource = context.Source;
			
			context.SetSourceRGB(StrokeColor.R,
				StrokeColor.G,
				StrokeColor.B);
			
			context.MoveTo(startPosition);
			context.LineTo(endPosition);
			
			context.Stroke();
			
			context.MoveTo(endPosition);
			
			double xx = startPosition.X - endPosition.X;
			double yy = startPosition.Y - endPosition.Y;
			double alpha = Math.Atan2(yy, xx);
			
			double gamma = alpha - arrowHalfAngle;
			context.LineTo(endPosition.X + arrowSideWith * Math.Cos(gamma), endPosition.Y + arrowSideWith * Math.Sin(gamma));
			
			double gamma2 = alpha + arrowHalfAngle;
			context.LineTo(endPosition.X + arrowSideWith * Math.Cos(gamma2), endPosition.Y + arrowSideWith * Math.Sin(gamma2));
			
			context.Fill();
			
			context.Source = oldSource;
		}
		
	}
}

