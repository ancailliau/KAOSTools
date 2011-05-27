// 
// AddObstacleRefinement.cs
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
using Beaver.Model;
using Gtk;
using Beaver.Controllers;
using Beaver.UI.Windows;
using System.Collections.Generic;

namespace Beaver.UI.Dialogs
{
	public partial class AddObstacleRefinementDialog : Gtk.Dialog
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
		public List<KAOSElement> Refinees {
			get;
			private set;
		}
		
		private MainController controller;
		
		public AddObstacleRefinementDialog (MainController controller, Obstacle parent)
			: this (controller, parent, new List<KAOSElement> ())
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Beaver.UI.Dialogs.AddRefinementDialog"/> class.
		/// </summary>
		/// <param name='window'>
		/// Window.
		/// </param>
		/// <param name='parent'>
		/// Parent.
		/// </param>
		public AddObstacleRefinementDialog (MainController controller, Obstacle parent, List<KAOSElement> refinees)
			: base (string.Format("Refine obstacle {0}", parent.Name), 
				controller.Window, DialogFlags.DestroyWithParent)
		{
			this.Build ();
			this.controller = controller;
			
			Refinees = new List<KAOSElement> ();
			
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
			
			foreach (var g in this.controller.ObstacleController.GetAll ()) {
				if (g != parent) {
					childrenComboStore.AppendValues(((Obstacle) g).Name, g as Obstacle);
				}
			}
			
			this.AddEvents((int) Gdk.EventMask.ButtonPressMask);
			childrenNodeView.ButtonPressEvent += HandleButtonPressEvent;
			
			foreach (var refinee in refinees) {
				AddRefinee (refinee as Obstacle);
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
				var element = (Obstacle) childrenComboStore.GetValue(iter, 1);
				AddRefinee (element);
			} else {
				if (childrenComboBox.ActiveText.Trim () != "") {
					string obsacleName = childrenComboBox.ActiveText.Trim ();
					var dialog = new MessageDialog (this.controller.Window,
						DialogFlags.DestroyWithParent, MessageType.Question,
						ButtonsType.YesNo, false, string.Format ("Create new obstacle '{0}'?", obsacleName));
					
					dialog.Response += delegate(object o, ResponseArgs args) {
						if (args.ResponseId == Gtk.ResponseType.Yes) {
							this.controller.ObstacleController.AddObstacle (obsacleName, delegate (Obstacle obstacle) {
								childrenComboStore.AppendValues(obstacle.Name, obstacle);
								AddRefinee (obstacle);
							});
						}
						dialog.Destroy ();
					};
					
					dialog.Present ();
				}
			}
		}
		
		private void AddRefinee (Obstacle element)
		{
			if (element == null) throw new ArgumentNullException ("element");
			
			Refinees.Add(element);
			childrenNodeStore.AppendValues(element.Name, element);
		}
	}
}

