using System;
using System.Collections.Generic;
using KAOSTools.Core;

namespace UCLouvain.KAOSTools.Core.Repositories.Memory
{
    public class FormalSpecRepository : IFormalSpecRepository
	{
        IDictionary<string, Predicate> Predicates;

        public FormalSpecRepository()
        {
            Predicates = new Dictionary<string, Predicate>();
        }

        public void Add(Predicate predicate)
		{
			if (Predicates.ContainsKey(predicate.Identifier))
			{
				throw new ArgumentException(string.Format("Predicate identifier already exist: {0}", predicate.Identifier));
			}

			Predicates.Add(predicate.Identifier, predicate);
        }

        public bool PredicateExists(string identifier)
        {
            return Predicates.ContainsKey(identifier);
		}

		public Predicate GetPredicate(string identifier)
		{
            return Predicates.ContainsKey(identifier) ? Predicates[identifier] : null;
		}
    }
}
