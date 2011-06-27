// 
// View.cs
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
using System.Collections.Generic;
using Cairo;
using Gtk;
using Beaver.Controllers;
using Beaver.Model;
using Beaver.UI.Shapes;
using Beaver.UI.Dialogs;
using Beaver.UI;
using Beaver.UI.Windows;
using Beaver.UI.Widgets;

namespace Beaver.Views
{
	
	/// <summary>
	/// Represents a view.
	/// </summary>
	public class ModelView
	{
		/// <summary>
		/// Handler executed when the view changed
		/// </summary>
		public delegate void ViewChangedHandler (object sender, EventArgs e);
		
		/// <summary>
		/// Occurs when view changed.
		/// </summary>
		public event ViewChangedHandler ViewChanged;
	
		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		public string Name {
			get;
			set;
		}
		
		/// <summary>
		/// Gets or sets the shapes.
		/// </summary>
		/// <value>
		/// The shapes.
		/// </value>
		public List<IShape> Shapes {
			get;
			private set;
		}
		
		/// <summary>
		/// Gets or sets the selected shape.
		/// </summary>
		/// <value>
		/// The selected shape.
		/// </value>
		public IShape SelectedShape {
			get;
			set;
		}
		
		public IShape ShapeToMove {
			get;
			set;
		}
		
		/// <summary>
		/// Gets or sets the selected point.
		/// </summary>
		/// <value>
		/// The selected point.
		/// </value>
		public PointD SelectedPoint {
			get;
			set;
		}
		
		/// <summary>
		/// Gets or sets the drawing area.
		/// </summary>
		/// <value>
		/// The drawing area.
		/// </value>
		public DiagramArea DrawingArea { 
			get; 
			set;
		}
		
		/// <summary>
		/// Gets or sets the controller.
		/// </summary>
		/// <value>
		/// The controller.
		/// </value>
		public MainController Controller {
			get;
			set;
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Editor.Model.View"/> class.
		/// </summary>
		/// <param name='name'>
		/// Name.
		/// </param>
		/// <param name='controller'>
		/// Controller.
		/// </param>
		public ModelView (string name, MainController controller)
		{
			Name = name;
			Shapes = new List<IShape>();
			
			Controller = controller;
		}
		
		/// <summary>
		/// Display all shapes on the specified context.
		/// </summary>
		/// <param name='context'>
		/// Context.
		/// </param>
		public void Display (Context context) 
		{
			// Draw all shapes
			foreach (var rect in Shapes) {
				rect.Display(context, this);
			}
		}
		
		/// <summary>
		/// Add the specified shape to the view
		/// </summary>
		/// <param name='shape'>
		/// Shape.
		/// </param>
		public void Add (IShape shape)
		{
			if (shape != null) {
				Shapes.Add (shape);
				NotifyChange ();
			}
		}
		
		public void Remove (IShape shape)
		{
			Shapes.Remove (shape);
			NotifyChange ();
		}
		
		/// <summary>
		/// Notifies the change.
		/// </summary>
		public void NotifyChange ()
		{
			if (ViewChanged != null) {
				ViewChanged (this, EventArgs.Empty);
			}
		}
		
		public IShape[] GetAllShapesFor (KAOSElement element) 
		{
			return Shapes.FindAll (x => {
				return x.RepresentedElement.Equals (element);
			}).ToArray();
		}
		
		/// <summary>
		/// Gets the nearest shape for a given kaos element
		/// </summary>
		/// <returns>
		/// The nearest shape for.
		/// </returns>
		/// <param name='element'>
		/// Element.
		/// </param>
		/// <param name='origin'>
		/// Origin.
		/// </param>
		public IShape GetNearestShapeFor (KAOSElement element, PointD origin)
		{
			double squaredDistance = double.PositiveInfinity;
			IShape shapeToReturn = null;
			var consideredShapes = Shapes.FindAll(v => { 
				return v.RepresentedElement.Equals(element);
			});
			foreach (IShape shape in consideredShapes) {
				double xx = shape.GetAnchor(origin).X - origin.X;
				double yy = shape.GetAnchor(origin).Y - origin.Y;
				double dist = (xx * xx + yy * yy);
				if (dist < squaredDistance) {
					shapeToReturn = shape;
					squaredDistance = dist;
				}
			}
			return shapeToReturn;
		}
		
		/// <summary>
		/// Redraw this instance.
		/// </summary>
		public void Redraw ()
		{
			// Drawing area is set only if the view is displayed
			if (this.DrawingArea != null) {
				this.DrawingArea.QueueDraw();
				
			} else {
			}
		}
		
		public void GetSize (out int minX, out int maxX, out int minY, out int maxY)
		{
			minX = int.MaxValue;
			minY = int.MaxValue;
			maxX = 0;
			maxY = 0;
			
			foreach (var shape in Shapes) {
				Bounds bounds = shape.GetBounds ();
				if (bounds.MinX != bounds.MaxX & bounds.MinY != bounds.MaxY) {
					minY = Math.Min(bounds.MinY, minY);
					minX = Math.Min(bounds.MinX, minX);
					maxY = Math.Max(bounds.MaxY, maxY);
					maxX = Math.Max(bounds.MaxX, maxX);
				}
			}
		}
		
	}
}
