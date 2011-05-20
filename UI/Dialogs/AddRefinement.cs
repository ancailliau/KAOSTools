// 
// AddRefinement.cs
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
using System.Collections.Generic;
using Gtk;
using KaosEditor.Controllers;
using KaosEditor.Model;
using KaosEditor.UI.Windows;

namespace KaosEditor.UI.Dialogs
{
	
	/// <summary>
	/// Represents the dialog to add a new refinement
	/// </summary>
	public partial class AddRefinement : Gtk.Dialog
	{
	
		/// <summary>
		/// The store for the combobox containing potential children
		/// </summary>
		private ListStore childrenComboStore;
		
		/// <summary>
		/// The store for the node view.
		/// </summary>
		private ListStore childrenNodeStore;
		
		/// <summary>
		/// The list of refinees.
		/// </summary>
		public List<IModelElement> Refinees {
			get;
			private set;
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="KaosEditor.UI.Dialogs.AddRefinement"/> class.
		/// </summary>
		/// <param name='window'>
		/// Window.
		/// </param>
		/// <param name='parent'>
		/// Parent.
		/// </param>
		public AddRefinement (MainWindow window, Goal parent)
			: base (string.Format("Refine goal {0}", parent.Name), 
				window, DialogFlags.DestroyWithParent)
		{
			this.Build ();
			Refinees = new List<IModelElement> ();
			
			childrenComboStore = new ListStore(typeof(string), typeof(object));
			childrenComboBox.Model = childrenComboStore;
			
			childrenNodeStore = new ListStore (typeof(string), typeof(object));
			childrenNodeView.Model = childrenNodeStore;
			
			CellRendererText cell = new CellRendererText ();
			var col = new TreeViewColumn();
			col.Title = "Children";
			col.PackStart(cell, true);
			col.AddAttribute(cell, "text", 0);
			
			childrenNodeView.AppendColumn(col);
			childrenNodeView.HeadersVisible = false;
			
			cell = new CellRendererText();
			// childrenComboBox.PackStart(cell, false);
			childrenComboBox.AddAttribute(cell, "text", 0);
			
			foreach (var g in window.Model.Elements.FindAll(e => e is Goal)) {
				if (g != parent) {
					childrenComboStore.AppendValues(((Goal) g).Name, g as Goal);
				}
			}
			
			childrenNodeView.RowActivated += delegate(object o, RowActivatedArgs args) {
				TreeIter iter;
				if (childrenNodeStore.GetIter(out iter, args.Path)) {
					childrenNodeStore.Remove(ref iter);
				}
			};
			
		}
		
		/// <summary>
		/// Handles the add button activated event.
		/// </summary>
		/// <param name='sender'>
		/// Sender.
		/// </param>
		/// <param name='e'>
		/// E.
		/// </param>
		protected virtual void OnAddButtonActivated (object sender, System.EventArgs e)
		{
			TreeIter iter = new TreeIter();
			
			if (childrenComboBox.GetActiveIter(out iter)) {
				// Get the element
				var element = (Goal) childrenComboStore.GetValue(iter, 1);
				
				Refinees.Add(element);
				childrenNodeStore.AppendValues(element.Name, element);
			}
		}
	}
}

