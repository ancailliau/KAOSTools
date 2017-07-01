using System;
using UCLouvain.KAOSTools.Core;
namespace UCLouvain.KAOSTools.Propagators
{
    public class SamplingVector
    {
        protected KAOSModel _model;
    
        public SamplingVector (KAOSModel model)
        {
            this._model = model;
        }
        
        public virtual double Sample (Obstacle o) 
        {
			return _model.satisfactionRateRepository.GetObstacleSatisfactionRate (o.Identifier).Sample ();
        }

        internal bool ContainsKey (string identifier)
        {
            return _model.satisfactionRateRepository.ObstacleSatisfactionRateExists (identifier);
        }
    }
    
    public class RandomizedSamplingVector : SamplingVector
    {
		Random _random = null;
    
        public RandomizedSamplingVector (KAOSModel model) : base(model)
        {
			_random = new Random();
        }
        
        public RandomizedSamplingVector (KAOSModel model, Random random) : base(model)
        {
			this._random = random;
        }
        
        public override double Sample (Obstacle o) 
        {
			return _model.satisfactionRateRepository.GetObstacleSatisfactionRate (o.Identifier).Sample (_random);
        }
    }
}
