using Cairo;
using Beaver.Model;
using Gtk;
using System;
using Beaver.Views;
using System.Collections.Generic;
using Beaver.UI.Decoration;
using System.Linq;

namespace Beaver.UI.Shapes
{
	public abstract class PolygonalShape : Shape
	{
		protected IList<Func<PointD>> points = new List<Func<PointD>>();
		protected IList<Func<PointD>> anchors = new List<Func<PointD>>();
		
		protected double xPadding;
		protected double yPadding;
		
		public PolygonalShape (KAOSElement element)
			: this (element, new PointD ())
		{}	
	
		public PolygonalShape (KAOSElement element, PointD position)
			: base (element, position)
		{
		}
		
		public override void AbstractDisplay (Context cairoContext, Pango.Context pangoContext, ModelView view)
		{
			if (points.Count == 0) 
				return;
			
			cairoContext.Save ();
			var pangoLayout = new Pango.Layout(pangoContext);
			
			pangoLayout.Alignment = Pango.Alignment.Center;
			pangoLayout.SetText(getContent());
			pangoLayout.Width = Pango.Units.FromPixels(maxWidth);		
			
			int textWidth, textHeight;
			pangoLayout.GetPixelSize(out textWidth, out textHeight);
			
			width = (int) ( textWidth + 2 * xPadding );
			height = (int) ( textHeight + 2 * yPadding );
			
			cairoContext.MoveTo (points[0]());
			foreach (var p in from pp in points select pp())
				cairoContext.LineTo (p);
			
			cairoContext.ClosePath ();
			
			if (Selected)
				cairoContext.LineWidth *= 2;
			
			cairoContext.SetColor(this.FillColor);
			cairoContext.FillPreserve();
			
			cairoContext.SetColor(this.StrokeColor);
			cairoContext.Stroke();
			
			cairoContext.SetColor (this.TextColor);
			cairoContext.MoveTo (Position.X - maxWidth / 2f, Position.Y - textHeight / 2f);
			Pango.CairoHelper.ShowLayout(cairoContext, pangoLayout);
			
			foreach (var d in this.decorations) {
				var position = points[(int) d.Position]();
				d.Render (cairoContext, pangoContext, position.X, position.Y);
			}
			
			cairoContext.Restore ();
		}
		
		public override bool InBoundingBox (double x, double y, out PointD delta, ModelView view)
		{
			var computedPoints = from p in points select p();
			if (computedPoints.PointInPolygon (new PointD (x, y))) {
				delta.X = Position.X - x;
				delta.Y = Position.Y - y;
				return true;
			} 
			return false;
		}
		
		public override Bounds GetBounds (ModelView view)
		{
			var b = new Bounds ();
			foreach (var p in from pp in points select pp()) {
				if (p.X > b.MaxX)
					b.MaxX = (int) p.X;
				if (p.X < b.MinX)
					b.MinX = (int) p.X;
				if (p.Y > b.MaxY)
					b.MaxY = (int) p.Y;
				if (p.Y > b.MinY)
					b.MinY = (int) p.Y;
			}
			return b;
		}
		
		public override IEnumerable<PointD> GetAnchors (ModelView view)
		{
			return this.anchors.Select((arg) => arg());
		}		
	}
}

