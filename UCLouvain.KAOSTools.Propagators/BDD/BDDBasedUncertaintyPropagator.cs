using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Core.SatisfactionRates;

namespace UCLouvain.KAOSTools.Propagators.BDD
{
    public class BDDBasedUncertaintyPropagator : IPropagator
    {
        protected KAOSModel _model;
        protected ObstructionSuperset obstructionSuperset;
        protected KAOSCoreElement prebuilt_element;

		int _n_samples;
		int _seed;

        public BDDBasedUncertaintyPropagator (KAOSModel model, int n_samples)
        {
            _model = model;
			_n_samples = n_samples;
			_seed = 123;
        }

        public virtual ISatisfactionRate GetESR (Obstacle obstacle)
        {
            if (obstructionSuperset == null || prebuilt_element != obstacle)
                obstructionSuperset = new ObstructionSuperset (obstacle);

			var random = new Random(_seed);
			var s_sr = new double[_n_samples];
			for (int i = 0; i < _n_samples; i++) {
				var vector = new RandomizedSamplingVector (_model, random);
				s_sr[i] = obstructionSuperset.GetProbability(vector);
			}
			
			return new SimulatedSatisfactionRate(s_sr);
        }
        
        public virtual void PreBuildObstructionSet (Goal goal)
        {
            prebuilt_element = goal;
            obstructionSuperset = new ObstructionSuperset (goal);
        } 
        
        
        public virtual ISatisfactionRate GetESR (Goal goal)
        {
			return GetESR(goal, null);
        }

        public ISatisfactionRate GetESR (Goal goal, IEnumerable<Obstacle> onlyObstacles)
        {
            if (obstructionSuperset == null || prebuilt_element != goal)
                obstructionSuperset = new ObstructionSuperset (goal);

			var random = new Random(_seed);
			var s_sr = new double[_n_samples];
			for (int i = 0; i < _n_samples; i++) {
				var vector = new RandomizedSamplingVector (_model, random, onlyObstacles?.Select(x => x.Identifier)?.ToHashSet());
				s_sr[i] = 1.0 - obstructionSuperset.GetProbability(vector);
			}
			
			return new SimulatedSatisfactionRate(s_sr);
        }
    }
}
