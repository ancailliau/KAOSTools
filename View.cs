using System;
using Shapes;
using Arrows;
using System.Collections.Generic;
using Cairo;
using Gtk;
using Model;

namespace Editor
{
	public class View
	{
		
		public string Name {
			get;
			set;
		}
		
		public List<IShape> Shapes {
			get;
			set;
		}
		
		public List<FilledArrow> Arrows {
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
			Shapes = new List<IShape>();
			Arrows = new List<FilledArrow>();
		}
		
		public void Display (Context context) 
		{
			// Draw all arrows
			foreach (var arrow in Arrows) {
				arrow.Display(context, DrawingArea);
			}
			
			// Draw all shapes
			foreach (var rect in Shapes) {
				rect.Display(context, DrawingArea);
			}
		}
		
		public void Add (IModelElement element) 
		{
			IShape shape = ShapeFactory.Create (element);
			if (shape != null) {
				Shapes.Add (shape);
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
			SelectedShape = null;
			return true;
		}

		public bool OnButtonPressEvent (Gdk.EventButton args)
		{
			// Find the rectangle to move
			SelectedShape = null;
			var selectedPoint = new PointD(0,0);
			for (int i = Shapes.Count - 1; i >= 0; i--) {
				var shape = Shapes[i];
				
				if (shape.InBoundingBox(args.X, args.Y, out selectedPoint)) {
					SelectedShape = shape;
					break;
				}
			}
			SelectedPoint = selectedPoint;
			
			return true;
		}
		
		
	}
}

