using System;
using System.Linq;
using NUnit.Framework;
using KAOSTools.Parsing;
using ShallTests;

namespace KAOSTools.Core.Tests
{
    [TestFixture()]
    public class TestViews
    {
        #region Test assignments

        [Test()]
        public void TestCompleteGoalAssignments ()
        {
            var input = @"
declare goal
    id goal
    assignedto agent
end";
            var parser = new ModelBuilder ();
            var model = parser.Parse (input);
            
            var view = new KAOSView (model);
            view.Add (model.GoalAgentAssignments().Single());

            view.CompleteGoalAgentAssignments ();
            view.Elements.Count().ShallEqual (3);
        }

        [Test()]
        public void TestCompleteObstacleAssignments ()
        {
            var input = @"
declare obstacle
    id obstacle
    assignedto agent
end";
            var parser = new ModelBuilder ();
            var model = parser.Parse (input);

            var view = new KAOSView (model);
            view.Add (model.ObstacleAgentAssignments().Single());

            view.CompleteObstacleAgentAssignments ();
            view.Elements.Count().ShallEqual (3);
        }

        [Test()]
        public void TestCompleteAntiGoalAssignments ()
        {
            var input = @"
declare antigoal
    id goal
    assignedto agent
end";
            var parser = new ModelBuilder ();
            var model = parser.Parse (input);

            var view = new KAOSView (model);
            view.Add (model.AntiGoalAgentAssignments().Single());

            view.CompleteAntiGoalAgentAssignments ();
            view.Elements.Count().ShallEqual (3);
        }

        #endregion

        #region Test refinements
        
        [Test()]
        public void TestCompleteGoalRefinements ()
        {
            var input = @"
declare goal
    id goal
    refinedby child1, child2
end";
            var parser = new ModelBuilder ();
            var model = parser.Parse (input);

            var view = new KAOSView (model);
            view.Add (model.GoalRefinements().Single());

            view.CompleteGoalRefinements ();
            view.Elements.Count().ShallEqual (4);
        }

        #endregion

        
        #region Test refinements

        [Test()]
        public void TestResolution ()
        {
            var input = @"
declare obstacle
    id obstacle
    resolvedby goal
end";
            var parser = new ModelBuilder ();
            var model = parser.Parse (input);

            var view = new KAOSView (model);
            view.Add (model.Resolutions().Single());

            view.CompleteResolution ();
            view.Elements.Count().ShallEqual (3);
        }

        #endregion

        
        #region Test refinements

        [Test()]
        public void TestObstruction ()
        {
            var input = @"
declare goal
    id goal
    obstructedby obstacle
end";
            var parser = new ModelBuilder ();
            var model = parser.Parse (input);

            var view = new KAOSView (model);
            view.Add (model.Obstructions().Single());

            view.CompleteObstruction ();
            view.Elements.Count().ShallEqual (3);
        }

        #endregion
    }
}

