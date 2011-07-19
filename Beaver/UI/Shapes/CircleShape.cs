// 
// CircleShape.cs
//  
// Author:
//       Antoine Cailliau <antoine.cailliau@uclouvain.be>
// 
// Copyright (c) 2011 2011 Université Catholique de Louvain and Antoine Cailliau
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Linq;
using Beaver.Model;
using Cairo;
using Beaver.Views;
using System.Collections.Generic;

namespace Beaver.UI.Shapes
{
	public class CircleShape : Shape
	{
		protected int radius = 4;
		
		public CircleShape (KAOSElement element, PointD position)
			: base (element, position)
		{}
		
		public override IEnumerable<PointD> GetAnchors (ModelView view)
		{
			return new PointD[] { Position };
		}

		public override Bounds GetBounds (ModelView view)
		{
			return new Bounds () {
				MaxX = (int) (this.position.X + radius),
				MinX = (int) (this.position.X - radius),
				MaxY = (int) (this.position.Y + radius),
				MinY = (int) (this.position.Y - radius),
			};
		}

		public override bool InBoundingBox (double x, double y, out PointD delta, ModelView view)
		{
			double centerX = this.position.X;
			double centerY = this.position.Y;
			double xx = (x - centerX);
			double yy = (y - centerY);
			if (Math.Sqrt(xx * xx + yy * yy) <= radius) {
				delta.X = Position.X - x;
				delta.Y = Position.Y - y;
				return true;
			}
			return false;
		}

		public override void AbstractDisplay (Context cairoContext, Pango.Context pangoContext, ModelView view)
		{
			if (radius <= 0)
				return;
			
			cairoContext.Save ();
			string content = getContent ();
			
			int textWidth = 0, textHeight = 0;
			Pango.Layout pangoLayout = null;
			
			if (!string.IsNullOrEmpty(content)) {
				pangoLayout = new Pango.Layout(pangoContext);
			
				pangoLayout.Alignment = Pango.Alignment.Center;
				pangoLayout.SetText(content);
				pangoLayout.Width = Pango.Units.FromPixels(maxWidth);
			
				pangoLayout.GetPixelSize(out textWidth, out textHeight);
			
				width = (int) textWidth;
				height = (int) textHeight;
			}
			
			cairoContext.MoveTo (this.position.X + radius, this.Position.Y);
			cairoContext.Arc (this.position.X, this.position.Y, Math.Max (radius, Math.Max(width, height)), 0, Math.PI * 2);
			
			if (Selected)
				cairoContext.LineWidth *= 2;
			
			cairoContext.SetColor(this.FillColor);
			cairoContext.FillPreserve();
			
			cairoContext.SetColor(this.StrokeColor);
			cairoContext.Stroke();
			
			if (!string.IsNullOrEmpty(content)) {
				cairoContext.SetColor (this.TextColor);
				cairoContext.MoveTo (Position.X - maxWidth / 2f, Position.Y - textHeight / 2f);
				Pango.CairoHelper.ShowLayout(cairoContext, pangoLayout);
			}
				
			foreach (var d in this.decorations) {
				d.Render (cairoContext, pangoContext, this.position.X, this.position.Y);
			}
			
			cairoContext.Restore ();
		}
	}
}

