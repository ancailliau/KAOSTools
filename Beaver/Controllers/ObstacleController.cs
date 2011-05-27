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
	public class ObstacleController : IController, IPopulateTree, IPopulateMenu
	{
		
		private static Gdk.Pixbuf pixbuf;
		
		static ObstacleController () {
			try {
				pixbuf = Gdk.Pixbuf.LoadFromResource("Beaver.Images.Obstacle.png");
				
			} catch (Exception e) {
				Logger.Warning ("Cannot load images from ressources", e);
			}
		}
		
		public delegate void HandleObstacleAdded (Obstacle agent);
		public delegate void HandleObstacleRemoved (Obstacle agent);
		public delegate void HandleObstacleUpdated (Obstacle agent);
		
		public event HandleObstacleAdded ObstacleAdded;
		public event HandleObstacleRemoved ObstacleRemoved;
		public event HandleObstacleUpdated ObstacleUpdated;
		
		private MainController controller;
		private List<Obstacle> obstacles = new List<Obstacle>();
		
		public ObstacleController (MainController controller)
		{
			this.controller = controller;
			this.controller.Window.conceptTreeView.RegisterForTree (this);
			this.controller.Window.conceptTreeView.RegisterForMenu (this);
			this.controller.Window.viewsNotebook.RegisterForDiagramMenu (this);
		
			this.ObstacleAdded += UpdateLists;
			this.ObstacleRemoved += UpdateLists;
			this.ObstacleUpdated += UpdateLists;
		}
		
		private void UpdateLists (Obstacle obstacle) {
			this.controller.Window.conceptTreeView.Update ();
			this.controller.ViewController.RefreshCurrentView ();
		}
		
		
		public IEnumerable<Obstacle> GetAll ()
		{
			return this.obstacles.AsEnumerable ();
		}
		
		public void Add (Obstacle obstacle)
		{
			Add (obstacle, true);
		}
		
		public void Add (Obstacle obstacle, bool notify)
		{
			this.obstacles.Add (obstacle);
			if (ObstacleAdded != null & notify) {
				ObstacleAdded (obstacle);
			}
		}
		
		public void Remove (Obstacle obstacle)
		{
			this.obstacles.Remove (obstacle);
			if (ObstacleRemoved != null) {
				ObstacleRemoved (obstacle);
			}
		}
		
		public void Update (Obstacle obstacle)
		{
			if (ObstacleUpdated != null) {
				ObstacleUpdated (obstacle);
			}
		}
		
		public Obstacle Get (string id)
		{
			return this.obstacles.Find ((obj) => obj.Id == id);
		}
		
		public void AddObstacle ()
		{
			AddObstacle ("");
		}
		
		public void AddObstacle (string obstacleName)
		{
			AddObstacle (obstacleName, delegate (Obstacle obstacle) {});
		}
		
		public void AddObstacle (string obstacleName, System.Action<Obstacle> action)
		{
			var dialog = new AddObstacleDialog(controller.Window, obstacleName);
			dialog.Response += delegate(object o, Gtk.ResponseArgs args) {
				if (args.ResponseId == Gtk.ResponseType.Ok) {
					var obstacle = new Obstacle(dialog.ObstacleName, dialog.ObstacleDefinition);
					this.Add (obstacle);
					action (obstacle);
				}
				dialog.Destroy();
			};
			
			dialog.Present ();
		}
		
		public void EditObstacle (Obstacle obstacle)
		{
			var dialog = new AddObstacleDialog(controller.Window, obstacle);
			dialog.Response += delegate(object o, Gtk.ResponseArgs args) {
				if (args.ResponseId == Gtk.ResponseType.Ok) {
					obstacle.Name = dialog.ObstacleName;
					obstacle.Definition = dialog.ObstacleDefinition;
					this.Update (obstacle);
				}
				dialog.Destroy();
			};
			
			dialog.Present ();
		}
		
		public void RemoveObstacle (Obstacle obstacle)
		{
			var dialog = new MessageDialog (this.controller.Window,
				DialogFlags.DestroyWithParent, MessageType.Question,
				ButtonsType.YesNo, false, string.Format ("Delete obstacle '{0}'?", obstacle.Name));
			
			dialog.Response += delegate(object o, ResponseArgs args) {
				if (args.ResponseId == Gtk.ResponseType.Yes) {
					this.Remove (obstacle);
				}
				dialog.Destroy ();
			};
			
			dialog.Present ();
		}
		
		public bool PopulateContextMenu (Menu menu, object source, object clickedElement)
		{
			bool retVal = false;
			
			if (clickedElement == null & source is ConceptsTreeView) {				
				var addItem = new MenuItem("Add obstacle...");
				addItem.Activated += delegate(object sender2, EventArgs e) {
					this.AddObstacle ();
				};
				menu.Add(addItem);
				retVal = true;
			}
			
			if (clickedElement is Obstacle) {
				var clickedObstacle = (Obstacle) clickedElement;
				
				var editItem = new MenuItem("Edit...");
				editItem.Activated += delegate(object sender2, EventArgs e) {
					this.EditObstacle (clickedObstacle);
				};
				menu.Add(editItem);
				
				var deleteItem = new MenuItem("Delete");
				deleteItem.Activated += delegate(object sender2, EventArgs e) {
					this.RemoveObstacle (clickedObstacle);
				};
				menu.Add(deleteItem);
				retVal = true;
			}
			return retVal;
		}
		
		public void Populate (TreeStore store)
		{
			var iter = store.AppendValues ("Obstacles", null, pixbuf);
			Populate (this.GetAll().Cast<KAOSElement>(), store, iter);
		}
		
		public void Populate (IEnumerable<KAOSElement> elements, TreeStore store, TreeIter iter)
		{
			foreach (var obstacle in from e in elements where e is Obstacle select (Obstacle) e) {
				var subIter = store.AppendValues (iter, obstacle.Name, obstacle, pixbuf);
				
				var refinements = this.controller.ObstacleRefinementController.GetAll (obstacle).Cast<KAOSElement> ();
				this.controller.ObstacleRefinementController.Populate (refinements, store, subIter);
				
				var resolutions = this.controller.ResolutionController.GetAll (obstacle).Cast<KAOSElement> ();
				this.controller.ResolutionController.Populate (resolutions, store, subIter);
			}
		}
	}
}

