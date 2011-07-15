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
using System.Collections.Generic;
using Beaver.Logging;
using Beaver.UI;

namespace Beaver.Controllers
{
	public class ExceptionController : IController, IPopulateTree
	{
		private static Gdk.Pixbuf pixbuf;
		
		static ExceptionController () {
			try {
				pixbuf = Gdk.Pixbuf.LoadFromResource("Beaver.Images.Exception.png");
				
			} catch (Exception e) {
				Logger.Warning ("Cannot load images from ressources", e);
			}
		}
		
		public delegate void HandleExceptionAdded (ExceptionLink exception);
		public delegate void HandleExceptionRemoved (ExceptionLink exception);
		public delegate void HandleExceptionUpdated (ExceptionLink exception);
		
		public event HandleExceptionAdded ExceptionAdded;
		public event HandleExceptionRemoved ExceptionRemoved;
		public event HandleExceptionUpdated ExceptionUpdated;
		
		private MainController controller;
		private List<ExceptionLink> exceptions = new List<ExceptionLink>();
		
		public ExceptionController (MainController controller)
		{
			this.controller = controller;
			this.controller.Window.conceptTreeView.RegisterForMenu (this.PopulateContextMenu);
			this.controller.Window.viewsNotebook.RegisterForDiagramMenu (this.PopulateContextMenu);
		
			this.ExceptionAdded += UpdateLists;
			this.ExceptionRemoved += UpdateLists;
			this.ExceptionUpdated += UpdateLists;
		}
		
		private void UpdateLists (ExceptionLink exception) {
			this.controller.Window.conceptTreeView.Update ();
			this.controller.ViewController.RefreshCurrentView ();
		}
		
		public IEnumerable<ExceptionLink> GetAll ()
		{
			return this.exceptions.AsEnumerable ();
		}
		
		public IEnumerable<ExceptionLink> GetAll (Goal goal)
		{
			return this.exceptions.Where ((arg) => arg.Goal == goal);
		}
		
		public void Add (ExceptionLink exception)
		{
			Add (exception, true);
		}
		
		public void Add (ExceptionLink exception, bool notify)
		{
			this.exceptions.Add (exception);
			if (ExceptionAdded != null & notify) {
				ExceptionAdded (exception);
			}
		}
		
		public void Remove (ExceptionLink exception)
		{
			this.exceptions.Remove (exception);
			if (ExceptionRemoved != null) {
				ExceptionRemoved (exception);
			}
		}
		
		public void Update (ExceptionLink exception)
		{
			if (ExceptionUpdated != null) {
				ExceptionUpdated (exception);
			}
		}
		
		public ExceptionLink Get (string id)
		{
			return this.exceptions.Find ((obj) => obj.Id == id);
		}
		
		public void AddException (Goal goal)
		{
			var dialog = new AddExceptionDialog (this.controller, goal);
			dialog.Response += delegate(object o, ResponseArgs args) {
				if (args.ResponseId == ResponseType.Ok && dialog.ExceptionGoal != null) {
					var newException = new ExceptionLink (
						goal, dialog.ExceptionGoal, dialog.Condition);
					this.Add (newException);
					dialog.Destroy ();
				} else if (args.ResponseId == ResponseType.Cancel) {
					dialog.Destroy ();
				}
			};
			dialog.Present ();
		}
		
		public void EditException (ExceptionLink exception)
		{
			var dialog = new AddExceptionDialog (this.controller, exception.Goal, exception.ExceptionGoal, true) {
				Condition = exception.Condition
			};
			dialog.Response += delegate(object o, ResponseArgs args) {
				if (args.ResponseId == ResponseType.Ok && dialog.ExceptionGoal != null) {
					exception.ExceptionGoal = dialog.ExceptionGoal;
					exception.Condition = dialog.Condition;
					this.Update (exception);
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
					this.Remove (exception);
				}
				dialog.Destroy ();
			};
			
			dialog.Present ();
		}
		
		public void PopulateContextMenu (PopulateMenuArgs args)
		{
			if (args.ClickedElement is Goal) {
				var clickedGoal = args.ClickedElement as Goal;
				var exceptions = this.GetAll (clickedGoal);
				
				var exceptionMenu = new Menu ();
				
				var assignItem = new MenuItem("Add exception...");
				assignItem.Activated += delegate(object sender2, EventArgs e) {
					this.AddException (clickedGoal);
				};
				args.Menu.Add (assignItem);
				args.ElementsAdded = true;
			}
			
			if (args.ClickedElement is ExceptionLink) {
				var clickedException = (ExceptionLink) args.ClickedElement;
				
				var editItem = new MenuItem("Edit...");
				editItem.Activated += delegate(object sender2, EventArgs e) {
					this.EditException (clickedException);
				};
				args.Menu.Add(editItem);
				
				var deleteItem = new MenuItem("Delete");
				deleteItem.Activated += delegate(object sender2, EventArgs e) {
					this.RemoveException (clickedException);
				};
				args.Menu.Add(deleteItem);
				args.ElementsAdded = true;
			}
		}
		
		public void Populate (TreeStore store)
		{
			var iter = store.AppendValues ("Exceptions", null, pixbuf);
			Populate (this.GetAll().Cast<KAOSElement>(), store, iter);
		}
		
		public void Populate (IEnumerable<KAOSElement> elements, TreeStore store, TreeIter iter)
		{
			foreach (var exception in from e in elements where e is ExceptionLink select (ExceptionLink) e) {
				var subIter = store.AppendValues (iter, "Exception", exception, pixbuf);
				this.controller.GoalController.Populate (new List<KAOSElement> () { exception.ExceptionGoal }, store, subIter);
			}
		}
	}
}

