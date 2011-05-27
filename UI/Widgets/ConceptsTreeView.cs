// 
// ConceptsTreeView.cs
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
using System.Linq;
using System.Collections.Generic;
using Gtk;
using KaosEditor.Events;
using KaosEditor.Model;
using KaosEditor.UI.Windows;
using KaosEditor.UI.Dialogs;
using KaosEditor.Logging;
using KaosEditor.Controllers;

namespace KaosEditor.UI.Widgets
{
	
	/// <summary>
	/// Represents a tree view for the KAOS model elements.
	/// </summary>
	public class ConceptsTreeView : TreeView
	{
		public delegate void HandleElementActivated (object activatedObject);
		public event HandleElementActivated ElementActivated;
		
		public TreeStore store;
		
		public ConceptsTreeView ()
		{
			// Build column and renderers	
			var column = new TreeViewColumn ();
			this.AppendColumn (column);
			this.HeadersVisible = false;
			
			var cell = new CellRendererText();
			var iconCell = new CellRendererPixbuf();
			column.PackStart(iconCell, false);
			column.PackStart(cell, true);
			column.AddAttribute (iconCell, "pixbuf", 2);
			column.AddAttribute (cell, "text", 0);
			
			this.EnableSearch = true;
			this.SearchColumn = 1;
			
			// Bind the model
			store = new TreeStore(typeof(string), typeof(object), typeof(Gdk.Pixbuf));
			this.Model = store;
			
			// Bind the events
			this.AddEvents((int) Gdk.EventMask.ButtonPressMask);
			this.ButtonPressEvent += OnItemButtonPressed;
			this.RowActivated += OnRowActivated;	
			
			// Set up the content
			this.Update ();
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
			if (store.GetIter(out iter, args.Path)) {
				if (ElementActivated != null) {
					ElementActivated (store.GetValue (iter, 1));
				}
			}
		}
		
		/// <summary>
		/// Handles the item button pressed event.
		/// </summary>
		/// <param name='sender'>
		/// Sender.
		/// </param>
		/// <param name='args'>
		/// Arguments.
		/// </param>
		[GLib.ConnectBeforeAttribute]
		protected void OnItemButtonPressed (object sender, ButtonPressEventArgs args) 
		{
			if (args.Event.Button == 3) {
				var path = new TreePath();
				this.GetPathAtPos(System.Convert.ToInt16(args.Event.X), 
					System.Convert.ToInt16(args.Event.Y), out path);
				
				if (path == null) {
					var menu = new Menu();
					foreach (var p in menuPopulater) {
						p.PopulateContextMenu (menu, this, null);
					}
					menu.ShowAll ();
					menu.Popup ();
					
				} else {
					TreeIter iter;
					if (store.GetIter(out iter, path)) {
						object o = store.GetValue(iter, 1);
						var menu = new Menu();
						SeparatorMenuItem separator = null;
						foreach (var p in menuPopulater) {
							if (p.PopulateContextMenu (menu, this, o)) {
								separator = new SeparatorMenuItem ();
								menu.Add (separator);
							}
						}
						menu.Remove (separator);
						menu.ShowAll ();
						menu.Popup ();
					}
				}
			}
		}
		
		/// <summary>
		/// Saves the state.
		/// </summary>
		/// <param name='expandedNodes'>
		/// Expanded nodes.
		/// </param>
		private void SaveState (List<string> expandedNodes)
		{
			var iter = new TreeIter();
			if(store.GetIterFirst(out iter)) {
				do {
					SaveStateRecursiveInternal (expandedNodes, iter, "");
				} while (store.IterNext (ref iter));
			}
		}
		
		/// <summary>
		/// Saves the state (internal recursive function).
		/// </summary>
		/// <param name='expandedNodes'>
		/// Expanded nodes.
		/// </param>
		/// <param name='iter'>
		/// Iter.
		/// </param>
		/// <param name='prefix'>
		/// Prefix.
		/// </param>
		private void SaveStateRecursiveInternal (List<string> expandedNodes, TreeIter iter, string prefix)
		{
			var path = store.GetPath(iter);
			bool expanded = this.GetRowExpanded(path);
			string str = ((string)store.GetValue (iter, 0));
			if (expanded) {
				expandedNodes.Add(prefix + "." + str);
			}
			
			if (store.IterHasChild(iter)) {
				var childIter = new TreeIter();
				store.IterChildren(out childIter, iter);
				do {
					SaveStateRecursiveInternal (expandedNodes, childIter, prefix + "." + str);
				} while (store.IterNext(ref childIter));
			}
		}
		
		/// <summary>
		/// Restores the state.
		/// </summary>
		/// <param name='expandedNodes'>
		/// Expanded nodes.
		/// </param>
		private void RestoreState (List<string> expandedNodes)
		{
			var iter = new TreeIter();
			if(store.GetIterFirst(out iter)) {
				do {
					RestoreStateRecursiveInternal (expandedNodes, iter, "");
				} while (store.IterNext (ref iter));
			}
		}
		
		/// <summary>
		/// Restores the state (internal recursive function).
		/// </summary>
		/// <param name='expandedNodes'>
		/// Expanded nodes.
		/// </param>
		/// <param name='iter'>
		/// Iter.
		/// </param>
		/// <param name='prefix'>
		/// Prefix.
		/// </param>
		private void RestoreStateRecursiveInternal (List<string> expandedNodes, TreeIter iter, string prefix)
		{
			var path = store.GetPath(iter);
			string str = ((string)store.GetValue (iter, 0));
			if (expandedNodes.Contains(prefix + "." + str)) {
				this.ExpandRow(path, false);
			}
			
			if (store.IterHasChild(iter)) {
				var childIter = new TreeIter();
				store.IterChildren(out childIter, iter);
				do {
					RestoreStateRecursiveInternal (expandedNodes, childIter, prefix + "." + str);
				} while (store.IterNext(ref childIter));
			}
		}
		
		/// <summary>
		/// Update the list, according model
		/// </summary>
		public void Update()
		{
			// Save expand/collapse state
			List<string> expandedNodes = new List<string>();
			SaveState(expandedNodes);
			
			store.Clear();
			
			foreach (var p in treePopulater) {
				p.Populate (store);
			}
			
			RestoreState (expandedNodes);
		}
		
		private List<IPopulateTree> treePopulater = new List<IPopulateTree>();
		private List<IPopulateMenu> menuPopulater = new List<IPopulateMenu>();
		
		public void RegisterForTree (IPopulateTree populater)
		{
			this.treePopulater.Add (populater);
		}
		
		public void RegisterForMenu (IPopulateMenu populater)
		{
			this.menuPopulater.Add (populater);
		}
	}
}

