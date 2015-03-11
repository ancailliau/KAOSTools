using System;
using KAOSTools.MetaModel;
using BDDSharp;
using System.Linq;
using System.Collections.Generic;

namespace UncertaintySimulation
{
    static class BDDObstructionSetHelpers {

        public static BDDNode GetObstructionSet (this Goal goal, BDDManager manager, 
            Dictionary<KAOSMetaModelElement, int> mapping = null, 
            Dictionary<int, KAOSMetaModelElement> reverse_mapping = null)
        {
            if (mapping == null) 
                mapping = new Dictionary<KAOSMetaModelElement, int> ();

            if (reverse_mapping == null)
                reverse_mapping = new Dictionary<int, KAOSMetaModelElement> ();

            if ((goal.Refinements ().Count () + goal.Obstructions ().Count ()) > 1)
                throw new NotImplementedException ();

            var r = goal.Refinements ().SingleOrDefault ();
            var o = goal.Obstructions ().SingleOrDefault ();

            if (r != null)
                return r.GetObstructionSet (manager, mapping, reverse_mapping);

            if (o != null)
                return o.GetObstructionSet (manager, mapping, reverse_mapping);

            return manager.Zero;
        }

        public static BDDNode GetObstructionSet (this GoalRefinement refinement, BDDManager manager, 
            Dictionary<KAOSMetaModelElement, int> mapping, 
            Dictionary<int, KAOSMetaModelElement> reverse_mapping = null)
        {
            BDDNode acc = null;
            foreach (var child in refinement.SubGoals ()) {
                if (acc == null) 
                    acc = child.GetObstructionSet (manager, mapping, reverse_mapping);
                else {
                    var bDDNode = child.GetObstructionSet(manager, mapping, reverse_mapping);
                    acc = manager.Or(acc, bDDNode);
                }
            }
            foreach (var child in refinement.DomainHypotheses ()) {
                if (acc == null) 
                    acc = child.GetObstructionSet (manager, mapping, reverse_mapping);
                else {
                    var bDDNode = child.GetObstructionSet(manager, mapping, reverse_mapping);
                    acc = manager.Or(acc, bDDNode);
                }
            }
            return acc;
        }

        public static BDDNode GetObstructionSet (this Obstruction obstruction, BDDManager manager, 
            Dictionary<KAOSMetaModelElement, int> mapping, 
            Dictionary<int, KAOSMetaModelElement> reverse_mapping = null)
        {
            return obstruction.Obstacle ().GetObstructionSet (manager, mapping, reverse_mapping);
        }

        public static BDDNode GetObstructionSet (this Obstacle obstacle, BDDManager manager, 
            Dictionary<KAOSMetaModelElement, int> mapping, 
            Dictionary<int, KAOSMetaModelElement> reverse_mapping = null)
        {
            BDDNode acc2 = null;
            foreach (var r in obstacle.Refinements ()) {
                BDDNode acc = null;
                foreach (var c in r.SubObstacles ()) {
                    if (acc == null) {
                        acc = c.GetObstructionSet (manager, mapping, reverse_mapping);
                    } else {
                        var bDDNode = c.GetObstructionSet(manager, mapping, reverse_mapping);
                        acc = manager.And(acc, bDDNode);
                    }
                }
                foreach (var c in r.DomainHypotheses ()) {
                    if (acc == null) {
                        acc = c.GetObstructionSet (manager, mapping, reverse_mapping);
                    } else {
                        var bDDNode = c.GetObstructionSet(manager, mapping, reverse_mapping);
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

        public static BDDNode GetObstructionSet (this DomainHypothesis domhyp, BDDManager manager, 
            Dictionary<KAOSMetaModelElement, int> mapping, 
            Dictionary<int, KAOSMetaModelElement> reverse_mapping = null)
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

