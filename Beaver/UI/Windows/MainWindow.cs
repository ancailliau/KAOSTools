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
using Beaver.Controllers;
using Beaver.Events;
using Beaver.Model;
using Beaver.UI.Widgets;
using Beaver.UI.Shapes;
using Beaver.Logging;

namespace Beaver.UI.Windows {
	
	/// <summary>
	/// Main window.
	/// </summary>
	public partial class MainWindow: Gtk.Window
	{	
		private static Gdk.Pixbuf modelPixbuf;
		private static Gdk.Pixbuf viewPixbuf;
		static MainWindow () {
			try {
				modelPixbuf = Gdk.Pixbuf.LoadFromResource("Beaver.Images.Model.png");
				viewPixbuf = Gdk.Pixbuf.LoadFromResource("Beaver.Images.View.png");
			} catch (Exception e) {
				Logger.Warning ("Cannot load images from ressources", e);
			}
		}
		
		/// <summary>
		/// Gets or sets the controller.
		/// </summary>
		/// <value>
		/// The controller.
		/// </value>
		public MainController Controller;
		
		/// <summary>
		/// The views notebook.
		/// </summary>
		public ViewsNotebook viewsNotebook;
		
		/// <summary>
		/// The concept tree view.
		/// </summary>
		public ConceptsTreeView conceptTreeView;
		
		public ConceptsTreeView viewList;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Beaver.UI.Windows.MainWindow"/> class.
		/// </summary>
		/// <param name='model'>
		/// Model.
		/// </param>
		public MainWindow (): base (Gtk.WindowType.Toplevel)
		{
			Build ();
			
			viewsNotebook = new ViewsNotebook();
			conceptTreeView = new ConceptsTreeView ();
			
			var notebookModelView = new Notebook ();
			notebookModelView.TabPos = PositionType.Bottom;
			
			var modelLabel = new HBox ();
			modelLabel.PackStart (new Image (modelPixbuf), false, false, 0);
			modelLabel.PackEnd (new Label ("Model"), true, true, 0);
			
			var viewLabel = new HBox ();
			viewLabel.PackStart (new Image (viewPixbuf), false, false, 0);
			viewLabel.PackEnd (new Label ("Views"), true, true, 0);
			
			modelLabel.ShowAll ();
			viewLabel.ShowAll ();
			
			viewList = new ConceptsTreeView();
			var scroll2 = new ScrolledWindow ();
			scroll2.Add (viewList);
			
			var scroll = new ScrolledWindow ();
			scroll.Add (conceptTreeView);
			
			notebookModelView.AppendPage (scroll, modelLabel);
			notebookModelView.AppendPage (scroll2, viewLabel);
			
			hpaned1.Add1 (notebookModelView);
			hpaned1.Add2 (viewsNotebook);
			hpaned1.ShowAll();
		}

		public void Reset ()
		{
			viewsNotebook.CloseAll ();
			conceptTreeView.Clear ();
			viewList.Clear ();
		}
		
		public void Update ()
		{
			viewsNotebook.RedrawCurrentView();
			conceptTreeView.Update ();
			viewList.Update ();
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
		
		protected virtual void OnRevertToSavedActionActivated (object sender, System.EventArgs e)
		{
			this.Controller.ReloadCurrentProject ();
		}
		
		protected virtual void OnSaveAsActionActivated (object sender, System.EventArgs e)
		{
			this.Controller.SaveProjectAs ();
		}
		
		protected void OnExportCurrentViewActionActivated (object sender, System.EventArgs e)
		{
			this.Controller.ViewController.ExportCurrentView ();
		}

		protected void OnNewActionActivated (object sender, System.EventArgs e)
		{
			this.Controller.NewProject ();
		}

		protected void OnExportCurrentViewAsPDFActionActivated (object sender, System.EventArgs e)
		{
			this.Controller.ViewController.ExportCurrentViewAsPdf ();
		}

	}
}
