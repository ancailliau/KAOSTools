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
	
	public class ObstructionShape : Shape
	{
		public ObstructionShape (Obstruction obstruction) : base ()
		{
			RepresentedElement = obstruction;
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
			var oldLineWidth = context.LineWidth;
			if (Selected) {
				context.LineWidth = 2.5;
			}
			var oldSource = context.Source;
			
			var element = (Obstruction) RepresentedElement;
			
			var goalShapes = view.GetAllShapesFor (element.Goal);
			var obstacleShapes = view.GetAllShapesFor (element.Obstacle);
			
			IShape goalShape = null;
			IShape obstacleShape = null;
			double minDist = double.PositiveInfinity;
			foreach (var s in goalShapes) {
				foreach (var s2 in obstacleShapes) {
					var a = s.Position.X - s2.Position.X;
					var b = s.Position.Y - s2.Position.Y;
					var c = a * a + b * b;
					if (c < minDist) {
						minDist = c;
						goalShape = s;
						obstacleShape = s2;
					}
				}
			}
			
			if (goalShape != null & obstacleShape != null) {
				StrikedArrow arrow = new StrikedArrow() {
					Start = obstacleShape,
					End = goalShape,
					FillColor = "#f54326"
				};
				arrow.Display(context, view);
			}
			
			context.Source = oldSource;
			context.LineWidth = oldLineWidth;
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
			return new PointD ();
		}
			
		public override Bounds GetBounds ()
		{
			return new Bounds () {};
		}
		
		
		public override IShape Copy ()
		{
			return new ObstructionShape (this.RepresentedElement as Obstruction) {
				Position = new PointD (Position.X, Position.Y)
			};
		}
	}
}

