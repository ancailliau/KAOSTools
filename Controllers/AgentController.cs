// 
// AgentController.cs
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
using KaosEditor.UI.Dialogs;
using KaosEditor.Model;
using Gtk;
using KaosEditor.UI.Widgets;
using KaosEditor.Logging;
using System.Collections.Generic;

namespace KaosEditor.Controllers
{
	public class AgentController : IController, IPopulateTree, IPopulateMenu
	{
		private static Gdk.Pixbuf pixbuf;
		
		static AgentController () {
			try {
				pixbuf = Gdk.Pixbuf.LoadFromResource("KaosEditor.Images.Agent.png");
				
			} catch (Exception e) {
				Logger.Warning ("Cannot load images from ressources", e);
			}
		}
		
		public delegate void HandleAgentAdded (Agent agent);
		public delegate void HandleAgentRemoved (Agent agent);
		public delegate void HandleAgentUpdated (Agent agent);
		
		public event HandleAgentAdded AgentAdded;
		public event HandleAgentRemoved AgentRemoved;
		public event HandleAgentUpdated AgentUpdated;
		
		private List<Agent> agents = new List<Agent>();
		
		private MainController controller;
		
		public AgentController (MainController controller)
		{
			this.controller = controller;
			this.controller.Window.conceptTreeView.RegisterForTree (this);
			this.controller.Window.conceptTreeView.RegisterForMenu (this);
			this.controller.Window.viewsNotebook.RegisterForDiagramMenu (this);
		
			this.AgentAdded += UpdateLists;
			this.AgentRemoved += UpdateLists;
			this.AgentUpdated += UpdateLists;
		}
		
		private void UpdateLists (Agent agent) {
			this.controller.Window.conceptTreeView.Update ();
			this.controller.ViewController.RefreshCurrentView ();
		}
		
		
		public IEnumerable<Agent> GetAll ()
		{
			return this.agents.AsEnumerable ();
		}
		
		public void Add (Agent agent)
		{
			Add (agent, true);
		}
		
		public void Add (Agent agent, bool notify)
		{
			this.agents.Add (agent);
			if (AgentAdded != null & notify) {
				AgentAdded (agent);
			}
		}
		
		public void Remove (Agent agent)
		{
			this.agents.Remove (agent);
			if (AgentRemoved != null) {
				AgentRemoved (agent);
			}
		}
		
		public void Update (Agent agent)
		{
			if (AgentUpdated != null) {
				AgentUpdated (agent);
			}
		}
		
		public Agent Get (string id)
		{
			return this.agents.Find ((obj) => obj.Id == id);
		}
		
		public void AddAgent (string agentName, System.Action<Agent> action)
		{
			var dialog = new AddAgentDialog (this.controller.Window, agentName);
			dialog.Response += delegate(object o, Gtk.ResponseArgs args) {
				if (args.ResponseId == Gtk.ResponseType.Ok) {
					var agent = new Agent (dialog.AgentName);
					this.Add (agent);
					action (agent);
				}
				dialog.Destroy ();
			};
			dialog.Present ();
		}
		
		public void AddAgent (string agentName)
		{
			AddAgent (agentName, delegate (Agent a) {});
		}
		
		public void AddAgent ()
		{
			AddAgent ("");
		}
		
		public void EditAgent (Agent agent)
		{
			var dialog = new AddAgentDialog (this.controller.Window, agent);
			dialog.Response += delegate(object o, Gtk.ResponseArgs args) {
				if (args.ResponseId == Gtk.ResponseType.Ok) {
					agent.Name = dialog.AgentName;
					this.Update (agent);
				}
				dialog.Destroy ();
			};
			dialog.Present ();
		}
		
		public void RemoveAgent (Agent agent)
		{
			var dialog = new MessageDialog (this.controller.Window,
			DialogFlags.DestroyWithParent, MessageType.Question,
			ButtonsType.YesNo, false, string.Format ("Delete agent '{0}'?", agent.Name));
		
			dialog.Response += delegate(object o, ResponseArgs args) {
				if (args.ResponseId == Gtk.ResponseType.Yes) {
					this.Remove (agent);
				}
				dialog.Destroy ();
			};
			
			dialog.Present ();
		}
		
		
		public bool PopulateContextMenu (Menu menu, object source, object clickedElement)
		{
			bool retVal = false;
			
			if (clickedElement == null & source is ConceptsTreeView) {				
				var addItem = new MenuItem("Add agent...");
				addItem.Activated += delegate(object sender2, EventArgs e) {
					this.AddAgent ();
				};
				menu.Add(addItem);
				retVal = true;
			}
			
			if (clickedElement is Agent) {
				var clickedAgent = (Agent) clickedElement;
				
				var editItem = new MenuItem("Edit...");
				editItem.Activated += delegate(object sender2, EventArgs e) {
					this.EditAgent (clickedAgent);
				};
				menu.Add(editItem);
				
				var deleteItem = new MenuItem("Delete");
				deleteItem.Activated += delegate(object sender2, EventArgs e) {
					this.RemoveAgent (clickedAgent);
				};
				menu.Add(deleteItem);
				retVal = true;
			}
			
			return retVal;
		}
		
		public void Populate (TreeStore store)
		{
			var iter = store.AppendValues ("Agents", null, pixbuf);
			Populate (this.GetAll().Cast<KAOSElement>(), store, iter);
		}
		
		public void Populate (IEnumerable<KAOSElement> elements, TreeStore store, TreeIter iter)
		{
			foreach (var agent in from e in elements where e is Agent select (Agent) e) {
				store.AppendValues (iter, agent.Name, agent, pixbuf);
			}
		}
	}
}

