using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using LtlSharp.Utils;

namespace KAOSFormalTools.Domain
{
    public class GoalModel
    {
        public IList<Goal>             Goals             { get; set; }
        public IList<DomainProperty>   DomainProperties  { get; set; }
        public IList<DomainHypothesis> DomainHypotheses  { get; set; }
        public IList<Obstacle>         Obstacles         { get; set; }
        public IList<Agent>            Agents            { get; set; }

        public IList<System>      Systems      { get; set; }

        public IList<Goal>           RootGoals { 
            get {
                var goals = new List<Goal> (Goals);
                foreach (var goal in Goals)
                    foreach (var refinement in goal.Refinements) 
                        foreach (var child in refinement.Children)
                            goals.Remove (child);
                foreach (var obstacle in Obstacles)
                    foreach (var resolution in obstacle.Resolutions)
                        goals.Remove (resolution);
                return goals;
            }
        }

        public IList<System>           RootSystems { 
            get {
                var systems = new List<System> (Systems);
                foreach (var system in Systems)
                    foreach (var alternative in system.Alternatives) 
                        systems.Remove (alternative);

                return systems;
            }
        }

        public IEnumerable<Goal>           ObstructedGoals {
            get {
                return from g in Goals where g.Obstruction.Count > 0 select g;
            }
        }

        public IEnumerable<Obstacle>       ResolvedObstacles {
            get {
                return from o in Obstacles where o.Resolutions.Count > 0 select o;
            }
        }
        
        public GoalModel ()
        {
            Goals             = new List<Goal> ();
            DomainProperties  = new List<DomainProperty> ();
            DomainHypotheses  = new List<DomainHypothesis> ();
            Obstacles         = new List<Obstacle> ();
            Agents            = new List<Agent> ();
            Systems      = new List<System> ();
        }

        public System GetSystemByIdentifier (string identifier)
        {
            return Systems.Where (x => x.Identifier == identifier).SingleOrDefault ();
        }
        
        public IEnumerable<System> GetSystemsByName (string name)
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
        
        public void ReplaceGoal (Goal g1, Goal g2)
        {
            Goals.Remove (g1);
            Goals.Add (g2);

            foreach (var refinement in Goals.SelectMany (g => g.Refinements).Where (r => r.Children.Contains (g1))) {
                refinement.Children.Remove (g1);
                refinement.Children.Add (g2);
            }

            foreach (var obstacle in Obstacles.Where (o => o.Resolutions.Contains (g1))) {
                obstacle.Resolutions.Remove (g1);
                obstacle.Resolutions.Add (g2);
            }
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

