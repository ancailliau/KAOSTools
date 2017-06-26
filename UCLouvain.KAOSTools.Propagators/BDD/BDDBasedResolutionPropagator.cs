using System;
using System.Collections.Generic;
using KAOSTools.Core;
using UCLouvain.KAOSTools.Core.SatisfactionRates;

namespace UCLouvain.KAOSTools.Propagators.BDD
{
    public class BDDBasedResolutionPropagator : BDDBasedPropagator
    {
        protected ObstructionResolutionSuperset orSuperset;

        public BDDBasedResolutionPropagator (KAOSModel model) : base (model) {}
        
        public override void PreBuildObstructionSet (Goal goal)
        {
            prebuilt_goal = goal;
            orSuperset = new ObstructionResolutionSuperset (goal);
        } 

        public override ISatisfactionRate GetESR (Goal goal)
        {
            if (orSuperset == null || prebuilt_goal != goal)
                orSuperset = new ObstructionResolutionSuperset (goal);
            var vector = new SamplingVector (_model);
            return new DoubleSatisfactionRate (1.0 - orSuperset.GetProbability (vector));
        }
        
        public override ISatisfactionRate GetESR (Goal goal, IEnumerable<Resolution> activeResolutions)
        {
            if (orSuperset == null || prebuilt_goal != goal)
                orSuperset = new ObstructionResolutionSuperset (goal);
            var vector = new SamplingVector (_model);
            orSuperset.SetDefaultValue (activeResolutions, 1);
            return new DoubleSatisfactionRate (1.0 - orSuperset.GetProbability (vector));
        }
    }
}
