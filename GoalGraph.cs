using System;
using Cairo;
using System.Collections.Generic;
using Shapes;
using Gtk;
using Arrows;

namespace Editor
{
	public class GoalGraph : DrawingArea
	{
		private IShape selectedShape;
		private PointD selectionPositionDelta;
		
		public Color BackgroundColor {
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
		
		public GoalGraph ()
		{
			BackgroundColor = new Color(1, 1, 1);
			Shapes = new List<IShape>();
			Arrows = new List<FilledArrow>();
			
			Shapes.Add(new RectangleShape() {
				Position = new PointD(50, 50),
				Label = "Rectangle"
			});
			Shapes.Add(new CircleShape() {
				Label = "Circle",
				Position = new PointD(150, 50)
			});
			
			Arrows.Add(new FilledArrow() {
				Start = Shapes[0],
				End = Shapes[1]
			});
			
			this.AddEvents((int) Gdk.EventMask.PointerMotionMask
				| (int) Gdk.EventMask.ButtonPressMask
				| (int) Gdk.EventMask.ButtonReleaseMask);
		}
		
		protected override bool OnExposeEvent (Gdk.EventExpose evnt)
		{
			int width, height;
			evnt.Window.GetSize(out width, out height);
			
			using (Context context = Gdk.CairoHelper.Create(evnt.Window)) {
				PaintBackground(context);
				
				// For crisp lines
				context.Translate(0.5, 0.5);				
				context.LineWidth = 1;				
				
				// Draw all arrows
				foreach (var arrow in Arrows) {
					arrow.Display(context, this);
				}
				
				// Draw all shapes
				foreach (var rect in Shapes) {
					rect.Display(context, this);
				}
			}
			return true;
		}
		
		private void PaintBackground(Context context)
		{
			var oldSource = context.Source;
			context.SetSourceRGBA(BackgroundColor.R,
				BackgroundColor.G,
				BackgroundColor.B,
				BackgroundColor.A);
			context.Paint();
			context.Source = oldSource;
		}
		
		protected override bool OnMotionNotifyEvent (Gdk.EventMotion evnt)
		{
			if (selectedShape != null) {
				selectedShape.Position = new PointD(evnt.X + selectionPositionDelta.X, evnt.Y + selectionPositionDelta.Y);
				evnt.Window.InvalidateRect(new Gdk.Rectangle(0,0,Allocation.Width, Allocation.Height), true);
			}
			return true;
		}
		
		protected override bool OnButtonReleaseEvent (Gdk.EventButton evnt)
		{
			// Clear selected rectangle
			selectedShape = null;
			return base.OnButtonReleaseEvent (evnt);
		}

		protected override bool OnButtonPressEvent (Gdk.EventButton evnt)
		{
			// Find the rectangle to move
			selectedShape = null;
			selectionPositionDelta = new PointD(0,0);
			for (int i = Shapes.Count - 1; i >= 0; i--) {
				var rect = Shapes[i];
				if (rect.InBoundingBox(evnt.X, evnt.Y, out selectionPositionDelta)) {
					selectedShape = rect;
					break;
				}
			}
			return base.OnButtonPressEvent (evnt);
		}
		
	}
}

