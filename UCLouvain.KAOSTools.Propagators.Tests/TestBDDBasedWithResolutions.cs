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
		public void TestBDD2 ()
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
	}
}
