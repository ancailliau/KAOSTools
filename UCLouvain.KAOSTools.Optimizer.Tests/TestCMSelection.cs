using NUnit.Framework;
using System;
using UCLouvain.KAOSTools.Parsing;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Propagators.BDD;
using System.Linq;
using UCLouvain.KAOSTools.Core.SatisfactionRates;
using System.IO;

namespace UCLouvain.KAOSTools.Optimizer.Tests
{
    [TestFixture ()]
    public class TestCMSelection
    {
    	[Test ()]
        public void TestCECountermeasureSelectionFDS ()
        {
			var input = File.ReadAllText("/Users/acailliau/Google Drive/PhD/Dissertation/running-example-fds/model2.kaos");

            ModelBuilder parser = new ModelBuilder ();
            var model = parser.Parse (input);
            
            var optimizer = new MOCECountermeasureSelectionOptimizer (model);
            var propagator = new BDDBasedPropagator (model);

			var root = model.Goal("locals_warned_when_risk_imminent");
            
            var sr = (DoubleSatisfactionRate) propagator.GetESR (root);
            Console.WriteLine ("Satisfaction Rate without countermeasures: " + sr.SatisfactionRate);
            Console.WriteLine ("Required Satisfaction Rate: " + root.RDS);

			var optimalSelections = optimizer.GetOptimalSelections(root, propagator);

			if (optimalSelections.Count() == 0)
			{
				Console.WriteLine("Optimal selections: No countermeasure to select.");
				Console.WriteLine();
			}
			else
			{
				Console.WriteLine($"Optimal selections ({optimalSelections.Count()}):");
				foreach (var o in optimalSelections.Distinct().OrderBy(x => x.Cost).ThenBy(x => x.SatisfactionRate))
				{
					Console.WriteLine("* " + o);
				}
			}

        }
        
    	[Test ()]
        public void TestCECountermeasureSelection ()
        {
            var input = @"declare goal [ root ] rsr .75 refinedby child1, child2 end
                          declare goal [ child1 ] obstructedby o1 end
                          declare goal [ child2 ] obstructedby o2 end
                          declare obstacle [ o1 ] 
                          	probability .2
                          	refinedby so4
                          	refinedby so5, so6
                          end
                          declare obstacle [ so4 ]
                          	probability .1
                          end
                          declare obstacle [ so5 ]
                          	probability .4
                          	resolvedby [substitution:root] cm5
                          end
                          declare obstacle [ so6 ]
                          	probability .4
                          	resolvedby [substitution:root] cm4
                          end
                          declare obstacle [ o2 ] 
                          	refinedby so1
                          	refinedby so2
                          end
                          declare obstacle [ so1 ] 
                          	probability .1
                          	resolvedby [substitution:root] cm2
                          end
                          declare obstacle [ so2 ]
                          	probability .3 
                          	resolvedby [substitution:root] cm3
                          end
                          declare obstacle [ so3 ]
                          	probability .4
                          	resolvedby [substitution:root] cm4
                          end
                          ";

			//input = @"declare goal [ root ] rsr .75 refinedby child1, child2 end
                          //declare goal [ child1 ] obstructedby o1 end
                          //declare goal [ child2 ] obstructedby o2 end
                          //declare obstacle [ o1 ] 
                          //	probability .2
                          //	resolvedby [substitution:root] cm1
                          //end
                          //declare obstacle [ o2 ] 
                          //	refinedby so1
                          //	refinedby so2
                          //end
                          //declare obstacle [ so1 ] 
                          //	probability .1
                          //	resolvedby [substitution:root] cm2
                          //end
                          //declare obstacle [ so2 ]
                          //	probability .3 
                          //	resolvedby [substitution:root] cm3
                          //end";

            ModelBuilder parser = new ModelBuilder ();
            var model = parser.Parse (input);
            
            var optimizer = new MOCECountermeasureSelectionOptimizer (model);
            var propagator = new BDDBasedPropagator (model);

			var root = model.Goal("root");
            
            var sr = (DoubleSatisfactionRate) propagator.GetESR (root);
            Console.WriteLine ("Satisfaction Rate without countermeasures: " + sr.SatisfactionRate);
            Console.WriteLine ("Required Satisfaction Rate: " + root.RDS);

			//var minCost = optimizer.GetMinimalCost(root, propagator);
			//Console.WriteLine("Minimal cost: " + minCost);

			var optimalSelections = optimizer.GetOptimalSelections(root, propagator);

			if (optimalSelections.Count() == 0)
			{
				Console.WriteLine("Optimal selections: No countermeasure to select.");
				Console.WriteLine();
			}
			else
			{
				Console.WriteLine($"Optimal selections ({optimalSelections.Count()}):");
				foreach (var o in optimalSelections.Distinct().OrderBy(x => x.Cost).ThenBy(x => x.SatisfactionRate))
				{
					Console.WriteLine("* " + o);
				}
			}

        }
        
        
    	[Test ()]
        public void TestCECountermeasureSelection2 ()
        {
			const string Path1 = "/Users/acailliau/Google Drive/PhD/Dissertation/case-studies/ambulance-dispatching-system/Models/runtime.kaos";
			var input = File.ReadAllText(Path1);

			//input = @"declare goal [ root ] rsr .75 refinedby child1, child2 end
                          //declare goal [ child1 ] obstructedby o1 end
                          //declare goal [ child2 ] obstructedby o2 end
                          //declare obstacle [ o1 ] 
                          //	probability .2
                          //	resolvedby [substitution:root] cm1
                          //end
                          //declare obstacle [ o2 ] 
                          //	refinedby so1
                          //	refinedby so2
                          //end
                          //declare obstacle [ so1 ] 
                          //	probability .1
                          //	resolvedby [substitution:root] cm2
                          //end
                          //declare obstacle [ so2 ]
                          //	probability .3 
                          //	resolvedby [substitution:root] cm3
                          //end";

            ModelBuilder parser = new ModelBuilder ();
            var model = parser.Parse (input, Path1);
            
            var optimizer = new MOCECountermeasureSelectionOptimizer (model);
            var propagator = new BDDBasedPropagator (model);

			var root = model.Goal("achieve_incident_resolved");
            
            var sr = (DoubleSatisfactionRate) propagator.GetESR (root);
            Console.WriteLine ("Satisfaction Rate without countermeasures: " + sr.SatisfactionRate);
            Console.WriteLine ("Required Satisfaction Rate: " + root.RDS);

			//var minCost = optimizer.GetMinimalCost(root, propagator);
			//Console.WriteLine("Minimal cost: " + minCost);

			var optimalSelections = optimizer.GetOptimalSelections(root, propagator);

			if (optimalSelections.Count() == 0)
			{
				Console.WriteLine("Optimal selections: No countermeasure to select.");
				Console.WriteLine();
			}
			else
			{
				Console.WriteLine($"Optimal selections ({optimalSelections.Count()}):");
				foreach (var o in optimalSelections.Distinct().OrderBy(x => x.Cost).ThenBy(x => x.SatisfactionRate))
				{
					Console.WriteLine("* " + o);
				}
			}

        }
    
        [Test ()]
        public void TestBestCountermeasureSelection ()
        {
            var input = @"declare goal [ root ] rsr .75 refinedby child1, child2 end
                          declare goal [ child1 ] obstructedby o1 end
                          declare goal [ child2 ] obstructedby o2 end
                          declare obstacle [ o1 ] 
                          	probability .2
                          	resolvedby [substitution:root] cm1
                          end
                          declare obstacle [ o2 ] 
                          	refinedby so1
                          	refinedby so2
                          end
                          declare obstacle [ so1 ] 
                          	probability .1
                          	resolvedby [substitution:root] cm2
                          end
                          declare obstacle [ so2 ]
                          	probability .3 
                          	resolvedby [substitution:root] cm3
                          end";
                          
            ModelBuilder parser = new ModelBuilder ();
            var model = parser.Parse (input);
            
            var optimizer = new NaiveCountermeasureSelectionOptimizer (model);
            var propagator = new BDDBasedPropagator (model);

			var root = model.Goal("root");
            
            var sr = (DoubleSatisfactionRate) propagator.GetESR (root);
            Console.WriteLine ("Satisfaction Rate without countermeasures: " + sr.SatisfactionRate);
            Console.WriteLine ("Required Satisfaction Rate: " + root.RDS);
        
            var minimalCost = optimizer.GetMinimalCost (root, propagator);
            Console.WriteLine ("Minimal cost to guarantee RSR: " + minimalCost);

			if (minimalCost >= 0)
			{
				var optimalSelections = optimizer.GetOptimalSelections(minimalCost, root, propagator);

				if (optimalSelections.Count() == 0)
				{
					Console.WriteLine("Optimal selections: No countermeasure to select.");
					Console.WriteLine();
				}
				else
				{
					Console.WriteLine($"Optimal selections ({optimalSelections.Count()}):");
					foreach (var o in optimalSelections.Distinct())
					{
						Console.WriteLine("* " + o);
					}
				}
			}

        }
    }
}
