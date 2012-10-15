using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using LtlSharp.Utils;

namespace KAOSFormalTools.Domain
{
    public class GoalModel
    {
        public IList<Goal>           Goals             { get; set; }
        public IList<DomainProperty> DomainProperties  { get; private set; }
        public IList<Obstacle>       Obstacles         { get; set; }
        public IList<Agent>          Agents            { get; set; }

        public IList<Goal>           RootGoals { 
            get {
                var goals = new List<Goal> (Goals);
                foreach (var goal in Goals)
                    foreach (var refinement in goal.Refinements) 
                        foreach (var child in refinement.Children)
                            goals.Remove (child);
                return goals;
            }
        }

        public IEnumerable<Goal>           ObstructedGoals {
            get {
                return from g in Goals where g.Obstruction.Count > 0 select g;
            }
        }
        
        public GoalModel ()
        {
            Goals             = new List<Goal> ();
            DomainProperties  = new List<DomainProperty> ();
            Obstacles         = new List<Obstacle> ();
            Agents            = new List<Agent> ();
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

