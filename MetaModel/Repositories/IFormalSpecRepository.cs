using System;
using KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;

namespace UCLouvain.KAOSTools.Core.Repositories
{
    public interface IFormalSpecRepository
	{
        void Add(Predicate goal);
		bool PredicateExists(string identifier);
	}
}
