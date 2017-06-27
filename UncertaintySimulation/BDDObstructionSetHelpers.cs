using System;
using UCLouvain.KAOSTools.Core;
using UCLouvain.BDDSharp;
using System.Linq;
using System.Collections.Generic;
using NLog;

namespace UncertaintySimulation
{
    public class ObstructionSuperset {

        public BDDNode bdd;
        public Dictionary<KAOSCoreElement, int> mapping;
        public Dictionary<int, KAOSCoreElement> reverse_mapping;

        public ObstructionSuperset (BDDNode bdd, Dictionary<KAOSCoreElement, int> mapping, Dictionary<int, KAOSCoreElement> reverse_mapping)
        {
            this.bdd = bdd;

            this.mapping = mapping;
            this.reverse_mapping = reverse_mapping;
        }

        public double GetProbability (Dictionary<int, double> samplingVector) 
        {
            return bdd.GetProbability (samplingVector);
        }
    }

    public static class BDDObstructionSetHelpers {

		static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static ObstructionSuperset GetObstructionSuperset (this Goal goal, bool take_exception_in_account = true) {
            var manager = new BDDManager (0);

            var mapping = new Dictionary<KAOSCoreElement, int> ();
            var reverse_mapping = new Dictionary<int, KAOSCoreElement> ();
            var bDDNode = GetObstructionSet (goal, manager, mapping, reverse_mapping, take_exception_in_account);
            bDDNode = manager.Sifting (bDDNode);

			return new ObstructionSuperset (bDDNode, mapping, reverse_mapping);
        }

		public static ObstructionSuperset GetObstructionSuperset(this Obstacle obstacle)
		{
			var manager = new BDDManager(0);

			var mapping = new Dictionary<KAOSCoreElement, int>();
			var reverse_mapping = new Dictionary<int, KAOSCoreElement>();
			var bDDNode = GetObstructionSet(obstacle, manager, mapping, reverse_mapping);
			bDDNode = manager.Sifting(bDDNode);

			return new ObstructionSuperset(bDDNode, mapping, reverse_mapping);
		}

        public static BDDNode GetObstructionSet (Goal goal, BDDManager manager, 
            Dictionary<KAOSCoreElement, int> mapping = null, 
            Dictionary<int, KAOSCoreElement> reverse_mapping = null, bool take_exception_in_account = true)
        {
            if (mapping == null) 
                mapping = new Dictionary<KAOSCoreElement, int> ();

            if (reverse_mapping == null)
                reverse_mapping = new Dictionary<int, KAOSCoreElement> ();

            if ((goal.Refinements ().Count () + goal.Obstructions ().Count ()) > 1)
                throw new NotImplementedException ("More than a refinement and an obstruction");

            var r = goal.Refinements ().SingleOrDefault ();
            var o = goal.Obstructions ().SingleOrDefault ();

			if (take_exception_in_account) {
				var replacement = goal.model.Replacements().Where(x => x.AnchorGoalIdentifier == goal.Identifier);
				if (replacement.Any()) {
					if (replacement.Count() > 1 & replacement.Select(x => x.ResolvingGoal().FriendlyName).Distinct().Count() >= 2) {
						// More than one replacement for different goals. Which one to choose?
						logger.Info("Goal {0} : ", goal.FriendlyName);
						foreach (var r2 in replacement) {
							logger.Info("- " + r2.ResolvingGoal().FriendlyName);
						}
						throw new NotImplementedException("Multiple replacement");

					} else {
						return GetObstructionSet(replacement.First().ResolvingGoal(), manager, mapping, reverse_mapping);
					}
				}
			}

			if (r != null) {
				BDDNode acc = null;
				foreach (var child in goal.Exceptions()) {
					if (acc == null)
						acc = GetObstructionSet(child, manager, mapping, reverse_mapping);
					else {
						var bDDNode = GetObstructionSet(child, manager, mapping, reverse_mapping);
						acc = manager.Or(acc, bDDNode);
					}
				}

				if (acc == null)
					acc = GetObstructionSet(r, manager, mapping, reverse_mapping);
				else {
					var bDDNode = GetObstructionSet(r, manager, mapping, reverse_mapping);
					acc = manager.Or(acc, bDDNode);
				}
				return acc;
			}

            if (o != null)
                return GetObstructionSet (o, manager, mapping, reverse_mapping);

            return manager.Zero;
        }

		public static BDDNode GetObstructionSet(GoalException exception, BDDManager manager,
			Dictionary<KAOSCoreElement, int> mapping,
			Dictionary<int, KAOSCoreElement> reverse_mapping = null)
		{
			var child = exception.ResolvingGoal();
			var bDDNode = GetObstructionSet(child, manager, mapping, reverse_mapping);

			return bDDNode;
		}

        public static BDDNode GetObstructionSet (GoalRefinement refinement, BDDManager manager, 
            Dictionary<KAOSCoreElement, int> mapping, 
            Dictionary<int, KAOSCoreElement> reverse_mapping = null)
        {
            BDDNode acc = null;
            foreach (var child in refinement.SubGoals ()) {
                if (acc == null) 
                    acc = GetObstructionSet (child, manager, mapping, reverse_mapping);
                else {
                    var bDDNode = GetObstructionSet(child, manager, mapping, reverse_mapping);
                    acc = manager.Or(acc, bDDNode);
                }
            }
            foreach (var child in refinement.DomainHypotheses ()) {
                if (acc == null) 
                    acc = GetObstructionSet (child, manager, mapping, reverse_mapping);
                else {
                    var bDDNode = GetObstructionSet(child, manager, mapping, reverse_mapping);
                    acc = manager.Or(acc, bDDNode);
                }
            }
            return acc;
        }

        public static BDDNode GetObstructionSet (Obstruction obstruction, BDDManager manager, 
            Dictionary<KAOSCoreElement, int> mapping, 
            Dictionary<int, KAOSCoreElement> reverse_mapping = null)
        {
            return GetObstructionSet (obstruction.Obstacle (), manager, mapping, reverse_mapping);
        }

        public static BDDNode GetObstructionSet (Obstacle obstacle, BDDManager manager, 
            Dictionary<KAOSCoreElement, int> mapping, 
            Dictionary<int, KAOSCoreElement> reverse_mapping = null)
        {
            BDDNode acc2 = null;
            foreach (var r in obstacle.Refinements ()) {
                BDDNode acc = null;
                foreach (var c in r.SubObstacles ()) {
                    if (acc == null) {
                        acc = GetObstructionSet (c, manager, mapping, reverse_mapping);
                    } else {
                        var bDDNode = GetObstructionSet(c, manager, mapping, reverse_mapping);
                        acc = manager.And(acc, bDDNode);
                    }
                }
                foreach (var c in r.DomainHypotheses ()) {
                    if (acc == null) {
                        acc = GetObstructionSet (c, manager, mapping, reverse_mapping);
                    } else {
                        var bDDNode = GetObstructionSet(c, manager, mapping, reverse_mapping);
                        acc = manager.And(acc, bDDNode);
                    }
                }
                if (acc2 == null) {
                    acc2 = acc;
                } else {
                    acc2 = manager.Or (acc2, acc);
                }
            }

            // Leaf obstacle
            if (acc2 == null) {
                int idx;
                if (mapping.ContainsKey (obstacle)) {
                    idx = mapping[obstacle];
                } else {
                    idx = manager.CreateVariable ();
                    mapping.Add (obstacle, idx);
                    reverse_mapping.Add (idx, obstacle);
                }
                acc2 = manager.Create(idx, manager.One, manager.Zero);
            }

            return acc2;
        }

        public static BDDNode GetObstructionSet (DomainHypothesis domhyp, BDDManager manager, 
            Dictionary<KAOSCoreElement, int> mapping, 
            Dictionary<int, KAOSCoreElement> reverse_mapping = null)
        {
            BDDNode acc2 = null;

            if (acc2 == null) {
                int idx;
                if (mapping.ContainsKey (domhyp)) {
                    idx = mapping[domhyp];
                } else {
                    idx = manager.CreateVariable ();
                    mapping.Add (domhyp, idx);
                    reverse_mapping.Add (idx, domhyp);
                }
                acc2 = manager.Create(idx, manager.One, manager.Zero);
            }

            return acc2;
        }
    }




}

