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
			IShape shape = ShapeFactory.Create (element);
			if (shape != null) {
				Shapes.Add (element.Id, shape);
			}
		}
		
		public void Add (IShape shape)
		{
			Shapes.Add (shape.RepresentedElement.Id, shape);
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
			SelectedShape = null;
			return true;
		}

		public bool OnButtonPressEvent (Gdk.EventButton args)
		{
			// Find the rectangle to move
			SelectedShape = Shapes.Values.FirstOrDefault();
			var selectedPoint = new PointD(0,0);
			foreach (var shape in Shapes.Values) {
				if (shape.InBoundingBox(args.X, args.Y, out selectedPoint)) {
					if (shape.Depth > SelectedShape.Depth) { 
						SelectedShape = shape;
					}
				}
			}
			SelectedPoint = selectedPoint;
			
			return true;
		}
		
		public IShape ContainsShapeFor (IModelElement element)
		{
			if (Shapes.ContainsKey(element.Id)) {
				return Shapes[element.Id];
			}
			return null;
		}
		
	}
}

