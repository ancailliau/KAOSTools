using System;
using Cairo;
using System.Collections.Generic;
using Shapes;
using Gtk;

namespace Editor
{
	public class GoalGraph : DrawingArea
	{
		private RectangleShape selectedRectangle;
		private PointD selectionPositionDelta;
		
		public Color BackgroundColor {
			get;
			set;
		}
		
		public List<RectangleShape> Rectangles {
			get;
			set;
		}
		
		public GoalGraph ()
		{
			BackgroundColor = new Color(1, 1, 1);
			Rectangles = new List<RectangleShape>();
			Rectangles.Add(new RectangleShape() {
				TopLeft = new PointD(50, 50),
				Label = "Rectangle\n1"
			});
			Rectangles.Add(new RectangleShape() {
				TopLeft = new PointD(100, 50),
				Label = "Rectangle 2"
			});
			Rectangles.Add(new RectangleShape() {
				TopLeft = new PointD(50, 100),
				Label = "Rectangle 3"
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
				
				foreach (var rect in Rectangles) {
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
			if (selectedRectangle != null) {
				selectedRectangle.TopLeft = new PointD(evnt.X + selectionPositionDelta.X, evnt.Y + selectionPositionDelta.Y);
				evnt.Window.InvalidateRect(new Gdk.Rectangle(0,0,Allocation.Width, Allocation.Height), true);
			}
			return true;
		}
		
		protected override bool OnButtonReleaseEvent (Gdk.EventButton evnt)
		{
			// Clear selected rectangle
			selectedRectangle = null;
			return base.OnButtonReleaseEvent (evnt);
		}

		protected override bool OnButtonPressEvent (Gdk.EventButton evnt)
		{
			// Find the rectangle to move
			selectedRectangle = null;
			selectionPositionDelta = new PointD(0,0);
			for (int i = Rectangles.Count - 1; i >= 0; i--) {
				var rect = Rectangles[i];
				if (rect.InBoundingBox(evnt.X, evnt.Y, out selectionPositionDelta)) {
					selectedRectangle = rect;
					break;
				}
			}
			return base.OnButtonPressEvent (evnt);
		}
		
	}
}

