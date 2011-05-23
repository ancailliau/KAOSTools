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
using KaosEditor.UI.Dialogs;
using KaosEditor.Model;
using Gtk;

namespace KaosEditor.Controllers
{
	public class AgentController
	{
		
		private MainController controller;
		
		public AgentController (MainController controller)
		{
			this.controller = controller;
		}
		
		public void AddAgent ()
		{
			var dialog = new AddAgentDialog (this.controller.Window, null);
			dialog.Response += delegate(object o, Gtk.ResponseArgs args) {
				if (args.ResponseId == Gtk.ResponseType.Ok) {
					var agent = new Agent (dialog.AgentName);
					this.controller.Model.Add (agent);
				}
				dialog.Destroy ();
			};
			dialog.Present ();
		}
		
		public void EditAgent (Agent agent)
		{
			var dialog = new AddAgentDialog (this.controller.Window, agent);
			dialog.Response += delegate(object o, Gtk.ResponseArgs args) {
				if (args.ResponseId == Gtk.ResponseType.Ok) {
					agent.Name = dialog.AgentName;
					this.controller.Model.Update (agent);
				}
				dialog.Destroy ();
			};
			dialog.Present ();
		}
		
		public void RemoveAgent (Agent agent)
		{
			throw new NotImplementedException ();
		}
		
		
		public void PopulateContextMenu (Menu menu, object source, KAOSElement clickedElement)
		{
			if (clickedElement == null) {				
				var addItem = new MenuItem("Add agent...");
				addItem.Activated += delegate(object sender2, EventArgs e) {
					this.AddAgent ();
				};
				menu.Add(addItem);
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
			}
		}
	}
}

