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
	public class RefinementController : IController, IPopulateTree, IPopulateMenu
	{
		
		private static Gdk.Pixbuf pixbuf;
		
		static RefinementController () {
			try {
				pixbuf = Gdk.Pixbuf.LoadFromResource("KaosEditor.Images.Refinement.png");
				
			} catch (Exception e) {
				Logger.Warning ("Cannot load images from ressources", e);
			}
		}
		
		public delegate void HandleRefinementAdded (Refinement agent);
		public delegate void HandleRefinementRemoved (Refinement agent);
		public delegate void HandleRefinementUpdated (Refinement agent);
		
		public event HandleRefinementAdded RefinementAdded;
		public event HandleRefinementRemoved RefinementRemoved;
		public event HandleRefinementUpdated RefinementUpdated;
		
		private MainController controller;
		private List<Refinement> refinements = new List<Refinement>();
		
		public RefinementController (MainController controller)
		{
			this.controller = controller;
			this.controller.Window.conceptTreeView.RegisterForMenu (this);
			this.controller.Window.viewsNotebook.RegisterForDiagramMenu (this);
		
			this.RefinementAdded += UpdateLists;
			this.RefinementRemoved += UpdateLists;
			this.RefinementUpdated += UpdateLists;
		}
		
		private void UpdateLists (Refinement refinement) {
			this.controller.Window.conceptTreeView.Update ();
			this.controller.ViewController.RefreshCurrentView ();
		}
		
		
		public IEnumerable <Refinement> GetAll ()
		{
			return this.refinements.AsEnumerable ();
		}
		
		public IEnumerable <Refinement> GetAll (Goal goal)
		{
			return this.refinements.Where ((arg) => arg.Refined == goal).AsEnumerable ();
		}
		
		public void Add (Refinement refinement)
		{
			Add (refinement, true);
		}
		
		public void Add (Refinement refinement, bool notify)
		{
			this.refinements.Add (refinement);
			if (RefinementAdded != null & notify) {
				RefinementAdded (refinement);
			}
		}
		
		public void Remove (Refinement refinement)
		{
			this.refinements.Remove (refinement);
			if (RefinementRemoved != null) {
				RefinementRemoved (refinement);
			}
		}
		
		public void Update (Refinement refinement)
		{
			if (RefinementUpdated != null) {
				RefinementUpdated (refinement);
			}
		}
		
		public Refinement Get (string id)
		{
			return this.refinements.Find ((obj) => obj.Id == id);
		}
		
		public void AddRefinement (Goal goal)
		{
			var dialog = new AddRefinementDialog (this.controller, goal);
			dialog.Response += delegate(object o, ResponseArgs args) {
				if (args.ResponseId == ResponseType.Ok) {
					var newRefinement = new Refinement (goal);
					foreach (var element in dialog.Refinees) {
						newRefinement.Add(element);
					}
					this.Add(newRefinement);
				}
				dialog.Destroy ();
			};
			dialog.Present ();
		}
		
		public void EditRefinement (Refinement refinement)
		{
			var dialog = new AddRefinementDialog (this.controller, refinement.Refined, refinement.Refinees);
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
		
		public void RemoveRefinement (Refinement refinement)
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
		
		public bool PopulateContextMenu (Menu menu, object source, object clickedElement)
		{
			bool retVal = false;
			
			if (clickedElement is Goal) {
				var clickedGoal = (Goal) clickedElement;				
				var refineItem = new MenuItem("Refine...");
				refineItem.Activated += delegate(object sender2, EventArgs e) {
					this.AddRefinement (clickedGoal);
				};
				menu.Add(refineItem);
				retVal = true;
			}
			
			if (clickedElement is Refinement) {
				var clickedRefinement = clickedElement as Refinement;
				
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
				retVal = true;
			}
			
			return retVal;
		}
		
		public void Populate (TreeStore store)
		{
			var iter = store.AppendValues ("Refinements", null, pixbuf);
			Populate (this.GetAll().Cast<KAOSElement>(), store, iter);
		}
		
		public void Populate (IEnumerable<KAOSElement> elements, TreeStore store, TreeIter iter)
		{
			foreach (var refinement in from e in elements where e is Refinement select (Refinement) e) {
				var subIter = store.AppendValues (iter, "Refinement", refinement, pixbuf);
				var list = refinement.Refinees.Cast<KAOSElement> ();
				this.controller.GoalController.Populate (list, store, subIter);
				this.controller.DomainPropertyController.Populate (list, store, subIter);
			}
		}
		
	}
}

