using System;
using NUnit.Framework;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Parsing;
using UCLouvain.KAOSTools.Propagators.BDD;

namespace UCLouvain.KAOSTools.Propagators.Tests
{
    [TestFixture ()]
	public class TestBDDBasedWithResolutions
	{
		[Test()]
		public void TestSingleExcept ()
        {
			var input = @"
				declare goal [ pg ] 
					except [ o2 ] cm
					refinedby sg1, sg2
				end
				declare goal [ sg1 ] obstructedby o1 end
				declare goal [ sg2 ] obstructedby o2 end
				declare goal [ cm ] obstructedby o3 end
			";
			var parser = new ModelBuilder ();
			var model = parser.Parse (input);
			var pg = model.Goal("pg");
            
			var p1 = new ObstructionResolutionSuperset (pg);
			Console.WriteLine(p1.ToDot());
		}
		
		[Test()]
		public void TestDualExcept ()
        {
			var input = @"
				declare goal [ pg ] 
					except [ o1 ] cm1
					except [ o2 ] cm2
					refinedby sg1, sg2
				end
				declare goal [ sg1 ] obstructedby o1 end
				declare goal [ sg2 ] obstructedby o2 end
				declare goal [ cm1 ] obstructedby o3 end
				declare goal [ cm2 ] obstructedby o4 end
			";
			var parser = new ModelBuilder ();
			var model = parser.Parse (input);
			var pg = model.Goal("pg");
            
			var p1 = new ObstructionResolutionSuperset (pg);
			Console.WriteLine(p1.ToDot());
		}
		
		[Test()]
		public void TestTwoLevelExcept ()
        {
			var input = @"
				declare goal [ pg ] 
					except [ o1 ] cm1
					except [ o2 ] cm2
					refinedby sg1, sg2
				end
				declare goal [ sg1 ] obstructedby o1 end
				declare goal [ sg2 ] obstructedby o2 end
				declare goal [ cm1 ] obstructedby o3 end
				declare goal [ cm2 ] 
					except [ o4 ] cm4 
					obstructedby o4
				end
			";
			var parser = new ModelBuilder ();
			var model = parser.Parse (input);
			var pg = model.Goal("pg");

			var p1 = new ObstructionResolutionSuperset (pg);
			Console.WriteLine(p1.ToDot());
		}
	}
}
