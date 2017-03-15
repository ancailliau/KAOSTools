using System;
using System.Linq;
using ShallTests;
using NUnit.Framework;

namespace KAOSTools.Core.Tests
{

    [TestFixture()]
    public class TestAlternativePropagation
    {
        [Test()]
        public void TestSimple ()
        {
            var model = new GoalModel ();

            var ag1 = new Agent ();
            var ag2 = new Agent ();
            var ag3 = new Agent ();
            var ag4 = new Agent ();

            var rootG = new Goal() { Name = "G" };
            model.Goals().Add (rootG);

            var G1 = new Goal() { Name = "G1" };
            var G2 = new Goal() { Name = "G2" };
            var G3 = new Goal() { Name = "G3" };
            var G4 = new Goal() { Name = "G4" };
            model.Goals().Add (G1);
            model.Goals().Add (G2);
            model.Goals().Add (G3);
            model.Goals().Add (G4);

            G1.AgentAssignments.Add (new AgentAssignment (ag1));
            G2.AgentAssignments.Add (new AgentAssignment (ag2));
            G3.AgentAssignments.Add (new AgentAssignment (ag3));
            G4.AgentAssignments.Add (new AgentAssignment (ag4));

            var alt1 = new AlternativeSystem () { Name = "Alt1" };
            var alt2 = new AlternativeSystem () { Name = "Alt2" };
            model.Systems.Add (alt1);
            model.Systems.Add (alt2);

            rootG.Refinements().Add (new GoalRefinement (G1, G2, G3) { SystemReference = alt1 });
            rootG.Refinements().Add (new GoalRefinement (G3, G4) { SystemReference = alt2 });

            var h = new AlternativeHelpers(); h.ComputeInAlternatives (model);

            Console.WriteLine ("G1: " + string.Join(", ", G1.InSystems.Select (x => x.Name)));
            Console.WriteLine ("G2: " + string.Join(", ", G2.InSystems.Select (x => x.Name)));
            Console.WriteLine ("G3: " + string.Join(", ", G3.InSystems.Select (x => x.Name)));
            Console.WriteLine ("G4: " + string.Join(", ", G4.InSystems.Select (x => x.Name)));

            G1.InSystems.ShallOnlyContain (new AlternativeSystem[] { alt1 });
            G2.InSystems.ShallOnlyContain (new AlternativeSystem[] { alt1 });
            G3.InSystems.ShallOnlyContain (new AlternativeSystem[] { alt1, alt2 });
            G4.InSystems.ShallOnlyContain (new AlternativeSystem[] { alt2 });
        }

    }
}

