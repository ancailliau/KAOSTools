using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using LtlSharp.Utils;

namespace KAOSFormalTools.Domain
{
    public class GoalModel
    {
        public IList<Goal>           Goals             { get; set; }
        public IList<DomainProperty> DomainProperties  { get; private set; }
        public IList<Obstacle>       Obstacles         { get; set; }
        public IList<Agent>          Agents            { get; set; }

        public IList<Goal>           RootGoals { 
            get {
                var goals = new List<Goal> (Goals);
                foreach (var goal in Goals)
                    foreach (var refinement in goal.Refinements) 
                        foreach (var child in refinement.Children)
                            goals.Remove (child);
                foreach (var obstacle in Obstacles)
                    foreach (var resolution in obstacle.Resolutions)
                        goals.Remove (resolution);
                return goals;
            }
        }

        public IEnumerable<Goal>           ObstructedGoals {
            get {
                return from g in Goals where g.Obstruction.Count > 0 select g;
            }
        }

        public IEnumerable<Obstacle>       ResolvedObstacles {
            get {
                return from o in Obstacles where o.Resolutions.Count > 0 select o;
            }
        }
        
        public GoalModel ()
        {
            Goals             = new List<Goal> ();
            DomainProperties  = new List<DomainProperty> ();
            Obstacles         = new List<Obstacle> ();
            Agents            = new List<Agent> ();
        }

        public Goal GetGoalByIdentifier (string identifier)
        {
            return Goals.Where (x => x.Identifier == identifier).SingleOrDefault ();
        }

        public IEnumerable<Goal> GetGoalsByName (string name)
        {
            return Goals.Where (x => x.Name == name);
        }

        public bool GoalExists (string identifier)
        {
            return Goals.Where (x => x.Identifier == identifier).Count () > 0;
        }
        
        public void ReplaceGoal (Goal g1, Goal g2)
        {
            Goals.Remove (g1);
            Goals.Add (g2);

            foreach (var refinement in Goals.SelectMany (g => g.Refinements).Where (r => r.Children.Contains (g1))) {
                refinement.Children.Remove (g1);
                refinement.Children.Add (g2);
            }

            foreach (var obstacle in Obstacles.Where (o => o.Resolutions.Contains (g1))) {
                obstacle.Resolutions.Remove (g1);
                obstacle.Resolutions.Add (g2);
            }
        }

        public Obstacle GetObstacleByIdentifier (string identifier)
        {
            return Obstacles.Where (x => x.Identifier == identifier).SingleOrDefault ();
        }

        public IEnumerable<Obstacle> GetObstaclesByName (string name)
        {
            return Obstacles.Where (x => x.Name == name);
        }

        public bool ObstacleExists (string identifier)
        {
            return Obstacles.Where (x => x.Identifier == identifier).Count () > 0;
        }

        public DomainProperty GetDomainPropertyByIdentifier (string identifier)
        {
            return DomainProperties.Where (x => x.Identifier == identifier).SingleOrDefault ();
        }

        public IEnumerable<DomainProperty> GetDomainPropertiesByName (string name)
        {
            return DomainProperties.Where (x => x.Name == name);
        }

        public bool DomainPropertyExists (string identifier)
        {
            return DomainProperties.Where (x => x.Identifier == identifier).Count () > 0;
        }

        public Agent GetAgentByIdentifier (string identifier)
        {
            return Agents.Where (x => x.Identifier == identifier).SingleOrDefault ();
        }

        public IEnumerable<Agent> GetAgentsByName (string name)
        {
            return Agents.Where (x => x.Name == name);
        }

        public bool AgentExists (string identifier)
        {
            return Agents.Where (x => x.Identifier == identifier).Count () > 0;
        }

        public ResponsibilityNode GetResponsibilities () 
        {
            var node = new ResponsibilityNode (null);
            foreach (var rootGoal in RootGoals) {
                _get_responsibilities (node, rootGoal);
            }

            Factorize (node);
            Collapse (node);

            return node;
        }

        private void Factorize (ResponsibilityNode node) {
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

        private void Collapse (ResponsibilityNode r) {
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

        private void _get_responsibilities (ResponsibilityNode current, Goal goal)
        {
            var hasAlternatives = (goal.Refinements.Count + goal.AssignedAgents.Count) > 1;
           
            if (hasAlternatives) {
                foreach (var refinement in goal.Refinements) {
                    var newNode = new ResponsibilityNode (current);
                    foreach (var childGoal in refinement.Children) 
                        _get_responsibilities (newNode, childGoal);
                }

                foreach (var agent in goal.AssignedAgents) {
                    var newNode = new ResponsibilityNode (current);
                    
                    if (!newNode.Responsibility.ContainsKey(agent))
                        newNode.Responsibility.Add (agent, new List<Goal> ());
                    newNode.Responsibility[agent].Add (goal);
                }

            } else {
                foreach (var refinement in goal.Refinements) {
                    foreach (var childGoal in refinement.Children) 
                        _get_responsibilities (current, childGoal);
                }
                foreach (var agent in goal.AssignedAgents) {
                    if (!current.Responsibility.ContainsKey(agent))
                        current.Responsibility.Add (agent, new List<Goal> ());
                    current.Responsibility[agent].Add (goal);
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

