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
using System.Collections.Generic;
using KaosEditor.UI.Shapes;
using KaosEditor.Controllers;
using System;
using KaosEditor.UI.Dialogs;

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
		
		private MainController controller;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="KaosEditor.UI.Widgets.DiagramArea"/> class.
		/// </summary>
		public DiagramArea (MainController controller)
		{
			this.controller = controller;
			
			this.AddEvents((int) Gdk.EventMask.PointerMotionMask
				| (int) Gdk.EventMask.ButtonPressMask
				| (int) Gdk.EventMask.ButtonReleaseMask
				| (int) Gdk.EventMask.KeyPressMask
				| (int) Gdk.EventMask.KeyReleaseMask);
			
			this.CanFocus = true;
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="KaosEditor.UI.Widgets.DiagramArea"/> class.
		/// </summary>
		/// <param name='view'>
		/// View.
		/// </param>
		public DiagramArea (View view, MainController controller) 
			: this (controller)
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
		
		protected override void OnSizeRequested (ref Gtk.Requisition requisition)
		{
			this.CurrentView.OnSizeRequested(ref requisition);
		}
		
		public void Update ()
		{
			this.QueueDraw();
		}
		
		private List<IShape> selectedShapes = new List<IShape>();
		private PointD lastClickedPoint;
		
		private bool moveShapes = false;
		private bool hasMoved = false;
		private IShape removalCandidate;
		
		private bool controlPressed = false;
		private bool shiftPressed = false;
		
		protected override bool OnMotionNotifyEvent (Gdk.EventMotion evnt)
		{
			hasMoved = true;
			if (moveShapes & selectedShapes.Count > 0) {
				var newPosition = new PointD (evnt.X, evnt.Y);
				
				var deltaX = lastClickedPoint.X - newPosition.X;
				var deltaY = lastClickedPoint.Y - newPosition.Y;
				
				lastClickedPoint = newPosition;
				
				foreach (var s in selectedShapes) {
					s.Position = new PointD(
						s.Position.X - deltaX,
						s.Position.Y - deltaY
						);
				}
				
				this.QueueDraw ();
			}
			
			return true;
		}
		
		protected override bool OnButtonReleaseEvent (Gdk.EventButton evnt)
		{
			if (removalCandidate != null && !hasMoved) {
				if (shiftPressed) {
					selectedShapes.Remove (removalCandidate);
					removalCandidate.Selected = false;
				} else {
					ClearSelection ();
					selectedShapes.Add (removalCandidate);
					removalCandidate.Selected = true;
				}
				removalCandidate = null;
			}
			
			moveShapes = false;
			hasMoved = false;
			
			this.QueueDraw();
			
			return true;
		}
		
		void ClearSelection ()
		{
				foreach (var sshape in selectedShapes) {
					sshape.Selected = false;
				}
				selectedShapes.Clear();
		}
		
		protected override bool OnButtonPressEvent (Gdk.EventButton evnt)
		{
			this.GrabFocus ();
			
			if (evnt.Button == 1) {
					
				moveShapes = true;
				
				lastClickedPoint = new PointD (evnt.X, evnt.Y);
				
				var selectedShape = FindShapeAtPosition(evnt.X, evnt.Y);
				if (selectedShape != null) {
					if (!selectedShapes.Contains(selectedShape)) {
						if (!shiftPressed) {
							ClearSelection ();
						}
						selectedShapes.Add (selectedShape);
						selectedShape.Selected = true;
						
					} else {
						removalCandidate = selectedShape;
					}
				} else {
					ClearSelection ();
				}
				
				this.QueueDraw();
			} else if (evnt.Button == 3) { // Right click
				
				var clickedShape = FindShapeAtPosition(evnt.X, evnt.Y);
				if (clickedShape != null) {
					this.controller.PopulateContextMenu (this, clickedShape.RepresentedElement);
				} else {
					this.controller.PopulateContextMenu (this, null);
				}
				
			}
			
			return true;
		}
		
		protected override bool OnKeyPressEvent (Gdk.EventKey evnt)
		{
			System.Console.WriteLine (evnt.Key);
			System.Console.WriteLine (evnt.KeyValue);
			
			if (evnt.Key == Gdk.Key.Shift_L
				| evnt.Key == Gdk.Key.Shift_R) {
				shiftPressed = true;
				
			} else if (evnt.Key == Gdk.Key.Control_L
				| evnt.Key == Gdk.Key.Control_R) {
				controlPressed = true;
				
			} else if (evnt.Key == Gdk.Key.Left) {
				foreach (var s in selectedShapes) {
					s.Position = new PointD (
						s.Position.X - 1,
						s.Position.Y
						);
				}	
				this.QueueDraw ();
				
			} else if (evnt.Key == Gdk.Key.Right) {
				foreach (var s in selectedShapes) {
					s.Position = new PointD (
						s.Position.X + 1,
						s.Position.Y
						);
				}	
				this.QueueDraw ();
				
			} else if (evnt.Key == Gdk.Key.Up) {
				foreach (var s in selectedShapes) {
					s.Position = new PointD (
						s.Position.X,
						s.Position.Y - 1
						);
				}	
				this.QueueDraw ();
				
			} else if (evnt.Key == Gdk.Key.Down) {
				foreach (var s in selectedShapes) {
					s.Position = new PointD (
						s.Position.X,
						s.Position.Y + 1
						);
				}	
				this.QueueDraw ();
				
			} else if (controlPressed & (evnt.Key == Gdk.Key.a
				| evnt.Key == Gdk.Key.A)) {
				
				selectedShapes.Clear ();
				selectedShapes.AddRange (this.CurrentView.Shapes);
				foreach (var s in selectedShapes) {
					s.Selected = true;
				}
				this.QueueDraw ();
				
			}
			
			return true;
		}
		
		protected override bool OnKeyReleaseEvent (Gdk.EventKey evnt)
		{
			if (evnt.Key == Gdk.Key.Shift_L | evnt.Key == Gdk.Key.Shift_R) {
				shiftPressed = false;
			} else if (evnt.Key == Gdk.Key.Control_L | evnt.Key == Gdk.Key.Control_R) {
				controlPressed = false;
			}
			
			return true;
		}
		
		protected IShape FindShapeAtPosition(double x, double y)
		{
			PointD selectedPoint;
			IShape selectedShape = null;
			foreach (var shape in this.CurrentView.Shapes) {
				if (shape.InBoundingBox(x, y, out selectedPoint)) {
					if (selectedShape == null || shape.Depth > selectedShape.Depth) { 
						selectedShape = shape;
					}
				}
			}
			return selectedShape;
		}
		
	}
}

