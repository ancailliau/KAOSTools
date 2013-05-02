using System;
using System.Linq;
using System.Collections.Generic;

namespace KAOSTools.MetaModel
{
    public class AlternativeHelpers
    {
        private GoalModel model;

        public void ComputeInAlternatives (GoalModel model)
        {
            this.model = model;

            foreach (var g in model.RootGoals) {
                g.InSystems = new HashSet<AlternativeSystem> (model.Systems);
                foreach (var r in g.Refinements) {
                    DownPropagate (g, r);
                }
            }

            foreach (var g in model.Goals) {
                foreach (var a in g.AgentAssignments) {
                    DownPropagate (g, a);
                }
            }

            foreach (var g in model.RootGoals)
                Simplify (g);
        }

        private void Simplify (Goal g) {
            g.InSystems = Simplify (g.InSystems);
            foreach (var refinement in g.Refinements) {
                refinement.InSystems = Simplify(refinement.InSystems);
                foreach (var child in refinement.Subgoals) {
                    Simplify (child);
                }
            }

            foreach (var assignement in g.AgentAssignments) {
                assignement.InSystems = Simplify (assignement.InSystems);
                foreach (var agent in assignement.Agents) {
                    agent.InSystems = Simplify (agent.InSystems);
                }
            }
        }

        private ISet<AlternativeSystem> Simplify (ISet<AlternativeSystem> systems)
        {
            var result = new HashSet<AlternativeSystem> (systems);
            foreach (var s in model.Systems) {
                bool contains_all = true;
                foreach (var ss in s.Alternatives) {
                    if (!result.Contains(ss)) {
                        contains_all = false;
                        break;
                    }
                }

                if (contains_all && s.Alternatives.Count > 0) {
                    foreach (var ss in s.Alternatives) {
                        result.Remove (ss);
                    }
                    
                    // Make sure that s is in the set
                    // it might not be the case if we have systems = {a,b}
                    // but there exists an alternative system c which is such that 
                    // begin system id c alternative a alternative b end
                    result.Add (s);
                }
            }
            return result;
        }

        private ISet<AlternativeSystem> GetAllSubsystems (ISet<AlternativeSystem> systems, AlternativeSystem system)
        {
            systems.Add (system);
            foreach (var s in system.Alternatives) {
                GetAllSubsystems (systems, s);
            }
            return systems;
        }

        private void DownPropagate (Goal parent, AgentAssignment assignment)
        {
            if (assignment.InSystems == null)
                assignment.InSystems = new HashSet<AlternativeSystem> ();

            IEnumerable<AlternativeSystem> systems_to_add;
            if (assignment.SystemReference != null) {
                systems_to_add = parent.InSystems.Where (a => a.Equals(assignment.SystemReference));
            } else {
                systems_to_add = parent.InSystems;
            }

            foreach (var s in systems_to_add) {
                assignment.InSystems.Add (s);
            }

            foreach (var agent in assignment.Agents) {
                if (agent.InSystems == null)
                    agent.InSystems = new HashSet<AlternativeSystem> ();
                
                foreach (var a in systems_to_add) {
                    agent.InSystems.Add (a);
                }
            }
        }

        private void DownPropagate (Goal parent, GoalRefinement refinement)
        {
            IList<AlternativeSystem> alternatives_to_add;
            if (refinement.SystemReference != null) {
                var alternatives_to_filter_on = GetAllSubsystems(new HashSet<AlternativeSystem> (), refinement.SystemReference);
                alternatives_to_add = new List<AlternativeSystem> (parent.InSystems.Where(r => alternatives_to_filter_on.Contains(r)));
            
            } else {
                alternatives_to_add = new List<AlternativeSystem> (parent.InSystems);
            }

            if (refinement.InSystems == null)
                refinement.InSystems = new HashSet<AlternativeSystem> ();

            foreach (var a in alternatives_to_add) {
                refinement.InSystems.Add (a);
            }

            foreach (var child in refinement.Subgoals) {
                if (child.InSystems == null)
                    child.InSystems = new HashSet<AlternativeSystem> ();

                foreach (var a in alternatives_to_add) {
                   child.InSystems.Add (a);
                }

                foreach (var childRefinement in child.Refinements) {
                    DownPropagate (child, childRefinement);
                }
            }
        }
    }
}

