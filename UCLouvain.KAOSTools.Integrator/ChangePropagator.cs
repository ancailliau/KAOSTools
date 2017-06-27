using System;
using System.Text.RegularExpressions;
using UCLouvain.KAOSTools.Core;
using System.Collections.Generic;
using System.Linq;

namespace UCLouvain.KAOSTools.Integrators
{
    public class ChangePropagator
    {
        KAOSModel _model;
    
        public ChangePropagator (KAOSModel model)
        {
            this._model = model;
        }

        HashSet<string> visited;
        
        public void DownPropagateAddConjunct (Resolution resolution, Formula formula, string name)
        {
            var goal = _model.Goal (resolution.AnchorIdentifier);
            var obstacle = resolution.Obstacle ();
            
            if (goal.FormalSpec != null) {
                visited = new HashSet<string> (new [] { resolution.ResolvingGoalIdentifier });
            
                var antecedant = GetAntecedant (goal.FormalSpec);
                var replacement = new And (antecedant, formula);

                DownPropagateReplacement (goal, antecedant, replacement);
                UpPropagateReplacement (obstacle, antecedant, replacement);
            }


            visited = new HashSet<string> (new [] { resolution.ResolvingGoalIdentifier });
            DownPropagateNameConjunct (goal, name);
            UpPropagateNameConjunct (obstacle, name);
        }
        
        void DownPropagateReplacement (Goal goal, Formula f1, Formula f2)
        {
            if (visited.Contains (goal.Identifier))
                return;
            
            visited.Add (goal.Identifier);
            
            if (goal.FormalSpec != null)
                Replace (goal, f1, f2);
                
            foreach (var r in goal.Refinements ()) {
                foreach (var sg in r.SubGoals ()) {
                    DownPropagateReplacement (sg, f1, f2);
                }
            }
            
            foreach (var r in goal.Obstructions ()) {
                var sg = r.Obstacle ();
                UpPropagateReplacement (sg, f1, f2);
            }
        }

        void DownPropagateNameConjunct (Goal goal, string name)
        {
            if (visited.Contains (goal.Identifier))
                return;
            
            visited.Add (goal.Identifier);

            if (goal.Name != null) {
                var regex = new Regex (@"Achieve([ ]*)\[(.+)When(.+)\]");
                var match = regex.Match (goal.Name);
                if (match.Success) {
                    goal.Name = $"Achieve{match.Groups [1]}[{match.Groups [2]}When{match.Groups [3]} And {name}]";
                } else {
                    regex = new Regex (@"Achieve([ ]*)\[(.+)\]");
                    match = regex.Match (goal.Name);
                    if (match.Success) {
                        goal.Name = $"Achieve{match.Groups [1]}[{match.Groups [2]}When{name}]";
                    }
                }
            }
                
            foreach (var r in goal.Refinements ()) {
                foreach (var sg in r.SubGoals ()) {
                    DownPropagateNameConjunct (sg, name);
                }
            }
            
            foreach (var r in goal.Obstructions ()) {
                var sg = r.Obstacle ();
                UpPropagateNameConjunct (sg, name);
            }
        }

        void UpPropagateReplacement (Obstacle obstacle, Formula f1, Formula f2)
        {
            if (visited.Contains (obstacle.Identifier))
                return;
            
            visited.Add (obstacle.Identifier);
            
            if (obstacle.FormalSpec != null)
                Replace (obstacle, f1, f2);

            foreach (var r in _model.ObstacleRefinements (x => x.SubobstacleIdentifiers.Any(y => y.Identifier == obstacle.Identifier))) {
                UpPropagateReplacement (r.ParentObstacle (), f1, f2);
            }
            
            foreach (var r in obstacle.Resolutions ()) {
                var sg = r.ResolvingGoal ();
                DownPropagateReplacement (sg, f1, f2);
            }
        }

        void UpPropagateNameConjunct (Obstacle obstacle, string name)
        {
            if (visited.Contains (obstacle.Identifier))
                return;
            
            visited.Add (obstacle.Identifier);
            
            obstacle.Name += " And " + name;
                
            foreach (var r in _model.ObstacleRefinements (x => x.SubobstacleIdentifiers.Any(y => y.Identifier == obstacle.Identifier))) {
                UpPropagateNameConjunct (r.ParentObstacle (), name);
            }
            
            foreach (var r in obstacle.Resolutions ()) {
                var sg = r.ResolvingGoal ();
                DownPropagateNameConjunct (sg, name);
            }
        }
        
        void Replace (Obstacle obstacle, Formula f1, Formula f2) 
        {
            obstacle.FormalSpec = Replace (obstacle.FormalSpec, f1, f2);            
        }
        
        void Replace (Goal goal, Formula f1, Formula f2) 
        {
            goal.FormalSpec = Replace (goal.FormalSpec, f1, f2);            
        }
        
        Formula Replace (Formula f, Formula f1, Formula f2) 
        {
            if (f.Equals (f1))
                return f2.Copy();
            
            if (f is Forall fa) {
                fa.Enclosed = Replace (fa, f1, f2);
                return fa;
            } 
            
            if (f is Exists e) {
                e.Enclosed = Replace (e, f1, f2);
                return e;
            }
            
            if (f is StrongImply si)  {
                si.Left = Replace (si.Left, f1, f2);
                si.Right = Replace (si.Right, f1, f2);
                return si;
            }
            
            if (f is Imply i) {
                i.Left = Replace (i.Left, f1, f2);
                i.Right = Replace (i.Right, f1, f2);
                return i;
            }
            
            if (f is Equivalence eq) {
                eq.Left = Replace (eq.Left, f1, f2);
                eq.Right = Replace (eq.Right, f1, f2);
                return eq;
            }
            
            if (f is Until ul) {
                ul.Left = Replace (ul.Left, f1, f2);
                ul.Right = Replace (ul.Right, f1, f2);
                return ul;
            }
            
            if (f is Release r) {
                r.Left = Replace (r.Left, f1, f2);
                r.Right = Replace (r.Right, f1, f2);
                return r;
            }
            
            if (f is Unless us) {
                us.Left = Replace (us.Left, f1, f2);
                us.Right = Replace (us.Right, f1, f2);
                return us;
            }
            
            if (f is And a) {
                a.Left = Replace (a.Left, f1, f2);
                a.Right = Replace (a.Right, f1, f2);
                return a;
            }
            
            if (f is Or o) {
                o.Left = Replace (o.Left, f1, f2);
                o.Right = Replace (o.Right, f1, f2);
                return o;
            }
            
            if (f is Not nt) {
                nt.Enclosed = Replace (nt.Enclosed, f1, f2);
                return nt;
            }
            
            if (f is Next nx) {
                nx.Enclosed = Replace (nx.Enclosed, f1, f2);
                return nx;
            }
            
            if (f is Eventually ev) {
                ev.Enclosed = Replace (ev.Enclosed, f1, f2);
                return ev;
            }
            
            if (f is Globally g) {
                g.Enclosed = Replace (g.Enclosed, f1, f2);
                return g;
            }

            return f;
        }
        
        Formula GetAntecedant (Formula formula)
        {
            if (formula is StrongImply si) {
                return si.Left;
            } else if (formula is Globally g) {
                if (g.Enclosed is Imply i) {
                    return i.Left;
                }
            }
            
            throw new PropagationException (
                string.Format (PropagationException.COULD_NOT_EXTRACT_ANTECEDANT, formula));
        }
        
        Formula GetConsequent (Formula formula)
        {
            if (formula is StrongImply si) {
                return si.Right;
            } else if (formula is Globally g) {
                if (g.Enclosed is Imply i) {
                    return i.Right;
                }
            }
            
            throw new PropagationException (
                string.Format (PropagationException.COULD_NOT_EXTRACT_CONSEQUENT, formula));
        }
    }
}
