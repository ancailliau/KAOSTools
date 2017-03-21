using System;
using KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;

namespace UCLouvain.KAOSTools.Core.Repositories
{

	public interface IModelMetadataRepository
	{
		void Add(Constraint goal);
		void Add(CostVariable goal);
		void Add(Expert goal);
		void Add(Calibration goal);

		bool ConstraintExists(string identifier);
		bool CostVariableExists(string identifier);
		bool ExpertExists(string identifier);
		bool CalibrationExists(string identifier);
	}
    
}
