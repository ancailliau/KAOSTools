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
using Beaver.UI;

namespace Beaver.Controllers
{
	public class ViewController : IController, IPopulateTree
	{
		private static Gdk.Pixbuf pixbuf;
		private static Gdk.Pixbuf responsibilityPixbuf;
		private static Gdk.Pixbuf agentPixbuf;
		private static Gdk.Pixbuf domainPropertyPixbuf;
		private static Gdk.Pixbuf exceptionPixbuf;
		private static Gdk.Pixbuf goalPixbuf;
		private static Gdk.Pixbuf obstaclePixbuf;
		private static Gdk.Pixbuf refinementPixbuf;
		private static Gdk.Pixbuf obstructionPixbuf;
		private static Gdk.Pixbuf resolutionPixbuf;
		
		static ViewController () {
			try {
				pixbuf = Gdk.Pixbuf.LoadFromResource("Beaver.Images.View.png");
				responsibilityPixbuf = Gdk.Pixbuf.LoadFromResource("Beaver.Images.Responsibility.png");
				agentPixbuf = Gdk.Pixbuf.LoadFromResource("Beaver.Images.Agent.png");
				domainPropertyPixbuf = Gdk.Pixbuf.LoadFromResource("Beaver.Images.DomainProperty.png");
				exceptionPixbuf = Gdk.Pixbuf.LoadFromResource("Beaver.Images.Exception.png");
				goalPixbuf = Gdk.Pixbuf.LoadFromResource("Beaver.Images.Goal.png");
				obstaclePixbuf = Gdk.Pixbuf.LoadFromResource("Beaver.Images.Obstacle.png");
				refinementPixbuf = Gdk.Pixbuf.LoadFromResource("Beaver.Images.Refinement.png");
				obstructionPixbuf = Gdk.Pixbuf.LoadFromResource("Beaver.Images.Obstruction.png");
				resolutionPixbuf = Gdk.Pixbuf.LoadFromResource("Beaver.Images.Resolution.png");

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
			this.controller.Window.viewsNotebook.RegisterForDiagramMenu (this.PopulateContextMenu);
			
			this.controller.Window.conceptTreeView.RegisterForMenu (this.PopulateContextMenu);
			this.controller.Window.viewList.RegisterForTree (this);
			this.controller.Window.viewList.RegisterForMenu (this.PopulateContextMenu);
			this.controller.Window.viewList.RegisterForMenu (this.PopulateContextMenuView);
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
		
		public void RemoveFromCurrentView (Shape element)
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
		
		public void PopulateContextMenu (PopulateMenuArgs args)
		{
			if (args.ClickedElement != null && args.ClickedElement is Shape) {
				var removeFromCurrentViewItem = new MenuItem("Remove from current view...");
				removeFromCurrentViewItem.Activated += delegate(object sender2, EventArgs e) {
					this.RemoveFromCurrentView (args.ClickedElement as Shape);
				};
				args.Menu.Add (removeFromCurrentViewItem);
				args.ElementsAdded = true;
			}
			
			if (args.ClickedElement != null && args.ClickedElement is KAOSElement & !(args.Source is DiagramArea)) {
				var addToCurrentViewItem = new MenuItem("Add to current view...");
				addToCurrentViewItem.Activated += delegate(object sender2, EventArgs e) {
					this.AddToCurrentView (args.ClickedElement as KAOSElement);
				};
				args.Menu.Add(addToCurrentViewItem);
				args.ElementsAdded = true;
			}
			
			if (args.ClickedElement is ModelView) {
				var clickedView = args.ClickedElement as ModelView;
				
				var editItem = new MenuItem("Edit...");
				editItem.Activated += delegate(object sender2, EventArgs e) {
					this.EditView (clickedView);
				};
				args.Menu.Add(editItem);
				
				var removeItem = new MenuItem("Delete");
				removeItem.Activated += delegate(object sender2, EventArgs e) {
					this.RemoveView (clickedView);
				};
				args.Menu.Add(removeItem);
				args.ElementsAdded = true;
			}
		}
		
		public void PopulateContextMenuView (PopulateMenuArgs args)
		{
			if (args.ClickedElement == null) {
				var addViewItem = new MenuItem("Add view...");
				addViewItem.Activated += delegate(object sender2, EventArgs e) {
					this.AddView ();
				};
				args.Menu.Add(addViewItem);
				args.ElementsAdded = true;
			}
		}
		
		public void Populate (TreeStore store)
		{
			foreach (var view in this.GetAll ()) {
				var iter = store.AppendValues (view.Name, view, pixbuf);
				
				Func<KAOSElement, string> getName = (arg) => {
					if (arg is Agent) {
						return (arg as Agent).Name;
					} else if (arg is DomainProperty) {
						return (arg as DomainProperty).Name;
					} else if (arg is ExceptionLink) {
						return string.Format ("Exception to '{0}'", (arg as ExceptionLink).Goal.Name);
					} else if (arg is Goal) {
						return (arg as Goal).Name;
					} else if (arg is Obstacle) {
						return (arg as Obstacle).Name;
					} else if (arg is ObstacleRefinement) {
						return string.Format("Refinement for '{0}'", (arg as ObstacleRefinement).Refined.Name);
					} else if (arg is Obstruction) {
						return string.Format("Obstruction to '{0}'", (arg as Obstruction).Goal.Name);
					} else if (arg is Refinement) {
						return string.Format("Refinement for '{0}'", (arg as Refinement).Refined.Name);
					} else if (arg is Resolution) {
						return string.Format("Resolution to '{0}'", (arg as Resolution).Obstacle.Name);
					} else if (arg is Responsibility) {
						return string.Format("Responsibility for '{0}'", (arg as Responsibility).Agent.Name);
					} else {
						Logger.Error ("Unable to get the name of '{0}'", arg.GetType().Name);
						return "";
					}
				};
				
				Func<KAOSElement, Gdk.Pixbuf> getPixbuf = (arg) => {
					if (arg is Agent) {
						return agentPixbuf;
					} else if (arg is DomainProperty) {
						return domainPropertyPixbuf;
					} else if (arg is ExceptionLink) {
						return exceptionPixbuf;
					} else if (arg is Goal) {
						return goalPixbuf;
					} else if (arg is Obstacle) {
						return obstaclePixbuf;
					} else if (arg is ObstacleRefinement) {
						return refinementPixbuf;
					} else if (arg is Obstruction) {
						return obstructionPixbuf;
					} else if (arg is Refinement) {
						return refinementPixbuf;
					} else if (arg is Resolution) {
						return resolutionPixbuf;
					} else if (arg is Responsibility) {
						return responsibilityPixbuf;
					} else {
						Logger.Error ("Unable to get the pixbuf of '{0}'", arg.GetType().Name);
						return null;
					}
				};
				
				foreach (var shape in view.Shapes) {
					store.AppendValues (iter, getName(shape.RepresentedElement), shape, getPixbuf (shape.RepresentedElement));
				}
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

