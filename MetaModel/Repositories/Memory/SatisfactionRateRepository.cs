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
        Dictionary<string, List<ISatisfactionRate>> DomainHypothesisSatisfactionRates;
        Dictionary<string, List<ISatisfactionRate>> GoalSatisfactionRates;

        public SatisfactionRateRepository()
        {
			ObstacleSatisfactionRates = new Dictionary<string, List<ISatisfactionRate>>();
            DomainPropertySatisfactionRates = new Dictionary<string, List<ISatisfactionRate>> ();
            DomainHypothesisSatisfactionRates = new Dictionary<string, List<ISatisfactionRate>> ();
            GoalSatisfactionRates = new Dictionary<string, List<ISatisfactionRate>> ();
        }

        #region Obstacle

        public void AddObstacleSatisfactionRate (string obstacleId, ISatisfactionRate satRate)
        {
            if (!ObstacleSatisfactionRates.ContainsKey (obstacleId)) {
                ObstacleSatisfactionRates.Add (obstacleId, new List<ISatisfactionRate> ());
            }
            ObstacleSatisfactionRates [obstacleId].Add (satRate);
        }

        public ISatisfactionRate GetObstacleSatisfactionRate (string obstacleId)
        {
            if (ObstacleSatisfactionRates.ContainsKey (obstacleId)) {
                return ObstacleSatisfactionRates [obstacleId].Single ();
            }
            return null;
        }

        public IEnumerable<ISatisfactionRate> GetObstacleSatisfactionRates (string obstacleId)
        {
            if (ObstacleSatisfactionRates.ContainsKey (obstacleId)) {
                return ObstacleSatisfactionRates [obstacleId];
            }
            return null;
        }

        #endregion

        #region Domain Property

        public void AddDomPropSatisfactionRate (string dompropId, ISatisfactionRate satRate)
        {
            if (!DomainPropertySatisfactionRates.ContainsKey (dompropId)) {
                DomainPropertySatisfactionRates.Add (dompropId, new List<ISatisfactionRate> ());
            }
            DomainPropertySatisfactionRates [dompropId].Add (satRate);
        }

        public ISatisfactionRate GetDomPropSatisfactionRate (string dompropId)
        {
            if (DomainPropertySatisfactionRates.ContainsKey (dompropId)) {
                return DomainPropertySatisfactionRates [dompropId].Single ();
            }
            return null;
        }

        public IEnumerable<ISatisfactionRate> GetDomPropSatisfactionRates (string dompropId)
        {
            if (DomainPropertySatisfactionRates.ContainsKey (dompropId)) {
                return DomainPropertySatisfactionRates [dompropId];
            }
            return null;
        }

        #endregion

        #region Domain Hypothesis

        public void AddDomHypothesisSatisfactionRate (string dompropId, ISatisfactionRate satRate)
        {
            if (!DomainHypothesisSatisfactionRates.ContainsKey (dompropId)) {
                DomainHypothesisSatisfactionRates.Add (dompropId, new List<ISatisfactionRate> ());
            }
            DomainHypothesisSatisfactionRates [dompropId].Add (satRate);
        }

        public ISatisfactionRate GetDomHypothesisSatisfactionRate (string dompropId)
        {
            if (DomainHypothesisSatisfactionRates.ContainsKey (dompropId)) {
                return DomainHypothesisSatisfactionRates [dompropId].Single ();
            }
            return null;
        }

        public IEnumerable<ISatisfactionRate> GetDomHypothesisSatisfactionRates (string dompropId)
        {
            if (DomainHypothesisSatisfactionRates.ContainsKey (dompropId)) {
                return DomainHypothesisSatisfactionRates [dompropId];
            }
            return null;
        }

        #endregion

        #region Goals

        public void AddGoalSatisfactionRate (string dompropId, ISatisfactionRate satRate)
        {
            if (!GoalSatisfactionRates.ContainsKey (dompropId)) {
                GoalSatisfactionRates.Add (dompropId, new List<ISatisfactionRate> ());
            }
            GoalSatisfactionRates [dompropId].Add (satRate);
        }

        public ISatisfactionRate GetGoalSatisfactionRate (string dompropId)
        {
            if (GoalSatisfactionRates.ContainsKey (dompropId)) {
                return GoalSatisfactionRates [dompropId].Single ();
            }
            return null;
        }

        public IEnumerable<ISatisfactionRate> GetGoalSatisfactionRates (string dompropId)
        {
            if (GoalSatisfactionRates.ContainsKey (dompropId)) {
                return GoalSatisfactionRates [dompropId];
            }
            return null;
        }

        #endregion

    }
}
