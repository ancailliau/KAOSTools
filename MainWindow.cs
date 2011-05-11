using System;
using Gtk;
using Editor;
using Model;
using System.Xml;
using System.Collections.Generic;

public partial class MainWindow: Gtk.Window
{	
	
	private GoalModel model;
	private List<View> views;
	
	private DiagramArea da;
	
	private string filename;
	
	public MainWindow (GoalModel model): base (Gtk.WindowType.Toplevel)
	{
		this.model = model;
		
		views = new List<View>();
		views.Add(new View());
		
		Build ();
		da = new DiagramArea (views[0]);
		scrolledWindow.Add(da);
		
		ShowAll();
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
