﻿using System;
using System.Collections.Generic;
using KAOSTools.Core;

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
    }
}