using System;
using Cairo;
using Gtk;
using Model;

namespace Shapes
{
	public class RefinementShape : Shape
	{
		private double radius;
		
		public RefinementShape (Refinement refinement) : base ()
		{
			XPadding = 4;
			YPadding = 4;
			RepresentedElement = refinement;
		}
		
		public override void Display (Context context, DrawingArea drawingArea)
		{
			var oldSource = context.Source;
			
			var pangoLayout = new Pango.Layout(drawingArea.PangoContext);
			pangoLayout.SetText(this.Label);
			pangoLayout.Alignment = Pango.Alignment.Center;
			
			int textWidth, textHeight;
			pangoLayout.GetPixelSize(out textWidth, out textHeight);
			
			double width = textWidth + 2 * XPadding;
			double height = textHeight + 2 * YPadding;			
			if (Label != "") {
				radius = Math.Max(width, height)/2;
			} else {
				radius = 4;
			}
						
			context.MoveTo(Position.X + radius, Position.Y);
			context.Arc(Position.X, Position.Y, radius, 0, Math.PI * 2);
			
			context.SetSourceRGBA(BackgroundColor.R,
				BackgroundColor.G,
				BackgroundColor.B,
				BackgroundColor.A);
			context.FillPreserve();
			
			context.SetSourceRGBA(BorderColor.R,
				BorderColor.G,
				BorderColor.B,
				BorderColor.A);
			context.Stroke();
			
			if (Label != "") {
				context.MoveTo(Position.X - textWidth/2, Position.Y - textHeight/2);
				Pango.CairoHelper.ShowLayout(context, pangoLayout);
			}
			
			context.Source = oldSource;
		}
		
		
		public override bool InBoundingBox (double x, double y, out PointD delta)
		{
			double centerX = Position.X;
			double centerY = Position.Y;
			double xx = (x - centerX);
			double yy = (y - centerY);
			if (Math.Sqrt(xx * xx + yy * yy) <= radius) {
				delta.X = Position.X - x;
				delta.Y = Position.Y - y;
				return true;
			}
			return false;
		}
		
		public override PointD GetAnchor (PointD point)
		{
			// Compute the angle between point and center
			double angle = Math.Atan2(Position.Y - point.Y, Position.X - point.X);
			return new PointD(Position.X - radius * Math.Cos(angle), Position.Y - radius * Math.Sin(angle));
		}
		
	}
}

