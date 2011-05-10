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
		new XmlExporter("example2.xml", model, views).Export();
	}
	
	protected virtual void OnOpenActionActivated (object sender, System.EventArgs e)
	{
		var importer = new XmlImporter("example2.xml");
		importer.Import();
		this.model = importer.Model;
		this.views = importer.Views;
		
		if (this.views.Count > 0) {
			this.da.UpdateCurrentView(this.views[0]);		
		} else {
			Console.WriteLine ("Something went wrong");
		}
	}
	
	
	
}
