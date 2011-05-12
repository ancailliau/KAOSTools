using System;
using Shapes;
using Arrows;
using System.Collections.Generic;
using Cairo;
using Gtk;
using System.Linq;
using Model;

namespace Editor
{
	public class View
	{
		private int counter = 0;
		
		public string Name {
			get;
			set;
		}
		
		public Dictionary<string, IShape> Shapes {
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
		
		public View ()
		{
			Name = "(default)";
			Shapes = new Dictionary<string, IShape>();
		}
		
		public void Display (Context context) 
		{
			// Draw all shapes
			foreach (var rect in Shapes.Values) {
				rect.Display(context, this);
			}
		}
		
		public void Add (IModelElement element) 
		{
			IShape shape = ShapeFactory.Create (Guid.NewGuid().ToString(), element);
			if (shape != null) {
				Console.WriteLine ("Add '{0}' to view '{1}'", shape.Id, this.Name);
				Shapes.Add (shape.Id, shape);
			}
		}
		
		public void Add (IShape shape)
		{
			if (shape != null && shape.Id != null) {
				Shapes.Add (shape.Id, shape);
			} else {
				Console.WriteLine ("Ignoring a shape");
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
			this.DrawingArea.QueueDraw();
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
						this.Shapes.Remove(selectedShape.Id);
						this.DrawingArea.QueueDraw();
					};
					var menu = new Menu();
					menu.Add(deleteItem);
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
			foreach (var shape in Shapes.Values) {
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
			return Shapes.Values.ToList().Find(v => { 
				return v.RepresentedElement.Equals(element);
			});
		}
		
		public IShape GetNearestShapeFor (IModelElement element, PointD origin)
		{
			double squaredDistance = double.PositiveInfinity;
			IShape shapeToReturn = null;
			var consideredShapes = Shapes.Values.ToList().FindAll(v => { 
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
		
	}
}

