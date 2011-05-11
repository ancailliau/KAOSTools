using System;
using Cairo;
using Gtk;
using Model;
using Editor;
using Arrows;

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
		
		public override void Display (Context context, View view)
		{
			var drawingArea = view.DrawingArea;
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
			
			// If the refined goal is present as a shape in the view, draw the arrow to.
			IShape refinedShape = null;
			if ((refinedShape = view.ContainsShapeFor(((Refinement) RepresentedElement).Refined)) != null) {
				var arrow = new FilledArrow() {
					Start = this,
					End = refinedShape
				};
				arrow.Display(context, view);
			}
			
			// If refinees are present as shapes, draw the arrows to.
			foreach (var refinee in ((Refinement) RepresentedElement).Refinees) {
				if ((refinedShape = view.ContainsShapeFor(refinee)) != null) {
					var arrow = new Arrows.Arrow() {
						Start = refinedShape,
						End = this
					};
					arrow.Display(context, view);
				}
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

