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

        public bool ObstacleSatisfactionRateExists (string obstacleId)
        {
            if (ObstacleSatisfactionRates.ContainsKey (obstacleId)) {
                return true;
            }
            return false;
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

        public bool DomPropSatisfactionRateExists (string dompropId)
        {
            if (DomainPropertySatisfactionRates.ContainsKey (dompropId)) {
                return true;
            }
            return false;
        }

        #endregion

        #region Domain Hypothesis

        public void AddDomHypothesisSatisfactionRate (string domhypId, ISatisfactionRate satRate)
        {
            if (!DomainHypothesisSatisfactionRates.ContainsKey (domhypId)) {
                DomainHypothesisSatisfactionRates.Add (domhypId, new List<ISatisfactionRate> ());
            }
            DomainHypothesisSatisfactionRates [domhypId].Add (satRate);
        }

        public ISatisfactionRate GetDomHypothesisSatisfactionRate (string domhypId)
        {
            if (DomainHypothesisSatisfactionRates.ContainsKey (domhypId)) {
                return DomainHypothesisSatisfactionRates [domhypId].Single ();
            }
            return null;
        }

        public IEnumerable<ISatisfactionRate> GetDomHypothesisSatisfactionRates (string domhypId)
        {
            if (DomainHypothesisSatisfactionRates.ContainsKey (domhypId)) {
                return DomainHypothesisSatisfactionRates [domhypId];
            }
            return null;
        }

        public bool DomHypothesisSatisfactionRateExists (string domhypId)
        {
            if (DomainHypothesisSatisfactionRates.ContainsKey (domhypId)) {
                return true;
            }
            return false;
        }

        #endregion

        #region Goals

        public void AddGoalSatisfactionRate (string goalId, ISatisfactionRate satRate)
        {
            if (!GoalSatisfactionRates.ContainsKey (goalId)) {
                GoalSatisfactionRates.Add (goalId, new List<ISatisfactionRate> ());
            }
            GoalSatisfactionRates [goalId].Add (satRate);
        }

        public ISatisfactionRate GetGoalSatisfactionRate (string goalId)
        {
            if (GoalSatisfactionRates.ContainsKey (goalId)) {
                return GoalSatisfactionRates [goalId].Single ();
            }
            return null;
        }

        public IEnumerable<ISatisfactionRate> GetGoalSatisfactionRates (string goalId)
        {
            if (GoalSatisfactionRates.ContainsKey (goalId)) {
                return GoalSatisfactionRates [goalId];
            }
            return null;
        }

        public bool GoalSatisfactionRateExists (string goalId)
        {
            if (GoalSatisfactionRates.ContainsKey (goalId)) {
                return true;
            }
            return false;
        }

        #endregion

    }
}
