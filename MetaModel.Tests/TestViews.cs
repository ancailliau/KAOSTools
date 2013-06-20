using System;
using System.Linq;
using NUnit.Framework;
using KAOSTools.Parsing;
using ShallTests;

namespace KAOSTools.MetaModel.Tests
{
    [TestFixture()]
    public class TestViews
    {
        [Test()]
        public void TestCompleteAssignments ()
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
    }
}

