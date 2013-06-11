using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace KAOSTools.MetaModel
{
    public class GoalModel
    {
        public ISet<Goal>             Goals             { get; set; }
        public ISet<DomainProperty>   DomainProperties  { get; set; }
        public ISet<DomainHypothesis> DomainHypotheses  { get; set; }
        public ISet<Obstacle>         Obstacles         { get; set; }
        public ISet<Agent>            Agents            { get; set; }

        public ISet<AlternativeSystem>      Systems      { get; set; }

        public ISet<Goal>           RootGoals { 
            get {
                var goals = new HashSet<Goal> (Goals);
                foreach (var goal in Goals)
                    foreach (var refinement in goal.Refinements) 
                        foreach (var child in refinement.Subgoals)
                            goals.Remove (child);
                foreach (var obstacle in Obstacles)
                    foreach (var resolution in obstacle.Resolutions)
                        goals.Remove (resolution.ResolvingGoal);
                return goals;
            }
        }

        public ISet<AlternativeSystem>           RootSystems { 
            get {
                var systems = new HashSet<AlternativeSystem> (Systems);
                foreach (var system in Systems)
                    foreach (var alternative in system.Alternatives) 
                        systems.Remove (alternative);

                return systems;
            }
        }

        public IEnumerable<Goal>           ObstructedGoals {
            get {
                return from g in Goals where g.Obstructions.Count > 0 select g;
            }
        }

        public IEnumerable<Obstacle>       ResolvedObstacles {
            get {
                return from o in Obstacles where o.Resolutions.Count > 0 select o;
            }
        }
        
        public GoalModel ()
        {
            Goals             = new HashSet<Goal> ();
            DomainProperties  = new HashSet<DomainProperty> ();
            DomainHypotheses  = new HashSet<DomainHypothesis> ();
            Obstacles         = new HashSet<Obstacle> ();
            Agents            = new HashSet<Agent> ();
            Systems      = new HashSet<AlternativeSystem> ();
        }

        public AlternativeSystem GetSystemByIdentifier (string identifier)
        {
            return Systems.Where (x => x.Identifier == identifier).SingleOrDefault ();
        }
        
        public IEnumerable<AlternativeSystem> GetSystemsByName (string name)
        {
            return Systems.Where (x => x.Name == name);
        }

        public bool SystemExists (string identifier)
        {
            return Systems.Where (x => x.Identifier == identifier).Count () > 0;
        }

        public Goal GetGoalByIdentifier (string identifier)
        {
            return Goals.Where (x => x.Identifier == identifier).SingleOrDefault ();
        }

        public IEnumerable<Goal> GetGoalsByName (string name)
        {
            return Goals.Where (x => x.Name == name);
        }

        public bool GoalExists (string identifier)
        {
            return Goals.Where (x => x.Identifier == identifier).Count () > 0;
        }
        
        public Obstacle GetObstacleByIdentifier (string identifier)
        {
            return Obstacles.Where (x => x.Identifier == identifier).SingleOrDefault ();
        }

        public IEnumerable<Obstacle> GetObstaclesByName (string name)
        {
            return Obstacles.Where (x => x.Name == name);
        }

        public bool ObstacleExists (string identifier)
        {
            return Obstacles.Where (x => x.Identifier == identifier).Count () > 0;
        }

        public DomainProperty GetDomainPropertyByIdentifier (string identifier)
        {
            return DomainProperties.Where (x => x.Identifier == identifier).SingleOrDefault ();
        }

        public IEnumerable<DomainProperty> GetDomainPropertiesByName (string name)
        {
            return DomainProperties.Where (x => x.Name == name);
        }

        public bool DomainPropertyExists (string identifier)
        {
            return DomainProperties.Where (x => x.Identifier == identifier).Count () > 0;
        }

        public DomainHypothesis GetDomainHypothesisByIdentifier (string identifier)
        {
            return DomainHypotheses.Where (x => x.Identifier == identifier).SingleOrDefault ();
        }
        
        public IEnumerable<DomainHypothesis> GetDomainHypothesesByName (string name)
        {
            return DomainHypotheses.Where (x => x.Name == name);
        }
        
        public bool DomainHypothesisExists (string identifier)
        {
            return DomainHypotheses.Where (x => x.Identifier == identifier).Count () > 0;
        }

        public Agent GetAgentByIdentifier (string identifier)
        {
            return Agents.Where (x => x.Identifier == identifier).SingleOrDefault ();
        }

        public IEnumerable<Agent> GetAgentsByName (string name)
        {
            return Agents.Where (x => x.Name == name);
        }

        public bool AgentExists (string identifier)
        {
            return Agents.Where (x => x.Identifier == identifier).Count () > 0;
        }

    }

}

