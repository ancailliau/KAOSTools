using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Parsing;
using UCLouvain.KAOSTools.Propagators.BDD;
using UCLouvain.KAOSTools.Core.SatisfactionRates;

namespace UCLouvain.KAOSTools.Propagators.Tests
{
    [TestFixture ()]
	public class TestBDDBasedWithCaseConditions : TestPropagator
	{
	
		// This is a bogus idea. The theory behind does not look sound
		// at all. Don't use that.
	
		[Test()]
		public void TestSinglePartial ()
        {
			var input = @"
				declare goal [ pg ] 
					refinedby sg1 [ .9 ], sg2
				end
				declare goal [ sg1 ] obstructedby o1 end
				declare goal [ sg2 ] obstructedby o2 end
				declare obstacle [ o1 ] esr .1 end
				declare obstacle [ o2 ] esr .1 end
			";
			var parser = new ModelBuilder ();
			var model = parser.Parse (input);
			var pg = model.Goal("pg");
            
			var p1 = new ObstructionCaseSuperset (pg);
			Console.WriteLine(p1.ToDot());

			var p2 = new BDDBasedCasePropagator(model);
			Console.WriteLine(p2.GetESR(pg));
		}
		
		[Test()]
		public void TestBug ()
        {
			var input = @"
				declare goal [ root ]
				    refinedby [ case ] m1 [ .3 ], m2 [ .7 ]
				end
				
				declare goal [ m1 ] obstructedby o1 end
				declare goal [ m2 ] obstructedby o2 end
				
				declare obstacle [ o1 ] esr .5 end
				declare obstacle [ o2 ] esr .5 end
			";
			var parser = new ModelBuilder ();
			var model = parser.Parse (input);
			var pg = model.Goal("root");
            
			var p1 = new ObstructionCaseSuperset (pg);
			Console.WriteLine("***");
			Console.WriteLine(p1.ToDot());

			var p2 = new BDDBasedCasePropagator(model);
			Console.WriteLine(p2.GetESR(pg));

			var satisfactionRate = (DoubleSatisfactionRate) p2.GetESR(pg);
			Assert.AreEqual(0.3 * 0.5 + 0.5 * 0.7, satisfactionRate.SatisfactionRate);
		}
		
		
		[TestCase (0,   0,   .5, .5, 1)]
        [TestCase (0.5, .5,  .5, .5, 0.5 * 0.5 + 0.5 * 0.5)]
        [TestCase (1,   0,   .5, .5, 0.5)]
        [TestCase (0,   1,   .5, .5, 0.5)]
        [TestCase (.99, .99, .5, .5, 0.5 * (1 - .99) + 0.5 * (1 - .99))]
        [TestCase (.01, .01, .5, .5, 0.5 * (1 - .01) + 0.5 * (1 - .01))]
        [TestCase (0,   0,   .3, .7, 1)]
        [TestCase (0.5, .5,  .3, .7, 0.3 * 0.5 + 0.5 * 0.7)]/**/
        [TestCase (1,   0,   .3, .7, 0.7)]
        [TestCase (0,   1,   .3, .7, 0.3)]
        [TestCase (.9,  .8,  .3, .7, 0.3 * (1 - .90) + 0.7 * (1 - .80))]
        [TestCase (.02, .01, .3, .7, 0.3 * (1 - .02) + 0.7 * (1 - .01))]
        public void TestCaseRefinement (double o1_esr, double o2_esr, double c1, double c2, double expected_root_esr)
        {
            TestCase (o1_esr, o2_esr, c1, c2, expected_root_esr);
        }

		protected override IPropagator GetPropagator(KAOSModel model)
		{
			return new BDDBasedCasePropagator(model);
		}
	}
}
