using System;
using System.Collections.Generic;
using System.Linq;
using UCLouvain.KAOSTools.Core;

namespace UCLouvain.KAOSTools.Core.Repositories.Memory
{
    public class ObstacleRepository : IObstacleRepository
	{
		IDictionary<string, Obstruction> Obstructions;
		IDictionary<string, Resolution> Resolutions;
		IDictionary<string, ObstacleRefinement> ObstacleRefinements;
		IDictionary<string, ObstacleAssumption> ObstacleAssumptions;
		IDictionary<string, Obstacle> Obstacles;

		public ObstacleRepository()
		{
            Obstructions = new Dictionary<string, Obstruction> ();
            Resolutions = new Dictionary<string, Resolution> ();
            ObstacleRefinements = new Dictionary<string, ObstacleRefinement> ();
            ObstacleAssumptions = new Dictionary<string, ObstacleAssumption> ();
            Obstacles = new Dictionary<string, Obstacle> ();
		}

        public void Add(Obstruction obstruction)
		{
			if (Obstructions.ContainsKey(obstruction.Identifier))
			{
				throw new ArgumentException(string.Format("Obstruction identifier already exist: {0}", obstruction.Identifier));
			}

			Obstructions.Add(obstruction.Identifier, obstruction);
        }

        public void Add(Resolution resolution)
		{
            if (Resolutions.ContainsKey(resolution.Identifier))
			{
				throw new ArgumentException(string.Format("Resolution identifier already exist: {0}", resolution.Identifier));
			}

			Resolutions.Add(resolution.Identifier, resolution);
        }

        public void Add(ObstacleRefinement refinement)
		{
			if (ObstacleRefinements.ContainsKey(refinement.Identifier))
			{
				throw new ArgumentException(string.Format("Obstacle refinement identifier already exist: {0}", refinement.Identifier));
			}

			ObstacleRefinements.Add(refinement.Identifier, refinement);
        }

        public void Add(ObstacleAssumption assumption)
		{
			if (ObstacleAssumptions.ContainsKey(assumption.Identifier))
			{
				throw new ArgumentException(string.Format("Obstacle assumption identifier already exist: {0}", assumption.Identifier));
			}

            ObstacleAssumptions.Add(assumption.Identifier, assumption);
        }

        public void Add(Obstacle obstacle)
		{
			if (Obstacles.ContainsKey(obstacle.Identifier))
			{
				throw new ArgumentException(string.Format("Obstacle identifier already exist: {0}", obstacle.Identifier));
			}

            Obstacles.Add(obstacle.Identifier, obstacle);
        }

        public bool ObstacleAssumptionExists(string identifier)
		{
            return ObstacleAssumptions.ContainsKey(identifier);
        }

        public bool ObstacleExists(string identifier)
		{
            return Obstacles.ContainsKey(identifier);
        }

        public bool ObstacleRefinementExists(string identifier)
		{
            return ObstacleRefinements.ContainsKey(identifier);
        }

        public bool ObstructionExists(string identifier)
		{
            return Obstructions.ContainsKey(identifier);
        }

        public bool ResolutionExists(string identifier)
		{
            return Resolutions.ContainsKey(identifier);
        }

        public Obstacle GetObstacle(string identifier)
		{
			return Obstacles.ContainsKey(identifier) ? Obstacles[identifier] : null;
        }

        public ObstacleAssumption GetObstacleAssumption(string identifier)
		{
			return ObstacleAssumptions.ContainsKey(identifier) ? ObstacleAssumptions[identifier] : null;
        }

        public ObstacleRefinement GetObstacleRefinement(string identifier)
		{
			return ObstacleRefinements.ContainsKey(identifier) ? ObstacleRefinements[identifier] : null;
        }

        public Obstruction GetObstruction(string identifier)
		{
			return Obstructions.ContainsKey(identifier) ? Obstructions[identifier] : null;
        }

        public Resolution GetResolution(string identifier)
		{
			return Resolutions.ContainsKey(identifier) ? Resolutions[identifier] : null;
        }

        public Obstacle GetObstacle(Predicate<Obstacle> predicate)
        {
            return Obstacles.Values.SingleOrDefault(x => predicate(x));
        }

        public ObstacleAssumption GetObstacleAssumption(Predicate<ObstacleAssumption> predicate)
		{
            return ObstacleAssumptions.Values.SingleOrDefault(x => predicate(x));
        }

        public ObstacleRefinement GetObstacleRefinement(Predicate<ObstacleRefinement> predicate)
		{
            return ObstacleRefinements.Values.SingleOrDefault(x => predicate(x));
        }

        public Obstruction GetObstruction(Predicate<Obstruction> predicate)
		{
            return Obstructions.Values.SingleOrDefault(x => predicate(x));
        }

        public Resolution GetResolution(Predicate<Resolution> predicate)
		{
            return Resolutions.Values.SingleOrDefault(x => predicate(x));
        }

        public IEnumerable<Obstacle> GetObstacles()
		{
            return Obstacles.Values;
        }

        public IEnumerable<ObstacleAssumption> GetObstacleAssumptions()
		{
            return ObstacleAssumptions.Values;
        }

        public IEnumerable<ObstacleRefinement> GetObstacleRefinements()
		{
            return ObstacleRefinements.Values;
        }

        public IEnumerable<Obstruction> GetObstructions()
		{
            return Obstructions.Values;
        }

        public IEnumerable<Resolution> GetResolutions()
		{
            return Resolutions.Values;
        }

        public IEnumerable<Obstacle> GetObstacles(Predicate<Obstacle> predicate)
		{
            return Obstacles.Values.Where(x => predicate(x));
        }

        public IEnumerable<ObstacleAssumption> GetObstacleAssumptions(Predicate<ObstacleAssumption> predicate)
		{
            return ObstacleAssumptions.Values.Where(x => predicate(x));
        }

        public IEnumerable<ObstacleRefinement> GetObstacleRefinements(Predicate<ObstacleRefinement> predicate)
		{
            return ObstacleRefinements.Values.Where(x => predicate(x));
        }

        public IEnumerable<Obstruction> GetObstructions(Predicate<Obstruction> predicate)
		{
            return Obstructions.Values.Where(x => predicate(x));
        }

        public IEnumerable<Resolution> GetResolutions(Predicate<Resolution> predicate)
		{
            return Resolutions.Values.Where(x => predicate(x));
        }

        public void Remove (Resolution resolution)
        {
            Resolutions.Remove (resolution.Identifier);
        }

        public void Remove (Obstruction obstruction)
        {
            Obstructions.Remove (obstruction.Identifier);
        }

        public void Remove (ObstacleAssumption assumption)
        {
            ObstacleAssumptions.Remove (assumption.Identifier);
        }
    }
}
