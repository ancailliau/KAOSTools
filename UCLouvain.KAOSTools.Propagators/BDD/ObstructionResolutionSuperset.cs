using System;
using System.Collections.Generic;
using System.Linq;
using KAOSTools.Core;
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
        
        public void SetDefaultValue (IEnumerable<KAOSCoreElement> element, double value)
        {
            foreach (var e in element)
                SetDefaultValue (e, value);       
        }
        
        public void SetDefaultValue (KAOSCoreElement element, double value)
        {
            if (_mapping.ContainsKey (element)) {
                var key = _mapping [element];
                if (defaultValue.ContainsKey (key)) {
                    defaultValue [key] = value;
                } else {
                    defaultValue.Add (key, value);
                }
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

            double v = 0;
            if (_rmapping[node.Index] is Obstacle obstacle) {
	            if (samplingVector.ContainsKey (obstacle.Identifier)) {
	                v = samplingVector.Sample (obstacle);
	            }
	        } else if (defaultValue.ContainsKey (node.Index)) {
                v = defaultValue [node.Index];
            }
            
            double value = GetProbability (node.Low, samplingVector, cache) * (1 - v)
                   + GetProbability (node.High, samplingVector, cache) * v;
            cache [node] = value;
            return value;
        }

        #region GetObstructionSet

        // Code from Stackoverflow
        // https://stackoverflow.com/questions/7802822/all-possible-combinations-of-a-list-of-values
        static List<List<T>> GetAllCombinations<T> (List<T> list)
        {
            if (list.Count == 0)
                return new List<List<T>> ();
            List<List<T>> result = new List<List<T>> ();
            result.Add (new List<T> ());
            result.Last ().Add (list [0]);
            if (list.Count == 1)
                return result;
            List<List<T>> tailCombos = GetAllCombinations (list.Skip (1).ToList ());
            tailCombos.ForEach (combo => {
                result.Add (new List<T> (combo));
                combo.Add (list [0]);
                result.Add (new List<T> (combo));
            });
            return result;
        }

        public override BDDNode GetObstructionSet (Goal goal)
        {
            IEnumerable<GoalRefinement> refinements = goal.Refinements ();
            IEnumerable<Obstruction> obstructions = goal.Obstructions ();
            
            if ((refinements.Count () + obstructions.Count ()) > 1)
                throw new PropagationException (PropagationException.INVALID_MODEL);

            BDDNode ideal = _manager.Zero;

            var r = refinements.SingleOrDefault ();
            
            if (r != null)
                ideal = GetObstructionSet(r);
            
            var o = obstructions.SingleOrDefault ();

            if (o != null)
                ideal = GetObstructionSet (o);

            BDDNode ideal_condition = _manager.One;
            
            BDDNode resolution_bdd = ideal;
            foreach (var ar in GetAllCombinations (goal.AnchoredResolutions().ToList ())) {
                var alternative_os = GetObstructionSet (goal.model, goal.Identifier, ar, ideal);

                // Build the condition
                BDDNode combination_condition = _manager.One;
                foreach (var resolution in ar) {
                    int idx;
                    if (_mapping.ContainsKey (resolution)) {
                        idx = _mapping [resolution];
                    } else {
                        idx = _manager.CreateVariable ();
                        _mapping.Add (resolution, idx);
                        _rmapping.Add (idx, resolution);
                        defaultValue.Add (idx, 0);
                    }
                    var condition = _manager.Create (idx, _manager.One, _manager.Zero);
                    combination_condition = _manager.And (combination_condition, condition);
                }

                resolution_bdd = _manager.ITE (combination_condition, alternative_os, resolution_bdd);
            }

            return resolution_bdd;
        }

        //BDDNode GetObstructionSet(KAOSModel model, string anchorId, IEnumerable<Resolution> resolution, BDDNode ideal)
        //{
            //if (resolution.ResolutionPattern == ResolutionPattern.GoalSubstitution
            //|| resolution.ResolutionPattern == ResolutionPattern.GoalWeakening)
            //{
            //    Console.WriteLine ("Remove obstructed goal");
            //    return GetObstructionSetRemoveObstructedGoal (model, anchorId, resolution);
                
            //} else if (resolution.ResolutionPattern == ResolutionPattern.ObstaclePrevention
            //|| resolution.ResolutionPattern == ResolutionPattern.ObstacleMitigation
            //|| resolution.ResolutionPattern == ResolutionPattern.ObstacleWeakMitigation
            //|| resolution.ResolutionPattern == ResolutionPattern.ObstacleStrongMitigation
            //|| resolution.ResolutionPattern == ResolutionPattern.ObstacleReduction
            //|| resolution.ResolutionPattern == ResolutionPattern.GoalRestoration)
            //{
            //    Console.WriteLine ("Keep obstructed goal");
                //throw new NotImplementedException ();
            //    // return GetObstructionSetKeepObstructedGoal (resolution, ideal);
                
            //} else if (resolution.ResolutionPattern == ResolutionPattern.None) {
            //    throw new PropagationException (PropagationException.INVALID_MODEL);
                
            //} else {
            //    throw new PropagationException (PropagationException.INVALID_MODEL);
            //}
        //}

        //BDDNode GetObstructionSetKeepObstructedGoal(Resolution r, BDDNode ideal)
        //{
        //    // Get obstructed goal variable
        //    int idx;
        //    var obstacle = r.Obstacle ();
        //    if (_mapping.ContainsKey (obstacle)) {
        //        idx = _mapping[obstacle];
        //    } else {
        //        idx = _manager.CreateVariable ();
        //        _mapping.Add (obstacle, idx);
        //        _rmapping.Add (idx, obstacle);
        //    }
        //    var not_leaf_obstacle = _manager.Create(idx, _manager.Zero, _manager.One);
            
        //    // Get the os for the "ideal" goal
        //    var os_ideal = _manager.And (ideal, not_leaf_obstacle);
            
        //    // Get the os for countermeasuer goal
        //    var os_countermeasure = GetObstructionSet (r.ResolvingGoal ());

        //    // Combine and return
        //    return _manager.Or (os_countermeasure, os_ideal);
        //}

        //BDDNode GetObstructionSetRemoveObstructedGoal(Resolution r)
        //{
        //    // Get the os for the countermeasure goal
        //    var os_countermeasure = GetObstructionSet (r.ResolvingGoal ());
        //    Console.WriteLine ("*** Obstruction set for the countermeasure");
        //        Console.WriteLine (ToDot (os_countermeasure));
        //    Console.WriteLine ("***");
            
        //    // Get the obstructed goals
        //    var obstructed_goals = r.GetObstructedGoalIdentifiers ();
        //    Console.WriteLine ("Obstructed goals are: " + string.Join (",", obstructed_goals));

        //    // Get the non-obstructed children
        //    var anchorRefinements = r.model.GoalRefinements (x => x.ParentGoalIdentifier == r.AnchorIdentifier);
        //    if (anchorRefinements.Count () > 1 | anchorRefinements.Count () == 0) 
        //        throw new PropagationException (PropagationException.INVALID_MODEL);
        //    var anchorRefinement = anchorRefinements.Single ();
        //    var nonObstructedChildren = anchorRefinement.SubGoalIdentifiers.Select (x => x.Identifier).Except (obstructed_goals);
        //    Console.WriteLine ("Non obstructed children are: " + string.Join (",", nonObstructedChildren));

        //    // Build the new refinement
        //    foreach (var nO in nonObstructedChildren) {
        //        BDDNode bDDNode = GetObstructionSet (r.model.Goal (nO));
        //        os_countermeasure = _manager.Or (os_countermeasure, bDDNode);
        //    }
            
        //    Console.WriteLine ("**** Obstruction set for the new refinement : ");
        //    Console.WriteLine (ToDot (os_countermeasure));
        //    Console.WriteLine ("****");
            
        //    return os_countermeasure;
        //}

        BDDNode GetObstructionSet(KAOSModel model, string anchor_id, IEnumerable<Resolution> r, BDDNode ideal)
        {
            IEnumerable<Resolution> keepObstructedGoalResolutions
             = r.Where (x => x.ResolutionPattern == ResolutionPattern.ObstaclePrevention
            || x.ResolutionPattern == ResolutionPattern.ObstacleMitigation
            || x.ResolutionPattern == ResolutionPattern.ObstacleWeakMitigation
            || x.ResolutionPattern == ResolutionPattern.ObstacleStrongMitigation
            || x.ResolutionPattern == ResolutionPattern.ObstacleReduction
            || x.ResolutionPattern == ResolutionPattern.GoalRestoration);
             
            IEnumerable<Resolution> removeObstructedGoalResolutions
             = r.Where (x => x.ResolutionPattern == ResolutionPattern.GoalSubstitution
             || x.ResolutionPattern == ResolutionPattern.GoalWeakening);

            // The condition to propagate on the ideal goal
            BDDNode kog_condition = _manager.One;
            
            // The "partial" refinement with the cm goals
            BDDNode ko = _manager.Zero;

            // Adds all countermeasure required by "keep obstructed goal" cm
            
            foreach (var kogr in keepObstructedGoalResolutions) {
                // Get obstructed goal variable
                int idx;
                var obstacle = kogr.Obstacle ();
                if (_mapping.ContainsKey (obstacle)) {
                    idx = _mapping [obstacle];
                } else {
                    idx = _manager.CreateVariable ();
                    _mapping.Add (obstacle, idx);
                    _rmapping.Add (idx, obstacle);
                }
                kog_condition = _manager.And (kog_condition, _manager.Create (idx, _manager.Zero, _manager.One));

                // Get and combine the os for countermeasure goal
                ko =  _manager.Or (ko, GetObstructionSet (kogr.ResolvingGoal ()));
            }

            // Removes all children obstructed by "remove obstructed goal" cm
            
            // Get the obstructed goals
            var obstructed_goals = removeObstructedGoalResolutions.SelectMany (x => x.GetObstructedGoalIdentifiers ());

            // Get the non-obstructed children
            var anchorRefinements = model.GoalRefinements (x => x.ParentGoalIdentifier == anchor_id);
            if (anchorRefinements.Count () > 1 | anchorRefinements.Count () == 0) 
                throw new PropagationException (PropagationException.INVALID_MODEL);
            var anchorRefinement = anchorRefinements.Single ();
            var nonObstructedChildren = anchorRefinement.SubGoalIdentifiers.Select (x => x.Identifier).Except (obstructed_goals);
            
            // Get the os for the countermeasure goal
            var os_countermeasure = _manager.Zero;
            foreach (var resolution in removeObstructedGoalResolutions) {
                var os = GetObstructionSet (resolution.ResolvingGoal ());
                os_countermeasure = _manager.Or (os_countermeasure, os);
            }
            // Adds the non-obstructed goals
            foreach (var nO in nonObstructedChildren) {
                BDDNode bDDNode = GetObstructionSet (model.Goal (nO));
                os_countermeasure = _manager.Or (os_countermeasure, bDDNode);
            }
            
            return _manager.Or (ko, _manager.And (os_countermeasure, kog_condition));
        }

        //// Returns the os given that the specified resolutions are active
        //BDDNode GetObstructionSetRemoveObstructedGoal(KAOSModel model, string anchorId, IEnumerable<Resolution> r)
        //{
        //    // Get the os for the countermeasure goal
        //    var os_countermeasure = _manager.Zero;
        //    foreach (var resolution in r) {
        //        var os = GetObstructionSet (resolution.ResolvingGoal ());
        //        Console.WriteLine ("*** Obstruction set for the countermeasure");
        //        Console.WriteLine (ToDot (os_countermeasure));
        //        Console.WriteLine ("***");
        //        os_countermeasure = _manager.Or (os_countermeasure, os);
        //    }
            
        //    // Get the obstructed goals
        //    var obstructed_goals = r.SelectMany (x => x.GetObstructedGoalIdentifiers ());
        //    Console.WriteLine ("Obstructed goals are: " + string.Join (",", obstructed_goals));

        //    // Get the non-obstructed children
        //    var anchorRefinements = model.GoalRefinements (x => x.ParentGoalIdentifier == anchorId);
        //    if (anchorRefinements.Count () > 1 | anchorRefinements.Count () == 0) 
        //        throw new PropagationException (PropagationException.INVALID_MODEL);
        //    var anchorRefinement = anchorRefinements.Single ();
        //    var nonObstructedChildren = anchorRefinement.SubGoalIdentifiers.Select (x => x.Identifier).Except (obstructed_goals);
        //    Console.WriteLine ("Non obstructed children are: " + string.Join (",", nonObstructedChildren));

        //    // Build the new refinement
        //    foreach (var nO in nonObstructedChildren) {
        //        BDDNode bDDNode = GetObstructionSet (model.Goal (nO));
        //        os_countermeasure = _manager.Or (os_countermeasure, bDDNode);
        //    }
            
        //    Console.WriteLine ("**** Obstruction set for the new refinement : ");
        //    Console.WriteLine (ToDot (os_countermeasure));
        //    Console.WriteLine ("****");
            
        //    return os_countermeasure;
        //}
        
        #endregion
    }
}
