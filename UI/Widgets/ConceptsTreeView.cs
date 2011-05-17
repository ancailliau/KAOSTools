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
using System.Collections.Generic;
using Gtk;
using KaosEditor.Events;
using KaosEditor.Model;
using KaosEditor.UI.Windows;
using KaosEditor.UI.Dialogs;

namespace KaosEditor.UI.Widgets
{
	
	/// <summary>
	/// Represents a tree view for the KAOS model elements.
	/// </summary>
	public class ConceptsTreeView : TreeView
	{
		
		/// <summary>
		/// The window.
		/// </summary>
		private MainWindow window;
		
		/// <summary>
		/// The store.
		/// </summary>
		public TreeStore store;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="KaosEditor.UI.Widgets.ConceptsTreeView"/> class.
		/// </summary>
		/// <param name='window'>
		/// Window.
		/// </param>
		public ConceptsTreeView (MainWindow window)
		{
			// Bind main window
			this.window = window;
			
			// Build column and renderers	
			var column = new TreeViewColumn ();
			column.Title = "Concepts";
			this.AppendColumn (column);
			this.HeadersVisible = false;
			
			var cell = new CellRendererText();
			column.PackStart(cell, true);
			column.AddAttribute(cell, "text", 0);
			
			// Bind the model
			store = new TreeStore(typeof(string), typeof(object));
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
			store.GetIter(out iter, args.Path);
			
			bool inViews = false;
			if (args.Path.Depth > 1 && args.Path.Up()) {
				TreeIter iterParent;
				Console.WriteLine ();
				store.GetIter(out iterParent, args.Path);
				if (((string) store.GetValue(iterParent, 0)) == "Views") {
					inViews = true;
				}
			}
			
			if (inViews) {
				string name = (string) store.GetValue(iter, 0);
				this.window.DisplayView(name);
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
				
				TreeIter iter;
				if (store.GetIter(out iter, path)) {
					object o = store.GetValue(iter, 1);
					if (o != null & o is IContextMenu) {
						var m = new Menu();
						((IContextMenu) o).PopulateContextMenu(m, new MenuContext(this, this.window.Controller));
						m.ShowAll();
						m.Popup();
						
					} else if (((string) store.GetValue(iter, 0)) == "Goals") {
						var m = new Menu();
						var addGoal = new MenuItem("Add goal");
						addGoal.Activated += delegate(object sender2, EventArgs e) {
							var ag = new AddGoal(window, new MenuContext (this, this.window.Controller));
							ag.Present();
						};
						m.Add(addGoal);
						m.ShowAll();
						m.Popup();
						
					} else if (((string) store.GetValue(iter, 0)) == "Views") {
						var m = new Menu();
						var addView = new MenuItem("Add view");
						addView.Activated += delegate(object sender2, EventArgs e) {
							var ag = new AddView(window);
							ag.Present();
						};
						m.Add(addView);
						m.ShowAll();
						m.Popup();
					
					} else if (((string) store.GetValue(iter, 0)) == "Agents") {
						var m = new Menu();
						var addAgent = new MenuItem("Add agent");
						addAgent.Activated += delegate(object sender2, EventArgs e) {
							var ag = new AddAgent(window, new MenuContext (this, this.window.Controller));
							ag.Present();
						};
						m.Add(addAgent);
						m.ShowAll();
						m.Popup();
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
			
			foreach (var str in expandedNodes) {
				Console.WriteLine (str);
			}
			
			store.Clear();
			
			var iter = store.AppendValues("Goals", null);
			foreach (var element in this.window.Model.Elements.FindAll(e => e is Goal)) {
				AddGoalElement (iter, element as Goal, expandedNodes);
			}
			
			iter = store.AppendValues("Agents", null);
			foreach (var element in this.window.Model.Elements.FindAll(e => e is Agent)) {
				store.AppendValues(iter, element.Name, element);
			}
			
			iter = store.AppendValues("Views", null);
			foreach (var view in this.window.Model.Views) {
				store.AppendValues(iter, view.Name, view);
			}
			
			RestoreState (expandedNodes);
		}
		
		/// <summary>
		/// Adds the goal item element.
		/// </summary>
		/// <param name='iter'>
		/// Iter.
		/// </param>
		/// <param name='g'>
		/// G.
		/// </param>
		/// <param name='expandedNodes'>
		/// Expanded nodes.
		/// </param>
		private void AddGoalElement (TreeIter iter, Goal g, List<string> expandedNodes)
		{
			var iiter = store.AppendValues(iter, g.Name.Replace("\n", ""), g);
			foreach (var refinement in g.Refinements) {
				var iiiter = store.AppendValues(iiter, refinement.Name, refinement);
				foreach (var g2 in refinement.Refinees) {
					if (g2 is Goal) {
						AddGoalElement (iiiter, g2 as Goal, expandedNodes);
					} else {
						store.AppendValues (iiiter, g2.Id, g2);
					}
				}
			}
			
			foreach (var responsibility in g.Responsibilities) {
				var iiiter = store.AppendValues(iiter, responsibility.Name, responsibility);
				store.AppendValues (iiiter, responsibility.Agent.Name, responsibility);
			}
		}
	}
}

