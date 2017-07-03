using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace UCLouvain.KAOSTools.Core
{
    public class QuantileList : UncertainSatisfactionRate {
        public List<double> Quantiles;
        public QuantileList ()
        {
            Quantiles = new List<double> ();
        }
        
        public QuantileList(List<double> quantiles)
		{
			Quantiles = quantiles;
		}

		public override double Sample(Random r)
		{
			throw new NotImplementedException();
		}
	}
}
