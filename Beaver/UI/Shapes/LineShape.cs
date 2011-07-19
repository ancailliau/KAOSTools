// 
// LineShape.cs
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
	public class LineShape : Shape
	{
		protected KAOSElement startElement;
		protected KAOSElement endElement;
		
		protected PointD anchor1;
		protected PointD anchor2;
		
		public LineShape (KAOSElement element, PointD point)
			: base (element, point)
		{
		}

		private bool ComputeAnchors (ModelView view)
		{
			double dist = double.MaxValue;
			Shape s = null;
			PointD _anchor1, _anchor2;
			
			var shapes = view.GetAllShapesFor (startElement);
			for (int i = 0; i < shapes.Length; i++) {
				var _s = shapes [i];
				if (view.GetNearestShapeFor (_s, endElement, out _anchor1, out _anchor2) != null) {
					var x = _anchor1.X - _anchor2.X;
					var y = _anchor1.Y - _anchor2.Y;
					var _dist = x * x + y * y;
					if (_dist < dist) {
						s = _s;
						anchor1 = _anchor1;
						anchor2 = _anchor2;
						dist = _dist;
					}
				}
			}
			return s != null;
		}
		
		public override IEnumerable<PointD> GetAnchors (ModelView view)
		{
			return new PointD[] { anchor1, anchor2 };
		}

		public override Bounds GetBounds (ModelView view)
		{
			return new Bounds () {
				MaxX = (int) Math.Max (anchor1.X, anchor2.X),
				MinX = (int) Math.Min (anchor1.X, anchor2.X),
				MaxY = (int) Math.Max (anchor1.Y, anchor2.Y),
				MinY = (int) Math.Min (anchor1.Y, anchor2.Y)
			};
		}

		public override bool InBoundingBox (double x, double y, out Cairo.PointD delta, ModelView view)
		{
			return false;
		}

		public override void AbstractDisplay (Cairo.Context cairoContext, Pango.Context pangoContext, Beaver.Views.ModelView view)
		{
			cairoContext.Save ();
			
			if (ComputeAnchors(view)) {
				cairoContext.MoveTo (anchor1);
				cairoContext.LineTo (anchor2);
				cairoContext.SetColor ("#000");
				cairoContext.Stroke ();
			}
			
			foreach (var d in this.decorations) {
				var dpos = Math.Max (Math.Min (d.Position, 1), 0);
				var position = new PointD (
					anchor1.X + (anchor2.X - anchor1.X) * dpos, 
					anchor1.Y + (anchor2.Y - anchor1.Y) * dpos);
				
				d.Render (cairoContext, pangoContext, position.X, position.Y);
			}
			
			cairoContext.Restore ();
			
		}
		
	}
}

