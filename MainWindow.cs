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
	
	public MainWindow (GoalModel model): base (Gtk.WindowType.Toplevel)
	{
		this.model = model;
		
		views = new List<View>();
		views.Add(new View());
		
		Build ();
		scrolledWindow.Add(new DiagramArea (views[0]));
		
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
		var refinements = new Dictionary<string, List<string>>();
		model = new GoalModel();
		using (var reader = XmlReader.Create("example.xml")) {
			while (reader.Read()) {
				if (reader.IsStartElement("goal")) {
					string id = "", name = "";
					List<string> children = new List<string>();
					
					// Read all nodes of <goal>
					while (reader.Read()) {
						if (reader.IsStartElement("id")) {
							reader.Read();
							id = reader.Value.Trim();
							
						} else if (reader.IsStartElement("name")) {
							reader.Read();
							name = reader.Value.Trim();
							
						} else if (reader.IsStartElement("children")) {
							
							// Read all nodes of <children>
							while(reader.Read()) {
								if (reader.IsStartElement("child")) {
									reader.Read();
									string childId = reader.Value.Trim();
									children.Add(childId);
									
								} else if (reader.IsEndElement("children")) {
									break;
								}
							}
							
						} else if (reader.IsEndElement("goal")) {
							break;	
						}
					}
					
					model.Add(new Goal() { Name = name, Id = id });
					refinements.Add(id, children);
				}
			}
		}
		
		foreach (string k in refinements.Keys) {
			var refinement = new Refinement() { Id = "refinement1" };
			var g2 = model.Goals.Find(l => l.Id == k);
			if (g2 != null) {
				foreach (var childId in refinements[k]) {
					var g3 = model.Goals.Find(l2 => l2.Id == childId);
					if (g3 != null) {
						refinement.Add(g3);
					}
				}
				if (refinement.Refinees.Count > 0) { g2.Refinements.Add(refinement); }
			}
		}
		
		foreach (var view in views) {
			foreach (var goal in model.Goals) {
				view.Add(goal);
			}
		}
	}
	
	
	
}
