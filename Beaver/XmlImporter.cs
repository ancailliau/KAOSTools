using System;
using System.Collections.Generic;
using System.Xml;
using Cairo;
using Beaver;
using Beaver.Controllers;
using Beaver.Model;
using Beaver.UI.Shapes;
using Beaver.Logging;
using Beaver.Views;

namespace Beaver
{
	public class XmlImporter
	{
		private string filename;
		
		private List<FutureGoal> futureGoals;
		private List<FutureAgent> futureAgents;
		private List<FutureView> futureViews;
		private List<FutureObstacle> futureObstacles;
		private List<FutureDomainProperty> futureDomainProperties;
		
		private class FutureGoal {
			public string id = "";
			public string name = "";
			public string definition = "";
			public List<FutureRefinement> refinements = new List<FutureRefinement>();
			public List<FutureResponsibility> futureResponsibilities = new List<FutureResponsibility>();
			public List<FutureObstruction> futureObstructions = new List<FutureObstruction>();
			public List<FutureResolution> futureResolutions = new List<FutureResolution>();
			public List<FutureException> futureExceptions = new List<FutureException>();
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
		
		private class FutureResolution {
			public string id = "";
			public string obstacleId = "";
		}
		
		private class FutureException {
			public string id = "";
			public string goalId = "";
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
			public List<FutureObstacleRefinement> refinements = new List<FutureObstacleRefinement>();
		}
		
		private class FutureObstacleRefinement {
			public string id = "";
			public List<string> refinees = new List<string>();
		}
		
		private class FutureDomainProperty {
			public string id = "";
			public string name = "";
			public string definition = "";
		}
		
		private MainController controller;
		
		public XmlImporter (string filename, MainController controller)
		{
			this.filename = filename;
			
			this.futureGoals = new List<FutureGoal>();
			this.futureViews = new List<FutureView>();
			this.futureAgents = new List<FutureAgent>();
			this.futureObstacles = new List<FutureObstacle> ();
			this.futureDomainProperties = new List<FutureDomainProperty> ();
			
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
					var futureGoal = new FutureGoal () { id = id, name = name };
					this.futureGoals.Add (futureGoal);
					
					while (reader.Read()) {
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
							futureGoal.refinements.Add(refinement);
							
						} else if (reader.IsStartElement("responsibility")) {
							var responsibility = new FutureResponsibility() {
								id = reader.GetAttribute("id"),
								name = reader.GetAttribute("name"),
								goalId = id,
								agentId = reader.GetAttribute("agent-id")
							};
							futureGoal.futureResponsibilities.Add(responsibility);
						
						} else if (reader.IsStartElement ("obstruction")) {
							var obstruction = new FutureObstruction () {
								id = reader.GetAttribute ("id"),
								obstacleId = reader.GetAttribute ("obstacle-id")
							};
							futureGoal.futureObstructions.Add (obstruction);
							
						} else if (reader.IsStartElement ("resolution")) {
							var resolution = new FutureResolution () {
								id = reader.GetAttribute ("id"),
								obstacleId = reader.GetAttribute ("obstacle-id")
							};
							futureGoal.futureResolutions.Add (resolution);
						
						} else if (reader.IsStartElement ("exception")) {
							var exception = new FutureException () {
								id = reader.GetAttribute ("id"),
								goalId = reader.GetAttribute ("goal-id")
							};
							futureGoal.futureExceptions.Add (exception);
							
						} else if (reader.IsStartElement ("definition")) {
							reader.Read ();
							futureGoal.definition = reader.Value.Trim();
							
						} else if (reader.IsEndElement("goal")) {
							break;
						
						}
					}
					
				} else if (reader.IsStartElement ("agent")) {
					string id = reader.GetAttribute ("id") ?? Guid.NewGuid().ToString();
					string name = reader.GetAttribute ("name");
					var futurAgent = new FutureAgent () { id = id, name = name };
					this.futureAgents.Add (futurAgent);					
					
				} else if (reader.IsStartElement ("domainproperty")) {
					string id = reader.GetAttribute ("id") ?? Guid.NewGuid().ToString();
					string name = reader.GetAttribute ("name");
					var futureDomProp = new FutureDomainProperty () { id = id, name = name };
					while (!reader.IsEmptyElement && reader.Read()) {
						if (reader.IsStartElement ("definition")) {
							reader.Read ();
							futureDomProp.definition = reader.Value.Trim();
							
						} else if (reader.IsEndElement ("domainproperty")) {
							break;
						}
					}
					futureDomainProperties.Add (futureDomProp);
					
				} else if (reader.IsStartElement ("obstacle")) {
					string id = reader.GetAttribute ("id") ?? Guid.NewGuid().ToString();
					string name = reader.GetAttribute ("name");
					var futurObstacle = new FutureObstacle () { id = id, name = name };
					while (!reader.IsEmptyElement && reader.Read()) {
						if (reader.IsStartElement("refinement")) {
							var refinement = new FutureObstacleRefinement() {
								id = reader.GetAttribute("id")
							};
							while (reader.Read()) {
								if (reader.IsStartElement("refinee")) {
									string refineeId = reader.GetAttribute("id");
									refinement.refinees.Add(refineeId);
									
								} else if (reader.IsEndElement("refinement")) {
									break;
								}
							}
							futurObstacle.refinements.Add(refinement);
							
						} else if (reader.IsStartElement ("definition")) {
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
				this.controller.GoalController.Add (new Goal(futureGoal.name, futureGoal.definition) {
					Id = futureGoal.id
				}, false);
			}
			
			foreach (var futureAgent in futureAgents) {
				this.controller.AgentController.Add(new Agent (futureAgent.name, futureAgent.id), false);
			}
			
			foreach (var futureObstacle in futureObstacles) {
				this.controller.ObstacleController.Add(new Obstacle (futureObstacle.name, futureObstacle.definition) {
					Id = futureObstacle.id
				}, false);
			}
			
			foreach (var futureDomainProperty in futureDomainProperties) {
				this.controller.DomainPropertyController.Add (new DomainProperty (futureDomainProperty.name, futureDomainProperty.definition) {
					Id = futureDomainProperty.id
				}, false);
			}
			
			foreach (var futureGoal in futureGoals) {
				foreach (var futureRefinement in futureGoal.refinements) {
					Goal goal = this.controller.GoalController.Get(futureGoal.id);
					if (goal != null) {
						var refinement = new Refinement(goal) { Id = futureRefinement.id };
						foreach (var futureElement in futureRefinement.refinees) {
							IGoalRefinee iGoalRefinee = (IGoalRefinee)this.controller.Get (futureElement);
							if (iGoalRefinee != null) {
								refinement.Add(iGoalRefinee);
							} else {
								Logger.Warning ("Ignoring '{0}'", futureElement);
							}
						}
						this.controller.RefinementController.Add (refinement, false);
					}
				}
				foreach (var futureResponsibility in futureGoal.futureResponsibilities) {
					this.controller.ResponsibilityController.Add(new Responsibility (futureResponsibility.id,
						this.controller.GoalController.Get(futureResponsibility.goalId),
						this.controller.AgentController.Get(futureResponsibility.agentId)), false);
				}
				foreach (var futureObstruction in futureGoal.futureObstructions) {
					this.controller.ObstructionController.Add(new Obstruction (
						this.controller.GoalController.Get(futureGoal.id),
						this.controller.ObstacleController.Get(futureObstruction.obstacleId)) { Id = futureObstruction.id}, false);
				}
				foreach (var futureResolution in futureGoal.futureResolutions) {
					this.controller.ResolutionController.Add(new Resolution (
						this.controller.ObstacleController.Get(futureResolution.obstacleId),
						this.controller.GoalController.Get(futureGoal.id)) { Id = futureResolution.id}, false);
				}
				foreach (var futureException in futureGoal.futureExceptions) {
					this.controller.ExceptionController.Add(new ExceptionLink (
						this.controller.GoalController.Get(futureGoal.id),
						this.controller.GoalController.Get(futureException.goalId)) { Id = futureException.id}, false);
				}
			}
			
			foreach (var futureObstacle in futureObstacles) {
				foreach (var futureRefinement in futureObstacle.refinements) {
					Obstacle obstacle = this.controller.ObstacleController.Get(futureObstacle.id);
					if (obstacle != null) {
						var refinement = new ObstacleRefinement(obstacle) { Id = futureRefinement.id };
						foreach (var futureElement in futureRefinement.refinees) {
							refinement.Add(this.controller.ObstacleController.Get(futureElement));
						}
						this.controller.ObstacleRefinementController.Add (refinement, false);
					}
				}
			}
			
			foreach (var futureView in futureViews) {
				var view = new ModelView(futureView.name, controller);
				foreach (var futureElement in futureView.elements) {
					var element = ShapeFactory.Create(this.controller.Get(futureElement.elementId));
					if (element != null) {
						element.Position = futureElement.position;
						element.Depth = futureElement.depth;
						view.Add(element);
					}
				}
				this.controller.ViewController.Add(view, false);
			}
			
		}
		
	}
}

