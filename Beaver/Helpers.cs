using System;

namespace KAOSFormalTools.Domain
{
    public static class Helpers
    {
        public static Goal Connect (this Goal @this, Goal goal)
        {
            @this.Refinements.Add (new GoalRefinement (goal));
            return @this;
        }

        public static Goal Connect (this Goal @this, params Goal[] goals)
        {
            @this.Refinements.Add (new GoalRefinement (goals));
            return @this;
        }

        public static Goal Connect (this Goal @this, Obstacle obstacle)
        {
            @this.Obstruction.Add (obstacle);
            return @this;
        }
    }
}

