using System;
using Gtk;
using Editor.Windows;
using Editor.Dialogs;
using Model;

namespace Editor.Widgets
{
	public class ConceptsTreeView : TreeView
	{
		private MainWindow window;
		private TreeStore store;
		
		public ConceptsTreeView (MainWindow window)
		{
			// Bind main window
			this.window = window;
			
			// Build column and renderers	
			var column = new TreeViewColumn ();
			column.Title = "Concepts";
			this.AppendColumn (column);
			this.HeadersVisible = false;
			
			var cell = new CellRendererText();
			column.PackStart(cell, true);
			column.AddAttribute(cell, "text", 0);
			
			// Bind the model
			store = new TreeStore(typeof(string), typeof(object));
			this.Model = store;
			
			// Bind the events
			this.AddEvents((int) Gdk.EventMask.ButtonPressMask);
			this.ButtonPressEvent += OnItemButtonPressed;
			this.RowActivated += HandleHandleRowActivated;	
			
			// Set up the content
			this.Update ();
		}

		void HandleHandleRowActivated (object o, RowActivatedArgs args)
		{
			TreeIter iter;
			store.GetIter(out iter, args.Path);
			
			bool inViews = false;
			if (args.Path.Depth > 1 && args.Path.Up()) {
				TreeIter iterParent;
				Console.WriteLine ();
				store.GetIter(out iterParent, args.Path);
				if (((string) store.GetValue(iterParent, 0)) == "Views") {
					inViews = true;
				}
			}
			
			if (inViews) {
				string name = (string) store.GetValue(iter, 0);
				this.window.DisplayView(name);
			}
		}
		
		
		[GLib.ConnectBeforeAttribute]
		protected void OnItemButtonPressed (object sender, ButtonPressEventArgs args) 
		{
			if (args.Event.Button == 3) {
				var path = new TreePath();
				this.GetPathAtPos(System.Convert.ToInt16(args.Event.X), 
					System.Convert.ToInt16(args.Event.Y), out path);
				
				TreeIter iter;
				if (store.GetIter(out iter, path)) {
					object o = store.GetValue(iter, 1);
					if (o != null) {
						var m = new Menu();
						var addToView = new MenuItem("Add to current view");
						addToView.Activated += delegate(object sender2, EventArgs e) {
							this.window.AddToCurrentView (o as Goal);
						};
						m.Add(addToView);
						m.ShowAll();
						m.Popup();
						
					} else if (((string) store.GetValue(iter, 0)) == "Goals") {
						var m = new Menu();
						var addGoal = new MenuItem("Add goal");
						addGoal.Activated += delegate(object sender2, EventArgs e) {
							var ag = new AddGoal(window.Controller);
							ag.Present();
						};
						m.Add(addGoal);
						m.ShowAll();
						m.Popup();
						
					} else if (((string) store.GetValue(iter, 0)) == "Views") {
						var m = new Menu();
						var addView = new MenuItem("Add view");
						addView.Activated += delegate(object sender2, EventArgs e) {
							var ag = new AddView(window.Controller);
							ag.Present();
						};
						m.Add(addView);
						m.ShowAll();
						m.Popup();
					}
				}
			}
		}
		
		public void Update()
		{
			store.Clear();
			
			var iter = store.AppendValues("Goals", null);
			foreach (var element in this.window.Model.Goals) {
				store.AppendValues(iter, element.Name.Replace("\n", ""), element);
			}
			
			iter = store.AppendValues("Views", null);
			foreach (var view in this.window.Views) {
				store.AppendValues(iter, view.Name, view);
			}
		}
		
		
	}
}

