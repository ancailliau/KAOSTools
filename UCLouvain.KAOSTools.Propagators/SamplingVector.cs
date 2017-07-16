using System;
using System.Collections.Generic;
using UCLouvain.KAOSTools.Core;
namespace UCLouvain.KAOSTools.Propagators
{
    public class SamplingVector
    {
        protected KAOSModel _model;
		HashSet<string> _onlyObstacles = null;
    
        public SamplingVector (KAOSModel model)
        {
            this._model = model;
        }
        
        public SamplingVector (KAOSModel model, HashSet<string> onlyObstacles) : this(model)
        {
			this._onlyObstacles = onlyObstacles;
        }
        
        public virtual double Sample (Obstacle o) 
        {
			if (_onlyObstacles == null || (_onlyObstacles != null && _onlyObstacles.Contains(o.Identifier))) {
				return _model.satisfactionRateRepository.GetObstacleSatisfactionRate(o.Identifier).Sample();
			} else {
				return 0;
			}
        }

        internal bool ContainsKey (string identifier)
        {
            return _model.satisfactionRateRepository.ObstacleSatisfactionRateExists (identifier);
        }
    }
    
    public class RandomizedSamplingVector : SamplingVector
    {
		Random _random = null;
		HashSet<string> _onlyObstacles = null;
    
        public RandomizedSamplingVector (KAOSModel model) : base(model)
        {
			_random = new Random();
        }
        
        public RandomizedSamplingVector (KAOSModel model, Random random) : base(model)
        {
			this._random = random;
        }
        
        public RandomizedSamplingVector (KAOSModel model, Random random, HashSet<string> onlyObstacles) : base(model)
        {
			this._random = random;
			this._onlyObstacles = onlyObstacles;
        }
        
        public override double Sample (Obstacle o) 
        {
			if (_onlyObstacles == null || (_onlyObstacles != null && _onlyObstacles.Contains(o.Identifier))) {
				return _model.satisfactionRateRepository.GetObstacleSatisfactionRate(o.Identifier).Sample(_random);
			} else {
				return 0;
			}
        }
    }
}
