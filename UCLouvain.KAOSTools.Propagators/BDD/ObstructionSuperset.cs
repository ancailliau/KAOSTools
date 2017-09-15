using System;
using System.Collections.Generic;
using System.Linq;
using UCLouvain.KAOSTools.Core;
using NLog;
using UCLouvain.BDDSharp;

namespace UCLouvain.KAOSTools.Propagators.BDD
{
    public class ObstructionSuperset
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();
        
        protected readonly BDDManager _manager;
        protected readonly Dictionary<KAOSCoreElement, int> _mapping;
        protected readonly Dictionary<int, KAOSCoreElement> _rmapping;
        
        BDDNode _root;

        public int NodesNumber { get {
                return _manager.nextId;
            }
        }

        #region Constructors

        ObstructionSuperset ()
        {
            _manager = new BDDManager (0);
            _mapping = new Dictionary<KAOSCoreElement, int> ();
            _rmapping = new Dictionary<int, KAOSCoreElement> ();
        }

        public ObstructionSuperset (Goal goal) : this ()
        {
            _root = GetObstructionSet (goal);
            //_root = _manager.Sifting (_root);
        }

        public ObstructionSuperset (Obstacle obstacle) : this ()
        {
            _root = GetObstructionSet (obstacle);
            //_root = _manager.Sifting (_root);
        }

        #endregion

        #region GetProbability

        public virtual double GetProbability (SamplingVector samplingVector)
        {
            return GetProbability (_root, samplingVector);
        }

        protected virtual double GetProbability (BDDNode node, SamplingVector samplingVector, Dictionary<BDDNode, double> cache = null)
        {
            if (node.IsOne) return 1.0;
            if (node.IsZero) return 0.0;

            if (cache == null)
                cache = new Dictionary<BDDNode, double> ();
            else if (cache.TryGetValue (node, out double cached))
                return cached;

			if (_rmapping[node.Index] is Obstacle obstacle)
			{

				var v = samplingVector.ContainsKey(obstacle.Identifier) & !obstacle.Resolved ?
							samplingVector.Sample(obstacle) : 0;

				double value = GetProbability(node.Low, samplingVector, cache) * (1 - v)
					   + GetProbability(node.High, samplingVector, cache) * v;
				cache[node] = value;
				return value;
			} else if (_rmapping[node.Index] is DomainProperty domProp)
			{
				var v = samplingVector.ContainsKey(domProp.Identifier) ?
							samplingVector.Sample(domProp) : 0;

				double value = GetProbability(node.Low, samplingVector, cache) * (1 - v)
					   + GetProbability(node.High, samplingVector, cache) * v;
				cache[node] = value;
				return value;
			}  else if (_rmapping[node.Index] is DomainHypothesis domHyp)
			{
				var v = samplingVector.ContainsKey(domHyp.Identifier) ?
							samplingVector.Sample(domHyp) : 0;

				double value = GetProbability(node.Low, samplingVector, cache) * (1 - v)
					   + GetProbability(node.High, samplingVector, cache) * v;
				cache[node] = value;
				return value;
			} else {
				throw new NotImplementedException("Type " + _rmapping[node.Index].GetType() + " is not yet supported.");
			}
        }

		#endregion

		#region GetObstructionSet

		protected Dictionary<Goal, BDDNode> goalCache = new Dictionary<Goal, BDDNode>();
		protected Dictionary<Obstacle, BDDNode> obstacleCache = new Dictionary<Obstacle, BDDNode>();
		
		public virtual void ResetCache ()
		{
			goalCache = new Dictionary<Goal, BDDNode>();
			obstacleCache = new Dictionary<Obstacle, BDDNode>();
		}

        public virtual BDDNode GetObstructionSet (Goal goal)
        {
			if (goalCache.ContainsKey(goal))
				return goalCache[goal];
        
            IEnumerable<GoalRefinement> refinements = goal.Refinements ();
            IEnumerable<Obstruction> obstructions = goal.Obstructions ();
            
            if ((refinements.Count () + obstructions.Count ()) > 1)
                throw new PropagationException (PropagationException.INVALID_MODEL + $". Check '{goal.Identifier}' for obstructions and refinements.");

            var r = refinements.SingleOrDefault ();
            
            if (r != null) {
                BDDNode acc = null;
                foreach (var child in goal.Exceptions()) {
                    if (acc == null)
                        acc = GetObstructionSet(child);
                    else {
                        var bDDNode = GetObstructionSet(child);
                        acc = _manager.Or(acc, bDDNode);
                    }
                }

                if (acc == null)
                    acc = GetObstructionSet(r);
                else {
                    var bDDNode = GetObstructionSet(r);
                    acc = _manager.Or(acc, bDDNode);
                }
                goalCache.Add(goal, acc);
                return acc;
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

        public BDDNode GetObstructionSet(GoalException exception)
        {
            var child = exception.ResolvingGoal();
            return GetObstructionSet (child);
        }

        public BDDNode GetObstructionSet (GoalRefinement refinement)
        {
            BDDNode acc = null;
            foreach (var child in refinement.SubGoals ()) {
                if (acc == null) 
                    acc = GetObstructionSet (child);
                else {
                    var bDDNode = GetObstructionSet(child);
                    acc = _manager.Or(acc, bDDNode);
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
            return acc;
        }

        public BDDNode GetObstructionSet (Obstruction obstruction)
        {
            return GetObstructionSet (obstruction.Obstacle ());
        }

        public BDDNode GetObstructionSet (Obstacle obstacle)
        {
			if (obstacleCache.ContainsKey(obstacle))
				return obstacleCache[obstacle];
        
            BDDNode acc2 = null;
            foreach (var r in obstacle.Refinements ()) {
                BDDNode acc = null;
                foreach (var c in r.SubObstacles ()) {
                    if (acc == null) {
                        acc = GetObstructionSet (c);
                    } else {
                        var bDDNode = GetObstructionSet(c);
                        acc = _manager.And(acc, bDDNode);
                    }
                }
                foreach (var c in r.DomainHypotheses ()) {
                    if (acc == null) {
                        acc = GetObstructionSet (c);
                    } else {
                        var bDDNode = GetObstructionSet(c);
                        acc = _manager.And(acc, bDDNode);
                    }
                }
                foreach (var c in r.DomainProperties ()) {
                    if (acc == null) {
                        acc = GetObstructionSet (c);
                    } else {
                        var bDDNode = GetObstructionSet(c);
                        acc = _manager.And(acc, bDDNode);
                    }
                }
                if (acc2 == null) {
                    acc2 = acc;
                } else {
                    acc2 = _manager.Or (acc2, acc);
                }
            }

            // Leaf obstacle
            if (acc2 == null) {
                int idx;
                if (_mapping.ContainsKey (obstacle)) {
                    idx = _mapping[obstacle];
                } else {
                    idx = _manager.CreateVariable ();
                    _mapping.Add (obstacle, idx);
                    _rmapping.Add (idx, obstacle);
                }
                acc2 = _manager.Create(idx, _manager.One, _manager.Zero);
            }

			obstacleCache.Add(obstacle, acc2);
            return acc2;
        }

        public BDDNode GetObstructionSet (DomainHypothesis domhyp)
        {
            int idx;
            if (_mapping.ContainsKey (domhyp)) {
                idx = _mapping[domhyp];
            } else {
                idx = _manager.CreateVariable ();
                _mapping.Add (domhyp, idx);
                _rmapping.Add (idx, domhyp);
            }
            return _manager.Create(idx, _manager.One, _manager.Zero);
        }

        public BDDNode GetObstructionSet (DomainProperty domprop)
        {
            int idx;
            if (_mapping.ContainsKey (domprop)) {
                idx = _mapping[domprop];
            } else {
                idx = _manager.CreateVariable ();
                _mapping.Add (domprop, idx);
                _rmapping.Add (idx, domprop);
            }
            return _manager.Create(idx, _manager.One, _manager.Zero);
        }

        #endregion
        
        public string ToDot (BDDNode node)
        {
            return _manager.ToDot (node, (x) => _rmapping [x.Index].FriendlyName, false);
        }
        
        public string ToDot ()
        {
            return ToDot (_root);
        }
    }
}
