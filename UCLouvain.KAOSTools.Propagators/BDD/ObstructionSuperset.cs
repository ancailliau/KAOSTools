using System;
using System.Collections.Generic;
using System.Linq;
using KAOSTools.Core;
using NLog;
using UCLouvain.BDDSharp;

namespace UCLouvain.KAOSTools.Propagators.BDD
{
    public class ObstructionSuperset
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();
        
        readonly BDDManager _manager;
        readonly Dictionary<KAOSCoreElement, int> _mapping;
        readonly Dictionary<int, KAOSCoreElement> _rmapping;
        
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

        public double GetProbability (SamplingVector samplingVector)
        {
            return GetProbability (_root, samplingVector);
        }

        double GetProbability (BDDNode node, SamplingVector samplingVector, Dictionary<BDDNode, double> cache = null)
        {
            if (node.IsOne) return 1.0;
            if (node.IsZero) return 0.0;

            if (cache == null)
                cache = new Dictionary<BDDNode, double> ();
            else if (cache.TryGetValue (node, out double cached))
                return cached;

            Obstacle obstacle = (Obstacle)_rmapping [node.Index];

            var v = samplingVector.ContainsKey (obstacle.Identifier) ?
                        samplingVector.Sample (obstacle) : 0;

            double value = GetProbability (node.Low, samplingVector, cache) * (1 - v)
                   + GetProbability (node.High, samplingVector, cache) * v;
            cache [node] = value;
            return value;
        }

        #endregion

        #region GetObstructionSet

        public BDDNode GetObstructionSet (Goal goal)
        {
            IEnumerable<GoalRefinement> refinements = goal.Refinements ();
            IEnumerable<Obstruction> obstructions = goal.Obstructions ();
            
            if ((refinements.Count () + obstructions.Count ()) > 1)
                throw new PropagationException (PropagationException.INVALID_MODEL);

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
                return acc;
            }
            
            var o = obstructions.SingleOrDefault ();

            if (o != null)
                return GetObstructionSet (o);

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
    }
}
