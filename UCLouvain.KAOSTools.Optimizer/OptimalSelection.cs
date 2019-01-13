using System;
using System.Collections.Generic;
using System.Linq;
using UCLouvain.KAOSTools.Core;

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
        
        public HashSet<string> SatRoots {
			get;
			set;
        }
        
        public double SatisfactionRate {
            get;
            set;
        }
    
        public OptimalSelection (IEnumerable<Resolution> resolutions, int cost, double satisfactionRate, IEnumerable<string> satRoots)
        {
            Resolutions = resolutions;
            Cost = cost;
            SatisfactionRate = satisfactionRate;
			SatRoots = new HashSet<string>(satRoots);
        }
        
        public override string ToString ()
        {
            return string.Format ("[OptimalSelection: Resolutions={{{0}}}, Cost={1}, SatisfactionRate={2}]", 
                string.Join (",", Resolutions.Select(x => x.ResolvingGoalIdentifier).Distinct ()), 
                Cost, 
                SatisfactionRate);
        }
    }
    
    public class OptimalSelectionWithUncertainty
    {
        public IEnumerable<Resolution> Resolutions {
            get;
            set;
        }
        
        public int Cost {
            get;
            set;

        }
        
        public double ViolationUncertainty {
            get;
            set;
        }
        
        public double UncertaintySpread {
			get;
			set;
		}

		public OptimalSelectionWithUncertainty (IEnumerable<Resolution> resolutions, int cost, double violation_uncertainty, double uncertainty_spread)
        {
            Resolutions = resolutions;
            Cost = cost;
            ViolationUncertainty = violation_uncertainty;
            UncertaintySpread = uncertainty_spread;
        }
        
        public override string ToString ()
        {
            return string.Format ("[OptimalSelection: Resolutions={{{0}}}, Cost={1}, ViolationUncertainty={2}, UncertaintySpread={3}]", 
                string.Join (",", Resolutions.Select(x => x.ResolvingGoalIdentifier).Distinct ()), 
                Cost, 
                ViolationUncertainty,
                UncertaintySpread);
        }
    }
}
