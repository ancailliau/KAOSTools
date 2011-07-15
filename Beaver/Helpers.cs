using System;
using System.Xml;
using Cairo;

namespace Beaver
{
	public static class Helpers
	{
		
		public static bool IsEndElement (this XmlReader reader, string name)
		{
			return reader.NodeType == XmlNodeType.EndElement && reader.Name == name;
		}
		
		public static void SetColor (this Context context, string hexa)
		{
			Gdk.Color col = new Gdk.Color (0, 0, 0);			
			Gdk.Color.Parse (hexa, ref col);
			context.SetSourceRGB (col.Red / 65535.0, col.Green / 65535.0, col.Blue / 65535.0);
		}
		
		public static void SetColor (this Context context, string hexa, float opacity)
		{
			Gdk.Color col = new Gdk.Color (0, 0, 0);			
			Gdk.Color.Parse (hexa, ref col);
			context.SetSourceRGBA (col.Red / 65535.0, col.Green / 65535.0, col.Blue / 65535.0, opacity);
		}
		
		public static void RoundedRectangle (this Cairo.Context gr, double x, double y, double width, double height, double radius)
		{
			gr.Save ();
			
			if ((radius > height / 2) || (radius > width / 2))
				radius = Math.Min (height / 2, width / 2);
			
			gr.MoveTo (x, y + radius);
			gr.Arc (x + radius, y + radius, radius, Math.PI, -Math.PI / 2);
			gr.LineTo (x + width - radius, y);
			gr.Arc (x + width - radius, y + radius, radius, -Math.PI / 2, 0);
			gr.LineTo (x + width, y + height - radius);
			gr.Arc (x + width - radius, y + height - radius, radius, 0, Math.PI / 2);
			gr.LineTo (x + radius, y + height);
			gr.Arc (x + radius, y + height - radius, radius, Math.PI / 2, Math.PI);
			gr.ClosePath ();
			gr.Restore ();
		}
		
		public static bool PointInPolygon (this PointD[] poly, PointD p)
		{
			PointD p1, p2;
			bool inside = false;
			if (poly.Length < 3) {
				return inside;
			}

			PointD oldPoint = new PointD (
				poly [poly.Length - 1].X, poly [poly.Length - 1].Y);

			for (int i = 0; i < poly.Length; i++) {
				PointD newPoint = new PointD (poly [i].X, poly [i].Y);
				if (newPoint.X > oldPoint.X) {
					p1 = oldPoint;
					p2 = newPoint;
				} else {
					p1 = newPoint;
					p2 = oldPoint;
				}

				if ((newPoint.X < p.X) == (p.X <= oldPoint.X)
					&& ((long)p.Y - (long)p1.Y) * (long)(p2.X - p1.X)
					< ((long)p2.Y - (long)p1.Y) * (long)(p.X - p1.X)) {
					inside = !inside;
				}
				
				oldPoint = newPoint;
			}
			return inside;
		}
		
	}
}

