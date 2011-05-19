// 
// MainWindow.cs
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
using Gtk;
using KaosEditor.Controllers;
using KaosEditor.Events;
using KaosEditor.Model;
using KaosEditor.UI.Widgets;
using KaosEditor.UI.Shapes;
using KaosEditor.Logging;

namespace KaosEditor.UI.Windows {
	
	/// <summary>
	/// Main window.
	/// </summary>
	public partial class MainWindow: Gtk.Window
	{	
		/// <summary>
		/// Gets or sets the model.
		/// </summary>
		/// <value>
		/// The model.
		/// </value>
		public EditorModel Model {
			get;
			set;
		}
		
		/// <summary>
		/// Gets or sets the controller.
		/// </summary>
		/// <value>
		/// The controller.
		/// </value>
		public MainController Controller {
			get;
			set;
		}
		
		/// <summary>
		/// The views notebook.
		/// </summary>
		private ViewsNotebook viewsNotebook;
		
		/// <summary>
		/// The concept tree view.
		/// </summary>
		private ConceptsTreeView conceptTreeView;
		
		private ViewList viewList;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="KaosEditor.UI.Windows.MainWindow"/> class.
		/// </summary>
		/// <param name='model'>
		/// Model.
		/// </param>
		public MainWindow (EditorModel model, MainController controller): base (Gtk.WindowType.Toplevel)
		{
			this.Controller = controller;
			this.Model = model;
			Build ();
			
			viewsNotebook = new ViewsNotebook();
			conceptTreeView = new ConceptsTreeView (this);
			
			var notebookModelView = new Notebook ();
			notebookModelView.TabPos = PositionType.Bottom;
			var modelLabel = new Label ("Model");
			var viewLabel = new Label ("Views");
			
			viewList = new ViewList(this.Controller);
			var scroll2 = new ScrolledWindow ();
			scroll2.Add (viewList);
			
			var scroll = new ScrolledWindow ();
			scroll.Add (conceptTreeView);
			
			notebookModelView.AppendPage (scroll, modelLabel);
			notebookModelView.AppendPage (scroll2, viewLabel);
			
			hpaned1.Add1 (notebookModelView);
			hpaned1.Add2 (viewsNotebook);
			hpaned1.ShowAll();
					
			Model.Changed += UpdateWidgets;
			Model.Views.ViewsChanged += UpdateWidgets;
			Model.Views.AddedView += UpdateWidgets;
		}
		
		/// <summary>
		/// Updates the widgets.
		/// </summary>
		/// <param name='sender'>
		/// Sender.
		/// </param>
		/// <param name='args'>
		/// Arguments.
		/// </param>
		private void UpdateWidgets (object sender, EventArgs args)
		{
			viewsNotebook.RedrawCurrentView();
			conceptTreeView.Update();
			viewList.UpdateList();
		}
		
		/// <summary>
		/// Displaies the view specified by the given name
		/// </summary>
		/// <param name='name'>
		/// Name of the view to display.
		/// </param>
		public void DisplayView (string name)
		{
			viewsNotebook.DisplayView (Model.Views.Get(name));
		}
			
		/// <summary>
		/// Adds given model element to current view.
		/// </summary>
		/// <param name='element'>
		/// Element to add
		/// </param>
		public void AddToCurrentView (IModelElement element)
		{
			AddToCurrentView (element, 10, 10);
		}	
		
		/// <summary>
		/// Adds given model element to current view.
		/// </summary>
		/// <param name='element'>
		/// Element to add
		/// </param>
		public void AddToCurrentView (IModelElement element, double x, double y)
		{
			viewsNotebook.AddToCurrentView(element, x, y);
		}
			
		/// <summary>
		/// Removes from current view.
		/// </summary>
		/// <param name='element'>
		/// Element.
		/// </param>
		public void RemoveFromCurrentView (IShape element)
		{
			viewsNotebook.CurrentView.Shapes.Remove ( element );
			viewsNotebook.RedrawCurrentView ();
		}
		
		/// <summary>
		/// Determines whether this instance has current view.
		/// </summary>
		/// <returns>
		/// <c>true</c> if this instance has current view; otherwise, <c>false</c>.
		/// </returns>
		public bool HasCurrentView ()
		{
			return viewsNotebook.CurrentView != null;
		}
		
		/// <summary>
		/// Handles the delete event.
		/// </summary>
		/// <param name='sender'>
		/// Sender.
		/// </param>
		/// <param name='a'>
		/// A.
		/// </param>
		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			this.Controller.Quit ();
			a.RetVal = true;
		}
		
		/// <summary>
		/// Handles the quit action activated event.
		/// </summary>
		/// <param name='sender'>
		/// Sender.
		/// </param>
		/// <param name='e'>
		/// E.
		/// </param>
		protected virtual void OnQuitActionActivated (object sender, System.EventArgs e)
		{
			this.Controller.Quit ();
		}
		
		/// <summary>
		/// Handles the save action activated event.
		/// </summary>
		/// <param name='sender'>
		/// Sender.
		/// </param>
		/// <param name='e'>
		/// E.
		/// </param>
		protected virtual void OnSaveActionActivated (object sender, System.EventArgs e)
		{
			this.Controller.SaveProject ();
		}
		
		/// <summary>
		/// Handles the open action activated event.
		/// </summary>
		/// <param name='sender'>
		/// Sender.
		/// </param>
		/// <param name='e'>
		/// E.
		/// </param>
		protected virtual void OnOpenActionActivated (object sender, System.EventArgs e)
		{
			this.Controller.LoadProject ();
		}
		
		/// <summary>
		/// Handles the save as action activated event.
		/// </summary>
		/// <param name='sender'>
		/// Sender.
		/// </param>
		/// <param name='e'>
		/// E.
		/// </param>
		protected virtual void OnSaveAsActionActivated (object sender, System.EventArgs e)
		{
			this.Controller.SaveProjectAs ();
		}
			
	}
}
