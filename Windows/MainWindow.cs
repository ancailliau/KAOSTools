using System;
using Gtk;
using Editor;
using Model;
using System.Xml;
using System.Collections.Generic;
using Shapes;
using Editor.Dialogs;
using Editor.Widgets;
using Editor.Controllers;
using Editor.Model;

namespace Editor.Windows {

public partial class MainWindow: Gtk.Window
{	
	
	public GoalModel Model {
		get;
		set;
	}
	
	public MainController Controller {
		get;
		set;
	}
	
	public Views Views {
		get;
		set;
	}
	
	private ViewsNotebook viewsNotebook;
	
	private TreeStore ls;
	
	private string filename;
	
	public MainWindow (GoalModel model, Views views): base (Gtk.WindowType.Toplevel)
	{
		Build ();
		
		viewsNotebook = new ViewsNotebook();
		hpaned1.Add2(viewsNotebook);
		
		this.Model = model;
		this.Views = views;
				
		// Build tree view		
		var conceptCol = new TreeViewColumn ();
		conceptCol.Title = "Concepts";
		modelTreeView.AppendColumn (conceptCol);
		
		var goalNameCell = new CellRendererText();
		conceptCol.PackStart(goalNameCell, true);
		conceptCol.AddAttribute(goalNameCell, "text", 0);
		
		ls = new TreeStore(typeof(string), typeof(object));
		modelTreeView.Model = ls;
		UpdateListStore();
		
		modelTreeView.AddEvents((int) Gdk.EventMask.ButtonPressMask);
		
		modelTreeView.ButtonPressEvent += new ButtonPressEventHandler(OnItemButtonPressed);
		
		modelTreeView.RowActivated += delegate(object o, RowActivatedArgs args) {
			TreeIter iter;
			ls.GetIter(out iter, args.Path);
			
			bool inViews = false;
			if (args.Path.Depth > 1 && args.Path.Up()) {
				TreeIter iterParent;
				Console.WriteLine ();
				ls.GetIter(out iterParent, args.Path);
				if (((string) ls.GetValue(iterParent, 0)) == "Views") {
					inViews = true;
				}
			}
			
			if (inViews) {
				string name = (string) ls.GetValue(iter, 0);
				viewsNotebook.DisplayView (Views.Get(name));
			}
		};
		
		Model.Changed += delegate(object sender, EventArgs e) {
			UpdateListStore ();
		};
		
		Views.ViewsChanged += delegate(object sender, EventArgs e) {
			UpdateListStore();
		};
			
		Views.AddedView += delegate(object sender, EventArgs e) {
			UpdateListStore ();
		};
	}
	
	[GLib.ConnectBeforeAttribute]
	protected void OnItemButtonPressed (object sender, ButtonPressEventArgs args) 
	{
		if (args.Event.Button == 3) {
			var path = new TreePath();
			modelTreeView.GetPathAtPos(System.Convert.ToInt16(args.Event.X), 
				System.Convert.ToInt16(args.Event.Y), out path);
			
			TreeIter iter;
			if (ls.GetIter(out iter, path)) {
				object o = ls.GetValue(iter, 1);
				if (o != null) {
					var m = new Menu();
					var addToView = new MenuItem("Add to current view");
					addToView.Activated += delegate(object sender2, EventArgs e) {
						HandleAddToViewActivated(o as Goal);
					};
					m.Add(addToView);
					m.ShowAll();
					m.Popup();
				} else if (((string) ls.GetValue(iter, 0)) == "Goals") {
					var m = new Menu();
					var addGoal = new MenuItem("Add goal");
					addGoal.Activated += delegate(object sender2, EventArgs e) {
						var ag = new AddGoal(Model);
						ag.Present();
					};
					m.Add(addGoal);
					m.ShowAll();
					m.Popup();
				} else if (((string) ls.GetValue(iter, 0)) == "Views") {
					var m = new Menu();
					var addView = new MenuItem("Add view");
					addView.Activated += delegate(object sender2, EventArgs e) {
						var ag = new AddView(Model, Views);
						ag.Present();
					};
					m.Add(addView);
					m.ShowAll();
					m.Popup();
				}
			}
		}
	}

	void HandleAddToViewActivated (Goal g)
	{
		viewsNotebook.CurrentView.Add(g);
	}
	
	protected void UpdateListStore()
	{
		ls.Clear();
		
		var iter = ls.AppendValues("Goals", null);
		if (Model.Goals.Count > 0) {
			foreach (var element in Model.Goals) {
				ls.AppendValues(iter, element.Name.Replace("\n", ""), element);
			}
		}
		
		iter = ls.AppendValues("Views", null);
		if (Views.Count > 0) {
			foreach (var view in Views) {
				ls.AppendValues(iter, view.Name, view);
			}
		}
		
		if (this.viewsNotebook != null && this.viewsNotebook.CurrentView != null) {
			this.viewsNotebook.CurrentView.Redraw();
		} else {
			Console.WriteLine (this.viewsNotebook + " " + this.viewsNotebook.CurrentView);	
		}
	}
	
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}
	
	protected virtual void OnQuitActionActivated (object sender, System.EventArgs e)
	{
		Application.Quit();
	}
		
	protected virtual void OnSaveActionActivated (object sender, System.EventArgs e)
	{
		if (this.filename == null | this.filename == "") {
			SaveAs();
		} else {
			Save();
		}
	}
	
	protected void Save ()
	{
		new XmlExporter(this.filename, Model, Views).Export();
	}
	
	protected virtual void OnOpenActionActivated (object sender, System.EventArgs e)
	{
		
		var dialog = new FileChooserDialog ("Choose file to open...",
			this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);
		
		if (dialog.Run() == (int) ResponseType.Accept) {
			this.filename = dialog.Filename;
			var importer = new XmlImporter(dialog.Filename);
			importer.Import();
			
			this.Model = importer.Model;
			this.Views = importer.Views;
			
			if (this.Views.Count > 0) {
				foreach (var v in importer.Views) {
					Views.Add(v);
				}
				
			} else {
				var errorDialog = new MessageDialog(this, DialogFlags.DestroyWithParent, MessageType.Error,
					ButtonsType.Ok, false, "File is malformed.");
				
				if (errorDialog.Run() > 0) {
					errorDialog.Destroy();
				}
			}
		}
		
		dialog.Destroy();
	}
	
	protected virtual void OnSaveAsActionActivated (object sender, System.EventArgs e)
	{
		SaveAs();
	}
	
	protected void SaveAs() {
		var dialog = new FileChooserDialog("Save file", this,
			FileChooserAction.Save, "Cancel", ResponseType.Cancel, "Save", ResponseType.Accept);
		
		if (dialog.Run() == (int) ResponseType.Accept) {
			this.filename = dialog.Filename;
			Save();
		}
		
		dialog.Destroy();		
	}
	
}
}
