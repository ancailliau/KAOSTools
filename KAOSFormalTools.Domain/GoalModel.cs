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
        }
    }
}

