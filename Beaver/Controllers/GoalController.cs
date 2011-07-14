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
using Beaver.Model;
using Beaver.UI.Dialogs;
using Gtk;
using Beaver.UI.Widgets;
using Beaver.Logging;
using System.Collections.Generic;

namespace Beaver.Controllers
{
	public class GoalController : IController, IPopulateTree, IPopulateMenu
	{
		private static Gdk.Pixbuf pixbuf;
		
		static GoalController () {
			try {
				pixbuf = Gdk.Pixbuf.LoadFromResource("Beaver.Images.Goal.png");
				
			} catch (Exception e) {
				Logger.Warning ("Cannot load images from ressources", e);
			}
		}
		
		public delegate void HandleGoalAdded (Goal goal);
		public delegate void HandleGoalRemoved (Goal goal);
		public delegate void HandleGoalUpdated (Goal goal);
		
		public event HandleGoalAdded GoalAdded;
		public event HandleGoalRemoved GoalRemoved;
		public event HandleGoalUpdated GoalUpdated;
		
		private MainController controller;
		private List<Goal> goals = new List<Goal> ();
		
		public GoalController (MainController controller)
		{
			this.controller = controller;
			this.controller.Window.conceptTreeView.RegisterForTree (this);
			this.controller.Window.conceptTreeView.RegisterForMenu (this);
			this.controller.Window.viewsNotebook.RegisterForDiagramMenu (this);
			
			this.GoalAdded += UpdateLists;
			this.GoalRemoved += UpdateLists;
			this.GoalUpdated += UpdateLists;
		}
		
		private void UpdateLists (Goal goal) {
			this.controller.Window.conceptTreeView.Update ();
			this.controller.ViewController.RefreshCurrentView ();
		}
		
		public IEnumerable<Goal> GetAll ()
		{
			return this.goals.AsEnumerable ();
		}
		
		public void Add (Goal goal)
		{
			Add (goal, true);
		}
		
		public void Add (Goal goal, bool notify)
		{
			this.goals.Add (goal);
			if (GoalAdded != null & notify) {
				GoalAdded (goal);
			}
		}
		
		public void Remove (Goal goal)
		{
			this.goals.Remove (goal);
			if (GoalRemoved != null) {
				GoalRemoved (goal);
			}
		}
		
		public void Update (Goal goal)
		{
			if (GoalUpdated != null) {
				GoalUpdated (goal);
			}
		}
		
		public Goal Get (string id)
		{
			return this.goals.Find ((obj) => obj.Id == id);
		}
		
		public void AddGoal (string goalName, System.Action<Goal> action)
		{
			var dialog = new AddGoalDialog(controller.Window, goalName);
			dialog.Response += delegate(object o, Gtk.ResponseArgs args) {
				if (args.ResponseId == Gtk.ResponseType.Ok) {
					var goal = new Goal(dialog.GoalName, dialog.GoalDefinition);
					this.Add (goal);
					action(goal);
				}
				dialog.Destroy();
			};
		}
		
		public void AddGoal (string goalName)
		{
			AddGoal (goalName, (obj) => {});
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
					this.Update (goal);
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
					this.Remove (goal);
				}
				dialog.Destroy ();
			};
			
			dialog.Present ();
		}
		
		public bool PopulateContextMenu (Menu menu, object source, object clickedElement)
		{
			bool retVal = false;
			
			if (clickedElement == null) {
				var addItem = new MenuItem("Add goal...");
				addItem.Activated += delegate(object sender2, EventArgs e) {
					this.AddGoal ("", (obj) => {
						if (source is DiagramArea) {
							this.controller.ViewController.AddToCurrentView (obj);
						}
					});
				};
				menu.Add(addItem);
				retVal = true;
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
				
				var computeItem = new MenuItem("Compute likelihood");
				computeItem.Activated += delegate(object sender2, EventArgs e) {
					this.ComputeLikelihood (clickedGoal);
					if (GoalUpdated != null)
						GoalUpdated (clickedGoal);
				};
				menu.Add(computeItem);
				
				retVal = true;
			}
			
			return retVal;
		}
		
		public void Populate (TreeStore store)
		{
			var iter = store.AppendValues ("Goals", null, pixbuf);
			Populate (this.GetAll().Cast<KAOSElement>(), store, iter);
		}
		
		public void Populate (IEnumerable<KAOSElement> elements, TreeStore store, TreeIter iter)
		{
			foreach (var goal in from e in elements where e is Goal select (Goal) e) {
				var subIter = store.AppendValues (iter, goal.Name, goal, pixbuf);
				
				var refinements = this.controller.RefinementController.GetAll (goal).Cast<KAOSElement> ();
				this.controller.RefinementController.Populate (refinements, store, subIter);
				
				var obstructions = this.controller.ObstructionController.GetAll (goal).Cast<KAOSElement> ();
				this.controller.ObstructionController.Populate (obstructions, store, subIter);
				
				var reponsibilities = this.controller.ResponsibilityController.GetAll (goal).Cast<KAOSElement> ();
				this.controller.ResponsibilityController.Populate (reponsibilities, store, subIter);
				
				var exceptions = this.controller.ExceptionController.GetAll (goal).Cast<KAOSElement> ();
				this.controller.ExceptionController.Populate (exceptions, store, subIter);
				
			}
		}
		
		public float ComputeLikelihood (Goal g)
		{
			float l = 0;
			if (this.controller.ResponsibilityController.GetAll(g).Count() > 0) {
				l = 1;
				foreach (var obst in this.controller.ObstructionController.GetAll (g)) {
					l -= this.controller.ObstacleController.ComputeLikelihood (obst.Obstacle);
				}
				
			} else {
				l = 0;
				foreach (var refinement in this.controller.RefinementController.GetAll (g)) {
					l += this.controller.RefinementController.ComputeLikelihood (refinement);
				}
			}
			g.Likelihood = Math.Min (1, Math.Max (0, l));
			
			return l;
		}
	}
}

