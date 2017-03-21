using System;
using System.Collections.Generic;
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
    }
}
