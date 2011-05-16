using System;
using System.Xml;
using Model;
using System.Collections.Generic;
using Cairo;
using Shapes;
using Editor.Model;
using Editor.Controllers;

namespace Editor
{
	public class XmlImporter
	{
		private string filename;
		
		private GoalModel model;
		public GoalModel Model { get { return model ; } }
		
		private Views views;
		public Views Views { get { return views ; } }
		
		private List<FutureGoal> futureGoals;
		private List<FutureView> futureViews;
		
		private class FutureGoal {
			public string id = "";
			public string name = "";
			public List<FutureRefinement> refinements = new List<FutureRefinement>();
		}
		
		private class FutureRefinement {
			public string id = "";
			public string name = "";
			public List<string> refinees = new List<string>();
		}
		
		private class FutureView {
			public string name = "";
			public List<FutureShape> elements = new List<FutureShape>();
		}
		
		private class FutureShape {
			public string elementId;
			public PointD position;
			public int depth;
		}
		
		private MainController controller;
		
		public XmlImporter (string filename, MainController controller)
		{
			this.filename = filename;
			this.model = new GoalModel();
			this.views = new Views();
			
			this.futureGoals = new List<FutureGoal>();
			this.futureViews = new List<FutureView>();
			
			this.controller = controller;
		}
		
		public void Import ()
		{
			Version version = null;
			using (var reader = XmlReader.Create(this.filename)) {
				while (reader.Read()) {
					if (reader.IsStartElement("kaoseditor")) {
						version = new Version(reader.GetAttribute("version"));
						
					} else if (reader.IsStartElement("models")) {
						ReadModels (reader);
						
					} else if (reader.IsStartElement("views")) {
						ReadViews (reader);
						
					} else if (reader.IsEndElement("kaoseditor")) {
						break;
					}
				}
			}
			
			Compile();
		}
		
		public void ReadModels (XmlReader reader)
		{
			while (reader.Read ()) {
				if (reader.IsStartElement ("goal")) {
					string id = reader.GetAttribute ("id") ?? Guid.NewGuid().ToString();
					string name = reader.GetAttribute ("name");
					var futurGoal = new FutureGoal () { id = id, name = name };
					this.futureGoals.Add (futurGoal);
					
					while (!reader.IsEmptyElement && reader.Read()) {
						if (reader.IsStartElement("refinement")) {
							var refinement = new FutureRefinement() {
								id = reader.GetAttribute("id"),
								name = reader.GetAttribute("name")
							};
							while (reader.Read()) {
								if (reader.IsStartElement("refinee")) {
									string refineeId = reader.GetAttribute("id");
									refinement.refinees.Add(refineeId);
									
								} else if (reader.IsEndElement("refinement")) {
									break;
								}
							}
							futurGoal.refinements.Add(refinement);
							
						} else if (reader.IsEndElement("goal")) {
							break;
						
						}
					}
					
				} else if (reader.IsEndElement ("models")) {
					break;
				}
			}
		}
		
		public void ReadViews (XmlReader reader)
		{
			while (reader.Read()) {
				if (reader.IsStartElement ("view")) {
					var view = new FutureView () {
						name = reader.GetAttribute ("name")
					};
					futureViews.Add(view);
					while (reader.Read()) {
						if (reader.IsStartElement ()) {
							string elementId = reader.GetAttribute("element-id");
							string x = reader.GetAttribute("x");
							string y = reader.GetAttribute("y");
							string depth = reader.GetAttribute("depth");
							view.elements.Add(new FutureShape() { 
								elementId = elementId,
								position = new PointD(Double.Parse(x), Double.Parse(y)),
								depth = int.Parse(depth)
							});
							
						} else if (reader.IsEndElement ("view")) {
							break;
						}	
					}
					
				} else if (reader.IsEndElement ("views")) {
					break;
				}
			}
		}
		
		public void Compile ()
		{
			foreach (var futureGoal in futureGoals) {
				var goal = new Goal() {
					Id = futureGoal.id, Name = futureGoal.name
				};
				Model.Add(goal);
			}
			
			foreach (var futureGoal in futureGoals) {
				foreach (var futureRefinement in futureGoal.refinements) {
					var refinement = new Refinement() { Id = futureRefinement.id, Name = futureRefinement.name };
					foreach (var futureElement in futureRefinement.refinees) {
						refinement.Add(Model.Get(futureElement));
					}
					Goal goal = (Goal) Model.Get(futureGoal.id);
					if (goal != null) {
						refinement.Refined = goal;
						goal.Refinements.Add(refinement);
						Model.Add(refinement);
					}
				}
			}
			
			foreach (var futureView in futureViews) {
				var view = new View(controller) { Name = futureView.name };
				foreach (var futureElement in futureView.elements) {
					var element = ShapeFactory.Create(Model.Get(futureElement.elementId));
					if (element != null) {
						element.Position = futureElement.position;
						element.Depth = futureElement.depth;
						view.Add(element);
					} else {
						Console.WriteLine ("Ignoring " + Model.Get(futureElement.elementId));
					}
				}
				Views.Add(view);
			}
			
		}
		
	}
}

