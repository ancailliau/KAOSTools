using System;
using KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;

namespace UCLouvain.KAOSTools.Core.Repositories
{

    public interface IObstacleRepository
	{
		void Add(Obstacle goal);
		void Add(ObstacleAssumption goal);
		void Add(ObstacleRefinement goal);
		void Add(Obstruction goal);
		void Add(Resolution goal);

		bool ObstacleExists(string identifier);
		bool ObstacleAssumptionExists(string identifier);
		bool ObstacleRefinementExists(string identifier);
		bool ObstructionExists(string identifier);
		bool ResolutionExists(string identifier);

		Obstacle GetObstacle(string identifier);
		ObstacleAssumption GetObstacleAssumption(string identifier);
		ObstacleRefinement GetObstacleRefinement(string identifier);
		Obstruction GetObstruction(string identifier);
		Resolution GetResolution(string identifier);
	}
}
