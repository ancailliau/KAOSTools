using System;
namespace UCLouvain.KAOSTools.Optimizer
{
    public class OptimizationStatistics
    {
        public int NbResolution {
            get;
            set;
        }
        
        public int NbSelections {
            get;
            set;
        }
        
        public int NbTestedSelections {
            get;
            set;
        }
    
        public int NbTestedSelectionsForOptimality {
            get;
            set;
        }
        
        public int NbSafeSelections {
            get;
            set;
        }

        public int MaxSafeCost {
            get;
            set;
        }
    
        public TimeSpan TimeToComputeMinimalCost {
            get;
            set;
        }
        
        public TimeSpan TimeToComputeSelection {
            get;
            set;
        }
    }
}
