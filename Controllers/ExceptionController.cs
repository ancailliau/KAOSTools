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
	public class ExceptionController
	{
		
		private MainController controller;
		
		public ExceptionController (MainController controller)
		{
			this.controller = controller;
		}
		
		public void AddException (Goal goal)
		{
			var dialog = new AddExceptionDialog (this.controller.Window, goal);
			dialog.Response += delegate(object o, ResponseArgs args) {
				if (args.ResponseId == ResponseType.Ok && dialog.ExceptionGoal != null) {
					var newException = new ExceptionLink (
						goal, dialog.ExceptionGoal);
					this.controller.Model.Add (newException);
					dialog.Destroy ();
				} else if (args.ResponseId == ResponseType.Cancel) {
					dialog.Destroy ();
				}
			};
			dialog.Present ();
		}
		
		public void EditException (ExceptionLink exception)
		{
			var dialog = new AddExceptionDialog (this.controller.Window, exception.Goal, exception.ExceptionGoal);
			dialog.Response += delegate(object o, ResponseArgs args) {
				if (args.ResponseId == ResponseType.Ok && dialog.ExceptionGoal != null) {
					exception.ExceptionGoal = dialog.ExceptionGoal;
					this.controller.Model.Update (exception);
					dialog.Destroy ();
				} else if (args.ResponseId == ResponseType.Cancel) {
					dialog.Destroy ();
				}
			};
			dialog.Present ();
		}
		
		public void RemoveException (ExceptionLink exception)
		{
			var dialog = new MessageDialog (this.controller.Window,
				DialogFlags.DestroyWithParent, MessageType.Question,
				ButtonsType.YesNo, false, string.Format ("Delete exception for '{0}' by '{1}'?", exception.Goal.Name, exception.ExceptionGoal.Name));
			
			dialog.Response += delegate(object o, ResponseArgs args) {
				if (args.ResponseId == Gtk.ResponseType.Yes) {
					this.controller.Model.Remove (exception);
				}
				dialog.Destroy ();
			};
			
			dialog.Present ();
		}
		
		public void PopulateContextMenu (Menu menu, object source, object clickedElement)
		{
			if (clickedElement is Goal) {	
				var clickedGoal = clickedElement as Goal;
				var assignItem = new MenuItem("Add exception...");
				assignItem.Activated += delegate(object sender2, EventArgs e) {
					this.AddException (clickedGoal);
				};
				menu.Add(assignItem);
			
				var exceptions = from e in this.controller.Model.Elements 
					where e is ExceptionLink && ((ExceptionLink) e).Goal.Equals (clickedGoal) 
					select (ExceptionLink) e;
				
				if (exceptions.Count () > 0) {
					menu.Add (new SeparatorMenuItem ());
				}
				
				int i = 1;
				foreach (var e in exceptions) {
					var subMenuItem = new MenuItem (string.Format ("Exception {0}", i));
					subMenuItem.TooltipText = string.Format ("Exception to '{0}' by '{1}'",
						clickedGoal.Name, e.ExceptionGoal.Name);
					subMenuItem.HasTooltip = true;
					
					var subMenu = new Menu ();
					
					var editItem = new MenuItem("Edit...");
					editItem.Activated += delegate(object sender2, EventArgs args) {
						this.EditException (e);
					};
					subMenu.Add(editItem);
					
					var deleteItem = new MenuItem("Delete");
					deleteItem.Activated += delegate(object sender2, EventArgs args) {
						this.RemoveException (e);
					};
					subMenu.Add(deleteItem);
					
					subMenuItem.Submenu = subMenu;
					menu.Add (subMenuItem);
				}
			}
		}
	}
}

