using System;
using System.Collections.Generic;
using System.Xml;
using Cairo;
using KaosEditor;
using KaosEditor.Controllers;
using KaosEditor.Model;
using KaosEditor.UI.Shapes;
using KaosEditor.Logging;

namespace KaosEditor
{
	public class XmlImporter
	{
		private string filename;
		
		private EditorModel model;
		public EditorModel Model { get { return model ; } }
		
		private List<FutureGoal> futureGoals;
		private List<FutureAgent> futureAgents;
		private List<FutureView> futureViews;
		private List<FutureObstacle> futureObstacles;
		
		private class FutureGoal {
			public string id = "";
			public string name = "";
			public string definition = "";
			public List<FutureRefinement> refinements = new List<FutureRefinement>();
			public List<FutureResponsibility> futureResponsibilities = new List<FutureResponsibility>();
			public List<FutureObstruction> futureObstructions = new List<FutureObstruction>();
		}
		
		private class FutureAgent {
			public string id = "";
			public string name = "";
		}
		
		private class FutureRefinement {
			public string id = "";
			public string name = "";
			public List<string> refinees = new List<string>();
		}
		
		private class FutureResponsibility {
			public string id = "";
			public string name = "";
			public string agentId = "";
			public string goalId = "";
		}
		
		private class FutureObstruction {
			public string id = "";
			public string obstacleId = "";
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
		
		private class FutureObstacle {
			public string id = "";
			public string name = "";
			public string definition = "";
		}
		
		private MainController controller;
		
		public XmlImporter (string filename, MainController controller)
		{
			this.filename = filename;
			this.model = new EditorModel();
			
			this.futureGoals = new List<FutureGoal>();
			this.futureViews = new List<FutureView>();
			this.futureAgents = new List<FutureAgent>();
			this.futureObstacles = new List<FutureObstacle> ();
			
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
							
						} else if (reader.IsStartElement("responsibility")) {
							var responsibility = new FutureResponsibility() {
								id = reader.GetAttribute("id"),
								name = reader.GetAttribute("name"),
								goalId = id,
								agentId = reader.GetAttribute("agent-id")
							};
							futurGoal.futureResponsibilities.Add(responsibility);
						
						} else if (reader.IsStartElement ("obstruction")) {
							var obstruction = new FutureObstruction () {
								id = reader.GetAttribute ("id"),
								obstacleId = reader.GetAttribute ("obstacle-id")
							};
							futurGoal.futureObstructions.Add (obstruction);
							
						} else if (reader.IsStartElement ("definition")) {
							reader.Read ();
							futurGoal.definition = reader.Value.Trim();
							
						} else if (reader.IsEndElement("goal")) {
							break;
						
						}
					}
					
				} else if (reader.IsStartElement ("agent")) {
					string id = reader.GetAttribute ("id") ?? Guid.NewGuid().ToString();
					string name = reader.GetAttribute ("name");
					var futurAgent = new FutureAgent () { id = id, name = name };
					this.futureAgents.Add (futurAgent);					
				
				} else if (reader.IsStartElement ("obstacle")) {
					string id = reader.GetAttribute ("id") ?? Guid.NewGuid().ToString();
					string name = reader.GetAttribute ("name");
					var futurObstacle = new FutureObstacle () { id = id, name = name };
					while (!reader.IsEmptyElement && reader.Read()) {
						if (reader.IsStartElement ("definition")) {
							reader.Read ();
							futurObstacle.definition = reader.Value.Trim();
							
						} else if (reader.IsEndElement ("obstacle")) {
							break;
						}
					}
					futureObstacles.Add (futurObstacle);
					
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
				var goal = new Goal(futureGoal.name, futureGoal.definition) {
					Id = futureGoal.id
				};
				Model.Add (goal);
			}
			
			foreach (var futureAgent in futureAgents) {
				Model.Add(new Agent (futureAgent.name, futureAgent.id));
			}
			
			foreach (var futureObstacle in futureObstacles) {
				Model.Add(new Obstacle (futureObstacle.name, futureObstacle.definition) {
					Id = futureObstacle.id
				});
			}
			
			foreach (var futureGoal in futureGoals) {
				foreach (var futureRefinement in futureGoal.refinements) {
					Goal goal = (Goal) Model.Get(futureGoal.id);
					if (goal != null) {
						var refinement = new Refinement(goal) { Id = futureRefinement.id };
						foreach (var futureElement in futureRefinement.refinees) {
							refinement.Add(Model.Get(futureElement));
						}
						Model.Add (refinement);
					}
				}
				foreach (var futureResponsibility in futureGoal.futureResponsibilities) {
					Model.Add(new Responsibility (futureResponsibility.id,
						(Goal) Model.Get(futureResponsibility.goalId),
						(Agent) Model.Get(futureResponsibility.agentId)));
				}
				foreach (var futureObstruction in futureGoal.futureObstructions) {
					Model.Add(new Obstruction (
						(Goal) Model.Get(futureGoal.id),
						(Obstacle) Model.Get(futureObstruction.obstacleId)) { Id = futureObstruction.id});
				}
			}
			
			foreach (var futureView in futureViews) {
				var view = new View(futureView.name, controller);
				foreach (var futureElement in futureView.elements) {
					var element = ShapeFactory.Create(Model.Get(futureElement.elementId));
					if (element != null) {
						element.Position = futureElement.position;
						element.Depth = futureElement.depth;
						view.Add(element);
					}
				}
				Model.Views.Add(view);
			}
			
		}
		
	}
}

