using System;
using System.Collections.Generic;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;
using UCLouvain.KAOSTools.Core.Model;

namespace UCLouvain.KAOSTools.Core.Repositories
{

	public interface IModelMetadataRepository
	{
		void Add(Constraint goal);
		void Add(CostVariable goal);
		void Add(Expert goal);
		void Add(Calibration goal);
		void Add(Context goal);

		bool ConstraintExists(string identifier);
		bool CostVariableExists(string identifier);
		bool ExpertExists(string identifier);
		bool CalibrationExists(string identifier);
		bool ContextExists(string identifier);

		Constraint GetConstraint(string identifier);
		CostVariable GetCostVariable(string identifier);
		Expert GetExpert(string identifier);
		Calibration GetCalibration(string identifier);
		Context GetContext(string identifier);

		Constraint GetConstraint(Predicate<Constraint> predicate);
		CostVariable GetCostVariable(Predicate<CostVariable> predicate);
		Expert GetExpert(Predicate<Expert> predicate);
		Calibration GetCalibration(Predicate<Calibration> predicate);
		Context GetContext(Predicate<Context> predicate);

		IEnumerable<CostVariable> GetCostVariables();
		IEnumerable<Expert> GetExperts();
		IEnumerable<Calibration> GetCalibrations();
		IEnumerable<Constraint> GetConstraints();
		IEnumerable<Context> GetContexts();

		IEnumerable<CostVariable> GetCostVariables(Predicate<CostVariable> predicate);
		IEnumerable<Expert> GetExperts(Predicate<Expert> predicate);
		IEnumerable<Calibration> GetCalibrations(Predicate<Calibration> predicate);
		IEnumerable<Constraint> GetConstraints(Predicate<Constraint> predicate);
		IEnumerable<Context> GetContexts(Predicate<Context> predicate);
	}
    
}
