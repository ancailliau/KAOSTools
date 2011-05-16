using System;
using Shapes;
using Arrows;
using System.Collections.Generic;
using Cairo;
using Gtk;
using System.Linq;
using Model;
using Editor.Controllers;
using Editor.Dialogs;

namespace Editor.Model
{
	public class View
	{
		private int counter = 0;
		
		public delegate void ViewChangedHandler (object sender, EventArgs e);
		public event ViewChangedHandler ViewChanged;
	
		public string Name {
			get;
			set;
		}
		
		public List<IShape> Shapes {
			get;
			set;
		}
		
		public IShape SelectedShape {
			get;
			set;
		}
		
		public PointD SelectedPoint {
			get;
			set;
		}
		
		public DrawingArea DrawingArea { 
			get; 
			set;
		}
		
		private MainController controller;
		
		public View (MainController controller) 
			: this ("Untitled view")
		{
			this.controller = controller;
		}
		
		public View (string name)
		{
			Name = name;
			Shapes = new List<IShape>();
		}
		
		public void Display (Context context) 
		{
			// Draw all shapes
			foreach (var rect in Shapes) {
				rect.Display(context, this);
			}
		}
		
		public void Add (IModelElement element) 
		{
			IShape shape = ShapeFactory.Create (element);
			if (shape != null) {
				Add (shape);
			}
		}
		
		public void Add (IShape shape)
		{
			if (shape != null) {
				Shapes.Add (shape);
				NotifyChange ();
			}
		}
		
		public void NotifyChange ()
		{
			if (ViewChanged != null) {
				ViewChanged (this, EventArgs.Empty);
			}
		}
		
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

		public bool OnButtonPressEvent (Gdk.EventButton args)
		{
			if (args.Button == 3) { // Right click
				
				var selectedPoint = new PointD();
				var selectedShape = FindShape(args.X, args.Y, out selectedPoint);
				if (selectedShape != null && selectedShape.RepresentedElement is Goal) {
					var deleteItem = new MenuItem("Remove from view");
					deleteItem.Activated += delegate(object sender, EventArgs e) {
						this.Shapes.Remove(selectedShape);
						this.DrawingArea.QueueDraw();
					};
					var editItem = new MenuItem("Edit");
					editItem.Activated += delegate(object sender, EventArgs e) {
						var eg = new EditGoal(this.controller, selectedShape.RepresentedElement as Goal);
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
				SelectedShape = FindShape(args.X, args.Y, out selectedPoint);
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
		
		protected IShape FindShape(double x, double y, out PointD selectedPoint)
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
		
		public IShape ContainsShapeFor (IModelElement element)
		{
			return Shapes.Find(v => { 
				return v.RepresentedElement.Equals(element);
			});
		}
		
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

