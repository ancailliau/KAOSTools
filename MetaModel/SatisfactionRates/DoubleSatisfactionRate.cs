using System;
namespace UCLouvain.KAOSTools.Core.SatisfactionRates
{
    public class DoubleSatisfactionRate : ISatisfactionRate
    {
        public double SatisfactionRate {
            get;
            set;
        }

        public string ExpertIdentifier { 
            get; 
            set;
        }

        public DoubleSatisfactionRate(double sr)
        {
            SatisfactionRate = sr;
        }
    }
}
