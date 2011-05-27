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

namespace Beaver.Controllers
{
	public class ResponsibilityController : IController, IPopulateMenu
	{
		private static Gdk.Pixbuf pixbuf;
		
		static ResponsibilityController () {
			try {
				pixbuf = Gdk.Pixbuf.LoadFromResource("Beaver.Images.Responsibility.png");
				
			} catch (Exception e) {
				Logger.Warning ("Cannot load images from ressources", e);
			}
		}
		
		public delegate void HandleResponsibilityAdded (Responsibility agent);
		public delegate void HandleResponsibilityRemoved (Responsibility agent);
		public delegate void HandleResponsibilityUpdated (Responsibility agent);
		
		public event HandleResponsibilityAdded ResponsibilityAdded;
		public event HandleResponsibilityRemoved ResponsibilityRemoved;
		public event HandleResponsibilityUpdated ResponsibilityUpdated;
		
		private MainController controller;
		private List<Responsibility> responsibilities = new List<Responsibility>();
		
		public ResponsibilityController (MainController controller)
		{
			this.controller = controller;
			this.controller.Window.conceptTreeView.RegisterForMenu (this);
			this.controller.Window.viewsNotebook.RegisterForDiagramMenu (this);
		
			this.ResponsibilityAdded += UpdateLists;
			this.ResponsibilityRemoved += UpdateLists;
			this.ResponsibilityUpdated += UpdateLists;
		}
		
		private void UpdateLists (Responsibility responsibility) {
			this.controller.Window.conceptTreeView.Update ();
			this.controller.ViewController.RefreshCurrentView ();
		}
		

		public IEnumerable<Responsibility> GetAll ()
		{
			return this.responsibilities.AsEnumerable ();
		}
		
		public IEnumerable<Responsibility> GetAll (Goal goal)
		{
			return this.responsibilities.Where ((arg) => arg.Goal == goal).AsEnumerable ();
		}
		
		public void Add (Responsibility responsibility)
		{
			Add (responsibility, true);
		}
		
		public void Add (Responsibility responsibility, bool notify)
		{
			this.responsibilities.Add (responsibility);
			if (ResponsibilityAdded != null & notify) {
				ResponsibilityAdded (responsibility);
			}
		}
		
		public void Remove (Responsibility responsibility)
		{
			this.responsibilities.Remove (responsibility);
			if (ResponsibilityRemoved != null) {
				ResponsibilityRemoved (responsibility);
			}
		}
		
		public void Update (Responsibility responsibility)
		{
			if (ResponsibilityUpdated != null) {
				ResponsibilityUpdated (responsibility);
			}
		}
		
		public Responsibility Get (string id)
		{
			return this.responsibilities.Find ((obj) => obj.Id == id);
		}
		
		public void AddResponsibility (Goal goal)
		{
			var dialog = new AddResponsibilityDialog (this.controller, goal);
			dialog.Response += delegate(object o, ResponseArgs args) {
				if (args.ResponseId == ResponseType.Ok && dialog.ResponsibleAgentName != null
					& dialog.ResponsibleAgentName != "") {
					if (dialog.ResponsibleAgent != null) {
						var newResponsibility = new Responsibility (
							goal, dialog.ResponsibleAgent);
						this.Add (newResponsibility);
						dialog.Destroy ();
					} else {
						var subDialog = new MessageDialog (this.controller.Window,
						DialogFlags.DestroyWithParent, MessageType.Question,
						ButtonsType.YesNo, false, string.Format ("Create new agent '{0}'?", dialog.ResponsibleAgentName));
					
						subDialog.Response += delegate(object o2, ResponseArgs args2) {
							if (args2.ResponseId == Gtk.ResponseType.Yes) {
								controller.AgentController.AddAgent (dialog.ResponsibleAgentName, delegate (Agent agent) {
									dialog.ResponsibleAgent = agent;
								});
							}
							subDialog.Destroy ();
						};
						subDialog.Present ();
					}
				}
			};
			dialog.Present ();
		}
		
		public void EditResponsibility (Responsibility responsibility)
		{
			var dialog = new AddResponsibilityDialog (this.controller, responsibility.Goal, responsibility.Agent);
			dialog.Response += delegate(object o, ResponseArgs args) {
				if (args.ResponseId == ResponseType.Ok && dialog.ResponsibleAgent != null) {
					responsibility.Agent = dialog.ResponsibleAgent;
					this.Update (responsibility);
				}
				dialog.Destroy ();
			};
			dialog.Present ();
		}
		
		public void RemoveResponsibility (Responsibility responsibility)
		{
			var dialog = new MessageDialog (this.controller.Window,
				DialogFlags.DestroyWithParent, MessageType.Question,
				ButtonsType.YesNo, false, string.Format ("Delete responsibility for '{0}' by '{1}'?", responsibility.Goal.Name, responsibility.Agent.Name));
			
			dialog.Response += delegate(object o, ResponseArgs args) {
				if (args.ResponseId == Gtk.ResponseType.Yes) {
					this.Remove (responsibility);
				}
				dialog.Destroy ();
			};
			
			dialog.Present ();
		}
		
		public bool PopulateContextMenu (Menu menu, object source, object clickedElement)
		{
			bool retVal = false;
			
			if (clickedElement is Goal) {
				var clickedGoal = clickedElement as Goal;
				var responsibilities = this.GetAll (clickedGoal);
				
				
				var assignItem = new MenuItem("Assign responsibility...");
				assignItem.Activated += delegate(object sender2, EventArgs e) {
					this.AddResponsibility (clickedGoal);
				};
				if (responsibilities.Count() > 0) {
					var responsibilityMenu = new Menu ();
					responsibilityMenu.Add (assignItem);
					
					int i = 0;
					foreach (var responsibility in responsibilities) {
						var subMenu = new Menu();
						var clickedResponsibility = clickedElement as Responsibility;
						
						var editItem = new MenuItem("Edit...");
						editItem.Activated += delegate(object sender2, EventArgs e) {
							this.EditResponsibility (responsibility);
						};
						subMenu.Add(editItem);
						
						var deleteItem = new MenuItem("Delete");
						deleteItem.Activated += delegate(object sender2, EventArgs e) {
							this.RemoveResponsibility (responsibility);
						};
						subMenu.Add(deleteItem);
						
						var subMenuItem = new MenuItem (string.Format ("Responsibility for '{0}'", responsibility.Agent.Name));
						subMenuItem.Submenu = subMenu;
						responsibilityMenu.Add (subMenuItem);
					}
					
					var menuItem = new MenuItem ("Responsibilities");
					menuItem.Submenu = responsibilityMenu;
					menu.Add (menuItem);
					retVal = true;
					
				} else {
					menu.Add(assignItem);
					retVal = true;
				}
			}
			
			if (clickedElement is Responsibility) {
				var clickedResponsibility = (Responsibility) clickedElement;
				
				var editItem = new MenuItem("Edit...");
				editItem.Activated += delegate(object sender2, EventArgs e) {
					this.EditResponsibility (clickedResponsibility);
				};
				menu.Add(editItem);
				
				var deleteItem = new MenuItem("Delete");
				deleteItem.Activated += delegate(object sender2, EventArgs e) {
					this.RemoveResponsibility (clickedResponsibility);
				};
				menu.Add(deleteItem);
				retVal = true;
			}
			
			return retVal;
		}
		
		public void Populate (TreeStore store)
		{
			var iter = store.AppendValues ("Responsibilities", null, pixbuf);
			Populate (this.GetAll().Cast<KAOSElement>(), store, iter);
		}
		
		public void Populate (IEnumerable<KAOSElement> elements, TreeStore store, TreeIter iter)
		{
			foreach (var responsibility in from e in elements where e is Responsibility select (Responsibility) e) {
				var subIter = store.AppendValues (iter, "Responsibility", responsibility, pixbuf);
				this.controller.AgentController.Populate (new List<KAOSElement> () { responsibility.Agent }, store, subIter);
			}
		}
	}
}

