using System;
using TechTalk.SpecFlow;
using Beaver.Domain;
using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;

namespace Beaver.Tests
{
	[Binding]
	public class ElementsSteps
	{
        private GoalModel model;

        private IEnumerable<Refinement> lastRefinements;

		[Given(@"a model")]
        public void GivenAModel()
        {
            model = new GoalModel ();
        }

        [Given(@"a goal '([a-zA-Z0-9-_ ]+)'")]
        public void GivenAGoal(string id)
        {
            model.Add (new Goal (id));
        }

        [Given(@"a domain property '([a-zA-Z0-9-_ ]+)'")]
        public void GivenADomainProperty(string id)
        {
            model.Add (new DomainProperty (id));
        }

        [Given(@"an agent '([a-zA-Z0-9-_ ]+)'")]
        public void GivenAnAgent(string id)
        {
            model.Add (new Agent (id));
        }

        [Given(@"an obstacle '([a-zA-Z0-9-_ ]+)'")]
        public void GivenAnObstacle(string id)
        {
            model.Add (new Obstacle (id));
        }

        [Given(@"a view '([a-zA-Z0-9-_ ]+)'")]
        public void GivenAView(string id)
        {
            model.AddView (new View (id));
        }

        [Given(@"the view '([a-zA-Z0-9-_ ]+)' contains goal '([a-zA-Z0-9-_ ]+)'")]
        public void GivenTheViewContainsElement(string viewId, string id)
        {
            model.Add (new Goal (id));
            model.GetView (viewId).Add (id);
        }

        [Given(@"a refinement '([a-zA-Z0-9-_ ]+)'")]
        public void GivenARefinement (string id)
        {
            model.Add (new GoalRefinement (id));
        }

        [Given(@"an obstacle refinement '([a-zA-Z0-9-_ ]+)'")]
        public void GivenAnObstacleRefinement (string id)
        {
            model.Add (new ObstacleRefinement (id));
        }
        
        [Given(@"a refinement '([a-zA-Z0-9-_ ]+)' for '([a-zA-Z0-9-_ ]+)'")]
        public void GivenARefinement (string id, string parentId)
        {
            model.Add (new GoalRefinement (id) { Parent = parentId });
        }

        [Given(@"an obstacle refinement '([a-zA-Z0-9-_ ]+)' for '([a-zA-Z0-9-_ ]+)'")]
        public void GivenAnObstacleRefinement (string id, string parentId)
        {
            model.Add (new ObstacleRefinement (id) { Parent = parentId });
        }

        [Given(@"refinement '([a-zA-Z0-9-_ ]+)' contains '([a-zA-Z0-9-_ ]+)'")]
        public void GivenARefinementContainsElement (string id, string elementId)
        {
            var refinement = model.Get (id);

            if (refinement is GoalRefinement)
                ((GoalRefinement) refinement).Children.Contains (elementId);

            else if (refinement is ObstacleRefinement)
                ((ObstacleRefinement) refinement).Children.Contains (elementId);
        }

        [When(@"I add a goal '([a-zA-Z0-9-_ ]+)'")]
        public void WhenIAddAGoal(string id)
        {
            model.Add (new Goal (id));
        }

        [When(@"I add an obstacle '([a-zA-Z0-9-_ ]+)'")]
        public void WhenIAddAnObstacle(string id)
        {
            model.Add (new Obstacle (id));
        }

        [When(@"I add a domain property '([a-zA-Z0-9-_ ]+)'")]
        public void WhenIAddADomainProperty(string id)
        {
            model.Add (new DomainProperty (id));
        }

        [When(@"I add an agent '([a-zA-Z0-9-_ ]+)'")]
        public void WhenIAddAnAgent(string id)
        {
            model.Add (new Agent (id));
        }

        [When(@"I (?:remove|delete) '([a-zA-Z0-9-_ ]+)'")]
        public void WhenIRemove(string id)
        {
            model.Remove(id);
        }

        [When(@"I add a new view '([a-zA-Z0-9-_ ]+)'")]
        public void WhenIAddANewView (string id)
        {
            model.AddView (new View (id));
        }

        [When(@"I remove the view '([a-zA-Z0-9-_ ]+)'")]
        public void WhenIRemoveTheView(string id)
        {
            model.RemoveView (id);
        }

        [When(@"I add '([a-zA-Z0-9-_ ]+)' to '([a-zA-Z0-9-_ ]+)'")]
        public void WhenIAddElementToView(string id, string viewId) 
        {
            var view = model.GetView (viewId);
            view.Add (id);
        }
        
        [When(@"I (?:remove|delete) '([a-zA-Z0-9-_ ]+)' from '([a-zA-Z0-9-_ ]+)'")]
        public void WhenIRemoveElementFromView(string id, string viewId)
        {
            var view = model.GetView (viewId);
            view.Remove (id);
        }

        [When(@"I connect '([a-zA-Z0-9-_ ]+)' to '([a-zA-Z0-9-_ ]+)'")]
        public void WhenIConnectElements(string id1, string id2)
        {
            model.Connect (id1, id2);
        }

        [Then(@"the model contains '([a-zA-Z0-9-_ ]+)'")]
        public void ThenTheModelContains(string id)
        {
            Assert.That (model.Contains(id));
        }

        [Then(@"the model does not contains '([a-zA-Z0-9-_ ]+)'")]
        public void ThenTheModelDoesNotContains(string id)
        {
            Assert.That (!model.Contains(id));
        }

        [Then(@"the model contains a view named '([a-zA-Z0-9-_ ]+)'")]
        public void ThenTheModelContainsAViewNamed (string id)
        {
            Assert.That (model.ContainsView (id));
        }

        [Then(@"the model does not contains a view named '([a-zA-Z0-9-_ ]+)'")]
        public void ThenTheModelDoesNotContainsAViewNamed (string id)
        {
            Assert.That (!model.ContainsView (id));
        }

        [Then(@"the view '([a-zA-Z0-9-_ ]+)' contains '([a-zA-Z0-9-_ ]+)'")]
        public void ThenTheViewContains(string viewId, string id)
        {
            Assert.That (model.GetView (viewId).Contains (id));
        }

        [Then(@"the view '([a-zA-Z0-9-_ ]+)' does not contains '([a-zA-Z0-9-_ ]+)'")]
        public void ThenTheViewDoesNotContains(string viewId, string id)
        {
            Assert.That (!model.GetView (viewId).Contains (id));
        }

        [Then(@"'([a-zA-Z0-9-_ ]+)' is a goal")]
        public void ThenElementIsAGoal(string id)
        {
            Assert.That (model.Get (id) is Goal);
        }
        
        [Then(@"'([a-zA-Z0-9-_ ]+)' is a domain property")]
        public void ThenElementIsADomainProperty(string id)
        {
            Assert.That (model.Get (id) is DomainProperty);
        }
        
        [Then(@"'([a-zA-Z0-9-_ ]+)' is an agent")]
        public void ThenElementIsAnAgent(string id)
        {
            Assert.That (model.Get (id) is Agent);
        }
        
        [Then(@"'([a-zA-Z0-9-_ ]+)' is an obstacle")]
        public void ThenElementIsAnObstacle(string id)
        {
            Assert.That (model.Get (id) is Obstacle);
        }

        [Then(@"there exists a refinement for '([a-zA-Z0-9-_ ]+)'")]
        public void ThenThereExistsARefinementForElement (string id)
        {
            lastRefinements = model.Find<GoalRefinement> (x => x.Parent == id);
            Assert.That (lastRefinements.Count() > 0);
        }
        
        [Then(@"there exists an obstacle refinement for '([a-zA-Z0-9-_ ]+)'")]
        public void ThenThereExistsAnObstacleRefinementForElement (string id)
        {
            lastRefinements = model.Find<ObstacleRefinement> (x => x.Parent == id);
            Assert.That (lastRefinements.Count() > 0);
        }

        [Then(@"this refinement contains '([a-zA-Z0-9-_ ]+)'")]
        public void ThenThisRefinementContainsElement (string id)
        {
            lastRefinements = lastRefinements.Where(x => x.Children.Contains (id));
            Assert.That (lastRefinements.Count() > 0);
        }

        [Then(@"'([a-zA-Z0-9-_ ]+)' is assigned to '([a-zA-Z0-9-_ ]+)'")]
        public void ThenAgentIsAssignedToGoal (string agentId, string goalId)
        {
            Assert.NotNull (model.Find <Responsibility> (x => x.Agent.Id == agentId && x.Goal.Id == goalId));
        }

        [Then(@"'([a-zA-Z0-9-_ ]+)' obstructs '([a-zA-Z0-9-_ ]+)'")]
        public void ThenObstacleObstructsGoal (string obstacleId, string goalId)
        {
            Assert.NotNull (model.Find <Obstruction> (x => x.Obstacle.Id == obstacleId && x.Goal.Id == goalId));
        }

        [Then(@"refinement '([a-zA-Z0-9-_ ]+)' contains '([a-zA-Z0-9-_ ]+)'")]
        public void ThenRefinementContains (string refinementId, string id)
        {
            Assert.NotNull (model.Find <GoalRefinement> (x => x.Id == refinementId && x.Children.Contains (id)));
        }

    }
}

