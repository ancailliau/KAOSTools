using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Core.SatisfactionRates;

namespace UCLouvain.KAOSTools.Propagators.BDD
{
    public class BDDBasedPropagator : IPropagator
    {
        protected KAOSModel _model;
        
        protected Dictionary<string, ObstructionSuperset> obstructionSupersets;
        
        protected HashSet<Goal> prebuilt_goal;

        public BDDBasedPropagator (KAOSModel model)
        {
            _model = model;
			prebuilt_goal = new HashSet<Goal>();
			obstructionSupersets = new Dictionary<string, ObstructionSuperset>();
        }

        public virtual ISatisfactionRate GetESR (Obstacle obstacle)
        {
            throw new NotImplementedException ();
        }
        
        public virtual void PreBuildObstructionSet (Goal goal)
        {
			if (prebuilt_goal.Contains(goal))
			{
				obstructionSupersets[goal.Identifier] = new ObstructionSuperset(goal);
			} else {
				obstructionSupersets.Add(goal.Identifier, new ObstructionSuperset(goal));
				prebuilt_goal.Add(goal);
			}
        } 

        public virtual ISatisfactionRate GetESR (Goal goal)
        {
			ObstructionSuperset os;
			if (!obstructionSupersets.ContainsKey(goal.Identifier)) {
				//Console.WriteLine("Computing new OS for " + goal.Identifier);
				os = new ObstructionSuperset(goal);
				//Console.WriteLine("---");
				//Console.WriteLine(os.ToDot());
				//Console.WriteLine("---");
			} else
				os = obstructionSupersets[goal.Identifier];
            var vector = new SamplingVector (_model);
            
			double v = os.GetProbability(vector);
            //Console.WriteLine("Obstruction set probability: "  + v);
			return new DoubleSatisfactionRate(1.0 - v);
        }

        public virtual ISatisfactionRate GetESR (Obstacle obstacle, IEnumerable<Resolution> activeResolutions)
        {
            throw new NotImplementedException ();
        }

        public virtual ISatisfactionRate GetESR (Goal goal, IEnumerable<Resolution> activeResolutions)
        {
            throw new NotImplementedException ();
        }

        public ISatisfactionRate GetESR (Goal goal, IEnumerable<Obstacle> onlyObstacles)
        {
			ObstructionSuperset os;
			if (!obstructionSupersets.ContainsKey(goal.Identifier))
				os = new ObstructionSuperset(goal);
			else
				os = obstructionSupersets[goal.Identifier];

			var vector = new SamplingVector (_model, onlyObstacles?.Select(x => x.Identifier)?.ToHashSet());
			var value = 1.0 - os.GetProbability(vector);
			
			return new DoubleSatisfactionRate(value);
        }
    }
}
