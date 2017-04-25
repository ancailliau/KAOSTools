using System;
namespace UCLouvain.KAOSTools.Core.SatisfactionRates
{
    public interface ISatisfactionRate
    {
        string ExpertIdentifier { get; set; }

        ISatisfactionRate OneMinus ();
        ISatisfactionRate Product (ISatisfactionRate x);
        ISatisfactionRate Product (double x);
        ISatisfactionRate Sum (ISatisfactionRate x);
        
        double Sample ();
    }
}
