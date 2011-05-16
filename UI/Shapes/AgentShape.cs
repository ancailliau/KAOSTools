using System;
using Cairo;
using Editor;
using KaosEditor.Model;
using KaosEditor;

namespace Shapes
{
	public class AgentShape : Shape
	{
		private int width;
		private int height;
		
		public AgentShape (Agent agent) : base () 
		{
			XPadding = 10;
			YPadding = 4;
			RepresentedElement = agent;
		}
		
		public override void Display (Context context, View view)
		{
			var drawingArea = view.DrawingArea;
			var oldSource = context.Source;
			
			var pangoLayout = new Pango.Layout(drawingArea.PangoContext);
			pangoLayout.SetText(((Agent) this.RepresentedElement).Name);
			pangoLayout.Alignment = Pango.Alignment.Center;
			
			int textWidth, textHeight;
			pangoLayout.GetPixelSize(out textWidth, out textHeight);
			
			width = (int) ( textWidth + 2 * XPadding );
			height = (int) ( textHeight + 2 * YPadding );
			
			context.Rectangle(Position.X - width / 2, Position.Y - height / 2, width, height);
			
			context.SetColor("#fefec3");
			context.FillPreserve();
			
			var oldLineWidth = context.LineWidth;
			context.SetSourceRGBA(BorderColor.R,
				BorderColor.G,
				BorderColor.B,
				BorderColor.A);
			if (Selected) {
				context.LineWidth = 2.5;
			}
			context.Stroke();
			context.LineWidth = oldLineWidth;
			
			context.MoveTo(Position.X - textWidth/2, Position.Y - textHeight/2);
			Pango.CairoHelper.ShowLayout(context, pangoLayout);
			
			context.Source = oldSource;
		}
		
		public override bool InBoundingBox (double x, double y, out PointD delta)
		{
			if ((x > Position.X - width/2 && x < Position.X + width/2)
				& (y > Position.Y - height /2 && y < Position.Y + height/2)) {
				delta.X = Position.X - x;
				delta.Y = Position.Y - y;
				return true;
			} 
			return false;
		}
		
		public override PointD GetAnchor (PointD point)
		{
			double refAngle = Math.Atan2(height,width);
			double angle = Math.Atan2(Position.Y - point.Y, Position.X - point.X);
			
			if (angle < refAngle & angle > - refAngle) {
				// left
				return new PointD(Position.X - width/2, Position.Y);
				
			} else if (angle > - Math.PI + refAngle & angle < - refAngle) {
				// bottom
				return new PointD(Position.X, Position.Y + height / 2);
				
			} else if (angle > refAngle & angle < Math.PI - refAngle) {
				// top
				return new PointD(Position.X, Position.Y - height / 2);
				
			} else {
				// right
				return new PointD(Position.X + width / 2, Position.Y);
				
			}
		}
		
	}
}

