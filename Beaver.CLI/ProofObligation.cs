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

    public static class ProofObligationHelpers {

        private static void AddCompletenessProofObligation (this IList<ProofObligation> list,
                                                            Goal parent,
                                                            GoalRefinement refinement)
        {
            var specifications = from child in refinement.Children select child.FormalSpec;
            var names = from child in refinement.Children select child.Name;

            list.Add (new ProofObligation () {
                Formula        = new LtlSharp.Implication (new LtlSharp.Conjunction (specifications.ToArray ()), parent.FormalSpec),
                ExpectedResult = true,
                FailureMessage = string.Format ("Refinement '{0}' for '{1}' is not complete", string.Join (", ", names), parent.Name),
                SuccessMessage = string.Format ("Refinement '{0}' for '{1}' is complete",     string.Join (", ", names), parent.Name)
            });
        }
        
        private static void AddMinimalityProofObligation (this IList<ProofObligation> list, 
                                                          Goal parent,
                                                          GoalRefinement refinement)
        {
            var names = from child in refinement.Children select child.Name;

            foreach (var child in refinement.Children) {
                var conjunction = new LtlSharp.Conjunction ();
                foreach (var otherChild in refinement.Children.Where(c => c != child)) {
                    conjunction.Push (otherChild.FormalSpec);
                }

                list.Add (new ProofObligation () {
                    Formula        = new LtlSharp.Implication (conjunction, parent.FormalSpec),
                    ExpectedResult = false,
                    FailureMessage = string.Format ("Refinement '{0}' for '{1}' is not minimal",                   string.Join (", ", names), parent.Name),
                    SuccessMessage = string.Format ("Refinement '{0}' for '{1}' is minimal regarding goal '{2}'",  string.Join (", ", names), parent.Name, child.Name)
                });
            }
        }
        
        private static void AddConsistencyProofObligation (this IList<ProofObligation> list, 
                                                           Goal parent,
                                                           GoalRefinement refinement)
        {
            var specifications = from child in refinement.Children select child.FormalSpec;
            var names = from child in refinement.Children select child.Name;

            list.Add (new ProofObligation () {
                Formula        = new LtlSharp.Negation (new LtlSharp.Conjunction (specifications.ToArray ())),
                ExpectedResult = false,
                FailureMessage = string.Format ("Refinement '{0}' for '{1}' is not consistent", string.Join (", ", names), parent.Name),         
                SuccessMessage = string.Format ("Refinement '{0}' for '{1}' is consistent",     string.Join (", ", names), parent.Name)         
            });
        }

        private static void AddProofObligationsForGoal (this IList<ProofObligation> list, Goal goal)
        {
            foreach (var refinement in goal.Refinements) {
                list.AddCompletenessProofObligation (goal, refinement);
                list.AddMinimalityProofObligation   (goal, refinement);
                list.AddConsistencyProofObligation  (goal, refinement);

                foreach (var child in refinement.Children)
                    list.AddProofObligationsForGoal (child);
            }
        }

        public static IList<ProofObligation> GetProofObligations (this GoalModel model)
        {
            var list = new List<ProofObligation> ();
            foreach (var root in model.RootGoals)
                list.AddProofObligationsForGoal (root);

            return list;
        }

    }
}

