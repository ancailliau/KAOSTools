using System;
using System.Collections.Generic;
using UCLouvain.KAOSTools.Core;
using System.Linq;

namespace UCLouvain.KAOSTools.Core.Repositories.Memory
{
    public class DomainRepository : IDomainRepository
	{
		IDictionary<string, DomainProperty> DomainProperties;
		IDictionary<string, DomainHypothesis> DomainHypotheses;

        public DomainRepository()
        {
            DomainProperties = new Dictionary<string, DomainProperty>();
            DomainHypotheses = new Dictionary<string, DomainHypothesis>();
        }

        public void Add(DomainHypothesis domHyp)
        {
			if (DomainHypotheses.ContainsKey(domHyp.Identifier))
			{
				throw new ArgumentException(string.Format("Domain hypothesis identifier already exist: {0}", domHyp.Identifier));
			}

			DomainHypotheses.Add(domHyp.Identifier, domHyp);
        }

        public void Add(DomainProperty domProp)
		{
			if (DomainProperties.ContainsKey(domProp.Identifier))
			{
				throw new ArgumentException(string.Format("Domain property identifier already exist: {0}", domProp.Identifier));
			}

			DomainProperties.Add(domProp.Identifier, domProp);
        }

        public bool DomainHypothesisExists(string identifier)
        {
            return DomainHypotheses.ContainsKey(identifier);
        }

        public bool DomainPropertyExists(string identifier)
		{
            return DomainProperties.ContainsKey(identifier);
        }

        public DomainProperty GetDomainProperty(string identifier)
        {
            return DomainProperties.ContainsKey(identifier) ? DomainProperties[identifier] : null;
        }

        public DomainHypothesis GetDomainHypothesis(string identifier)
		{
			return DomainHypotheses.ContainsKey(identifier) ? DomainHypotheses[identifier] : null;
        }

        public DomainProperty GetDomainProperty(Predicate<DomainProperty> predicate)
        {
            return DomainProperties.Values.SingleOrDefault(x => predicate(x));
        }

        public DomainHypothesis GetDomainHypothesis(Predicate<DomainHypothesis> predicate)
		{
            return DomainHypotheses.Values.SingleOrDefault(x => predicate(x));
        }

        public IEnumerable<DomainProperty> GetDomainProperties()
		{
			return DomainProperties.Values;
        }

        public IEnumerable<DomainHypothesis> GetDomainHypotheses()
		{
			return DomainHypotheses.Values;
        }

        public IEnumerable<DomainProperty> GetDomainProperties(Predicate<DomainProperty> predicate)
		{
            return DomainProperties.Values.Where(x => predicate(x));
        }

        public IEnumerable<DomainHypothesis> GetDomainHypotheses(Predicate<DomainHypothesis> predicate)
		{
			return DomainHypotheses.Values.Where(x => predicate(x));
        }
    }
}
