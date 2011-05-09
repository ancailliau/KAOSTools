using System;
using Cairo;
using Gtk;

namespace Shapes
{
	public class RectangleShape : Shape
	{
		private double width;
		private double height;
		
		public RectangleShape () : base () 
		{
			XPadding = 10;
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
			
			width = textWidth + 2 * XPadding;
			height = textHeight + 2 * YPadding;
			
			context.Rectangle(Position.X - width / 2, Position.Y - height / 2, width, height);
			
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
			
			context.MoveTo(Position.X - textWidth/2, Position.Y - textHeight/2);
			Pango.CairoHelper.ShowLayout(context, pangoLayout);
			
			context.Source = oldSource;
		}
		
		public override bool InBoundingBox (double x, double y, out PointD delta)
		{
			if ((x > Position.X && x < Position.X + width)
				& (y > Position.Y && y < Position.Y + height)) {
				delta.X = Position.X - x;
				delta.Y = Position.Y - y;
				return true;
			} 
			return false;
		}
		
	}
}

