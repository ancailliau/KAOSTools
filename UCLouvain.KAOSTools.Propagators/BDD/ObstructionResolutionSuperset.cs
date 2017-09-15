using System;
using System.Collections.Generic;
using System.Linq;
using UCLouvain.KAOSTools.Core;
using NLog;
using UCLouvain.BDDSharp;

namespace UCLouvain.KAOSTools.Propagators.BDD
{
    public class ObstructionResolutionSuperset : ObstructionSuperset
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        Dictionary<int, double> defaultValue = new Dictionary<int, double> ();
        
        #region Constructors

        public ObstructionResolutionSuperset (Goal goal) : base (goal)
        {
        }

        public ObstructionResolutionSuperset (Obstacle obstacle) : base (obstacle)
        {
        }

        #endregion
        
        public override BDDNode GetObstructionSet (Goal goal)
        {
			if (goalCache.ContainsKey(goal))
				return goalCache[goal];
        
            IEnumerable<GoalRefinement> refinements = goal.Refinements ();
            IEnumerable<Obstruction> obstructions = goal.Obstructions ();
            
            if ((refinements.Count () + obstructions.Count ()) > 1)
                throw new PropagationException (PropagationException.INVALID_MODEL + $". Check '{goal.Identifier}' for obstructions and refinements.");

            var r = refinements.SingleOrDefault ();
            
            if (r != null) {
                var bddForRefinement = GetObstructionSet(r);

				//if (goal.Exceptions().Count() > 0) {
				//// Get the exception
				//var singleException = goal.Exceptions().Single();
				foreach (var singleException in goal.Exceptions()) {
					// Get the BDD for the countermeasure goal
					var bddForCountermeasure = GetObstructionSet(singleException.ResolvingGoal());

					// Get the BDD for the sibling goal (i.e. remove the resolved obstacle)
					Obstacle obstacle = singleException.Obstacle();
					var obstacleVariableId = _mapping[obstacle];
					var bddForPGprime = _manager.Restrict(bddForRefinement, -1, obstacleVariableId);					
					
					// Create a variable for the resolution
					var idx = _manager.CreateVariable ();
					_mapping.Add (singleException, idx);
					_rmapping.Add (idx, singleException);

					var bddWhenIntegrated = _manager.Or(bddForPGprime, bddForCountermeasure);

					bddForRefinement = _manager.Create(idx, bddWhenIntegrated, bddForRefinement);
				}
                
                goalCache.Add(goal, bddForRefinement);
                return bddForRefinement;
            }
            
            var o = obstructions.SingleOrDefault ();

			if (o != null)
			{
				BDDNode bn = GetObstructionSet(o);
                goalCache.Add(goal, bn);
				return bn;
			}

			goalCache.Add(goal, _manager.Zero);
            return _manager.Zero;
        }
    }
}
