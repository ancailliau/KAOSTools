using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.Serialization;
using UCLouvain.KAOSTools.Core.SatisfactionRates;

namespace KAOSTools.Core
{
    public abstract class UncertainSatisfactionRate : ISatisfactionRate {
        
        public string ExpertIdentifier { get; set; }

        public abstract double Sample (Random r);
    }
    
}
