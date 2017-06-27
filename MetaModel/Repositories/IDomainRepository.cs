using System;
using System.Collections.Generic;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;

namespace UCLouvain.KAOSTools.Core.Repositories
{

	public interface IDomainRepository
	{
		void Add(DomainProperty goal);
		void Add(DomainHypothesis goal);

		bool DomainPropertyExists(string identifier);
		bool DomainHypothesisExists(string identifier);

		DomainProperty GetDomainProperty(string identifier);
		DomainHypothesis GetDomainHypothesis(string identifier);

		DomainProperty GetDomainProperty(Predicate<DomainProperty> predicate);
		DomainHypothesis GetDomainHypothesis(Predicate<DomainHypothesis> predicate);

		IEnumerable<DomainProperty> GetDomainProperties();
		IEnumerable<DomainHypothesis> GetDomainHypotheses();

		IEnumerable<DomainProperty> GetDomainProperties(Predicate<DomainProperty> predicate);
		IEnumerable<DomainHypothesis> GetDomainHypotheses(Predicate<DomainHypothesis> predicate);
	}
    
}
