using System;
using Cairo;
using Editor;
using KaosEditor.Model;

namespace Arrows
{
	public class FilledArrow : Arrow
	{
		
		public string FillColor {
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
			base.Display(context, view);
			
			var drawingArea = view.DrawingArea;
			var startPosition = Start.GetAnchor(End.Position);
			var endPosition = End.GetAnchor(Start.Position);
			
			var oldSource = context.Source;
			
			context.MoveTo(endPosition);
			
			double xx = startPosition.X - endPosition.X;
			double yy = startPosition.Y - endPosition.Y;
			double alpha = Math.Atan2(yy, xx);
			
			double gamma = alpha - arrowHalfAngle;
			context.LineTo(endPosition.X + arrowSideWith * Math.Cos(gamma), endPosition.Y + arrowSideWith * Math.Sin(gamma));
			
			double gamma2 = alpha + arrowHalfAngle;
			context.LineTo(endPosition.X + arrowSideWith * Math.Cos(gamma2), endPosition.Y + arrowSideWith * Math.Sin(gamma2));
			context.ClosePath();
			
			context.SetColor ("#000");
			context.LineWidth = 1.5;
			context.StrokePreserve();
			context.LineWidth = 1;
			
			context.SetColor (FillColor);
			context.Fill();
			
			context.Source = oldSource;
		}
		
	}
}

