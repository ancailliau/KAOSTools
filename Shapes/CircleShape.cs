using System;
using Cairo;
using Gtk;

namespace Shapes
{
	public class CircleShape : Shape
	{
		private double lastWidth;
		private double lastHeight;
		
		public CircleShape () : base ()
		{
			XPadding = 4;
			YPadding = 4;
		}
		
		public override void Display (Context context, DrawingArea drawingArea)
		{
			var oldSource = context.Source;
			
			var pangoLayout = new Pango.Layout(drawingArea.PangoContext);
			pangoLayout.SetText(this.Label);
			pangoLayout.Alignment = Pango.Alignment.Center;
			
			int textWidth, textHeight;
			pangoLayout.GetPixelSize(out textWidth, out textHeight);
			
			double Width = textWidth + 2 * XPadding;
			double Height = textHeight + 2 * YPadding;
			
			double radius = Math.Max(Width, Height)/2;
			lastWidth = 2 * radius;
			lastHeight = 2 * radius;
			
			context.MoveTo(TopLeft.X + radius * 2, TopLeft.Y + radius);
			context.Arc(TopLeft.X + radius, TopLeft.Y + radius, radius, 0, Math.PI * 2);
			
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
			
			context.MoveTo(TopLeft.X + radius - textWidth/2, TopLeft.Y + radius - textHeight/2);
			Pango.CairoHelper.ShowLayout(context, pangoLayout);
			
			context.Source = oldSource;
		}
		
		
		public override bool InBoundingBox (double x, double y, out PointD delta)
		{
			double centerX = TopLeft.X + lastWidth/2;
			double centerY = TopLeft.Y + lastHeight/2;
			double xx = (x - centerX);
			double yy = (y - centerY);
			if (Math.Sqrt(xx * xx + yy * yy) <= lastWidth/2) {
				delta.X = TopLeft.X - x;
				delta.Y = TopLeft.Y - y;
				return true;
			}
			return false;
		}
	}
}

