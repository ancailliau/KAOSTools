using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.Serialization;
using UCLouvain.KAOSTools.Core.SatisfactionRates;

namespace UCLouvain.KAOSTools.Core
{
    public abstract class UncertainSatisfactionRate : ISatisfactionRate {
        
        public string ExpertIdentifier { get; set; }
        
        public UncertainSatisfactionRate()
		{
			ExpertIdentifier = null;
		}

		public ISatisfactionRate Product (double x)
        {
            throw new NotImplementedException ();
        }

        public abstract double Sample (Random r);

        public ISatisfactionRate OneMinus ()
        {
            throw new NotImplementedException ();
        }

        public ISatisfactionRate Product (ISatisfactionRate x)
        {
            throw new NotImplementedException ();
        }

        public ISatisfactionRate Sum (ISatisfactionRate x)
        {
            throw new NotImplementedException ();
        }

        public double Sample ()
        {
            throw new NotImplementedException ();
        }
    }
    
}
