using System;
using KAOSTools.Core;
namespace UCLouvain.KAOSTools.Propagators
{
    public class SamplingVector
    {
        KAOSModel _model;
    
        public SamplingVector (KAOSModel model)
        {
            this._model = model;
        }
        
        public double Sample (Obstacle o) 
        {
            return _model.satisfactionRateRepository.GetObstacleSatisfactionRate (o.Identifier).Sample ();
        }

        internal bool ContainsKey (string identifier)
        {
            return _model.satisfactionRateRepository.ObstacleSatisfactionRateExists (identifier);
        }
    }
}
