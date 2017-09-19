using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Core.SatisfactionRates;

namespace UCLouvain.KAOSTools.Propagators.BDD
{
    public class BDDBasedResolutionPropagator : IPropagator
    {
        protected KAOSModel _model;
        
        protected Dictionary<string, ObstructionResolutionSuperset> obstructionSupersets;
        
        protected HashSet<Goal> prebuilt_goal;

        public BDDBasedResolutionPropagator (KAOSModel model)
        {
            _model = model;
			prebuilt_goal = new HashSet<Goal>();
			obstructionSupersets = new Dictionary<string, ObstructionResolutionSuperset>();
        }

        public virtual ISatisfactionRate GetESR (Obstacle obstacle)
        {
            throw new NotImplementedException ();
        }
        
        public virtual void PreBuildObstructionSet (Goal goal)
        {
			if (prebuilt_goal.Contains(goal))
			{
				obstructionSupersets[goal.Identifier] = new ObstructionResolutionSuperset(goal);
			} else {
				obstructionSupersets.Add(goal.Identifier, new ObstructionResolutionSuperset(goal));
				prebuilt_goal.Add(goal);
			}
        } 

        public virtual ISatisfactionRate GetESR (Goal goal)
        {
			ObstructionResolutionSuperset os;
			if (!obstructionSupersets.ContainsKey(goal.Identifier))
				os = new ObstructionResolutionSuperset(goal);
			else
				os = obstructionSupersets[goal.Identifier];
            var vector = new SamplingVector (_model);
            return new DoubleSatisfactionRate (1.0 - os.GetProbability (vector));
        }

        public virtual ISatisfactionRate GetESR (Obstacle obstacle, IEnumerable<Resolution> activeResolutions)
        {
            throw new NotImplementedException ();
        }

        public virtual ISatisfactionRate GetESR (Goal goal, IEnumerable<Resolution> activeResolutions)
        {
            throw new NotImplementedException ();
        }

        public virtual ISatisfactionRate GetESR (Goal goal, IEnumerable<GoalException> activeResolutions)
        {
			ObstructionResolutionSuperset os;
			if (!obstructionSupersets.ContainsKey(goal.Identifier))
				os = new ObstructionResolutionSuperset(goal);
			else
				os = obstructionSupersets[goal.Identifier];
            var vector = new SamplingVector (_model);
            return new DoubleSatisfactionRate (1.0 - os.GetProbability (vector, activeResolutions));
        }

        public ISatisfactionRate GetESR (Goal goal, IEnumerable<Obstacle> onlyObstacles)
        {
			ObstructionResolutionSuperset os;
			if (!obstructionSupersets.ContainsKey(goal.Identifier))
				os = new ObstructionResolutionSuperset(goal);
			else
				os = obstructionSupersets[goal.Identifier];

			var vector = new SamplingVector (_model, onlyObstacles?.Select(x => x.Identifier)?.ToHashSet());
			var value = 1.0 - os.GetProbability(vector);
			
			return new DoubleSatisfactionRate(value);
        }
    }
}
