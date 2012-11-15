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

        public IList<GoalRefinement>  Refinements     { get; set; }
        public IList<Obstacle>        Obstruction     { get; set; }
        public IList<Agent>           AssignedAgents   { get; set; }

        public Goal ()
        {
            Identifier     = Guid.NewGuid ().ToString ();
            Refinements    = new List<GoalRefinement> ();
            Obstruction    = new List<Obstacle> ();
            AssignedAgents = new List<Agent> ();
        }
    }

    public class Obstacle
    {
        public string                           Identifier   { get; set; }
        public string                           Name         { get; set; }
        public string                           Definition   { get; set; }
        public LTLFormula                       FormalSpec   { get; set; }

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

        public DomainProperty ()
        {
            Identifier  = Guid.NewGuid ().ToString ();
        }
    }

    public class Agent
    {
        public string  Identifier   { get; set; }
        public string  Name         { get; set; }
        public string  Description  { get; set; }
        public bool    Software     { get; set; }

        public Agent ()
        {
            Identifier  = Guid.NewGuid ().ToString ();
            Software    = false;
        }
    }

    public class GoalRefinement
    {
        public IList<Goal> Children { get; set; }
        public IList<DomainProperty> DomainProperties { get; set; }

        public GoalRefinement ()
        {
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
        public IList<Obstacle> Children { get; set; }
        public IList<DomainProperty> DomainProperties { get; set; }

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

