// 
// DiagramArea.cs
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

using Cairo;
using Gtk;
using KaosEditor.Model;

namespace KaosEditor.UI.Widgets
{
	
	/// <summary>
	/// Represents a diagram area.
	/// </summary>
	public class DiagramArea : DrawingArea
	{
		/// <summary>
		/// Gets or sets the view displayed by this diagram area.
		/// </summary>
		/// <value>
		/// The view.
		/// </value>
		public View CurrentView {
			get;
			set;
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="KaosEditor.UI.Widgets.DiagramArea"/> class.
		/// </summary>
		public DiagramArea ()
		{
			this.AddEvents((int) Gdk.EventMask.PointerMotionMask
				| (int) Gdk.EventMask.ButtonPressMask
				| (int) Gdk.EventMask.ButtonReleaseMask);
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="KaosEditor.UI.Widgets.DiagramArea"/> class.
		/// </summary>
		/// <param name='view'>
		/// View.
		/// </param>
		public DiagramArea (View view) 
			: this ()
		{
			this.CurrentView = view;
			this.CurrentView.DrawingArea = this;
		}
		
		/// <summary>
		/// Updates the current view.
		/// </summary>
		/// <param name='view'>
		/// View.
		/// </param>
		public void UpdateView (View view) 
		{
			this.CurrentView = view;
			view.DrawingArea = this;
			this.QueueDraw();
		}
		
		/// <summary>
		/// Handles the expose event.
		/// </summary>
		/// <param name='evnt'>
		/// If set to <c>true</c> evnt.
		/// </param>
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
		
		/// <summary>
		/// Handles the motion notify event.
		/// </summary>
		/// <param name='evnt'>
		/// If set to <c>true</c> evnt.
		/// </param>
		protected override bool OnMotionNotifyEvent (Gdk.EventMotion evnt)
		{
			return this.CurrentView.OnMotionNotifyEvent (evnt);
		}
		
		/// <summary>
		/// Raises the button release event event.
		/// </summary>
		/// <param name='evnt'>
		/// If set to <c>true</c> evnt.
		/// </param>
		protected override bool OnButtonReleaseEvent (Gdk.EventButton evnt)
		{
			return this.CurrentView.OnButtonReleaseEvent (evnt);
		}
		
		/// <summary>
		/// Raises the button press event event.
		/// </summary>
		/// <param name='evnt'>
		/// If set to <c>true</c> evnt.
		/// </param>
		protected override bool OnButtonPressEvent (Gdk.EventButton evnt)
		{
			return this.CurrentView.OnButtonPressEvent(evnt);
		}
		
		/// <summary>
		/// Paints the background.
		/// </summary>
		/// <param name='context'>
		/// Context.
		/// </param>
		private void PaintBackground(Context context)
		{
			var oldSource = context.Source;
			context.SetColor("#fff");
			context.Paint();
			context.Source = oldSource;
		}
		
	}
}

