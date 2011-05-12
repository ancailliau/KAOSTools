using System;
using Cairo;
using Gtk;
using Model;
using Editor;
using Arrows;
using Editor.Model;

namespace Shapes
{
	public class RefinementShape : Shape
	{
		private double radius;
		
		public RefinementShape (string id, Refinement refinement) : base (id)
		{
			XPadding = 4;
			YPadding = 4;
			RepresentedElement = refinement;
			radius = 4;
		}
		
		public override void Display (Context context, View view)
		{
			var oldSource = context.Source;
			
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
			
			// If the refined goal is present as a shape in the view, draw the arrow to.
			IShape refinedShape = null;
			if ((refinedShape = view.GetNearestShapeFor(((Refinement) RepresentedElement).Refined, this.Position)) != null) {
				var arrow = new FilledArrow() {
					Start = this,
					End = refinedShape
				};
				arrow.Display(context, view);
			}
			
			// If refinees are present as shapes, draw the arrows to.
			foreach (var refinee in ((Refinement) RepresentedElement).Refinees) {
				if ((refinedShape = view.GetNearestShapeFor(refinee, this.Position)) != null) {
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

