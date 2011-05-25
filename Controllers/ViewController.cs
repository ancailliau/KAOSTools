// 
// ViewController.cs
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
using System;
using KaosEditor.Model;
using KaosEditor.UI.Dialogs;
using Gtk;
using KaosEditor.UI.Widgets;

namespace KaosEditor.Controllers
{
	public class ViewController
	{
		
		private MainController controller;
		
		public ViewController (MainController controller)
		{
			this.controller = controller;
		}
		
		public void AddView ()
		{
			var addViewDialog = new AddViewDialog (this.controller.Window);
			addViewDialog.Response += delegate(object sender, Gtk.ResponseArgs args) {
				if (args.ResponseId == Gtk.ResponseType.Ok & addViewDialog.ViewName != "") {
					this.controller.Model.Views.Add (addViewDialog.ViewName);
					this.controller.Window.DisplayView (addViewDialog.ViewName);
				}
				addViewDialog.Destroy ();
			};
			addViewDialog.Present ();	
		}
		
		public void DuplicateView (View view)
		{
			var addViewDialog = new AddViewDialog (this.controller.Window, view.Name + " (copy)");
			addViewDialog.Response += delegate(object sender, Gtk.ResponseArgs args) {
				if (args.ResponseId == Gtk.ResponseType.Ok & addViewDialog.ViewName != "") {
					var newView = new View (addViewDialog.ViewName, this.controller);
					foreach (var s in view.Shapes) {
						newView.Add (s.Copy ());
					}
					this.controller.Model.Views.Add (newView);
					this.controller.Window.DisplayView (addViewDialog.ViewName);
				}
				addViewDialog.Destroy ();
			};
			addViewDialog.Present ();
		}
		
		public void EditView (View view)
		{
			throw new NotImplementedException ();
		}
		
		public void RemoveView (View view)
		{
			throw new NotImplementedException ();
		}
		
		public void PopulateContextMenu (Menu menu, object source, object clickedElement)
		{
			if (clickedElement == null & source is ViewList) {
				var addViewItem = new MenuItem("Add view...");
				addViewItem.Activated += delegate(object sender2, EventArgs e) {
					this.AddView ();
				};
				menu.Add(addViewItem);
			}
			
			if (clickedElement is View) {
				var clickedView = clickedElement as View;
				
				var duplicateItem = new MenuItem("Duplicate view...");
				duplicateItem.Activated += delegate(object sender2, EventArgs e) {
					this.DuplicateView (clickedView);
				};
				menu.Add(duplicateItem);
			}
		}
	}
}

