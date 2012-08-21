using LtlSharp;
using System.Collections.Generic;
using System.Linq;

namespace KAOSFormalTools.Domain
{
    public class Goal
    {
        public string                 Identifier      { get; set; }
        public string                 Name            { get; set; }
        public string                 Definition      { get; set; }
        public LTLFormula             FormalSpec      { get; set; }

        public IList<GoalRefinement>  Refinements     { get; set; }
        public IList<Agent>           AssignedAgents  { get; set; }
        public IList<Obstacle>        Obstruction     { get; set; }

        public Goal () {
            Refinements    = new List<GoalRefinement> ();
            AssignedAgents = new List<Agent> ();
            Obstruction    = new List<Obstacle> ();
        }
    }

    public class Obstacle
    {
        public string                           Identifier   { get; set; }
        public string                           Name         { get; set; }
        public string                           Definition   { get; set; }
        public LTLFormula                       FormalSpec   { get; set; }

        public IList<ObstacleRefinement>        Refinements  { get; set; }

        public Obstacle () {
            Refinements = new List<ObstacleRefinement> ();
        }
    }

    public class DomainProperty
    {   
        public string      Identifier  { get; set; }
        public string      Name        { get; set; }
        public string      Definition  { get; set; }
        public LTLFormula  FormalSpec  { get; set; }
    }

    public class Agent
    {
        public string  Name         { get; set; }
        public string  Description  { get; set; }
    }

    public class GoalRefinement
    {
        public IList<Goal> Children { get; set; }

        public GoalRefinement ()
        {
            Children = new List<Goal> ();
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

        public ObstacleRefinement ()
        {
            Children = new List<Obstacle> ();
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

