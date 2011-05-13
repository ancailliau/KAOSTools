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
		
	public void AddToCurrentView (IModelElement g)
	{
		if (g != null) {
			viewsNotebook.CurrentView.Add(g);
		} else {
			Console.WriteLine ("Ignoring element '{0}'", g.Id);
		}
	}
	
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		this.Controller.Quit ();
		a.RetVal = true;
	}
	
	protected virtual void OnQuitActionActivated (object sender, System.EventArgs e)
	{
		this.Controller.Quit ();
	}
		
	protected virtual void OnSaveActionActivated (object sender, System.EventArgs e)
	{
		this.Controller.Save ();
	}
	
	protected virtual void OnOpenActionActivated (object sender, System.EventArgs e)
	{
		this.Controller.Load ();
	}
	
	protected virtual void OnSaveAsActionActivated (object sender, System.EventArgs e)
	{
		this.Controller.SaveAs ();
	}
}
}
