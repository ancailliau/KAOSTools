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
using KaosEditor.Model;
using Gtk;
using KaosEditor.UI.Dialogs;

namespace KaosEditor.Controllers
{
	public class ResolutionController
	{
		
		private MainController controller;
		
		public ResolutionController (MainController controller)
		{
			this.controller = controller;
		}
		
		public void AddResolution (Obstacle obstacle)
		{
			var dialog = new AddResolutionDialog (this.controller.Window, obstacle);
			dialog.Response += delegate(object o, ResponseArgs args) {
				if (args.ResponseId == ResponseType.Ok && dialog.ResolvingGoal != null) {
					var newObstruction = new Resolution (
						obstacle, dialog.ResolvingGoal);
					this.controller.Model.Add (newObstruction);
					dialog.Destroy ();
				} else if (args.ResponseId == ResponseType.Ok && dialog.ResolvingGoalName != "") {
					var confirmDialog = new MessageDialog (this.controller.Window,
						DialogFlags.DestroyWithParent, MessageType.Question,
						ButtonsType.YesNo, false, string.Format ("Create new goal '{0}'?", dialog.ResolvingGoalName));
					confirmDialog.Response += delegate(object o2, ResponseArgs args2) {
						if (args2.ResponseId == ResponseType.Yes) {
							this.controller.GoalController.AddGoal (dialog.ResolvingGoalName, delegate (Goal goal) {
								dialog.ResolvingGoal = goal;
							});
						}
						confirmDialog.Destroy ();
					};
					confirmDialog.Present ();
				}
			};
			dialog.Present ();
		}
		
		public void EditResolution (Resolution resolution)
		{
			var dialog = new AddResolutionDialog (this.controller.Window, resolution.Obstacle, resolution.Goal);
			dialog.Response += delegate(object o, ResponseArgs args) {
				if (args.ResponseId == ResponseType.Ok && dialog.ResolvingGoal != null) {
					resolution.Goal = dialog.ResolvingGoal;
					this.controller.Model.Update (resolution);
				}
				dialog.Destroy ();
			};
			dialog.Present ();
		}
		
		public void RemoveResolution (Resolution resolution)
		{
			var dialog = new MessageDialog (this.controller.Window,
				DialogFlags.DestroyWithParent, MessageType.Question,
				ButtonsType.YesNo, false, string.Format ("Delete resolution of '{0}' by '{1}'?", resolution.Obstacle.Name, resolution.Goal.Name));
			
			dialog.Response += delegate(object o, ResponseArgs args) {
				if (args.ResponseId == Gtk.ResponseType.Yes) {
					this.controller.Model.Remove (resolution);
				}
				dialog.Destroy ();
			};
			
			dialog.Present ();
		}
		
		public void PopulateContextMenu (Menu menu, object source, KAOSElement clickedElement)
		{
			if (clickedElement is Obstacle) {	
				var clickedObstacle = clickedElement as Obstacle;
				var assignItem = new MenuItem("Resolve...");
				assignItem.Activated += delegate(object sender2, EventArgs e) {
					this.AddResolution (clickedObstacle);
				};
				menu.Add(assignItem);
			}
			
			if (clickedElement is Resolution) {
				var clickedResolution = clickedElement as Resolution;
				
				var editItem = new MenuItem("Edit...");
				editItem.Activated += delegate(object sender2, EventArgs e) {
					this.EditResolution (clickedResolution);
				};
				menu.Add(editItem);
				
				var deleteItem = new MenuItem("Delete");
				deleteItem.Activated += delegate(object sender2, EventArgs e) {
					this.RemoveResolution (clickedResolution);
				};
				menu.Add(deleteItem);
			}
			
		}
	}
}

