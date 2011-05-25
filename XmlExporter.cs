using System;
using System.Xml;
using System.Linq;
using KaosEditor.Model;

namespace KaosEditor
{
	public class XmlExporter
	{
		private Version version = new Version(0, 1);
		private string filename;
		
		private EditorModel model;
		
		private XmlWriterSettings settings;
		
		public XmlExporter (string filename, EditorModel model)
		{
			this.filename = filename;
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
			foreach (var goal in model.Elements.FindAll(e => e is Goal)) {
				WriteGoal(writer, goal as Goal);
			}
			writer.WriteEndElement();
			
			writer.WriteStartElement("agents");
			foreach (var agent in model.Elements.FindAll(e => e is Agent)) {
				WriteAgent (writer, agent as Agent);
			}
			writer.WriteEndElement();
			
			writer.WriteStartElement("obstacles");
			foreach (var obstacle in model.Elements.FindAll(e => e is Obstacle)) {
				WriteObstacle(writer, obstacle as Obstacle);
			}
			writer.WriteEndElement();
			
			
			writer.WriteEndElement();
		}
		
		public void WriteObstacle(XmlWriter writer, Obstacle obstacle)
		{	
			writer.WriteStartElement("obstacle");
			writer.WriteAttributeString("id", obstacle.Id);
			writer.WriteAttributeString("name", obstacle.Name);
			writer.WriteElementString("definition", obstacle.Definition);
			
			var refinements = from e in model.Elements 
				where e is ObstacleRefinement && ((ObstacleRefinement) e).Refined.Equals (obstacle) 
					select (ObstacleRefinement) e;
			
			foreach (var refinement in refinements) {
				writer.WriteStartElement("refinement");
				writer.WriteAttributeString("id", refinement.Id);
				
				foreach (var child in refinement.Refinees) {
					writer.WriteStartElement("refinee");
					writer.WriteAttributeString("id", child.Id);
					writer.WriteEndElement();
				}
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}
		
		public void WriteGoal(XmlWriter writer, Goal goal)
		{	
			writer.WriteStartElement("goal");
				
			writer.WriteAttributeString("id", goal.Id);
			writer.WriteAttributeString("name", goal.Name);
			
			writer.WriteElementString("definition", goal.Definition);
				
			var refinements = from e in model.Elements 
				where e is Refinement && ((Refinement) e).Refined.Equals (goal) 
					select (Refinement) e;
					
			foreach (var refinement in refinements) {
				writer.WriteStartElement("refinement");
				writer.WriteAttributeString("id", refinement.Id);
				
				foreach (var child in refinement.Refinees) {
					writer.WriteStartElement("refinee");
					writer.WriteAttributeString("id", child.Id);
					writer.WriteEndElement();
				}
				writer.WriteEndElement();
			}
			
			var responsibilities = from e in model.Elements 
				where e is Responsibility && ((Responsibility) e).Goal.Equals (goal) 
					select (Responsibility) e;
			
			foreach (var responsibility in responsibilities) {
				writer.WriteStartElement("responsibility");
				writer.WriteAttributeString("id", responsibility.Id);
				writer.WriteAttributeString("agent-id", responsibility.Agent.Id);
				writer.WriteEndElement();
			}
			
			var obstructions = from e in model.Elements 
				where e is Obstruction && ((Obstruction) e).Goal.Equals (goal) 
					select (Obstruction) e;
			
			foreach (var obstruction in obstructions) {
				writer.WriteStartElement("obstruction");
				writer.WriteAttributeString("id", obstruction.Id);
				writer.WriteAttributeString("obstacle-id", obstruction.Obstacle.Id);
				writer.WriteEndElement();
			}
			
			var resolutions = from e in model.Elements 
				where e is Resolution && ((Resolution) e).Goal.Equals (goal) 
					select (Resolution) e;
			
			foreach (var resolution in resolutions) {
				writer.WriteStartElement("resolution");
				writer.WriteAttributeString("id", resolution.Id);
				writer.WriteAttributeString("obstacle-id", resolution.Obstacle.Id);
				writer.WriteEndElement();
			}
			
			var exceptions = from e in model.Elements 
				where e is ExceptionLink && ((ExceptionLink) e).Goal.Equals (goal) 
					select (ExceptionLink) e;
			
			foreach (var exception in exceptions) {
				writer.WriteStartElement("exception");
				writer.WriteAttributeString("id", exception.Id);
				writer.WriteAttributeString("goal-id", exception.ExceptionGoal.Id);
				writer.WriteEndElement();
			}
			
			writer.WriteEndElement();
		}
		
		public void WriteAgent (XmlWriter writer, Agent agent)
		{	
			writer.WriteStartElement("agent");
				
			writer.WriteAttributeString("id", agent.Id);
			writer.WriteAttributeString("name", agent.Name);
			
			writer.WriteEndElement();
		}
		
		public void WriteViews (XmlWriter writer)
		{
			writer.WriteStartElement("views");
			foreach (var view in model.Views) {
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

