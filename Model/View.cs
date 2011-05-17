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
using KaosEditor.Controllers;
using KaosEditor.Model;
using KaosEditor.UI.Shapes;
using KaosEditor.UI.Dialogs;

namespace KaosEditor.Model
{
	
	/// <summary>
	/// Represents a view.
	/// </summary>
	public class View
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
		public DrawingArea DrawingArea { 
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
		public View (string name, MainController controller)
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
		
		/// <summary>
		/// Notifies the change.
		/// </summary>
		public void NotifyChange ()
		{
			if (ViewChanged != null) {
				ViewChanged (this, EventArgs.Empty);
			}
		}
		
		/// <summary>
		/// Handle the motion notify event event.
		/// </summary>
		/// <param name='args'>
		/// If set to <c>true</c> arguments.
		/// </param>
		public bool OnMotionNotifyEvent (Gdk.EventMotion args)
		{
			if (this.SelectedShape != null) {
				SelectedShape.Position = new PointD(
					args.X + SelectedPoint.X, 
					args.Y + SelectedPoint.Y);
				
				// Redraw
				args.Window.InvalidateRect(
					new Gdk.Rectangle(0, 0, 
						DrawingArea.Allocation.Width, 
						DrawingArea.Allocation.Height),
					true);
			}
			return true;
		}
		
		/// <summary>
		/// Handle the button release event event.
		/// </summary>
		/// <param name='args'>
		/// If set to <c>true</c> arguments.
		/// </param>
		public bool OnButtonReleaseEvent (Gdk.EventButton args)
		{
			if (SelectedShape != null) {
				SelectedShape.Selected = false;
			}
			SelectedShape = null;
			
			if (ViewChanged != null) {
				ViewChanged (this, EventArgs.Empty);
			}
			
			return true;
		}
		
		/// <summary>
		/// Handle the button press event event.
		/// </summary>
		/// <param name='args'>
		/// If set to <c>true</c> arguments.
		/// </param>
		public bool OnButtonPressEvent (Gdk.EventButton args)
		{
			if (args.Button == 3) { // Right click
				
				var selectedPoint = new PointD();
				var selectedShape = FindShapeAtPosition(args.X, args.Y, out selectedPoint);
				if (selectedShape != null && selectedShape.RepresentedElement is Goal) {
					var deleteItem = new MenuItem("Remove from view");
					deleteItem.Activated += delegate(object sender, EventArgs e) {
						this.Shapes.Remove(selectedShape);
						this.DrawingArea.QueueDraw();
					};
					var editItem = new MenuItem("Edit");
					editItem.Activated += delegate(object sender, EventArgs e) {
						var eg = new EditGoal(this.Controller.Window, selectedShape.RepresentedElement as Goal);
						eg.Present();
					};
					var menu = new Menu();
					menu.Add(deleteItem);
					menu.Add(editItem);
					menu.ShowAll();
					menu.Popup();
				}
				
				
			} else if (args.Button == 1) { // Left click
				
				// Find the rectangle to move
				var selectedPoint = new PointD();
				SelectedShape = FindShapeAtPosition(args.X, args.Y, out selectedPoint);
				SelectedPoint = selectedPoint;
				
				if (SelectedShape != null) {
					SelectedShape.Selected = true;
					Console.WriteLine ("Selected '{0}' in '{1}'", SelectedShape.RepresentedElement.Id, this.Name);
				} else {
					Console.WriteLine ("No element selected");
				}
			}
			
			return true;
		}
		
		/// <summary>
		/// Finds the shape at given position position.
		/// </summary>
		/// <returns>
		/// The shape at given position.
		/// </returns>
		/// <param name='x'>
		/// X.
		/// </param>
		/// <param name='y'>
		/// Y.
		/// </param>
		/// <param name='selectedPoint'>
		/// The point that was clicked.
		/// </param>
		protected IShape FindShapeAtPosition(double x, double y, out PointD selectedPoint)
		{
			IShape selectedShape = null;
			foreach (var shape in Shapes) {
				if (shape.InBoundingBox(x, y, out selectedPoint)) {
					if (selectedShape == null || shape.Depth > selectedShape.Depth) { 
						selectedShape = shape;
					}
				}
			}
			return selectedShape;
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
		public IShape GetNearestShapeFor (IModelElement element, PointD origin)
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
				Console.WriteLine ("oups");
			}
		}
		
	}
}

