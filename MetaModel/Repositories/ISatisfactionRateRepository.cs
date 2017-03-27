using System;
using UCLouvain.KAOSTools.Core.SatisfactionRates;
namespace UCLouvain.KAOSTools.Core.Repositories
{
    public interface ISatisfactionRateRepository
    {
		void AddObstacleSatisfactionRate(string obstacleId, ISatisfactionRate satRate);
		ISatisfactionRate GetObstacleSatisfactionRate(string obstacleId);

        void AddDomPropSatisfactionRate(string dompropId, ISatisfactionRate satRate);
        ISatisfactionRate GetDomPropSatisfactionRate(string dompropId);
    }
}
