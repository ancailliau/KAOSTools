using System;
using Gtk;
using Editor;
using Model;

public partial class MainWindow: Gtk.Window
{	
	
	private GoalModel model;
	
	public MainWindow (GoalModel model): base (Gtk.WindowType.Toplevel)
	{
		this.model = model;
		
		Build ();
		scrolledWindow.Add(new GoalGraph(new Graph(model)));
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
		
}
