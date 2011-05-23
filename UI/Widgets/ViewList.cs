// 
// ViewList.cs
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
using Gtk;
using KaosEditor.Controllers;
using KaosEditor.Model;
using KaosEditor.Logging;
using KaosEditor.UI.Dialogs;

namespace KaosEditor.UI.Widgets
{
	public class ViewList : TreeView
	{
		
		private static Gdk.Pixbuf viewPixbuf;
		static ViewList () {
			try {
				viewPixbuf = Gdk.Pixbuf.LoadFromResource("KaosEditor.Images.View.png");
			} catch (Exception e) {
				Logger.Warning ("Cannot load 'KaosEditor.Images.View.png' from ressources", e);
			}
		}
		
		private MainController controller;
		private ListStore store;
		
		public ViewList (MainController controller)
		{
			if (controller == null) {
				throw new ArgumentNullException("controller");
			}
			
			this.controller = controller;
			this.Build ();
			this.UpdateList ();
		}
		
		public void UpdateList ()
		{
			store.Clear ();
			
			foreach (var view in this.controller.Model.Views) {
				store.AppendValues (view.Name, view, viewPixbuf);
			}
		}
		
		private void Build ()
		{
			// Remove headers
			this.HeadersVisible = false;
			
			// Create the store
			store = new ListStore (typeof(string), typeof(View), typeof(Gdk.Pixbuf));
			this.Model = store;
			
			// Initialize columns
			//this.AppendColumn ("Icon", new CellRendererPixbuf (), "pixbuf", 2);
			var column = new TreeViewColumn ();
			var iconCellRenderer = new CellRendererPixbuf ();
			column.PackStart (iconCellRenderer, false);
			
			var textCellRenderer = new CellRendererText ();
			column.PackStart (textCellRenderer, true);
			
			column.AddAttribute (iconCellRenderer, "pixbuf", 2);
			column.AddAttribute (textCellRenderer, "text", 0);
			
			this.AppendColumn (column);
			
			this.RowActivated += OnRowActivated;
			
			this.AddEvents((int) Gdk.EventMask.ButtonPressMask);
			this.ButtonPressEvent += HandleHandleButtonPressEvent;;
			
		}

		[GLib.ConnectBeforeAttribute]
		void HandleHandleButtonPressEvent (object sender, ButtonPressEventArgs args)
		{
			if (args.Event.Button == 3) {
				var path = new TreePath();
				this.GetPathAtPos(System.Convert.ToInt16(args.Event.X), 
					System.Convert.ToInt16(args.Event.Y), out path);
				
				if (path != null) {
					TreeIter iter;
					if (store.GetIter(out iter, path)) {
						object o = store.GetValue (iter, 1);
						
					}
				} else {
					var menu = new Menu ();
					var addItem = new MenuItem ("Add view...");
					addItem.Activated += delegate(object sender3, EventArgs e3) {
						var dialog = new AddView (this.controller.Window);
						dialog.Response += delegate(object sender2, Gtk.ResponseArgs args2) {
							if (args2.ResponseId == Gtk.ResponseType.Ok & dialog.ViewName != "") {
								this.controller.Model.Views.Add (dialog.ViewName);
								this.controller.Window.DisplayView (dialog.ViewName);
							}
							dialog.Destroy ();
						};
						dialog.Present ();	
					};
					menu.Add (addItem);
					menu.ShowAll ();
					menu.Popup ();
				}
			}
		}
		
		/// <summary>
		/// Handles the event row activated.
		/// </summary>
		/// <param name='o'>
		/// O.
		/// </param>
		/// <param name='args'>
		/// Arguments.
		/// </param>
		void OnRowActivated (object o, RowActivatedArgs args)
		{
			TreeIter iter;
			store.GetIter(out iter, args.Path);
			
			string name = (string) store.GetValue(iter, 0);
			this.controller.Window.DisplayView(name);
		}
		
	}
}

