using System;
using KAOSTools.Core;
using UCLouvain.KAOSTools.Core.SatisfactionRates;

namespace UCLouvain.KAOSTools.Propagators
{
    public interface IPropagator
    {
        ISatisfactionRate GetESR (Obstacle obstacle);
        ISatisfactionRate GetESR (Goal goal);
    }
}
