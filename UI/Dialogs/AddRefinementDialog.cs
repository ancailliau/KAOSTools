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
	public partial class AddRefinementDialog : Gtk.Dialog
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
		public List<IGoalRefinee> Refinees {
			get;
			private set;
		}
		
		private MainController controller;
		
		public AddRefinementDialog (MainController controller, Goal parent)
			: this (controller, parent, new List<IGoalRefinee> ())
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="KaosEditor.UI.Dialogs.AddRefinementDialog"/> class.
		/// </summary>
		/// <param name='window'>
		/// Window.
		/// </param>
		/// <param name='parent'>
		/// Parent.
		/// </param>
		public AddRefinementDialog (MainController controller, Goal parent, List<IGoalRefinee> refinees)
			: base (string.Format("Refine goal {0}", parent.Name), 
				controller.Window, DialogFlags.DestroyWithParent)
		{
			this.Build ();
			this.controller = controller;
			
			Refinees = new List<IGoalRefinee> ();
			
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
			
			foreach (var g in this.controller.GoalController.GetAll ()) {
				if (g != parent) {
					childrenComboStore.AppendValues(g.Name, g);
				}
			}
			foreach (var domProp in this.controller.DomainPropertyController.GetAll ()) {
				childrenComboStore.AppendValues (domProp.Name, domProp);
			}
			
			this.AddEvents((int) Gdk.EventMask.ButtonPressMask);
			childrenNodeView.ButtonPressEvent += HandleButtonPressEvent;
			
			foreach (var refinee in refinees) {
				AddRefinee (refinee as Goal);
			}
		}
		
		[GLib.ConnectBeforeAttribute]
		private void HandleButtonPressEvent (object sender, ButtonPressEventArgs args) 
		{
			if (args.Event.Button == 3) {
				TreeIter iter;
				var path = new TreePath();
				childrenNodeView.GetPathAtPos(System.Convert.ToInt16(args.Event.X), 
					System.Convert.ToInt16(args.Event.Y), out path);
				
				if (childrenNodeStore.GetIter(out iter, path)) {
					var menu = new Menu();
					var removeItem = new MenuItem ("Remove");
					removeItem.Activated += delegate(object sender2, EventArgs e) {
						childrenNodeStore.Remove(ref iter);
					};
					menu.Add (removeItem);
					menu.ShowAll ();
					menu.Popup ();
				}
			}
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
				AddRefinee (childrenComboStore.GetValue(iter, 1) as IGoalRefinee);
				
			} else {
				if (childrenComboBox.ActiveText.Trim () != "") {
					string newGoalName = childrenComboBox.ActiveText.Trim ();
					var dialog = new MessageDialog (this.controller.Window,
						DialogFlags.DestroyWithParent, MessageType.Question,
						ButtonsType.YesNo, false, string.Format ("Create new goal '{0}'?", newGoalName));
					
					dialog.Response += delegate(object o, ResponseArgs args) {
						if (args.ResponseId == Gtk.ResponseType.Yes) {
							this.controller.GoalController.AddGoal (newGoalName, delegate (Goal newGoal) {
								childrenComboStore.AppendValues(newGoal.Name, newGoal);
								AddRefinee (newGoal);
							});
						}
						dialog.Destroy ();
					};
					
					dialog.Present ();
				}
			}
		}
		
		private void AddRefinee (IGoalRefinee element)
		{
			Refinees.Add(element);
			childrenNodeStore.AppendValues(element.Name, element);
		}
	}
}

