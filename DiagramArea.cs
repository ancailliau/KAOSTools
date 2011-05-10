using System;
using Cairo;
using System.Collections.Generic;
using Shapes;
using Gtk;
using Arrows;

namespace Editor
{
	public class DiagramArea : DrawingArea
	{
		
		public Color BackgroundColor {
			get;
			set;
		}
		
		public View CurrentView {
			get;
			set;
		}
		
		public DiagramArea ()
		{
			BackgroundColor = new Color(1, 1, 1);
			CurrentView = new View() { DrawingArea = this };
			
			this.AddEvents((int) Gdk.EventMask.PointerMotionMask
				| (int) Gdk.EventMask.ButtonPressMask
				| (int) Gdk.EventMask.ButtonReleaseMask);
		}
		
		public DiagramArea (View view) 
			: this ()
		{
			this.CurrentView = view;
			this.CurrentView.DrawingArea = this;
		}
		
		public void UpdateCurrentView (View view) 
		{
			this.CurrentView = view;
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
				
				CurrentView.Display(context);
			}
			return true;
		}
		
		protected override bool OnMotionNotifyEvent (Gdk.EventMotion evnt)
		{
			return this.CurrentView.OnMotionNotifyEvent (evnt);
		}
		
		protected override bool OnButtonReleaseEvent (Gdk.EventButton evnt)
		{
			return this.CurrentView.OnButtonReleaseEvent (evnt);
		}

		protected override bool OnButtonPressEvent (Gdk.EventButton evnt)
		{
			return this.CurrentView.OnButtonPressEvent(evnt);
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
		
	}
}

