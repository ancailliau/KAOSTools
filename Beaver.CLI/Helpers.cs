using System;
using System.Linq;
using System.Collections.Generic;
using LtlSharp.Utils;
using KAOSFormalTools.Domain;
using LtlSharp;

namespace KAOSFormalTools.RefinementChecker
{
    public static class Helpers
    {
        private static void AddElementsFor (this IList<string> alphabet, Goal goal)
        {
            alphabet.AddElementsFor (goal.FormalSpec);

            foreach (var refinement in goal.Refinements) 
                foreach (var child in refinement.Children)
                    alphabet.AddElementsFor (child);
        }

        private static void AddElementsFor (this IList<string> alphabet, DomainProperty domprop)
        {
            alphabet.AddElementsFor (domprop.FormalSpec);
        }

        private static void AddElementsFor (this IList<string> alphabet, LTLFormula formula)
        {
            var traversal = new ExtractAlphabet (formula);
            foreach (var element in traversal.Alphabet)
               if (!alphabet.Contains (element)) 
                    alphabet.Add (element);
        }

        public static IList<string> GetAlphabet (this GoalModel model)
        {
            var alphabet = new List<string> ();

            foreach (var goal in model.RootGoals)
                alphabet.AddElementsFor (goal);

            foreach (var domprop in model.DomainProperties)
                alphabet.AddElementsFor (domprop);

            return alphabet;
        }
    }
}

