using System;
using System.Collections.Generic;
using UCLouvain.KAOSTools.Core.SatisfactionRates;
namespace UCLouvain.KAOSTools.Core.Repositories
{
    public interface ISatisfactionRateRepository
    {
		void AddObstacleSatisfactionRate(string obstacleId, ISatisfactionRate satRate);
		ISatisfactionRate GetObstacleSatisfactionRate(string obstacleId);
		IEnumerable<ISatisfactionRate> GetObstacleSatisfactionRates(string obstacleId);

        void AddDomPropSatisfactionRate(string dompropId, ISatisfactionRate satRate);
		ISatisfactionRate GetDomPropSatisfactionRate(string dompropId);
		IEnumerable<ISatisfactionRate> GetDomPropSatisfactionRates(string dompropId);
    }
}
