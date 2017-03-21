using System;
using System.Collections.Generic;
using KAOSTools.Core;

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
    }
}
