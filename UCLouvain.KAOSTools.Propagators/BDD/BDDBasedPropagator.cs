using System;
using System.Collections.Generic;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Core.SatisfactionRates;

namespace UCLouvain.KAOSTools.Propagators.BDD
{
    public class BDDBasedPropagator : IPropagator
    {
        protected KAOSModel _model;
        protected ObstructionSuperset obstructionSuperset;
        protected Goal prebuilt_goal;

        public BDDBasedPropagator (KAOSModel model)
        {
            _model = model;
        }

        public virtual ISatisfactionRate GetESR (Obstacle obstacle)
        {
            throw new NotImplementedException ();
        }
        
        public virtual void PreBuildObstructionSet (Goal goal)
        {
            prebuilt_goal = goal;
            obstructionSuperset = new ObstructionSuperset (goal);
        } 

        public virtual ISatisfactionRate GetESR (Goal goal)
        {
            if (obstructionSuperset == null || prebuilt_goal != goal)
                obstructionSuperset = new ObstructionSuperset (goal);
            var vector = new SamplingVector (_model);
            return new DoubleSatisfactionRate (1.0 - obstructionSuperset.GetProbability (vector));
        }

        public virtual ISatisfactionRate GetESR (Obstacle obstacle, IEnumerable<Resolution> activeResolutions)
        {
            throw new NotImplementedException ();
        }

        public virtual ISatisfactionRate GetESR (Goal goal, IEnumerable<Resolution> activeResolutions)
        {
            throw new NotImplementedException ();
        }
    }
}
