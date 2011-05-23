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
	public class ObstructionController
	{
		
		private MainController controller;
		
		public ObstructionController (MainController controller)
		{
			this.controller = controller;
		}
		
		public void AddObstruction (Goal goal)
		{
			var dialog = new AddObstructionDialog (this.controller.Window, goal);
			dialog.Response += delegate(object o, ResponseArgs args) {
				if (args.ResponseId == ResponseType.Ok && dialog.Obstacle != null) {
					var newObstruction = new Obstruction (
						goal, dialog.Obstacle);
					this.controller.Model.Add (newObstruction);
					dialog.Destroy ();
					
				} else if (args.ResponseId == ResponseType.Ok && dialog.ObstacleName != null
					&& dialog.ObstacleName != "") {
					this.controller.ObstacleController.AddObstacle (dialog.ObstacleName,
						delegate (Obstacle obstacle) {
						dialog.Obstacle = obstacle;	
					});
				}
			};
			dialog.Present ();
		}
		
		public void EditObstruction (Obstruction obstruction)
		{
			var dialog = new AddObstructionDialog (this.controller.Window, obstruction.Goal, obstruction.Obstacle);
			dialog.Response += delegate(object o, ResponseArgs args) {
				if (args.ResponseId == ResponseType.Ok && dialog.Obstacle != null) {
					obstruction.Obstacle = dialog.Obstacle;
					this.controller.Model.Update (obstruction);
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
					this.controller.Model.Remove (obstruction);
				}
				dialog.Destroy ();
			};
			
			dialog.Present ();
		}
		
		public void PopulateContextMenu (Menu menu, object source, KAOSElement clickedElement)
		{
			if (clickedElement is Goal) {	
				var refinements = from n in this.controller.Model.Elements
					where n is Refinement && ((Refinement) n).Refined.Equals (clickedElement)
					select n;
				if (refinements.Count() == 0) {
					var clickedGoal = clickedElement as Goal;
					var assignItem = new MenuItem("Obstruct...");
					assignItem.Activated += delegate(object sender2, EventArgs e) {
						this.AddObstruction (clickedGoal);
					};
					menu.Add(assignItem);
				}
			}
			
			if (clickedElement is Obstruction) {
				var clickedObstruction = clickedElement as Obstruction;
				
				var editItem = new MenuItem("Edit...");
				editItem.Activated += delegate(object sender2, EventArgs e) {
					this.EditObstruction (clickedObstruction);
				};
				menu.Add(editItem);
				
				var deleteItem = new MenuItem("Delete");
				deleteItem.Activated += delegate(object sender2, EventArgs e) {
					this.RemoveObstruction (clickedObstruction);
				};
				menu.Add(deleteItem);
			}
			
		}
	}
}
