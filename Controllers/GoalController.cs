// 
// GoalController.cs
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
using KaosEditor.Model;
using KaosEditor.UI.Dialogs;
using Gtk;
using KaosEditor.UI.Widgets;
using KaosEditor.Logging;

namespace KaosEditor.Controllers
{
	public class GoalController : IController
	{
	
		private static Gdk.Pixbuf pixbuf;
		
		static GoalController () {
			try {
				pixbuf = Gdk.Pixbuf.LoadFromResource("KaosEditor.Images.Goal.png");
				
			} catch (Exception e) {
				Logger.Warning ("Cannot load images from ressources", e);
			}
		}
		
		private MainController controller;
		
		public GoalController (MainController controller)
		{
			this.controller = controller;
		}
		
		
		public void AddGoal (string goalName, System.Action<Goal> action)
		{
			var dialog = new AddGoalDialog(controller.Window, goalName);
			dialog.Response += delegate(object o, Gtk.ResponseArgs args) {
				if (args.ResponseId == Gtk.ResponseType.Ok) {
					var goal = new Goal(dialog.GoalName, dialog.GoalDefinition);
					controller.Model.Add (goal);
					action(goal);
				}
				dialog.Destroy();
			};
		}
		
		public void AddGoal (string goalName)
		{
			var dialog = new AddGoalDialog(controller.Window, goalName);
			dialog.Response += delegate(object o, Gtk.ResponseArgs args) {
				if (args.ResponseId == Gtk.ResponseType.Ok) {
					var goal = new Goal(dialog.GoalName, dialog.GoalDefinition);
					controller.Model.Add (goal);
				}
				dialog.Destroy();
			};
			
			dialog.Present ();
		}
		
		public void AddGoal ()
		{
			AddGoal ("");
		}
		
		public void EditGoal (Goal goal)
		{
			var dialog = new AddGoalDialog(controller.Window, goal);
			dialog.Response += delegate(object o, Gtk.ResponseArgs args) {
				if (args.ResponseId == Gtk.ResponseType.Ok) {
					goal.Name = dialog.GoalName;
					goal.Definition = dialog.GoalDefinition;
				}
				dialog.Destroy();
			};
			
			dialog.Present ();
		}
		
		public void RemoveGoal (Goal goal)
		{
			var dialog = new MessageDialog (this.controller.Window,
				DialogFlags.DestroyWithParent, MessageType.Question,
				ButtonsType.YesNo, false, string.Format ("Delete goal '{0}'?", goal.Name));
			
			dialog.Response += delegate(object o, ResponseArgs args) {
				if (args.ResponseId == Gtk.ResponseType.Yes) {
					this.controller.Model.Remove (goal);
				}
				dialog.Destroy ();
			};
			
			dialog.Present ();
		}
		
		public void PopulateContextMenu (Menu menu, object source, object clickedElement)
		{
			if (clickedElement == null & source is ConceptsTreeView) {				
				var addItem = new MenuItem("Add goal...");
				addItem.Activated += delegate(object sender2, EventArgs e) {
					this.AddGoal ();
				};
				menu.Add(addItem);
			}
			
			if (clickedElement is Goal) {
				var clickedGoal = (Goal) clickedElement;
				
				var editItem = new MenuItem("Edit...");
				editItem.Activated += delegate(object sender2, EventArgs e) {
					this.EditGoal (clickedGoal);
				};
				menu.Add(editItem);
				
				var deleteItem = new MenuItem("Delete");
				deleteItem.Activated += delegate(object sender2, EventArgs e) {
					this.RemoveGoal (clickedGoal);
				};
				menu.Add(deleteItem);
				
				menu.Add (new SeparatorMenuItem ());
				
			}
		}
		
		public void PopulateTree (TreeStore store, bool header)
		{
			if (header) {
				var iter = store.AppendValues ("Goals", null, pixbuf);
				PopulateTree (store, iter, false);
				
			} else {
				var goals = from e in this.controller.Model.Elements
					where e is Goal select (Goal) e;
			
				foreach (var goal in goals) {
					store.AppendValues (goal.Name, goal, pixbuf);
				}
			}
		}
		
		public void PopulateTree (TreeStore store, TreeIter iter, bool header)
		{
			var goals = from e in this.controller.Model.Elements
				where e is Goal select (Goal) e;
			
			if (header) {
				iter = store.AppendValues (iter, "Goals", null, pixbuf);
			}
			PopulateTree (goals.ToArray(), store, iter);
		}
		
		public void PopulateTree (KAOSElement[] elements, TreeStore store, TreeIter iter)
		{
			foreach (var goal in from e in elements where e is Goal select (Goal) e) {
				var subIter = store.AppendValues (iter, goal.Name, goal, pixbuf);
				
				var refinements = from e in this.controller.Model.Elements
					where e is Refinement && ((Refinement) e).Refined == goal
						select (Refinement) e;
				this.controller.PopulateTree (refinements.ToArray(), store, subIter);
				
				var obstructions = from e in this.controller.Model.Elements
					where e is Obstruction && ((Obstruction) e).Goal == goal
						select (Obstruction) e;
				this.controller.PopulateTree (obstructions.ToArray(), store, subIter);
				
				var reponsibilities = from e in this.controller.Model.Elements
					where e is Responsibility && ((Responsibility) e).Goal == goal
						select (Responsibility) e;
				this.controller.PopulateTree (reponsibilities.ToArray(), store, subIter);
				
			}
		}
	}
}

