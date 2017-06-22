using System;
using KAOSTools.Core;
namespace UCLouvain.KAOSTools.Integrator
{
    public class ResolutionIntegrator
    {
        KAOSModel _model;
    
        public ResolutionIntegrator (KAOSModel model)
        {
            _model = model;
        }
        
        public void Integrate (Resolution resolution)
        {
            if (resolution.ResolutionPattern == ResolutionPattern.GoalSubstitution
            || resolution.ResolutionPattern == ResolutionPattern.GoalWeakening)
            {
                RemoveObstructedGoal (resolution);
            } else if (resolution.ResolutionPattern == ResolutionPattern.ObstaclePrevention
            || resolution.ResolutionPattern == ResolutionPattern.ObstacleMitigation
            || resolution.ResolutionPattern == ResolutionPattern.ObstacleWeakMitigation
            || resolution.ResolutionPattern == ResolutionPattern.ObstacleStrongMitigation
            || resolution.ResolutionPattern == ResolutionPattern.ObstacleReduction)
            {
                KeepObstructedGoal (resolution);
            }
        }

        void KeepObstructedGoal (Resolution resolution)
        {
            throw new NotImplementedException ();
        }

        void RemoveObstructedGoal (Resolution resolution)
        {
            throw new NotImplementedException ();
        }
    }
}
