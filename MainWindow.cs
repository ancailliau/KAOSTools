using System;
using Gtk;
using Editor;
using Model;
using System.Xml;
using System.Collections.Generic;
using Shapes;

public partial class MainWindow: Gtk.Window
{	
	
	private GoalModel model;
	private List<View> views;
	
	private DiagramArea da;
	
	private TreeStore ls;
	
	private string filename;
	
	public MainWindow (GoalModel model): base (Gtk.WindowType.Toplevel)
	{
		this.model = model;
		
		views = new List<View>();
		views.Add(new View());
		
		Build ();
		da = new DiagramArea (views[0]);
		scrolledWindow.Add(da);
		
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
		
		// Maximize();
		ShowAll();
		Present();
		
		model.Changed += delegate(object sender, EventArgs e) {
			UpdateListStore();
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
						var ag = new AddGoal(model);
						ag.Present();
					};
					m.Add(addGoal);
					m.ShowAll();
					m.Popup();
				}
			}
		}
	}

	void HandleAddToViewActivated (Goal g)
	{
		views[0].Add(g);
		views[0].DrawingArea.QueueDraw();
	}
	
	protected void UpdateListStore()
	{
		ls.Clear();
		
		var iter = ls.AppendValues("Goals", null);
		if (model.Goals.Count > 0) {
			foreach (var element in model.Goals) {
				ls.AppendValues(iter, element.Name.Replace("\n", ""), element);
			}
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
		new XmlExporter(this.filename, model, views).Export();
	}
	
	protected virtual void OnOpenActionActivated (object sender, System.EventArgs e)
	{
		
		var dialog = new FileChooserDialog ("Choose file to open...",
			this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);
		
		if (dialog.Run() == (int) ResponseType.Accept) {
			this.filename = dialog.Filename;
			var importer = new XmlImporter(dialog.Filename);
			importer.Import();
			
			this.model = importer.Model;
			this.views = importer.Views;
			
			if (this.views.Count > 0) {
				this.da.UpdateCurrentView(this.views[0]);
				UpdateListStore();
				
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
