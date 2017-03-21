using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace KAOSTools.Core
{
    public class QuantileList {
        public List<double> Quantiles;
        public QuantileList ()
        {
            Quantiles = new List<double> ();
        }
    }
}
