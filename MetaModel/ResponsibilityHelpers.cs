using System;
using System.Linq;
using System.Collections.Generic;

namespace KAOSTools.MetaModel
{
    public static class ResponsibilityHelpers {
        
        public static ResponsibilityNode GetResponsibilities (this KAOSModel model) 
        {
            var node = new ResponsibilityNode (null);
            foreach (var rootGoal in model.RootGoals()) {
                RecursiveGetResponsibilities (node, rootGoal);
            }
            
            Factorize (node);
            Collapse (node);
            
            return node;
        }
        
        private static void Factorize (ResponsibilityNode node) {
            foreach (var childResponsibility in node.children) {
                foreach (var kv in childResponsibility.Responsibility.ToArray ()) {
                    var agent = kv.Key;
                    var goals = kv.Value;
                    
                    // For each goal assigned to an agent
                    foreach (var goal in goals.ToArray ()) {
                        if (node.children.All (x => x.Responsibility.ContainsKey(agent) && x.Responsibility[agent].Contains (goal))) {
                            if (!node.Responsibility.ContainsKey(agent))
                                node.Responsibility.Add (agent, new List<Goal> ());
                            
                            node.Responsibility[agent].Add (goal);
                            
                            foreach (var c in node.children) {
                                c.Responsibility[agent].Remove (goal);
                                if (c.Responsibility[agent].Count == 0)
                                    c.Responsibility.Remove (agent);
                            }
                        }
                    }
                }
            }
            
            // if two alternatives are the same, remove one of the two
            for (int i = 0; i < node.children.Count; i++) {
                var t = node.children[i];
                
                for (int j = i + 1; j < node.children.Count; j++) {
                    var t2 = node.children[j];
                    var eq = true;
                    
                    // All elements in t are in t2
                    foreach (var agent in t.Responsibility.Keys) {
                        if (t2.Responsibility.ContainsKey (agent)) {
                            foreach (var g in t.Responsibility[agent]) {
                                if (!t2.Responsibility[agent].Contains (g)) {
                                    eq = false;
                                }
                            }
                        } else {
                            eq = false;
                        }
                    }
                    
                    // All elements in t2 are in t
                    foreach (var agent in t2.Responsibility.Keys) {
                        if (t.Responsibility.ContainsKey (agent)) {
                            foreach (var g in t2.Responsibility[agent]) {
                                if (!t.Responsibility[agent].Contains (g)) {
                                    eq = false;
                                }
                            }
                        } else {
                            eq = false;
                        }
                    }
                    
                    if (eq)
                        node.children.Remove (t2);
                }
            }
        }
        
        private static void Collapse (ResponsibilityNode r) {
            if (r.Responsibility.Count () == 0 && r.parent != null) {
                r.parent.children.Remove (r);
                
                foreach (var r2 in r.children) {
                    r2.parent = r.parent;
                    r.parent.children.Add (r2);
                }            
            }
            
            foreach (var r2 in r.children.ToArray ()) {
                Collapse (r2);
            }
        }
        
        private static void RecursiveGetResponsibilities (ResponsibilityNode current, Goal goal)
        {
            var hasAlternatives = (goal.Refinements().Count() + goal.AgentAssignments().Count()) > 1;
            
            if (hasAlternatives) {
                foreach (var refinement in goal.Refinements()) {
                    var newNode = new ResponsibilityNode (current);
                    foreach (var childGoal in refinement.Subgoals) 
                        RecursiveGetResponsibilities (newNode, childGoal);
                }
                
                foreach (var assignment in goal.AgentAssignments()) {
                    foreach (var agent in assignment.Agents()) {
                        var newNode = new ResponsibilityNode (current);
                        
                        if (!newNode.Responsibility.ContainsKey(agent))
                            newNode.Responsibility.Add (agent, new List<Goal> ());
                        newNode.Responsibility[agent].Add (goal);
                    }
                }
                
            } else {
                foreach (var refinement in goal.Refinements()) {
                    foreach (var childGoal in refinement.Subgoals) 
                        RecursiveGetResponsibilities (current, childGoal);
                }
                
                foreach (var assignment in goal.AgentAssignments()) {
                    foreach (var agent in assignment.Agents()) {
                        if (!current.Responsibility.ContainsKey(agent))
                            current.Responsibility.Add (agent, new List<Goal> ());
                        current.Responsibility[agent].Add (goal);
                    }
                }
            }
        }
        
        
    }
    
    public class ResponsibilityNode {
        public IDictionary<Agent, IList<Goal>> Responsibility { get; set; }
        
        public ResponsibilityNode parent;
        public IList<ResponsibilityNode> children;
        
        public ResponsibilityNode (ResponsibilityNode parent)
        {
            if (parent != null) {
                this.parent = parent;
                this.parent.children.Add (this);
            }
            
            this.children = new List<ResponsibilityNode> ();
            Responsibility = new Dictionary<Agent, IList<Goal>>();
        }
    }
}

