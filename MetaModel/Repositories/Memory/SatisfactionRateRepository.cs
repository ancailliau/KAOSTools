using System;
using System.Collections.Generic;
using UCLouvain.KAOSTools.Core.SatisfactionRates;

namespace UCLouvain.KAOSTools.Core.Repositories.Memory
{
    public class SatisfactionRateRepository : ISatisfactionRateRepository
    {
		Dictionary<string, ISatisfactionRate> ObstacleSatisfactionRates;
		Dictionary<string, ISatisfactionRate> DomainPropertySatisfactionRates;

        public SatisfactionRateRepository()
        {
			ObstacleSatisfactionRates = new Dictionary<string, ISatisfactionRate>();
			DomainPropertySatisfactionRates = new Dictionary<string, ISatisfactionRate>();
        }

        public void AddDomPropSatisfactionRate(string dompropId, ISatisfactionRate satRate)
		{
			DomainPropertySatisfactionRates.Add(dompropId, satRate);
        }

        public void AddObstacleSatisfactionRate(string obstacleId, ISatisfactionRate satRate)
        {
			ObstacleSatisfactionRates.Add(obstacleId, satRate);
        }

        public ISatisfactionRate GetDomPropSatisfactionRate(string dompropId)
		{
            if (DomainPropertySatisfactionRates.ContainsKey(dompropId)) {
				return DomainPropertySatisfactionRates[dompropId];
            }
            return null;
        }

        public ISatisfactionRate GetObstacleSatisfactionRate(string obstacleId)
        {
            if (ObstacleSatisfactionRates.ContainsKey(obstacleId)) {
                return ObstacleSatisfactionRates[obstacleId];
            }
            return null;
        }
    }
}
