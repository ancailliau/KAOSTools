using System;
using KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;

namespace UCLouvain.KAOSTools.Core.Repositories
{

	public interface IDomainRepository
	{
		void Add(DomainProperty goal);
		void Add(DomainHypothesis goal);
		bool DomainPropertyExists(string identifier);
		bool DomainHypothesisExists(string identifier);
	}
    
}
