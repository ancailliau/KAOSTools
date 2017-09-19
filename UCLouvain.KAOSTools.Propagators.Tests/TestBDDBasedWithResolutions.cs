using System;
using System.Collections.Generic;
using System.Linq;
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
				
				declare obstacle [ o1 ] esr .1 end
				declare obstacle [ o2 ] esr .2 end
				declare obstacle [ o3 ] esr .3 end
				declare obstacle [ o4 ] esr .4 end
			";
			var parser = new ModelBuilder ();
			var model = parser.Parse (input);
			var pg = model.Goal("pg");

			var p0 = new ObstructionSuperset (pg);
			Console.WriteLine(p0.ToDot());

			var p1 = new ObstructionResolutionSuperset (pg);
			Console.WriteLine(p1.ToDot());

			Console.WriteLine("---");
			//var p2 = new BDDBasedPropagator(model);
			//Console.WriteLine(p2.GetESR(pg));
			
			var p3 = new BDDBasedResolutionPropagator(model);
			Console.WriteLine(p3.GetESR(pg));

			foreach (var e in GetAllCombinations(model.goalRepository.GetGoalExceptions().ToList())) {
				Console.Write(string.Join(",", e.Select(x => x.ResolvingGoalIdentifier)) + ": ");
				Console.WriteLine(p3.GetESR(pg, e));
			}
		}
		
		// Code from Stackoverflow
        // https://stackoverflow.com/questions/7802822/all-possible-combinations-of-a-list-of-values
        static List<List<T>> GetAllCombinations<T> (List<T> list)
        {
            List<List<T>> result = new List<List<T>> ();
            result.Add (new List<T> ());
            result.Last ().Add (list [0]);
            if (list.Count == 1)
                return result;
            List<List<T>> tailCombos = GetAllCombinations (list.Skip (1).ToList ());
            tailCombos.ForEach (combo => {
                result.Add (new List<T> (combo));
                combo.Add (list [0]);
                result.Add (new List<T> (combo));
            });
            return result;
        }
	}
}
