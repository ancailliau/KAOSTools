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
			
		}
		
	}
}

