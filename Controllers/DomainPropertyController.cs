// 
// GoalController.cs
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
using KaosEditor.UI.Dialogs;
using Gtk;
using KaosEditor.UI.Widgets;
using KaosEditor.Logging;
using System.Collections.Generic;

namespace KaosEditor.Controllers
{
	public class DomainPropertyController : IController, IPopulateTree, IPopulateMenu
	{
		
		private static Gdk.Pixbuf pixbuf;
		
		static DomainPropertyController () {
			try {
				pixbuf = Gdk.Pixbuf.LoadFromResource("KaosEditor.Images.DomainProperty.png");
				
			} catch (Exception e) {
				Logger.Warning ("Cannot load images from ressources", e);
			}
		}
		
		public delegate void HandleDomainPropertyAdded (DomainProperty domProp);
		public delegate void HandleDomainPropertyRemoved (DomainProperty domProp);
		public delegate void HandleDomainPropertyUpdated (DomainProperty domProp);
		
		public event HandleDomainPropertyAdded DomainPropertyAdded;
		public event HandleDomainPropertyRemoved DomainPropertyRemoved;
		public event HandleDomainPropertyUpdated DomainPropertyUpdated;
		
		private MainController controller;
		private List<DomainProperty> domainProperties = new List<DomainProperty>();
		
		public DomainPropertyController (MainController controller)
		{
			this.controller = controller;
			this.controller.Window.conceptTreeView.RegisterForTree (this);
			this.controller.Window.conceptTreeView.RegisterForMenu (this);
			this.controller.Window.viewsNotebook.RegisterForDiagramMenu (this);
		
			this.DomainPropertyAdded += UpdateLists;
			this.DomainPropertyRemoved += UpdateLists;
			this.DomainPropertyUpdated += UpdateLists;
		}
		
		private void UpdateLists (DomainProperty domProp) {
			this.controller.Window.conceptTreeView.Update ();
			this.controller.ViewController.RefreshCurrentView ();
		}
		
		
		public IEnumerable<DomainProperty> GetAll ()
		{
			return this.domainProperties.AsEnumerable ();
		}
		
		public void Add (DomainProperty domProp)
		{
			Add (domProp, true);
		}
		
		public void Add (DomainProperty domProp, bool notify)
		{
			this.domainProperties.Add (domProp);
			if (DomainPropertyAdded != null & notify) {
				DomainPropertyAdded (domProp);
			}
		}
		
		public void Remove (DomainProperty domProp)
		{
			this.domainProperties.Remove (domProp);
			if (DomainPropertyRemoved != null) {
				DomainPropertyRemoved (domProp);
			}
		}
		
		public void Update (DomainProperty domProp)
		{
			if (DomainPropertyUpdated != null) {
				DomainPropertyUpdated (domProp);
			}
		}
		
		public DomainProperty Get (string id)
		{
			return this.domainProperties.Find ((obj) => obj.Id == id);
		}
				
		public void AddDomainProperty (string domPropName, System.Action<DomainProperty> action)
		{
			var dialog = new AddDomainPropertyDialog(controller.Window, domPropName);
			dialog.Response += delegate(object o, Gtk.ResponseArgs args) {
				if (args.ResponseId == Gtk.ResponseType.Ok) {
					var domProp = new DomainProperty(dialog.DomainPropertyName, dialog.DomainPropertyDefinition);
					this.Add (domProp);
					action(domProp);
				}
				dialog.Destroy();
			};
		}
		
		public void AddDomainProperty (string domPropName)
		{
			AddDomainProperty (domPropName, (obj) => {});
		}
		
		public void AddDomainProperty ()
		{
			AddDomainProperty ("");
		}
		
		public void EditDomainProperty (DomainProperty domProp)
		{
			var dialog = new AddDomainPropertyDialog(controller.Window, domProp.Name);
			dialog.Response += delegate(object o, Gtk.ResponseArgs args) {
				if (args.ResponseId == Gtk.ResponseType.Ok) {
					domProp.Name = dialog.DomainPropertyName;
					domProp.Definition = dialog.DomainPropertyDefinition;
					this.Update (domProp);
				}
				dialog.Destroy();
			};
		}
		
		public void RemoveDomainProperty (DomainProperty domProp)
		{
			var dialog = new MessageDialog (this.controller.Window,
				DialogFlags.DestroyWithParent, MessageType.Question,
				ButtonsType.YesNo, false, string.Format ("Delete domain property '{0}'?", domProp.Name));
			
			dialog.Response += delegate(object o, ResponseArgs args) {
				if (args.ResponseId == Gtk.ResponseType.Yes) {
					this.Remove (domProp);
				}
				dialog.Destroy ();
			};
			
			dialog.Present ();
		}
		
		public void PopulateContextMenu (Menu menu, object source, object clickedElement)
		{
			if (clickedElement == null & source is ConceptsTreeView) {				
				var addItem = new MenuItem("Add domain property...");
				addItem.Activated += delegate(object sender2, EventArgs e) {
					this.AddDomainProperty ();
				};
				menu.Add(addItem);
			}
			
			if (clickedElement is DomainProperty) {
				var clickedDomProp = (DomainProperty) clickedElement;
				
				var editItem = new MenuItem("Edit...");
				editItem.Activated += delegate(object sender2, EventArgs e) {
					this.EditDomainProperty (clickedDomProp);
				};
				menu.Add(editItem);
				
				var deleteItem = new MenuItem("Delete");
				deleteItem.Activated += delegate(object sender2, EventArgs e) {
					this.RemoveDomainProperty (clickedDomProp);
				};
				menu.Add(deleteItem);
				
				menu.Add (new SeparatorMenuItem ());				
			}
		}
		
		public void Populate (TreeStore store)
		{
			var iter = store.AppendValues ("Domain properties", null, pixbuf);
			Populate (this.GetAll().Cast<KAOSElement>(), store, iter);
		}
		
		public void Populate (IEnumerable<KAOSElement> elements, TreeStore store, TreeIter iter)
		{
			foreach (var domProp in from e in elements where e is DomainProperty select (DomainProperty) e) {
				store.AppendValues (iter, domProp.Name, domProp, pixbuf);
			}
		}
	}
}

