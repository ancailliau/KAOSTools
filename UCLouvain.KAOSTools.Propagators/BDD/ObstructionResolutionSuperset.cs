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
        	this.ResetCache();
        }

        public ObstructionResolutionSuperset (Obstacle obstacle) : base (obstacle)
        {
        	this.ResetCache();
        }

        #endregion

        public virtual double GetProbability (SamplingVector samplingVector, IEnumerable<GoalException> activeExceptions)
        {
            return GetProbability (_root, samplingVector, activeExceptions);
        }
        
        protected virtual double GetProbability (BDDNode node, 
										         SamplingVector samplingVector,
												 IEnumerable<GoalException> activeExceptions,
										         Dictionary<BDDNode, double> cache = null)
        {
            if (node.IsOne) return 1.0;
            if (node.IsZero) return 0.0;
            
        	if (cache == null)
                cache = new Dictionary<BDDNode, double> ();
            else if (cache.TryGetValue (node, out double cached))
                return cached;
                
            if (_rmapping[node.Index] is GoalException exception)
			{
				double value;
				if (activeExceptions.Contains(exception)) {
					value = GetProbability(node.High, samplingVector, activeExceptions, cache);
				} else {
					value = GetProbability(node.Low, samplingVector, activeExceptions, cache);
				}
				cache[node] = value;
				return value;
				
			} else if (_rmapping[node.Index] is Obstacle obstacle)
			{

				var v = samplingVector.ContainsKey(obstacle.Identifier) & !obstacle.Resolved ?
							samplingVector.Sample(obstacle) : 0;

				double value = GetProbability(node.Low, samplingVector, activeExceptions, cache) * (1 - v)
					   + GetProbability(node.High, samplingVector, activeExceptions, cache) * v;
				cache[node] = value;
				return value;
			} else if (_rmapping[node.Index] is DomainProperty domProp)
			{
				var v = samplingVector.ContainsKey(domProp.Identifier) ?
							samplingVector.Sample(domProp) : 0;

				double value = GetProbability(node.Low, samplingVector, activeExceptions, cache) * (1 - v)
					   + GetProbability(node.High, samplingVector, activeExceptions, cache) * v;
				cache[node] = value;
				return value;
			}  else if (_rmapping[node.Index] is DomainHypothesis domHyp)
			{
				var v = samplingVector.ContainsKey(domHyp.Identifier) ?
							samplingVector.Sample(domHyp) : 0;

				double value = GetProbability(node.Low, samplingVector, activeExceptions, cache) * (1 - v)
					   + GetProbability(node.High, samplingVector, activeExceptions, cache) * v;
				cache[node] = value;
				return value;
			} else {
				throw new NotImplementedException("Type " + _rmapping[node.Index].GetType() + " is not yet supported.");
			}
        }
        
        protected override double GetProbability (BDDNode node, SamplingVector samplingVector, Dictionary<BDDNode, double> cache = null)
        {
            if (node.IsOne) return 1.0;
            if (node.IsZero) return 0.0;
            
        	if (cache == null)
                cache = new Dictionary<BDDNode, double> ();
            else if (cache.TryGetValue (node, out double cached))
                return cached;
                
            if (_rmapping[node.Index] is GoalException exception)
			{
				var v = 0;
				double value = GetProbability(node.Low, samplingVector, cache) * (1 - v)
					   + GetProbability(node.High, samplingVector, cache) * v;
				cache[node] = value;
				return value;
			} else {
				return base.GetProbability(node, samplingVector, cache);
			}
        }
        
        public override BDDNode GetObstructionSet (Goal goal)
        {
			if (goalCache.ContainsKey(goal))
				return goalCache[goal];
        
            IEnumerable<GoalRefinement> refinements = goal.Refinements ();
            IEnumerable<Obstruction> obstructions = goal.Obstructions ();
            
            if ((refinements.Count () + obstructions.Count ()) > 1)
                throw new PropagationException (PropagationException.INVALID_MODEL + $". Check '{goal.Identifier}' for obstructions and refinements.");

            var r = refinements.SingleOrDefault ();
            
            if (r != null)
			{
				var bddForRefinement = GetObstructionSet(r);
				bddForRefinement = AddExceptions(goal, bddForRefinement);

				goalCache.Add(goal, bddForRefinement);
				return bddForRefinement;
			}

			var o = obstructions.SingleOrDefault ();

			if (o != null)
			{
				BDDNode bn = GetObstructionSet(o);
				bn = AddExceptions(goal, bn);
                goalCache.Add(goal, bn);
				return bn;
			}

			goalCache.Add(goal, _manager.Zero);
            return _manager.Zero;
        }

		private BDDNode AddExceptions(Goal goal, BDDNode bddForRefinement)
		{
			foreach (var singleException in goal.Exceptions())
			{
				// Get the BDD for the countermeasure goal
				Goal cmGoal = singleException.ResolvingGoal();
				if (cmGoal == null)
					throw new ArgumentNullException($"Goal '{singleException.ResolvingGoalIdentifier}' not found.");
				var bddForCountermeasure = GetObstructionSet(cmGoal);

				// Get the BDD for the sibling goal (i.e. remove the resolved obstacle)
				Obstacle obstacle = singleException.Obstacle();
				var obstacleVariableId = _mapping[obstacle];
				var bddForPGprime = _manager.Restrict(bddForRefinement, -1, obstacleVariableId);

				// Create a variable for the resolution
				var idx = _manager.CreateVariable();
				_mapping.Add(singleException, idx);
				_rmapping.Add(idx, singleException);

				var bddWhenIntegrated = _manager.Or(bddForPGprime, bddForCountermeasure);

				bddForRefinement = _manager.Create(idx, bddWhenIntegrated, bddForRefinement);
			}

			return bddForRefinement;
		}
		
		public override string ToDot (BDDNode node)
        {
            return _manager.ToDot (node, (arg) => {
				if (_rmapping[arg.Index] is GoalException e)
				{
					return "Except " + e.ResolvingGoalIdentifier;
				}
				return _rmapping[arg.Index].FriendlyName;
            }, false);
        }
        
        public override string ToDot ()
        {
            return ToDot (_root);
        }
	}
}
