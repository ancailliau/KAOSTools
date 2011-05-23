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
using KaosEditor.Model;
using Gtk;
using KaosEditor.UI.Dialogs;

namespace KaosEditor.Controllers
{
	public class ResponsibilityController
	{
		
		private MainController controller;
		
		public ResponsibilityController (MainController controller)
		{
			this.controller = controller;
		}
		
		public void AddResponsibility (Goal goal)
		{
			var dialog = new AddResponsibilityDialog (this.controller.Window, goal);
			dialog.Response += delegate(object o, ResponseArgs args) {
				if (args.ResponseId == ResponseType.Ok && dialog.ResponsibleAgentName != null
					& dialog.ResponsibleAgentName != "") {
					if (dialog.ResponsibleAgent != null) {
						var newResponsibility = new Responsibility (
							goal, dialog.ResponsibleAgent);
						this.controller.Model.Add (newResponsibility);
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
			var dialog = new AddResponsibilityDialog (this.controller.Window, responsibility.Goal, responsibility.Agent);
			dialog.Response += delegate(object o, ResponseArgs args) {
				if (args.ResponseId == ResponseType.Ok && dialog.ResponsibleAgent != null) {
					responsibility.Agent = dialog.ResponsibleAgent;
					this.controller.Model.Update (responsibility);
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
					this.controller.Model.Remove (responsibility);
				}
				dialog.Destroy ();
			};
			
			dialog.Present ();
		}
		
		public void PopulateContextMenu (Menu menu, object source, KAOSElement clickedElement)
		{
			if (clickedElement is Goal) {	
				var clickedGoal = clickedElement as Goal;
				var assignItem = new MenuItem("Assign responsibility...");
				assignItem.Activated += delegate(object sender2, EventArgs e) {
					this.AddResponsibility (clickedGoal);
				};
				menu.Add(assignItem);
			}
			
			if (clickedElement is Responsibility) {
				var clickedResponsibility = clickedElement as Responsibility;
				
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
			}
			
		}
	}
}

