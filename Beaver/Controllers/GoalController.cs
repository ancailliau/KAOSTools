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
using Beaver.UI;

namespace Beaver.Controllers
{
	public class GoalController : IController, IPopulateTree
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
			this.controller.Window.conceptTreeView.RegisterForMenu (this.PopulateContextMenu);
			this.controller.Window.viewsNotebook.RegisterForDiagramMenu (this.PopulateContextMenu);
			
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
		
		public void AddGoal (string goalName, System.Action<Goal> action, 
			int x, int y, bool addToCurrentView)
		{
			var dialog = new AddGoalDialog(controller.Window, goalName) {
				AddToCurrentView = addToCurrentView
			};
			dialog.Response += delegate(object o, Gtk.ResponseArgs args) {
				
				// If user clicked 'Ok' and the fields are valid
				if (args.ResponseId == Gtk.ResponseType.Ok & dialog.IsValid) {
					
					// Create the goal
					var goal = new Goal(dialog.GoalName, dialog.GoalDefinition) {
						SoftThreshold = dialog.SoftThreshold,
						HardThreshold = dialog.HardThreshold
					};
					this.Add (goal);
					
					// Add it to current view if needed
					if (dialog.AddToCurrentView) {
						this.controller.ViewController.AddToCurrentView (goal, x, y);
					}
					
					// Execute callback actions
					action(goal);
				}
				
				if (dialog.IsValid) {
					dialog.Destroy();
				}
			};
		}
		
		public void AddGoal (string goalName, System.Action<Goal> action)
		{
			AddGoal (goalName, action, 10, 10, false);
		}
		
		public void AddGoal (string goalName, int x, int y)
		{
			AddGoal (goalName, (obj) => {}, x, y, false);
		}
		
		public void AddGoal (string goalName)
		{
			AddGoal (goalName, (obj) => {}, 10, 10, false);
		}
		
		public void AddGoal ()
		{
			AddGoal ("");
		}
		
		public void EditGoal (Goal goal)
		{
			var dialog = new AddGoalDialog(controller.Window, goal, true);
			dialog.Response += delegate(object o, Gtk.ResponseArgs args) {
				if (args.ResponseId == Gtk.ResponseType.Ok & dialog.IsValid) {
					goal.Name = dialog.GoalName;
					goal.Definition = dialog.GoalDefinition;
					goal.SoftThreshold = dialog.SoftThreshold;
					goal.HardThreshold = dialog.HardThreshold;
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
		
		public void PopulateContextMenu (PopulateMenuArgs args)
		{
			if ((args.ClickedElement == null) | (args.ClickedElement is TitleItem && (args.ClickedElement as TitleItem).Name == "Goals")) {
				var addItem = new MenuItem("Add goal...");
				addItem.Activated += delegate(object sender2, EventArgs e) {
					this.AddGoal ("", (obj) => {}, (int) args.X, (int) args.Y, args.Source is DiagramArea);
				};
				args.Menu.Add(addItem);
				args.ElementsAdded = true;
			}
			
			if (args.ClickedElement is Goal) {
				var clickedGoal = (Goal) args.ClickedElement;
				
				var editItem = new MenuItem("Edit...");
				editItem.Activated += delegate(object sender2, EventArgs e) {
					this.EditGoal (clickedGoal);
				};
				args.Menu.Add(editItem);
				
				var deleteItem = new MenuItem("Delete");
				deleteItem.Activated += delegate(object sender2, EventArgs e) {
					this.RemoveGoal (clickedGoal);
				};
				args.Menu.Add(deleteItem);
				
				var computeItem = new MenuItem("Compute likelihood");
				computeItem.Activated += delegate(object sender2, EventArgs e) {
					this.controller.Window.PushStatus ("Computing likelihoods...");
					this.ComputeLikelihood (clickedGoal);
					if (GoalUpdated != null)
						GoalUpdated (clickedGoal);
					this.controller.Window.PushStatus ("Likelihoods computed");
				};
				args.Menu.Add(computeItem);
				
				args.ElementsAdded = true;
			}
		}
		
		public void Populate (TreeStore store)
		{
			var iter = store.AppendValues ("Goals", new TitleItem () { Name = "Goals" }, pixbuf);
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
					l -= obst.Likelihood * this.controller.ObstacleController.ComputeLikelihood (obst.Obstacle);
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
		
		public void CheckModel ()
		{
			var goals = this.GetAll ();
			foreach (var g in goals) {
				if (g.HardThreshold > g.SoftThreshold) {
					this.controller.Window.ErrorList.AddError (string.Format ("Goal '{0}' has incoherent thresholds (Soft: {1} < Hard: {2})", g.Name, g.SoftThreshold, g.HardThreshold));
				}
				
				if (g.Likelihood < g.HardThreshold) {
					this.controller.Window.ErrorList.AddError (string.Format ("Goal '{0}' is hard-violated (Likelihood: {1}, Limit: {2})", g.Name, g.Likelihood, g.HardThreshold));
				} else if (g.Likelihood < g.SoftThreshold) {
					this.controller.Window.ErrorList.AddError (string.Format ("Goal '{0}' is soft-violated (Likelihood: {1}, Limit: {2})", g.Name, g.Likelihood, g.SoftThreshold));
				}
				
				if (g.Likelihood == 0) {
					this.controller.Window.ErrorList.AddWarning (string.Format ("Goal '{0}' has a zero likelihood", g.Name));
				}
			}
		}
	}
}

