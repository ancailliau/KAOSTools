using System;
using System.Collections.Generic;
using System.Linq;
using KAOSTools.Core;

namespace UCLouvain.KAOSTools.Core.Repositories.Memory
{
    public class ModelMetadataRepository : IModelMetadataRepository
	{
		IDictionary<string, Calibration> Calibrations;
		IDictionary<string, Expert> Experts;
		IDictionary<string, CostVariable> CostVariables;
		IDictionary<string, Constraint> Constraints;

        public ModelMetadataRepository()
		{
            Calibrations = new Dictionary<string, Calibration> ();
            Experts = new Dictionary<string, Expert> ();
            CostVariables = new Dictionary<string, CostVariable> ();
            Constraints = new Dictionary<string, Constraint> ();
        }

        public void Add(Calibration calibration)
		{
			if (Calibrations.ContainsKey(calibration.Identifier))
			{
				throw new ArgumentException(string.Format("Calibration identifier already exist: {0}", calibration.Identifier));
			}

			Calibrations.Add(calibration.Identifier, calibration);
        }

        public void Add(Expert expert)
		{
			if (Experts.ContainsKey(expert.Identifier))
			{
				throw new ArgumentException(string.Format("Expert identifier already exist: {0}", expert.Identifier));
			}

			Experts.Add(expert.Identifier, expert);
        }

        public void Add(CostVariable costVariable)
		{
			if (CostVariables.ContainsKey(costVariable.Identifier))
			{
				throw new ArgumentException(string.Format("Cost variable identifier already exist: {0}", costVariable.Identifier));
			}

			CostVariables.Add(costVariable.Identifier, costVariable);
        }

        public void Add(Constraint constraint)
		{
			if (Constraints.ContainsKey(constraint.Identifier))
			{
				throw new ArgumentException(string.Format("Constraints identifier already exist: {0}", constraint.Identifier));
			}

			Constraints.Add(constraint.Identifier, constraint);
        }

        public bool CalibrationExists(string identifier)
		{
            return Calibrations.ContainsKey(identifier);
        }

        public bool ConstraintExists(string identifier)
		{
            return Constraints.ContainsKey(identifier);
        }

        public bool CostVariableExists(string identifier)
		{
            return CostVariables.ContainsKey(identifier);
        }

        public bool ExpertExists(string identifier)
		{
            return Experts.ContainsKey(identifier);
        }

        public Constraint GetConstraint(string identifier)
        {
            return Constraints.ContainsKey(identifier) ? Constraints[identifier] : null;
        }

        public CostVariable GetCostVariable(string identifier)
		{
			return CostVariables.ContainsKey(identifier) ? CostVariables[identifier] : null;
        }

        public Expert GetExpert(string identifier)
		{
			return Experts.ContainsKey(identifier) ? Experts[identifier] : null;
        }

        public Calibration GetCalibration(string identifier)
		{
			return Calibrations.ContainsKey(identifier) ? Calibrations[identifier] : null;
        }

        public Constraint GetConstraint(Predicate<Constraint> predicate)
        {
            return Constraints.Values.SingleOrDefault(x => predicate(x));
        }

        public CostVariable GetCostVariable(Predicate<CostVariable> predicate)
		{
            return CostVariables.Values.SingleOrDefault(x => predicate(x));
        }

        public Expert GetExpert(Predicate<Expert> predicate)
		{
			return Experts.Values.SingleOrDefault(x => predicate(x));
        }

        public Calibration GetCalibration(Predicate<Calibration> predicate)
		{
            return Calibrations.Values.SingleOrDefault(x => predicate(x));
        }

        public IEnumerable<CostVariable> GetCostVariables()
		{
			return CostVariables.Values;
        }

        public IEnumerable<Expert> GetExperts()
		{
            return Experts.Values;
        }

        public IEnumerable<Calibration> GetCalibrations()
		{
            return Calibrations.Values;
        }

        public IEnumerable<Constraint> GetConstraints()
		{
            return Constraints.Values;
        }

        public IEnumerable<CostVariable> GetCostVariables(Predicate<CostVariable> predicate)
		{
            return CostVariables.Values.Where (x => predicate(x));
        }

        public IEnumerable<Expert> GetExperts(Predicate<Expert> predicate)
		{
            return Experts.Values.Where(x => predicate(x));
        }

        public IEnumerable<Calibration> GetCalibrations(Predicate<Calibration> predicate)
		{
            return Calibrations.Values.Where(x => predicate(x));
        }

        public IEnumerable<Constraint> GetConstraints(Predicate<Constraint> predicate)
		{
            return Constraints.Values.Where(x => predicate(x));
        }
    }
}
