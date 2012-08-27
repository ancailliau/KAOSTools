using KAOSFormalTools.Domain;
using LtlSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KAOSFormalTools.RefinementChecker
{
    public class ProofObligation
    {
        public LTLFormula Formula          { get; set; }
        public bool       ExpectedResult   { get; set; }

        public string     SuccessMessage   { get; set; }
        public string     FailureMessage   { get; set; }
    }

    public class ProofObligationGenerator {

        private LTLFormula PrefixWithDomainProperties (LTLFormula formula)
        {
            if (domainproperties != null)
                return new LtlSharp.Implication (domainproperties, formula);
            else 
                return formula;
        }

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
                    SuccessMessage = string.Format ("Refinement '{0}' for '{1}' is minimal regarding goal '{2}'",  string.Join (", ", names), parent.Name, child.Name)
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

        void AddProofObligationsForObstructedGoal (Goal goal)
        {
            foreach (var obstacle in goal.Obstruction) {
                if (obstacle.FormalSpec != null)
                    list.Add (new ProofObligation () {
                        Formula        = PrefixWithDomainProperties (new LtlSharp.Implication (obstacle.FormalSpec, new LtlSharp.Negation(goal.FormalSpec))),
                        ExpectedResult = true,
                        FailureMessage = string.Format ("Obstacle '{0}' does not obstruct '{1}'", obstacle.Name, goal.Name),         
                        SuccessMessage = string.Format ("Obstacle '{0}' obstructs '{1}'",         obstacle.Name, goal.Name)         
                    });
                else
                    Console.WriteLine ("Missing formal specification for obstacle '{0}'", obstacle.Name);
            }
        }

        private GoalModel model;
        private List<ProofObligation> list;
        private LTLFormula domainproperties;

        public List<ProofObligation> Obligations {
            get { return list; }
        }

        public ProofObligationGenerator (GoalModel model)
        {
            this.model = model;
            this.list = new List<ProofObligation> ();

            if (model.DomainProperties.Count > 0) {
                var conj = new List<LTLFormula> ();
                foreach (var domprop in model.DomainProperties) {
                    conj.Add (domprop.FormalSpec);
                }
                domainproperties = new Conjunction(conj.ToArray ());

                list.Add (new ProofObligation () {
                    Formula        = new LtlSharp.Negation(domainproperties),
                    ExpectedResult = false,
                    FailureMessage = string.Format ("Domain properties are inconsistent"),         
                    SuccessMessage = string.Format ("Domain properties are consistent")
                });
            }

            foreach (var root in model.RootGoals)
                AddProofObligationsForGoal (root);

            foreach (var obstructedGoal in model.ObstructedGoals)
                AddProofObligationsForObstructedGoal (obstructedGoal);
        }

    }
}

