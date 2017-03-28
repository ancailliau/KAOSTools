using System;
using System.Collections.Generic;
using System.Linq;
using UCLouvain.KAOSTools.Core.SatisfactionRates;

namespace UCLouvain.KAOSTools.Core.Repositories.Memory
{
    public class SatisfactionRateRepository : ISatisfactionRateRepository
    {
        Dictionary<string, List<ISatisfactionRate>> ObstacleSatisfactionRates;
		Dictionary<string, List<ISatisfactionRate>> DomainPropertySatisfactionRates;

        public SatisfactionRateRepository()
        {
			ObstacleSatisfactionRates = new Dictionary<string, List<ISatisfactionRate>>();
			DomainPropertySatisfactionRates = new Dictionary<string, List<ISatisfactionRate>>();
        }

        public void AddDomPropSatisfactionRate(string dompropId, ISatisfactionRate satRate)
		{
            if (!DomainPropertySatisfactionRates.ContainsKey(dompropId)) {
                DomainPropertySatisfactionRates.Add(dompropId, new List<ISatisfactionRate>());
            }
            DomainPropertySatisfactionRates[dompropId].Add (satRate);
        }

        public void AddObstacleSatisfactionRate(string obstacleId, ISatisfactionRate satRate)
		{
			if (!ObstacleSatisfactionRates.ContainsKey(obstacleId)) {
				ObstacleSatisfactionRates.Add(obstacleId, new List<ISatisfactionRate>());
			}
			ObstacleSatisfactionRates[obstacleId].Add(satRate);
        }

        public ISatisfactionRate GetDomPropSatisfactionRate(string dompropId)
		{
            if (DomainPropertySatisfactionRates.ContainsKey(dompropId)) {
                return DomainPropertySatisfactionRates[dompropId].Single ();
            }
            return null;
        }

        public ISatisfactionRate GetObstacleSatisfactionRate(string obstacleId)
        {
            if (ObstacleSatisfactionRates.ContainsKey(obstacleId)) {
				return ObstacleSatisfactionRates[obstacleId].Single();
            }
            return null;
        }

        public IEnumerable<ISatisfactionRate> GetObstacleSatisfactionRates(string obstacleId)
		{
			if (ObstacleSatisfactionRates.ContainsKey(obstacleId)) {
				return ObstacleSatisfactionRates[obstacleId];
			}
			return null;
        }

        public IEnumerable<ISatisfactionRate> GetDomPropSatisfactionRates(string dompropId)
		{
			if (DomainPropertySatisfactionRates.ContainsKey(dompropId)) {
				return DomainPropertySatisfactionRates[dompropId];
			}
			return null;
        }
    }
}
