using System;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Core.SatisfactionRates;
using System.Collections.Generic;

namespace UCLouvain.KAOSTools.Propagators
{
    public interface IPropagator
    {
        ISatisfactionRate GetESR (Obstacle obstacle);
        ISatisfactionRate GetESR (Goal goal);
        
        ISatisfactionRate GetESR (Obstacle obstacle, IEnumerable<Resolution> activeResolutions);
        ISatisfactionRate GetESR (Goal goal, IEnumerable<Resolution> activeResolutions);
    }
}
