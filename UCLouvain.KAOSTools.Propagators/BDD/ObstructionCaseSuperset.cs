using System;
using System.Collections.Generic;
using System.Linq;
using UCLouvain.KAOSTools.Core;
using NLog;
using UCLouvain.BDDSharp;

namespace UCLouvain.KAOSTools.Propagators.BDD
{
	public class ObstructionCaseSuperset : ObstructionSuperset
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        Dictionary<int, double> defaultValue = new Dictionary<int, double> ();
        
        #region Constructors

        public ObstructionCaseSuperset (Goal goal) : base (goal)
        {
        	this.ResetCache();
        }

        public ObstructionCaseSuperset (Obstacle obstacle) : base (obstacle)
        {
        	this.ResetCache();
        }

        #endregion
        
        
        protected override double GetProbability (BDDNode node, SamplingVector samplingVector, Dictionary<BDDNode, double> cache = null)
        {
            if (node.IsOne) return 1.0;
            if (node.IsZero) return 0.0;
            
        	if (cache == null)
                cache = new Dictionary<BDDNode, double> ();
            else if (cache.TryGetValue (node, out double cached))
                return cached;
                
            if (_rmapping[node.Index] is PrimitiveRefineeParameter<double> c)
			{
				double value = GetProbability(node.Low, samplingVector, cache) * (1 - c.Value)
					   + GetProbability(node.High, samplingVector, cache) * c.Value;
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
        
        public BDDNode GetObstructionSet (GoalRefinement refinement)
        {
            BDDNode acc = null;
			var dict = new Dictionary<Goal, BDDNode>();
			foreach (var child in refinement.SubGoals())
			{
				dict.Add(child, GetObstructionSet(child));
			}

			BDDNode acc2 = _manager.Zero;

			foreach (var child in refinement.SubGoalIdentifiers)
			{
				if (child.Parameters != null && child.Parameters is PrimitiveRefineeParameter<double> c)
				{
					Console.WriteLine("We have a partial satisfaction.");

					// Create or get a variable for the partial refinement
					int idx;
					if (_mapping.ContainsKey(c))
					{
						idx = _mapping[c];
					}
					else
					{
						idx = _manager.CreateVariable();
						_mapping.Add(c, idx);
						_rmapping.Add(idx, c);
					}

					// AND the negation of the variable to the other subgoals' bddnode
					var neg = _manager.Create(idx, 0, 1);

					acc2 = _manager.Xor(_manager.Create(idx, 1, 0), acc2);
					//BDDNode bDDNode1 = GetObstructionSet(refinement.model.Goal(child.Identifier));
					//var imp = _manager.Imply(bDDNode1, neg);
					//Console.WriteLine("---");
					//Console.WriteLine(this.ToDot(imp));
					//Console.WriteLine("---");

					foreach (var node in dict.ToArray()) {
						if (node.Key.Identifier != child.Identifier) {
					//Console.WriteLine("===");
					//Console.WriteLine(this.ToDot(dict[node.Key]));
							dict[node.Key] = _manager.And(node.Value, neg);
					//Console.WriteLine(this.ToDot(dict[node.Key]));
					//Console.WriteLine("===");
						}
					}
				}
			}

			foreach (var kv in dict) {
                if (acc == null) 
                    acc = kv.Value;
                else {
					acc = _manager.Or(acc, kv.Value);
                }
            }
            
            foreach (var child in refinement.DomainHypotheses ()) {
                if (acc == null) 
                    acc = GetObstructionSet (child);
                else {
                    var bDDNode = GetObstructionSet(child);
                    acc = _manager.Or(acc, bDDNode);
                }
            }
            return _manager.And(acc, acc2);
        }

		private BDDNode AddPartial(Goal goal, BDDNode bdd)
		{
			throw new NotImplementedException();
		}

		public override string ToDot (BDDNode node)
        {
            return _manager.ToDot (node, (arg) => {
				if (_rmapping[arg.Index] is PrimitiveRefineeParameter<double> e)
				{
					return $"Case [{e.Value}]";
				} else if (_rmapping[arg.Index] is KAOSCoreElement kce) {
					return kce.FriendlyName;
				} else {
					return _rmapping[arg.Index].ToString();
				}
            }, false);
        }
        
        public override string ToDot ()
        {
            return ToDot (_root);
        }
	}
}
