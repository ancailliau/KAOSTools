// 
// RefinementController.cs
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
using Gtk;
using KaosEditor.UI.Dialogs;
using KaosEditor.Logging;
using System.Collections.Generic;

namespace KaosEditor.Controllers
{
	public class ObstacleRefinementController : IController, IPopulateMenu
	{
		
		private static Gdk.Pixbuf pixbuf;
		
		static ObstacleRefinementController () {
			try {
				pixbuf = Gdk.Pixbuf.LoadFromResource("KaosEditor.Images.Refinement.png");
				
			} catch (Exception e) {
				Logger.Warning ("Cannot load images from ressources", e);
			}
		}
		
		public delegate void HandleObstacleRefinementAdded (ObstacleRefinement agent);
		public delegate void HandleObstacleRefinementRemoved (ObstacleRefinement agent);
		public delegate void HandleObstacleRefinementUpdated (ObstacleRefinement agent);
		
		public event HandleObstacleRefinementAdded ObstacleRefinementAdded;
		public event HandleObstacleRefinementRemoved ObstacleRefinementRemoved;
		public event HandleObstacleRefinementUpdated ObstacleRefinementUpdated;
		
		private MainController controller;
		private List<ObstacleRefinement> refinements = new List<ObstacleRefinement>();
		
		public ObstacleRefinementController (MainController controller)
		{
			this.controller = controller;
			this.controller.Window.conceptTreeView.RegisterForMenu (this);
		}
		
		public IEnumerable <ObstacleRefinement> GetAll () 
		{
			return this.refinements.AsEnumerable ();
		}
		
		public IEnumerable <ObstacleRefinement> GetAll (Obstacle obstacle) 
		{
			return this.refinements.Where ((arg) => arg.Refined == obstacle).AsEnumerable ();
		}
		
		public void Add (ObstacleRefinement refinement)
		{
			this.refinements.Add (refinement);
			if (ObstacleRefinementAdded != null) {
				ObstacleRefinementAdded (refinement);
			}
		}
		
		public void Remove (ObstacleRefinement refinement)
		{
			this.refinements.Remove (refinement);
			if (ObstacleRefinementRemoved != null) {
				ObstacleRefinementRemoved (refinement);
			}
		}
		
		public void Update (ObstacleRefinement refinement)
		{
			if (ObstacleRefinementUpdated != null) {
				ObstacleRefinementUpdated (refinement);
			}
		}
		
		public ObstacleRefinement Get (string id)
		{
			return this.refinements.Find ((obj) => obj.Id == id);
		}
		
		public void AddRefinement (Obstacle obstacle)
		{
			var dialog = new AddObstacleRefinementDialog (this.controller, obstacle);
			dialog.Response += delegate(object o, ResponseArgs args) {
				if (args.ResponseId == ResponseType.Ok) {
					var newRefinement = new ObstacleRefinement (obstacle);
					foreach (var element in dialog.Refinees) {
						newRefinement.Add(element);
					}
					this.Add(newRefinement);
				}
				dialog.Destroy ();
			};
			dialog.Present ();
		}
		
		public void EditRefinement (ObstacleRefinement refinement)
		{
			var dialog = new AddObstacleRefinementDialog (this.controller, refinement.Refined, refinement.Refinees);
			dialog.Response += delegate(object o, ResponseArgs args) {
				if (args.ResponseId == ResponseType.Ok) {
					refinement.Refinees.Clear();
					foreach (var element in dialog.Refinees) {
						refinement.Add(element);
					}
					this.Update (refinement);
				}
				dialog.Destroy ();
			};
			dialog.Present ();
		}
		
		public void RemoveRefinement (ObstacleRefinement refinement)
		{
			var dialog = new MessageDialog (this.controller.Window,
				DialogFlags.DestroyWithParent, MessageType.Question,
				ButtonsType.YesNo, false, string.Format ("Delete refinement for '{0}'?", refinement.Refined.Name));
			
			dialog.Response += delegate(object o, ResponseArgs args) {
				if (args.ResponseId == Gtk.ResponseType.Yes) {
					this.Remove (refinement);
				}
				dialog.Destroy ();
			};
			
			dialog.Present ();
		}
		
		public void PopulateContextMenu (Menu menu, object source, object clickedElement)
		{
			if (clickedElement is Obstacle) {
				var clickedObstacle = (Obstacle) clickedElement;				
				var refineItem = new MenuItem("Refine...");
				refineItem.Activated += delegate(object sender2, EventArgs e) {
					this.AddRefinement (clickedObstacle);
				};
				menu.Add(refineItem);	
			}
			
			if (clickedElement is ObstacleRefinement) {
				var clickedRefinement = clickedElement as ObstacleRefinement;
				
				var editItem = new MenuItem("Edit...");
				editItem.Activated += delegate(object sender2, EventArgs e) {
					this.EditRefinement (clickedRefinement);
				};
				menu.Add(editItem);
				
				var deleteItem = new MenuItem("Delete");
				deleteItem.Activated += delegate(object sender2, EventArgs e) {
					this.RemoveRefinement (clickedRefinement);
				};
				menu.Add(deleteItem);
			}
		}
		
		public void Populate (TreeStore store)
		{
			var iter = store.AppendValues ("Refinements", null, pixbuf);
			Populate (this.GetAll().Cast<KAOSElement>(), store, iter);
		}
		
		public void Populate (IEnumerable<KAOSElement> elements, TreeStore store, TreeIter iter)
		{
			foreach (var refinement in from e in elements where e is ObstacleRefinement select (ObstacleRefinement) e) {
				var subIter = store.AppendValues (iter, "Refinement", refinement, pixbuf);
				this.controller.ObstacleController.Populate (refinement.Refinees, store, subIter);
			}
		}
	}
}

