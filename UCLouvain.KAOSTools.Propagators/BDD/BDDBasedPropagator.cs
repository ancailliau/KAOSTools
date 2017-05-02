using System;
using KAOSTools.Core;
using UCLouvain.KAOSTools.Core.SatisfactionRates;

namespace UCLouvain.KAOSTools.Propagators.BDD
{
    public class BDDBasedPropagator : IPropagator
    {
        KAOSModel _model;
        ObstructionSuperset obstructionSuperset;
        Goal prebuilt_goal;

        public BDDBasedPropagator (KAOSModel model)
        {
            _model = model;
        }

        public ISatisfactionRate GetESR (Obstacle obstacle)
        {
            throw new NotImplementedException ();
        }
        
        public void PreBuildObstructionSet (Goal goal)
        {
            prebuilt_goal = goal;
            obstructionSuperset = new ObstructionSuperset (goal);
        } 

        public ISatisfactionRate GetESR (Goal goal)
        {
            if (obstructionSuperset == null || prebuilt_goal != goal)
                obstructionSuperset = new ObstructionSuperset (goal);
            var vector = new SamplingVector (_model);
            return new DoubleSatisfactionRate (1.0 - obstructionSuperset.GetProbability (vector));
        }
    }
}
