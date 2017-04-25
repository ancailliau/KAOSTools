using System;
using KAOSTools.Core;
using UCLouvain.KAOSTools.Core.SatisfactionRates;

namespace UCLouvain.KAOSTools.Propagators.BDD
{
    public class BDDBasedPropagator : IPropagator
    {
        KAOSModel _model;

        public BDDBasedPropagator (KAOSModel model)
        {
            _model = model;
        }

        public ISatisfactionRate GetESR (Obstacle obstacle)
        {
            throw new NotImplementedException ();
        }

        public ISatisfactionRate GetESR (Goal goal)
        {
            var obstructionSuperset = new ObstructionSuperset (goal);
            var vector = new SamplingVector (_model);
            return new DoubleSatisfactionRate (1.0 - obstructionSuperset.GetProbability (vector));
        }
    }
}
