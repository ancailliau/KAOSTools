// 
// GoalShape.cs
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
using Gtk;
using KaosEditor.UI.Windows;

namespace KaosEditor.UI.Shapes
{
	
	/// <summary>
	/// Represents the shape for goals.
	/// </summary>
	public class GoalShape : Shape, IContextMenu
	{
		
		/// <summary>
		/// The width.
		/// </summary>
		private int width;
		
		/// <summary>
		/// The height.
		/// </summary>
		private int height;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="KaosEditor.UI.Shapes.GoalShape"/> class.
		/// </summary>
		/// <param name='goal'>
		/// Goal.
		/// </param>
		public GoalShape (Goal goal) : base () 
		{
			XPadding = 10;
			YPadding = 4;
			RepresentedElement = goal;
			
			// Computing size of the shape
			var pangoLayout = new Pango.Layout(Gdk.PangoHelper.ContextGetForScreen(Gdk.Screen.Default));
			pangoLayout.SetText(((Goal) this.RepresentedElement).Name);
			pangoLayout.Alignment = Pango.Alignment.Center;
			
			int textWidth, textHeight;
			pangoLayout.GetPixelSize(out textWidth, out textHeight);
			
			width = (int) ( textWidth + 2 * XPadding );
			height = (int) ( textHeight + 2 * YPadding );
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
			var drawingArea = view.DrawingArea;
			var oldSource = context.Source;
			
			var pangoLayout = new Pango.Layout(drawingArea.PangoContext);
			pangoLayout.SetText(((Goal) this.RepresentedElement).Name);
			pangoLayout.Alignment = Pango.Alignment.Center;
			
			int textWidth, textHeight;
			pangoLayout.GetPixelSize(out textWidth, out textHeight);
			
			width = (int) ( textWidth + 2 * XPadding );
			height = (int) ( textHeight + 2 * YPadding );
			
			//context.Rectangle(Position.X - width / 2, Position.Y - height / 2, width, height);
			var shear = 4;
			
			context.MoveTo(Position.X - width / 2 + shear / 2,
				Position.Y - height/2);
			context.RelLineTo(width, 0);
			context.RelLineTo(-shear, height);
			context.RelLineTo(- width, 0);
			context.ClosePath();
			
			context.SetColor("#bdcbe0");
			context.FillPreserve();
			
			var oldLineWidth = context.LineWidth;
			context.SetColor("#000");
			if (Selected) {
				context.LineWidth = 2.5;
			}
			context.Stroke();
			context.LineWidth = oldLineWidth;
			
			context.MoveTo(Position.X - textWidth/2, Position.Y - textHeight/2);
			Pango.CairoHelper.ShowLayout(context, pangoLayout);
			
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
			if ((x > Position.X - width/2 && x < Position.X + width/2)
				& (y > Position.Y - height /2 && y < Position.Y + height/2)) {
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
		
		public void PopulateContextMenu (Menu menu, MainWindow window)
		{
			var deleteItem = new MenuItem("Remove from view");
			deleteItem.Activated += delegate(object sender, EventArgs e) {
				window.RemoveFromCurrentView (this);
			};
			menu.Add(deleteItem);
		}
		
		public override Bounds GetBounds ()
		{
			return new Bounds () {
				MinX = (int) (Position.X - width / 2 - 4),
				MaxX = (int) (Position.X + width / 2 + 4),
				MinY = (int) (Position.Y - height / 2),
				MaxY = (int) (Position.Y + height / 2)
			};
		}
		
	}
}

