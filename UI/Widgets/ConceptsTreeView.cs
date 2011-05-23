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

namespace KaosEditor.UI.Widgets
{
	
	/// <summary>
	/// Represents a tree view for the KAOS model elements.
	/// </summary>
	public class ConceptsTreeView : TreeView
	{
		
		private static Gdk.Pixbuf goalPixbuf;
		private static Gdk.Pixbuf refinementPixbuf;
		private static Gdk.Pixbuf agentPixbuf;
		private static Gdk.Pixbuf responsibilityPixbuf;
		
		static ConceptsTreeView () {
			try {
				goalPixbuf = Gdk.Pixbuf.LoadFromResource("KaosEditor.Images.Goal.png");
				refinementPixbuf = Gdk.Pixbuf.LoadFromResource("KaosEditor.Images.Refinement.png");
				agentPixbuf = Gdk.Pixbuf.LoadFromResource("KaosEditor.Images.Agent.png");
				responsibilityPixbuf = Gdk.Pixbuf.LoadFromResource("KaosEditor.Images.Responsibility.png");
				
			} catch (Exception e) {
				Logger.Warning ("Cannot load images from ressources", e);
			}
		}
		
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
			
			var iter = store.AppendValues("Goals", null, goalPixbuf);
			List<KAOSElement> goals = this.window.Model.Elements.FindAll (e => e is Goal);
			foreach (var element in goals) {
				AddGoalElement (iter, element as Goal, expandedNodes);
			}
			
			iter = store.AppendValues("Agents", null, agentPixbuf);
			var agents = from e in this.window.Model.Elements
				where e is Agent select (Agent) e;
			
			foreach (var element in agents) {
				store.AppendValues(iter, element.Name, element, agentPixbuf);
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
			var iiter = store.AppendValues(iter, g.Name, g, goalPixbuf);
			var refinements = from e in this.window.Controller.Model.Elements
				where e is Refinement && ((Refinement) e).Refined.Equals (g)
				select (Refinement) e;
				
			if (refinements.Count() > 1) {
				int i = 1;
				foreach (var refinement in refinements) {
					var iiiter = store.AppendValues(iiter, string.Format ("Alternative {0}", i++), refinement, refinementPixbuf);
					foreach (var g2 in refinement.Refinees) {
						if (g2 is Goal) {
							AddGoalElement (iiiter, g2 as Goal, expandedNodes);
						}
					}
				}
			} else if (refinements.Count() > 0) {
				foreach (var g2 in refinements.First().Refinees) {
					if (g2 is Goal) {
						AddGoalElement (iiter, g2 as Goal, expandedNodes);
					}
				}
			}
			
			var responsibilities = from e in this.window.Model.Elements 
				where e is Responsibility && ((Responsibility) e).Goal.Equals (g) 
					select (Responsibility) e;
			if (responsibilities.Count() > 1) {
				int i = 1;
				foreach (var responsibility in responsibilities) {
					var iiiter = store.AppendValues(iiter, string.Format("Alternative '{0}'", i++), responsibility, responsibilityPixbuf);
					store.AppendValues (iiiter, responsibility.Agent.Name, responsibility, agentPixbuf);
				}
			} else if (responsibilities.Count() > 0) {
				var responsibility = responsibilities.First();
				store.AppendValues (iiter, responsibility.Agent.Name, responsibility, agentPixbuf);
			}
		}
	}
}

