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
	public class ObstacleController : IController
	{
		
		private static Gdk.Pixbuf pixbuf;
		
		static ObstacleController () {
			try {
				pixbuf = Gdk.Pixbuf.LoadFromResource("KaosEditor.Images.Obstacle.png");
				
			} catch (Exception e) {
				Logger.Warning ("Cannot load images from ressources", e);
			}
		}
		
		private MainController controller;
		
		public ObstacleController (MainController controller)
		{
			this.controller = controller;
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
					controller.Model.Add (obstacle);
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
					controller.Model.Update (obstacle);
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
					this.controller.Model.Remove (obstacle);
				}
				dialog.Destroy ();
			};
			
			dialog.Present ();
		}
		
		public void PopulateContextMenu (Menu menu, object source, object clickedElement)
		{
			if (clickedElement == null & source is ConceptsTreeView) {				
				var addItem = new MenuItem("Add obstacle...");
				addItem.Activated += delegate(object sender2, EventArgs e) {
					this.AddObstacle ();
				};
				menu.Add(addItem);
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
				
				menu.Add (new SeparatorMenuItem ());
				
			}
		}
		
		public void PopulateTree (TreeStore store, bool header)
		{
			if (header) {
				var iter = store.AppendValues ("Obstacles", null, pixbuf);
				PopulateTree (store, iter, false);
				
			} else {
				var obstacles = from e in this.controller.Model.Elements
					where e is Obstacle select (Obstacle) e;
			
				foreach (var obstacle in obstacles) {
					store.AppendValues (obstacle.Name, obstacle, pixbuf);
				}
			}
		}
		
		public void PopulateTree (TreeStore store, TreeIter iter, bool header)
		{
			var obstacles = from e in this.controller.Model.Elements
				where e is Obstacle select (Obstacle) e;
			
			if (header) {
				iter = store.AppendValues (iter, "Obstacles", null, pixbuf);
			}
			PopulateTree (obstacles.ToArray(), store, iter);
		}
		
		public void PopulateTree (KAOSElement[] elements, TreeStore store, TreeIter iter)
		{
			foreach (var obstacle in from e in elements where e is Obstacle select (Obstacle) e) {
				var subIter = store.AppendValues (iter, obstacle.Name, obstacle, pixbuf);
				
				var refinements = from e in this.controller.Model.Elements
					where e is ObstacleRefinement && ((ObstacleRefinement) e).Refined == obstacle
						select (ObstacleRefinement) e;
				this.controller.PopulateTree (refinements.ToArray(), store, subIter);
				
				var resolutions = from e in this.controller.Model.Elements
					where e is Resolution && ((Resolution) e).Obstacle == obstacle
						select (Resolution) e;
				this.controller.PopulateTree (resolutions.ToArray(), store, subIter);
			}
		}
	}
}

