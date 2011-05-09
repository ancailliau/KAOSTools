using System;
using Gtk;
using Editor;
using Model;
using System.Xml;
using System.Collections.Generic;

public partial class MainWindow: Gtk.Window
{	
	
	private GoalModel model;
	private GoalGraph graph;
	
	public MainWindow (GoalModel model): base (Gtk.WindowType.Toplevel)
	{
		this.model = model;
		
		Build ();
		graph = new GoalGraph (new Graph (model));
		scrolledWindow.Add(graph);
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
		var settings = new XmlWriterSettings();
		settings.Indent = true;
		
		using (var writer = XmlWriter.Create("example.xml", settings)) {
			writer.WriteStartDocument();
			writer.WriteStartElement("goals");
			foreach (var g in model.Goals) {
				writer.WriteStartElement("goal");
				writer.WriteElementString("id", g.Id);
				writer.WriteStartElement("name");
				writer.WriteCData(g.Name);
				writer.WriteEndElement();
				if (g.Children.Count > 0) {
					writer.WriteStartElement("children");
					foreach (var g2 in g.Children) {
						writer.WriteElementString("child", g2.Id);
					}
					writer.WriteEndElement();
				}
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
			writer.WriteEndDocument();
		}
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
					
					model.Goals.Add(new Goal() { Name = name, Id = id });
					refinements.Add(id, children);
				}
			}
		}
		
		foreach (string k in refinements.Keys) {
			var g2 = model.Goals.Find(l => l.Id == k);
			if (g2 != null) {
				foreach (var childId in refinements[k]) {
					var g3 = model.Goals.Find(l2 => l2.Id == childId);
					if (g3 != null) {
						g2.Children.Add(g3);
					}
				}
			}
		}
		
		graph.Update(new Graph(model));
	}
	
	
	
}
