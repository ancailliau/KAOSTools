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
	private ConceptsTreeView conceptTreeView;
	
	private string filename;
	
	public MainWindow (GoalModel model, Views views): base (Gtk.WindowType.Toplevel)
	{
		this.Model = model;
		this.Views = views;
		Build ();
		
		viewsNotebook = new ViewsNotebook();
		conceptTreeView = new ConceptsTreeView (this);
		
		hpaned1.Add1 (conceptTreeView);
		hpaned1.Add2 (viewsNotebook);
		hpaned1.ShowAll();
				
		Model.Changed += UpdateWidgets;
		Views.ViewsChanged += UpdateWidgets;
		Views.AddedView += UpdateWidgets;
			
	}
	
	private void UpdateWidgets (object sender, EventArgs args)
	{
		viewsNotebook.Update();
		conceptTreeView.Update();
	}
	
	public void DisplayView (string name)
	{
		viewsNotebook.DisplayView (Views.Get(name));
	}
		
	public void AddToCurrentView (Goal g)
	{
		viewsNotebook.CurrentView.Add(g);
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
