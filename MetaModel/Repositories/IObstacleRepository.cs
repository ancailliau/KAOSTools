using System;
using System.Collections.Generic;
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

		Obstacle GetObstacle(Predicate<Obstacle> predicate);
		ObstacleAssumption GetObstacleAssumption(Predicate<ObstacleAssumption> predicate);
		ObstacleRefinement GetObstacleRefinement(Predicate<ObstacleRefinement> predicate);
		Obstruction GetObstruction(Predicate<Obstruction> predicate);
		Resolution GetResolution(Predicate<Resolution> predicate);

		IEnumerable<Obstacle> GetObstacles();
		IEnumerable<ObstacleAssumption> GetObstacleAssumptions();
		IEnumerable<ObstacleRefinement> GetObstacleRefinements();
		IEnumerable<Obstruction> GetObstructions();
		IEnumerable<Resolution> GetResolutions();

		IEnumerable<Obstacle> GetObstacles(Predicate<Obstacle> predicate);
		IEnumerable<ObstacleAssumption> GetObstacleAssumptions(Predicate<ObstacleAssumption> predicate);
		IEnumerable<ObstacleRefinement> GetObstacleRefinements(Predicate<ObstacleRefinement> predicate);
		IEnumerable<Obstruction> GetObstructions(Predicate<Obstruction> predicate);
		IEnumerable<Resolution> GetResolutions(Predicate<Resolution> predicate);
	}
}
