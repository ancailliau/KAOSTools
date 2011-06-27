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
using Beaver;
using Beaver.Model;
using Beaver.UI.Arrows;
using Beaver.Logging;
using Beaver.Views;

namespace Beaver.UI.Shapes
{
	
	public class ExceptionLinkShape : Shape
	{
		
		public ExceptionLinkShape (ExceptionLink exception) : base ()
		{
			RepresentedElement = exception;
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
		public override void Display (Context context, ModelView view)
		{
			var oldLineWidth = context.LineWidth;
			if (Selected) {
				context.LineWidth = 2.5;
			}
			var oldSource = context.Source;
			
			var element = (ExceptionLink) RepresentedElement;
			
			var goalShapes = view.GetAllShapesFor (element.Goal);
			var exceptionGoalShapes = view.GetAllShapesFor (element.ExceptionGoal);
			
			IShape goalShape = null;
			IShape exceptionGoalShape = null;
			double minDist = double.PositiveInfinity;
			foreach (var s in goalShapes) {
				foreach (var s2 in exceptionGoalShapes) {
					var a = s.Position.X - s2.Position.X;
					var b = s.Position.Y - s2.Position.Y;
					var c = a * a + b * b;
					if (c < minDist) {
						minDist = c;
						goalShape = s;
						exceptionGoalShape = s2;
					}
				}
			}
			
			if (goalShape != null & exceptionGoalShape != null) {
				DashedArrow arrow = new DashedArrow() {
					Start = exceptionGoalShape,
					End = goalShape
				};
				arrow.Display(context, view);
				
				double xEndAnchor = arrow.End.GetAnchor(arrow.Start.Position).X;
				double yEndAnchor = arrow.End.GetAnchor(arrow.Start.Position).Y;
				
				double xStartAnchor = arrow.Start.GetAnchor(arrow.End.Position).X;
				double yStartAnchor = arrow.Start.GetAnchor(arrow.End.Position).Y;
				
				double x = xEndAnchor - .5 * ( xEndAnchor - xStartAnchor );
				double y = yEndAnchor - .5 * ( yEndAnchor - yStartAnchor );
				
				context.MoveTo (x, y);
				context.Arc (x, y, 10, 0, Math.PI * 2);
				context.SetColor (view.Controller.CurrentColorScheme.ExceptionStrokeColor);
				context.LineWidth = 2;
				context.StrokePreserve ();
				context.LineWidth = 1;
				context.SetColor (view.Controller.CurrentColorScheme.ExceptionFillColor);
				context.Fill ();
				
				var pangoLayout = new Pango.Layout(view.DrawingArea.PangoContext);
				pangoLayout.Alignment = Pango.Alignment.Center;
				pangoLayout.SetMarkup("!");
				
				var fontDescr = new Pango.FontDescription ();
				fontDescr.Size = (int) (14 * Pango.Scale.PangoScale);
				fontDescr.Weight = Pango.Weight.Bold;
				pangoLayout.FontDescription = fontDescr;					
				
				int textWidth, textHeight;
				pangoLayout.GetPixelSize(out textWidth, out textHeight);
				context.SetColor (view.Controller.CurrentColorScheme.ExceptionTextColor);
				context.MoveTo(x - textWidth/2f, y - textHeight/2f);
				Pango.CairoHelper.ShowLayout(context, pangoLayout);
				
				// Displaying condition
				int widthCondition = 100;
				pangoLayout = new Pango.Layout(view.DrawingArea.PangoContext);
				pangoLayout.Alignment = Pango.Alignment.Center;
				pangoLayout.SetMarkup(((ExceptionLink) RepresentedElement).Condition);
				pangoLayout.Width = Pango.Units.FromPixels(widthCondition);
				
				fontDescr = new Pango.FontDescription ();
				fontDescr.Size = (int) (9 * Pango.Scale.PangoScale);
				pangoLayout.FontDescription = fontDescr;					
				
				pangoLayout.GetPixelSize(out textWidth, out textHeight);
				context.SetColor (view.Controller.CurrentColorScheme.ExceptionTextColor);
				context.MoveTo(x - widthCondition/2f, y + 15 /* - textHeight/2f */ );
				Pango.CairoHelper.ShowLayout(context, pangoLayout);
			
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
			return new Bounds () {
				MinX = (int) (Position.X - 10) - 2,
				MaxX = (int) (Position.X + 10) + 2,
				MinY = (int) (Position.Y - 10) - 2,
				MaxY = (int) (Position.Y + 10) + 2
			};
		}
		
		public override IShape Copy ()
		{
			return new ExceptionLinkShape (this.RepresentedElement as ExceptionLink) {
				Position = new PointD (Position.X, Position.Y)
			};
		}
	}
}

