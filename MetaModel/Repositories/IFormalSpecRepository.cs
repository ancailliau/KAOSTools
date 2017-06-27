using System;
using System.Collections.Generic;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;

namespace UCLouvain.KAOSTools.Core.Repositories
{
    public interface IFormalSpecRepository
	{
        void Add(Predicate goal);
		bool PredicateExists(string identifier);
		Predicate GetPredicate(string identifier);

		Predicate GetPredicate(Predicate<Predicate> predicate);
		IEnumerable<Predicate> GetPredicates();
		IEnumerable<Predicate> GetPredicates(Predicate<Predicate> predicate);
	}
}
