using System;
using System.Xml;
using Model;
using System.Collections.Generic;
using Editor.Model;

namespace Editor
{
	public class XmlExporter
	{
		private Version version = new Version(0, 1);
		private string filename;
		
		private GoalModel model;
		private Views views;
		
		private XmlWriterSettings settings;
		
		public XmlExporter (string filename, GoalModel model, Views views)
		{
			this.filename = filename;
			this.views = views;
			this.model = model;
			
			settings = new XmlWriterSettings();
			settings.Indent = true;
		}
		
		public void Export ()
		{		
			using (var writer = XmlWriter.Create(filename, settings)) {
				writer.WriteStartDocument();
				
				writer.WriteStartElement("kaoseditor");
				writer.WriteAttributeString("version", version.ToString());
			
				WriteModel (writer);
				WriteViews (writer);
				
				writer.WriteEndElement();
				
				writer.WriteEndDocument();
			}
		}
		
		public void WriteModel (XmlWriter writer)
		{
			
			writer.WriteStartElement("models");
			
			writer.WriteStartElement("goals");
			foreach (var goal in model.Goals) {
				WriteGoal(writer, goal);
			}
			writer.WriteEndElement();
			
			writer.WriteEndElement();
		}
		
		public void WriteGoal(XmlWriter writer, Goal goal)
		{	
			writer.WriteStartElement("goal");
				
			writer.WriteAttributeString("id", goal.Id);
			writer.WriteAttributeString("name", goal.Name);
				
			foreach (var refinement in goal.Refinements) {
				writer.WriteStartElement("refinement");
				writer.WriteAttributeString("id", refinement.Id);
				
				foreach (var child in refinement.Refinees) {
					writer.WriteStartElement("refinee");
					if (child.Id != null) {
						writer.WriteAttributeString("id", child.Id);
					}
					writer.WriteEndElement();
				}
				writer.WriteEndElement();
			}
			
			writer.WriteEndElement();
		}
		
		public void WriteViews (XmlWriter writer)
		{
			writer.WriteStartElement("views");
			foreach (var view in views) {
				writer.WriteStartElement("view");
				writer.WriteAttributeString("name", view.Name);
				foreach (var shape in view.Shapes) {
					writer.WriteStartElement("element");
					writer.WriteAttributeString("element-id", shape.RepresentedElement.Id);
					writer.WriteAttributeString("x", shape.Position.X.ToString());
					writer.WriteAttributeString("y", shape.Position.Y.ToString());
					writer.WriteAttributeString("depth", shape.Depth.ToString());
					writer.WriteEndElement();
				}
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}
		
	}
}

