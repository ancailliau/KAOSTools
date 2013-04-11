using LtlSharp;
using System.Collections.Generic;
using System.Linq;
using System;

namespace KAOSFormalTools.Domain
{
    public class Goal
    {
        public string                 Identifier      { get; set; }
        public string                 Name            { get; set; }
        public string                 Definition      { get; set; }
        public LTLFormula             FormalSpec      { get; set; }

        public double                 CPS             { get; set; }
        public double                 RDS             { get; set; }

        public ISet<GoalRefinement>   Refinements     { get; set; }
        public ISet<Obstacle>         Obstruction     { get; set; }
        public ISet<AgentAssignment>            AssignedAgents  { get; set; }

        public Goal ()
        {
            Identifier     = Guid.NewGuid ().ToString ();
            Refinements    = new HashSet<GoalRefinement> ();
            Obstruction    = new HashSet<Obstacle> ();
            AssignedAgents = new HashSet<AgentAssignment> ();
        }
    }

    public class Obstacle
    {
        public string                           Identifier   { get; set; }
        public string                           Name         { get; set; }
        public string                           Definition   { get; set; }
        public LTLFormula                       FormalSpec   { get; set; }

        public double                           EPS          { get; set; }
        public double                           CPS          { get; set; }

        public IList<ObstacleRefinement>        Refinements  { get; set; }
        public IList<Goal>                      Resolutions  { get; set; }

        public Obstacle ()
        {
            Identifier  = Guid.NewGuid ().ToString ();
            Refinements = new List<ObstacleRefinement> ();
            Resolutions = new List<Goal> ();
        }
    }

    public class DomainProperty
    {   
        public string      Identifier  { get; set; }
        public string      Name        { get; set; }
        public string      Definition  { get; set; }
        public LTLFormula  FormalSpec  { get; set; }
        public double       EPS         { get; set; }

        public DomainProperty ()
        {
            Identifier  = Guid.NewGuid ().ToString ();
        }
    }

    public class Agent
    {
        public string    Identifier   { get; set; }
        public string    Name         { get; set; }
        public string    Description  { get; set; }
        public AgentType Type         { get; set; }

        public Agent ()
        {
            Identifier  = Guid.NewGuid ().ToString ();
            Type        = AgentType.None;
        }

        public override string ToString ()
        {
            return string.Format ("[Agent: Identifier={0}, Name={1}, Description={2}, Type={3}]", Identifier, Name, Description, Type);
        }
        
    }

    public enum AgentType { None, Software, Environment }

    public class AgentAssignment
    {
        public string                AlternativeIdentifier { get; set; }
        public IList<Agent>          Agents                { get; set; }

        public AgentAssignment ()
        {
            AlternativeIdentifier = "";
            Agents = new List<Agent> ();   
        }
    }

    public class GoalRefinement
    {
        public string                AlternativeIdentifier { get; set; }
        public IList<Goal>           Children          { get; set; }
        public IList<DomainProperty> DomainProperties  { get; set; }

        public GoalRefinement ()
        {
            AlternativeIdentifier = "";
            Children = new List<Goal> ();
            DomainProperties = new List<DomainProperty> ();
        }

        public GoalRefinement (Goal goal) : this ()
        {
            Children.Add (goal);
        }

        public GoalRefinement (params Goal[] goals) : this ()
        {
            foreach (var goal in goals)
                Children.Add (goal);
        }
    }

    public class ObstacleRefinement
    {
        public IList<Obstacle>       Children          { get; set; }
        public IList<DomainProperty> DomainProperties  { get; set; }

        public ObstacleRefinement ()
        {
            Children = new List<Obstacle> ();
            DomainProperties = new List<DomainProperty> ();
        }

        public ObstacleRefinement (Obstacle obstacle) : this ()
        {
            Children.Add (obstacle);
        }

        public ObstacleRefinement (params Obstacle[] obstacles) : this ()
        {
            foreach (var obstacle in obstacles)
                Children.Add (obstacle);
        }
    }
}
