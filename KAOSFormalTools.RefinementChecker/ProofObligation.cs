using KAOSTools.MetaModel;
using LtlSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using LtlSharp.Utils;

namespace KAOSFormalTools.RefinementChecker
{
    public class ProofObligation
    {
        public LTLFormula Formula          { get; set; }
        public bool       ExpectedResult   { get; set; }

        public string     SuccessMessage   { get; set; }
        public string     FailureMessage   { get; set; }

        public bool       Critical         { get; set; }

        public ProofObligation ()
        {
            Critical = true;
        }
    }

    public class ProofObligationGenerator {

        private LTLFormula PrefixWithDomainProperties (LTLFormula formula)
        {
            var alphabetForFormula = new List<string> ();
            AddToAlphabet (alphabetForFormula, formula);

            var relevantdomprops = new List<DomainProperty> ();

            bool changed = true;
            while (changed) {
                changed = false;

                foreach (var domprop in model.DomainProperties) {
                    if (relevantdomprops.Contains (domprop)) {
                        continue;
                    }

                    var alphabetForDomProp = new ExtractAlphabet (domprop.FormalSpec).Alphabet;
                    if (alphabetForDomProp.Intersect (alphabetForFormula).Count () > 0) {
                        relevantdomprops.Add (domprop);
                        alphabetForFormula.AddRange (alphabetForDomProp.Except (alphabetForFormula));
                    }
                }
            }

            if (relevantdomprops.Count() > 0)
                return new LtlSharp.Implication (new LtlSharp.Conjunction(relevantdomprops.Select (x => x.FormalSpec).ToArray()), formula);
            else 
                return formula;
        }

        public IList<string> GetAlphabet ()
        {
            var alphabet = new List<string> ();

            foreach (var po in list)
                AddToAlphabet(alphabet, po.Formula);

            return alphabet;
        }

        private void AddToAlphabet (IList<string> alphabet, LTLFormula formula)
        {
            var traversal = new ExtractAlphabet (formula);
            foreach (var element in traversal.Alphabet)
               if (!alphabet.Contains (element)) 
                    alphabet.Add (element);
        }

        #region Goal refinement

        private void AddCompletenessProofObligation (Goal parent, GoalRefinement refinement)
        {
            var specifications = from child in refinement.Children select child.FormalSpec;
            var names = from child in refinement.Children select child.Name;

            list.Add (new ProofObligation () {
                Formula        = PrefixWithDomainProperties (new LtlSharp.Implication (new LtlSharp.Conjunction (specifications.ToArray ()), parent.FormalSpec)),
                ExpectedResult = true,
                FailureMessage = string.Format ("Refinement '{0}' for '{1}' is not complete", string.Join (", ", names), parent.Name),
                SuccessMessage = string.Format ("Refinement '{0}' for '{1}' is complete",     string.Join (", ", names), parent.Name)
            });
        }
        
        private void AddMinimalityProofObligation (Goal parent, GoalRefinement refinement)
        {
            var names = from child in refinement.Children select child.Name;
            if (refinement.Children.Count == 1)
                return;

            foreach (var child in refinement.Children) {
                var conjunction = new LtlSharp.Conjunction ();
                foreach (var otherChild in refinement.Children.Where(c => c != child)) {
                    conjunction.Push (otherChild.FormalSpec);
                }

                list.Add (new ProofObligation () {
                    Formula        = PrefixWithDomainProperties (new LtlSharp.Implication (conjunction, parent.FormalSpec)),
                    ExpectedResult = false,
                    FailureMessage = string.Format ("Refinement '{0}' for '{1}' is not minimal",                   string.Join (", ", names), parent.Name),
                    SuccessMessage = string.Format ("Refinement '{0}' for '{1}' is minimal regarding goal '{2}'",  string.Join (", ", names), parent.Name, child.Name),
                    Critical       = false
                });
            }
        }
        
        private void AddConsistencyProofObligation (Goal parent, GoalRefinement refinement)
        {
            var specifications = from child in refinement.Children select child.FormalSpec;
            var names = from child in refinement.Children select child.Name;

            list.Add (new ProofObligation () {
                Formula        = PrefixWithDomainProperties (new LtlSharp.Negation (new LtlSharp.Conjunction (specifications.ToArray ()))),
                ExpectedResult = false,
                FailureMessage = string.Format ("Refinement '{0}' for '{1}' is not consistent", string.Join (", ", names), parent.Name),         
                SuccessMessage = string.Format ("Refinement '{0}' for '{1}' is consistent",     string.Join (", ", names), parent.Name)         
            });
        }

        #endregion

        #region Obstacle refinement

        private void AddCompletenessProofObligation (Obstacle parent, ObstacleRefinement refinement)
        {
            var specifications = from child in refinement.Children select child.FormalSpec;
            var names = from child in refinement.Children select child.Name;

            list.Add (new ProofObligation () {
                Formula        = PrefixWithDomainProperties (new LtlSharp.Implication (new LtlSharp.Conjunction (specifications.ToArray ()), parent.FormalSpec)),
                ExpectedResult = true,
                FailureMessage = string.Format ("Obstacle refinement '{0}' for '{1}' is not complete", string.Join (", ", names), parent.Name),
                SuccessMessage = string.Format ("Obstacle refinement '{0}' for '{1}' is complete",     string.Join (", ", names), parent.Name)
            });
        }
        
        private void AddMinimalityProofObligation (Obstacle parent, ObstacleRefinement refinement)
        {
            var names = from child in refinement.Children select child.Name;
            if (refinement.Children.Count == 1)
                return;

            foreach (var child in refinement.Children) {
                var conjunction = new LtlSharp.Conjunction ();
                foreach (var otherChild in refinement.Children.Where(c => c != child)) {
                    conjunction.Push (otherChild.FormalSpec);
                }

                list.Add (new ProofObligation () {
                    Formula        = PrefixWithDomainProperties (new LtlSharp.Implication (conjunction, parent.FormalSpec)),
                    ExpectedResult = false,
                    FailureMessage = string.Format ("Obstacle refinement '{0}' for '{1}' is not minimal",                   string.Join (", ", names), parent.Name),
                    SuccessMessage = string.Format ("Obstacle refinement '{0}' for '{1}' is minimal regarding goal '{2}'",  string.Join (", ", names), parent.Name, child.Name),
                    Critical       = false
                });
            }
        }
        
        private void AddConsistencyProofObligation (Obstacle parent, ObstacleRefinement refinement)
        {
            var specifications = from child in refinement.Children select child.FormalSpec;
            var names = from child in refinement.Children select child.Name;

            list.Add (new ProofObligation () {
                Formula        = PrefixWithDomainProperties (new LtlSharp.Negation (new LtlSharp.Conjunction (specifications.ToArray ()))),
                ExpectedResult = false,
                FailureMessage = string.Format ("Obstacle refinement '{0}' for '{1}' is not consistent", string.Join (", ", names), parent.Name),         
                SuccessMessage = string.Format ("Obstacle refinement '{0}' for '{1}' is consistent",     string.Join (", ", names), parent.Name)         
            });
        }

        #endregion

        private void AddProofObligationsForGoal (Goal goal)
        {
            foreach (var refinement in goal.Refinements) {
                AddCompletenessProofObligation (goal, refinement);
                AddMinimalityProofObligation   (goal, refinement);
                AddConsistencyProofObligation  (goal, refinement);

                foreach (var child in refinement.Children)
                    AddProofObligationsForGoal (child);
            }
        }

        private void AddProofObligationsForObstacle (Obstacle obstacle)
        {
            if (obstacle.FormalSpec == null)
                Console.WriteLine ("Missing formal spec for '{0}'", obstacle.Name);

            var specifications = (from r in obstacle.Refinements from c in r.Children select new LtlSharp.Negation(c.FormalSpec));

            if (specifications.Count () > 0) {
                list.Add (new ProofObligation () {
                    Formula        = PrefixWithDomainProperties (new LtlSharp.Implication (new LtlSharp.Conjunction (specifications.ToArray ()), new LtlSharp.Negation(obstacle.FormalSpec))),
                    ExpectedResult = true,
                    FailureMessage = string.Format ("Obstacle refinement '{0}' is not domain complete", obstacle.Name),         
                    SuccessMessage = string.Format ("Obstacle refinement '{0}' is domain complete",     obstacle.Name),
                    Critical       = false
                });
            }

            foreach (var refinement in obstacle.Refinements) {
                AddCompletenessProofObligation (obstacle, refinement);
                AddMinimalityProofObligation   (obstacle, refinement);
                AddConsistencyProofObligation  (obstacle, refinement);

                foreach (var child in refinement.Children)
                    AddProofObligationsForObstacle (child);
            }
        }


        private void AddProofObligationsForObstructedGoal (Goal goal)
        {
            foreach (var obstacle in goal.Obstruction) {
                if (obstacle.FormalSpec != null) {
                    list.Add (new ProofObligation () {
                        Formula        = PrefixWithDomainProperties (new LtlSharp.Implication (obstacle.FormalSpec, new LtlSharp.Negation(goal.FormalSpec))),
                        ExpectedResult = true,
                        FailureMessage = string.Format ("Obstacle '{0}' does not obstruct '{1}'", obstacle.Name, goal.Name),         
                        SuccessMessage = string.Format ("Obstacle '{0}' obstructs '{1}'",         obstacle.Name, goal.Name)         
                    });

                    AddProofObligationsForObstacle (obstacle);
                } else {
                    Console.WriteLine ("Missing formal specification for obstacle '{0}'", obstacle.Name);
                }
            }
        }

        private GoalModel model;
        private List<ProofObligation> list;

        public List<ProofObligation> Obligations {
            get { return list; }
        }

        public ProofObligationGenerator (GoalModel model, IEnumerable<Goal> goals, IEnumerable<Goal> obstructedGoals)
        {
            this.model = model;
            this.list = new List<ProofObligation> ();

            /*
            if (model.DomainProperties.Count > 0) {
                var conj = new List<LTLFormula> ();
                foreach (var domprop in model.DomainProperties) {
                    conj.Add (domprop.FormalSpec);
                }
                var f = new Conjunction(conj.ToArray ());

                list.Add (new ProofObligation () {
                    Formula        = new LtlSharp.Negation(f),
                    ExpectedResult = false,
                    FailureMessage = string.Format ("Domain properties are inconsistent"),         
                    SuccessMessage = string.Format ("Domain properties are consistent")
                });
            }
            */

            if (goals != null)
                foreach (var root in goals)
                    AddProofObligationsForGoal (root);

            if (obstructedGoals != null)
                foreach (var obstructedGoal in obstructedGoals)
                    AddProofObligationsForObstructedGoal (obstructedGoal);
        }

        public ProofObligationGenerator (GoalModel model)
            : this(model, model.RootGoals, model.ObstructedGoals)
        {}
    }
}

