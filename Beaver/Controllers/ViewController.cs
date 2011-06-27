// 
// ViewController.cs
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
using Beaver.UI.Dialogs;
using Gtk;
using Beaver.UI.Widgets;
using Beaver.Logging;
using Beaver.UI.Shapes;
using Beaver.Views;
using System.Collections.Generic;
using Cairo;
using Beaver.UI.ColorSchemes;

namespace Beaver.Controllers
{
	public class ViewController : IController, IPopulateTree, IPopulateMenu
	{
		private static Gdk.Pixbuf pixbuf;
		
		static ViewController () {
			try {
				pixbuf = Gdk.Pixbuf.LoadFromResource("Beaver.Images.View.png");
				
			} catch (Exception e) {
				Logger.Warning ("Cannot load images from ressources", e);
			}
		}
		
		public delegate void HandleViewAdded (ModelView view);
		public delegate void HandleViewRemoved (ModelView view);
		public delegate void HandleViewUpdated (ModelView view);
		
		public event HandleViewAdded ViewAdded;
		public event HandleViewRemoved ViewRemoved;
		public event HandleViewUpdated ViewUpdated;
		
		private MainController controller;
		private List<ModelView> views = new List<ModelView> ();
		
		public ViewController (MainController controller)
		{
			this.controller = controller;
			this.controller.Window.viewsNotebook.RegisterForDiagramMenu (this);
			
			this.controller.Window.conceptTreeView.RegisterForMenu (this);
			this.controller.Window.viewList.RegisterForTree (this);
			this.controller.Window.viewList.RegisterForMenu (this);
			this.controller.Window.viewList.ElementActivated += ElementActivated;
		
			this.ViewAdded += UpdateLists;
			this.ViewRemoved += UpdateLists;
			this.ViewUpdated += UpdateLists;
		}
		
		private void UpdateLists (ModelView view) {
			this.controller.Window.viewList.Update ();
		}

		public IEnumerable<ModelView> GetAll ()
		{
			return this.views.AsEnumerable ();
		}
		
		public void Add (ModelView view)
		{
			Add (view, true);
		}
			
		public void Add (ModelView view, bool notify)
		{
			this.views.Add (view);
			if (ViewAdded != null & notify) {
				ViewAdded (view);
			}
		}
		
		public void Remove (ModelView view)
		{
			this.views.Remove (view);
			this.controller.Window.viewsNotebook.CloseView (view);
			if (ViewRemoved != null) {
				ViewRemoved (view);
			}
		}
		
		public void Update (ModelView view)
		{
			this.controller.Window.viewsNotebook.CloseView (view);
			this.controller.Window.viewsNotebook.DisplayView (view);
			if (ViewUpdated != null) {
				ViewUpdated (view);
			}
		}
		
		public ModelView Get (string name)
		{
			return this.views.Find ((obj) => obj.Name == name);
		}
		
		public void AddView ()
		{
			var addViewDialog = new AddViewDialog (this.controller.Window);
			addViewDialog.Response += delegate(object sender, Gtk.ResponseArgs args) {
				if (args.ResponseId == Gtk.ResponseType.Ok & addViewDialog.ViewName != "") {
					ModelView view = new ModelView (addViewDialog.ViewName, this.controller);
					this.Add (view);
					this.DisplayView (view);
				}
				addViewDialog.Destroy ();
			};
			addViewDialog.Present ();	
		}
		
		public void DuplicateView (ModelView view)
		{
			var addViewDialog = new AddViewDialog (this.controller.Window, view.Name + " (copy)");
			addViewDialog.Response += delegate(object sender, Gtk.ResponseArgs args) {
				if (args.ResponseId == Gtk.ResponseType.Ok & addViewDialog.ViewName != "") {
					var newView = new ModelView (addViewDialog.ViewName, this.controller);
					foreach (var s in view.Shapes) {
						newView.Add (s.Copy ());
					}
					this.Add (newView);
					this.DisplayView (newView);
				}
				addViewDialog.Destroy ();
			};
			addViewDialog.Present ();
		}
		
		public void EditView (ModelView view)
		{
			var dialog = new AddViewDialog (this.controller.Window, view);
			dialog.Response += delegate(object sender, Gtk.ResponseArgs args) {
				if (args.ResponseId == Gtk.ResponseType.Ok & dialog.ViewName != "") {
					view.Name = dialog.ViewName;
					this.Update (view);
				}
				dialog.Destroy ();
			};
			dialog.Present ();
		}
		
		public void RemoveView (ModelView view)
		{
			var dialog = new MessageDialog (this.controller.Window,
				DialogFlags.DestroyWithParent, MessageType.Question,
				ButtonsType.YesNo, false, string.Format ("Delete view '{0}'?", view.Name));
			
			dialog.Response += delegate(object o, ResponseArgs args) {
				if (args.ResponseId == Gtk.ResponseType.Yes) {
					this.Remove (view);
				}
				dialog.Destroy ();
			};
			
			dialog.Present ();
		}
		
		public void DisplayView (ModelView view)
		{
			this.controller.Window.viewsNotebook.DisplayView (view);
		}
		
		public void AddToCurrentView (KAOSElement element)
		{
			this.controller.Window.viewsNotebook.AddToCurrentView (element, 10, 10);
			this.RefreshCurrentView ();
		}
		
		public void AddToCurrentView (KAOSElement element, double x, double y)
		{
			this.controller.Window.viewsNotebook.AddToCurrentView (element, x, y);
			this.RefreshCurrentView ();
		}
		
		public void RemoveFromCurrentView (IShape element)
		{
			this.controller.Window.viewsNotebook.CurrentView.Shapes.Remove (element);
			this.RefreshCurrentView ();
		}

		public void RefreshCurrentView ()
		{
			if (this.controller.Window.viewsNotebook.CurrentView != null)
				this.controller.Window.viewsNotebook.CurrentView.Redraw ();
		}
				
		private void ElementActivated (object element)
		{
			if ((element as ModelView) != null) {
				this.DisplayView (element as ModelView);
			}
		}
		
		public bool PopulateContextMenu (Menu menu, object source, object clickedElement)
		{
			bool retVal = false;
			
			if (clickedElement != null && clickedElement is IShape) {
				var removeFromCurrentViewItem = new MenuItem("Remove from current view...");
				removeFromCurrentViewItem.Activated += delegate(object sender2, EventArgs e) {
					this.RemoveFromCurrentView (clickedElement as IShape);
				};
				menu.Add (removeFromCurrentViewItem);
				retVal = true;
			}
			
			if (clickedElement != null && clickedElement is KAOSElement & !(source is DiagramArea)) {
				var addToCurrentViewItem = new MenuItem("Add to current view...");
				addToCurrentViewItem.Activated += delegate(object sender2, EventArgs e) {
					this.AddToCurrentView (clickedElement as KAOSElement);
				};
				menu.Add(addToCurrentViewItem);
				retVal = true;
			}
			
			if (clickedElement == null) {
				var addViewItem = new MenuItem("Add view...");
				addViewItem.Activated += delegate(object sender2, EventArgs e) {
					this.AddView ();
				};
				menu.Add(addViewItem);
				retVal = true;
			}
			
			if (clickedElement is ModelView) {
				var clickedView = clickedElement as ModelView;
				
				var editItem = new MenuItem("Edit...");
				editItem.Activated += delegate(object sender2, EventArgs e) {
					this.EditView (clickedView);
				};
				menu.Add(editItem);
				
				var duplicateItem = new MenuItem("Duplicate...");
				duplicateItem.Activated += delegate(object sender2, EventArgs e) {
					this.DuplicateView (clickedView);
				};
				menu.Add(duplicateItem);
				
				var removeItem = new MenuItem("Delete");
				removeItem.Activated += delegate(object sender2, EventArgs e) {
					this.RemoveView (clickedView);
				};
				menu.Add(removeItem);
				retVal = true;
			}
			
			return retVal;
		}
		
		public void Populate (TreeStore store)
		{
			foreach (var view in this.GetAll ()) {
				store.AppendValues (view.Name, view, pixbuf);
			}
		}
		
		public void Populate (IEnumerable<KAOSElement> elements, TreeStore store, TreeIter iter)
		{
			throw new NotImplementedException ();
		}

		public void ExportCurrentView ()
		{
			var dialog = new FileChooserDialog("Save image as...", this.controller.Window,
			FileChooserAction.Save, "Cancel", ResponseType.Cancel, "Save", ResponseType.Accept);
			
			if (dialog.Run() == (int) ResponseType.Accept) {
				string filename = dialog.Filename.EndsWith (".png") ? dialog.Filename : dialog.Filename + ".png";
				
				int minX , minY, maxX, maxY;
				var currentView = this.controller.Window.viewsNotebook.CurrentView;
				
				var oldTheme = this.controller.CurrentColorScheme;
				this.controller.CurrentColorScheme = new BWColorScheme ();
				
				currentView.GetSize (out minX, out maxX, out minY, out maxY);
				
				using (var surface = new ImageSurface (Format.Argb32, maxX, maxY)) {
					using (Context context = new Context (surface)) {
						currentView.Display (context);
						surface.WriteToPng (filename);
					}
				}
				
				int padding = 10;
				using (var surface = new ImageSurface (filename)) {
					using (var surface2 = new ImageSurface (Format.Argb32, maxX-minX+2*padding, maxY-minY+2*padding)) {
						using (Context context = new Context (surface2)) {
							var oldSource = context.Source;
							context.SetColor("#fff");
							context.Paint();
							context.Source = oldSource;
							
							context.SetSourceSurface (surface, -minX+padding, -minY+padding);
							context.Rectangle (padding,padding,maxX-minX,maxY-minY);
							context.Fill ();
							surface2.WriteToPng (filename);
						}
					}
				}
				
				this.controller.CurrentColorScheme = oldTheme;
			}
			
			dialog.Destroy();	
			
			
		}
		
		public void ExportCurrentViewAsPdf ()
		{
			var dialog = new FileChooserDialog("Save image as...", this.controller.Window,
			FileChooserAction.Save, "Cancel", ResponseType.Cancel, "Save", ResponseType.Accept);
			
			if (dialog.Run() == (int) ResponseType.Accept) {
				string filename = dialog.Filename.EndsWith (".pdf") ? dialog.Filename : dialog.Filename + ".pdf";
				
				int minX , minY, maxX, maxY;
				var currentView = this.controller.Window.viewsNotebook.CurrentView;
				
				var oldTheme = this.controller.CurrentColorScheme;
				this.controller.CurrentColorScheme = new BWColorScheme ();
				
				currentView.GetSize (out minX, out maxX, out minY, out maxY);
				
				using (var surface = new PdfSurface (filename, maxX-minX, maxY-minY)) {
					using (Context context = new Context (surface)) {
						context.Translate (-minX, -minY);
						currentView.Display (context);
						context.ShowPage ();
					}
				}
				this.controller.CurrentColorScheme = oldTheme;
			}
			
			dialog.Destroy();	
			
			
		}
	}
}

