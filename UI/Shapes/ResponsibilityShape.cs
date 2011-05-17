// 
// RefinementShape.cs
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
using Cairo;
using KaosEditor;
using KaosEditor.Model;
using KaosEditor.UI.Arrows;

namespace KaosEditor.UI.Shapes
{
	
	/// <summary>
	/// Represents the shape for responsibility.
	/// </summary>
	public class ResponsibilityShape : Shape
	{
		
		/// <summary>
		/// The radius.
		/// </summary>
		private double radius;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="KaosEditor.UI.Shapes.RefinementShape"/> class.
		/// </summary>
		/// <param name='refinement'>
		/// Refinement.
		/// </param>
		public ResponsibilityShape (Responsibility responsibility) : base ()
		{
			RepresentedElement = responsibility;
			radius = 4;
		}
				
		/// <summary>
		/// Display the shape on the specified context and view.
		/// </summary>
		/// <param name='context'>
		/// Context.
		/// </param>
		/// <param name='view'>
		/// View.
		/// </param>
		public override void Display (Context context, View view)
		{
			var oldSource = context.Source;
			
			context.MoveTo(Position.X + radius, Position.Y);
			context.Arc(Position.X, Position.Y, radius, 0, Math.PI * 2);
			
			context.SetColor ("#f70104");
			context.FillPreserve();
			
			context.SetColor ("#000");
			context.Stroke();
			
			var element = (Responsibility) RepresentedElement;
			
			IShape goalShape = view.GetNearestShapeFor(element.Goal, this.Position);
			IShape agentShape = view.GetNearestShapeFor(element.Agent, this.Position);
			
			IArrow arrow = new FilledArrow() {
				Start = goalShape,
				End = this,
				FillColor = "#f70104"
			};
			arrow.Display(context, view);
			
			arrow = new Arrow () {
				Start = this,
				End = agentShape
			};
			arrow.Display(context, view);
			
			context.Source = oldSource;
		}
		
		/// <summary>
		/// Determines whether coordinates are in the form.
		/// </summary>
		/// <returns>
		/// The bounding box.
		/// </returns>
		/// <param name='x'>
		/// If set to <c>true</c> x.
		/// </param>
		/// <param name='y'>
		/// If set to <c>true</c> y.
		/// </param>
		/// <param name='delta'>
		/// If set to <c>true</c> delta.
		/// </param>
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
		
		/// <summary>
		/// Gets the anchor corresponding for the given point.
		/// </summary>
		/// <returns>
		/// The anchor.
		/// </returns>
		/// <param name='point'>
		/// Point.
		/// </param>
		public override PointD GetAnchor (PointD point)
		{
			// Compute the angle between point and center
			double angle = Math.Atan2(Position.Y - point.Y, Position.X - point.X);
			return new PointD(Position.X - radius * Math.Cos(angle), Position.Y - radius * Math.Sin(angle));
		}
		
	}
}

