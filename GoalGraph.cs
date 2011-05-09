using System;
using Cairo;
using System.Collections.Generic;
using Shapes;

namespace Editor
{
	public class GoalGraph : Gtk.DrawingArea
	{
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
				TopLeft = new PointD(100, 100),
				Width = 100,
				Height = 40
			});
		}
		
		protected override bool OnExposeEvent (Gdk.EventExpose evnt)
		{
			int width, height;
			evnt.Window.GetSize(out width, out height);
			Console.WriteLine ("{0} {1}", width, height);
			
			using (Context context = Gdk.CairoHelper.Create(evnt.Window)) {
				PaintBackground(context);
				
				// Y-Axis is inverted 
				context.Transform(new Matrix(1,0,0,-1,0,0));
				
				// Re-adjust center
				context.Translate(0, -height);
				
				// For crisp lines
				context.Translate(0.5, 0.5);				
				context.LineWidth = 1;				
				
				foreach (var rect in Rectangles) {
					rect.Display(context);
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
		
	}
}

