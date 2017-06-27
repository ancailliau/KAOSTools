using System;
using System.Collections.Generic;
using System.Linq;
using KAOSTools.Core;

namespace UCLouvain.KAOSTools.Optimizer
{
    public class OptimalSelection
    {
        public IEnumerable<Resolution> Resolutions {
            get;
            set;
        }
        
        public int Cost {
            get;
            set;

        }
        
        public double SatisfactionRate {
            get;
            set;
        }
    
        public OptimalSelection (IEnumerable<Resolution> resolutions, int cost, double satisfactionRate)
        {
            Resolutions = resolutions;
            Cost = cost;
            SatisfactionRate = satisfactionRate;
        }
        
        public override string ToString ()
        {
            return string.Format ("[OptimalSelection: Resolutions={{{0}}}, Cost={1}, SatisfactionRate={2}]", 
                string.Join (",", Resolutions.Select(x => x.ResolvingGoalIdentifier).Distinct ()), 
                Cost, 
                SatisfactionRate);
        }
    }
}
