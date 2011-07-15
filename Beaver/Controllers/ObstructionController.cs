// 
// ResponsibilityController.cs
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
using Gtk;
using Beaver.UI.Dialogs;
using Beaver.Logging;
using System.Collections.Generic;
using Beaver.UI;

namespace Beaver.Controllers
{
	public class ObstructionController : IController, IPopulateTree
	{
		
		private static Gdk.Pixbuf pixbuf;
		
		static ObstructionController () {
			try {
				pixbuf = Gdk.Pixbuf.LoadFromResource("Beaver.Images.Obstruction.png");
				
			} catch (Exception e) {
				Logger.Warning ("Cannot load images from ressources", e);
			}
		}
		
		public delegate void HandleObstructionAdded (Obstruction agent);
		public delegate void HandleObstructionRemoved (Obstruction agent);
		public delegate void HandleObstructionUpdated (Obstruction agent);
		
		public event HandleObstructionAdded ObstructionAdded;
		public event HandleObstructionRemoved ObstructionRemoved;
		public event HandleObstructionUpdated ObstructionUpdated;
		
		private MainController controller;
		private List<Obstruction> obstructions = new List<Obstruction> ();
		
		public ObstructionController (MainController controller)
		{
			this.controller = controller;
			this.controller.Window.conceptTreeView.RegisterForMenu (this.PopulateContextMenu);
			this.controller.Window.viewsNotebook.RegisterForDiagramMenu (this.PopulateContextMenu);
		
			this.ObstructionAdded += UpdateLists;
			this.ObstructionRemoved += UpdateLists;
			this.ObstructionUpdated += UpdateLists;
		}
		
		private void UpdateLists (Obstruction obstruction) {
			this.controller.Window.conceptTreeView.Update ();
			this.controller.ViewController.RefreshCurrentView ();
		}
		
		
		public IEnumerable<Obstruction> GetAll ()
		{
			return this.obstructions.AsEnumerable ();
		}
		
		public IEnumerable<Obstruction> GetAll (Goal goal)
		{
			return this.obstructions.Where ((arg) => arg.Goal == goal);
		}
		
		public IEnumerable<Obstruction> GetAll (Obstacle obstacle)
		{
			return this.obstructions.Where ((arg) => arg.Obstacle == obstacle);
		}
		
		public void Add (Obstruction obstruction)
		{
			Add (obstruction, true);
		}
		
		public void Add (Obstruction obstruction, bool notify)
		{
			this.obstructions.Add (obstruction);
			if (ObstructionAdded != null & notify) {
				ObstructionAdded (obstruction);
			}
		}
		
		public void Remove (Obstruction obstruction)
		{
			this.obstructions.Remove (obstruction);
			if (ObstructionRemoved != null) {
				ObstructionRemoved (obstruction);
			}
		}
		
		public void Update (Obstruction obstruction)
		{
			if (ObstructionUpdated != null) {
				ObstructionUpdated (obstruction);
			}
		}
		
		public Obstruction Get (string id)
		{
			return this.obstructions.Find ((obj) => obj.Id == id);
		}
		
		public void AddObstruction (Goal goal)
		{
			var dialog = new AddObstructionDialog (this.controller, goal);
			dialog.Response += delegate(object o, ResponseArgs args) {
				if (args.ResponseId == ResponseType.Ok && dialog.Obstacle != null) {
					var newObstruction = new Obstruction (
						goal, dialog.Obstacle) { Likelihood = dialog.Likelihood } ;
					this.Add (newObstruction);
					dialog.Destroy ();
					
				} else if (args.ResponseId == ResponseType.Ok && dialog.ObstacleName != null
					&& dialog.ObstacleName != "") {
					
					var confirmDialog = new MessageDialog (this.controller.Window,
						DialogFlags.DestroyWithParent, MessageType.Question,
						ButtonsType.YesNo, false, string.Format ("Create new obstacle '{0}'?", dialog.ObstacleName));
					confirmDialog.Response += delegate(object o2, ResponseArgs args2) {
						if (args2.ResponseId == ResponseType.Yes) {
							this.controller.ObstacleController.AddObstacle (dialog.ObstacleName,
							delegate (Obstacle obstacle) {
								dialog.Obstacle = obstacle;	
							});
						}
						confirmDialog.Destroy ();
					};
					confirmDialog.Present ();
				}
			};
			dialog.Present ();
		}
		
		public void EditObstruction (Obstruction obstruction)
		{
			var dialog = new AddObstructionDialog (this.controller, obstruction);
			dialog.Response += delegate(object o, ResponseArgs args) {
				if (args.ResponseId == ResponseType.Ok && dialog.Obstacle != null) {
					obstruction.Obstacle = dialog.Obstacle;
					obstruction.Likelihood = dialog.Likelihood;
					this.Update (obstruction);
				}
				dialog.Destroy ();
			};
			dialog.Present ();
		}
		
		public void RemoveObstruction (Obstruction obstruction)
		{
			var dialog = new MessageDialog (this.controller.Window,
				DialogFlags.DestroyWithParent, MessageType.Question,
				ButtonsType.YesNo, false, string.Format ("Delete obstruction of '{0}' by '{1}'?", obstruction.Goal.Name, obstruction.Obstacle.Name));
			
			dialog.Response += delegate(object o, ResponseArgs args) {
				if (args.ResponseId == Gtk.ResponseType.Yes) {
					this.Remove (obstruction);
				}
				dialog.Destroy ();
			};
			
			dialog.Present ();
		}
		
		public void PopulateContextMenu (PopulateMenuArgs args)
		{
			if (args.ClickedElement is Goal) {	
				var refinements = this.controller.RefinementController.GetAll (args.ClickedElement as Goal);
				if (refinements.Count() == 0) {
					var clickedGoal = args.ClickedElement as Goal;
					var assignItem = new MenuItem("Obstruct...");
					assignItem.Activated += delegate(object sender2, EventArgs e) {
						this.AddObstruction (clickedGoal);
					};
					args.Menu.Add(assignItem);
					args.ElementsAdded = true;
				}
			}
			
			if (args.ClickedElement is Obstruction) {
				var clickedObstruction = args.ClickedElement as Obstruction;
				
				var editItem = new MenuItem("Edit...");
				editItem.Activated += delegate(object sender2, EventArgs e) {
					this.EditObstruction (clickedObstruction);
				};
				args.Menu.Add(editItem);
				
				var deleteItem = new MenuItem("Delete");
				deleteItem.Activated += delegate(object sender2, EventArgs e) {
					this.RemoveObstruction (clickedObstruction);
				};
				args.Menu.Add(deleteItem);
				args.ElementsAdded = true;
			}
		}
		
		public void Populate (TreeStore store)
		{
			var iter = store.AppendValues ("Obstructions", null, pixbuf);
			Populate (this.GetAll().Cast<KAOSElement>(), store, iter);
		}
		
		public void Populate (IEnumerable<KAOSElement> elements, TreeStore store, TreeIter iter)
		{
			foreach (var obstruction in from e in elements where e is Obstruction select (Obstruction) e) {
				var subIter = store.AppendValues (iter, "Obstruction", obstruction, pixbuf);
				this.controller.ObstacleController.Populate (new List<KAOSElement> () { 
					obstruction.Obstacle
				}, store, subIter);
			}
		}
	}
}

