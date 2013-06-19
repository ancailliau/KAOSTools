using System;
using System.Linq;
using System.Collections.Generic;
using LtlSharp.Utils;
using KAOSTools.MetaModel;
using LtlSharp;

namespace KAOSTools.RefinementChecker
{
    public static class Helpers
    {
        private static void AddElementsFor (this IList<string> alphabet, Goal goal)
        {
            alphabet.AddElementsFor (goal.FormalSpec);
        }

        private static void AddElementsFor (this IList<string> alphabet, DomainProperty domprop)
        {
            alphabet.AddElementsFor (domprop.FormalSpec);
        }
                
        private static void AddElementsFor (this IList<string> alphabet, Obstacle obstacle)
        {
            alphabet.AddElementsFor (obstacle.FormalSpec);
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

            foreach (var goal in model.Goals())
                alphabet.AddElementsFor (goal);

            foreach (var domprop in model.DomainProperties)
                alphabet.AddElementsFor (domprop);

            foreach (var obstacle in model.Obstacles)
                alphabet.AddElementsFor (obstacle);

            return alphabet;
        }
    }
}

