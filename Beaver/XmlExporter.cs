using System;
using System.Xml;
using System.Linq;
using Beaver.Model;
using Beaver.Views;
using Beaver.Controllers;

namespace Beaver
{
	public class XmlExporter
	{
		private Version version = new Version(0, 1);
		private string filename;
		
		private MainController controller;
		
		private XmlWriterSettings settings;
		
		public XmlExporter (string filename, MainController controller)
		{
			this.filename = filename;
			this.controller = controller;
			
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
			foreach (var goal in this.controller.GoalController.GetAll()) {
				WriteGoal(writer, goal as Goal);
			}
			writer.WriteEndElement();
			
			writer.WriteStartElement("agents");
			foreach (var agent in this.controller.AgentController.GetAll()) {
				WriteAgent (writer, agent as Agent);
			}
			writer.WriteEndElement();
			
			writer.WriteStartElement("obstacles");
			foreach (var obstacle in this.controller.ObstacleController.GetAll()) {
				WriteObstacle(writer, obstacle as Obstacle);
			}
			writer.WriteEndElement();
			
			writer.WriteStartElement("domainproperties");
			foreach (var domProp in this.controller.DomainPropertyController.GetAll()) {
				WriteDomainProperty(writer, domProp as DomainProperty);
			}
			writer.WriteEndElement();
			
			writer.WriteEndElement();
		}
		
		public void WriteDomainProperty (XmlWriter writer, DomainProperty domProp)
		{
			writer.WriteStartElement("domainproperty");
			writer.WriteAttributeString("id", domProp.Id);
			writer.WriteAttributeString("name", domProp.Name);
			writer.WriteElementString("definition", domProp.Definition);
			writer.WriteEndElement();
		}
		
		public void WriteObstacle(XmlWriter writer, Obstacle obstacle)
		{	
			writer.WriteStartElement("obstacle");
			writer.WriteAttributeString("id", obstacle.Id);
			writer.WriteAttributeString("name", obstacle.Name);
			writer.WriteAttributeString("likelihood", obstacle.Likelihood.ToString());
			writer.WriteElementString("definition", obstacle.Definition);
			
			var refinements = this.controller.ObstacleRefinementController.GetAll (obstacle);
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
			writer.WriteAttributeString("likelihood", goal.Likelihood.ToString ());
			
			writer.WriteElementString("definition", goal.Definition);
				
			var refinements = this.controller.RefinementController.GetAll (goal);
					
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
			
			var responsibilities = this.controller.ResponsibilityController.GetAll (goal);
			
			foreach (var responsibility in responsibilities) {
				writer.WriteStartElement("responsibility");
				writer.WriteAttributeString("id", responsibility.Id);
				writer.WriteAttributeString("agent-id", responsibility.Agent.Id);
				writer.WriteEndElement();
			}
			
			var obstructions = this.controller.ObstructionController.GetAll (goal);
			
			foreach (var obstruction in obstructions) {
				writer.WriteStartElement("obstruction");
				writer.WriteAttributeString("id", obstruction.Id);
				writer.WriteAttributeString("obstacle-id", obstruction.Obstacle.Id);
				writer.WriteEndElement();
			}
			
			var resolutions = this.controller.ResolutionController.GetAll (goal);
			
			foreach (var resolution in resolutions) {
				writer.WriteStartElement("resolution");
				writer.WriteAttributeString("id", resolution.Id);
				writer.WriteAttributeString("obstacle-id", resolution.Obstacle.Id);
				writer.WriteEndElement();
			}
			
			var exceptions = this.controller.ExceptionController.GetAll (goal);
			
			foreach (var exception in exceptions) {
				writer.WriteStartElement("exception");
				writer.WriteAttributeString("id", exception.Id);
				writer.WriteAttributeString("goal-id", exception.ExceptionGoal.Id);
				writer.WriteAttributeString("condition", exception.Condition);
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
			foreach (var view in this.controller.ViewController.GetAll ()) {
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

