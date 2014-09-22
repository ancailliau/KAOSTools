using NUnit.Framework;
using System;
using System.IO;
using KAOSTools.MetaModel;

namespace DependantProbability.Tests
{
    [TestFixture()]
    public class Test
    {
        [Test()]
        public void TestCase ()
        {
            var parser = new KAOSTools.Parsing.ModelBuilder ();

            var filename = "/Users/acailliau/Dropbox/PhD/2013/Dependent obstacles/las.kaos";
            var input = File.ReadAllText (filename);
            var model = parser.Parse (input, filename);

            Console.WriteLine ("--- [Non Satisfaction Formulas]");

            foreach (var g in model.Goals () ) {
                var node = g.GetNonSatisfactionFormula ();
                Console.WriteLine ("{0}={1}", g.FriendlyName, node.Simplify ());
            }
            Console.WriteLine ();
            foreach (var g in model.Obstacles ()) {
                Console.WriteLine ("{0}={1}", g.FriendlyName, g.GetNonSatisfactionFormula ());
            }

            Console.WriteLine ("---\n");


            Console.WriteLine ("--- [Probabilities]");

            foreach (var g in model.Goals ()) {
                Console.WriteLine ("P({0})={1}%", g.FriendlyName, Math.Round (g.ComputeProbability (), 4)*100);
            }

            Console.WriteLine ();

            foreach (var o in model.Obstacles ()) {
                Console.WriteLine ("P({0})={1}%", o.FriendlyName, Math.Round (o.ComputeProbability (), 4)*100);
            }


            Console.WriteLine ("---\n");

            model.HasObstacleWithProbability ("GPS Not Working", 0.1);
            model.HasObstacleWithProbability ("Ambulance Not in Familiar Area", 0.2);
            model.HasObstacleWithProbability ("Ambulance Lost", 0.019);
            model.HasObstacleWithProbability ("Mobilized Ambulance Not On Scene In Time", 0.0429);

            model.HasGoalWithProbability ("Achieve [Ambulance On Scene When Mobilized]", 0.9571);
            model.HasGoalWithProbability ("Achieve [Ambulance Allocated When Incident Reported]", 0.98);
            model.HasGoalWithProbability ("Achieve [Allocated Ambulance Mobilized When On Road ]", 0.98);

            model.HasGoalWithProbability ("Achieve [Mobilized By Phone When Allocated At Station]", 0.95);
            model.HasGoalWithProbability ("Achieve [Mobilized By Fax When Allocated At Station]", 0.9);

            model.HasGoalWithProbability ("Achieve [Allocated Ambulance Mobilized When At Station]", 0.995);
            model.HasGoalWithProbability ("Achieve [Ambulance Mobilized When Allocated]", 0.99);
            model.HasGoalWithProbability ("Achieve [Ambulance On Scene When Allocated]", 0.9476);
            model.HasGoalWithProbability ("Achieve [Ambulance On Scene In Time When Incident Reported]", 0.9286);
        }
    }

    public static class TestHelper {
        public static void HasObstacleWithProbability (this KAOSModel model, string nameOrId, double value)
        {
            var proba = Math.Round (model.Obstacle (x => x.FriendlyName == nameOrId).ComputeProbability (), 4);
            Assert.AreEqual (Math.Round (value, 4), proba,
                             string.Format ("Obstacle '{0}' has an expected probability of {1:F4} but was {2:F4}",
                           nameOrId, value, proba));
        }
        public static void HasGoalWithProbability (this KAOSModel model, string nameOrId, double value)
        {
            var proba = Math.Round (model.Goal (x => x.FriendlyName == nameOrId).ComputeProbability (), 4);
            Assert.AreEqual (Math.Round (value, 4), proba,
                             string.Format ("Goal '{0}' has an expected probability of {1:F4} but was {2:F4}",
                           nameOrId, value, proba));
        }
    }

}

