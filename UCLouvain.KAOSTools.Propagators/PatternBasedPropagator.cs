using System;
using System.Linq;
using KAOSTools.Core;
namespace UCLouvain.KAOSTools.Propagators
{
    public class PatternBasedPropagator
    {
        KAOSModel _model;

        public PatternBasedPropagator (KAOSModel model)
        {
            _model = model;
        }

        public double GetESR (Goal g)
        {
            var refinements = _model.GoalRefinements (r => r.ParentGoalIdentifier == g.Identifier);
            if (refinements.Count () > 1)
                throw new PropagationException (PropagationException.MULTIPLE_REFINEMENTS);

            if (refinements.Count () == 1) {
                g.CPS = GetESR (refinements.Single ());
            }

            if (refinements.Count () == 0) {
                var obstructions = _model.Obstructions (r => r.ObstructedGoalIdentifier == g.Identifier);
                if (obstructions.Count () == 0) {
                    g.CPS = 1;
                } else {
                    g.CPS = obstructions.Select (x => 1 - GetESR (x)).Aggregate (1d, (x, y) => x * y);
                }
            }
            return g.CPS;
        }

        public double GetESR (GoalRefinement goalRefinement)
        {
            if (goalRefinement.RefinementPattern == RefinementPattern.Milestone) {
                return goalRefinement.SubGoals().Aggregate (1d, (x, y) => x * GetESR (y))
                    * goalRefinement.DomainProperties ().Aggregate (1d, (x, y) => x * GetESR (y))
                    * goalRefinement.DomainHypotheses().Aggregate (1d, (x, y) => x * GetESR (y));
            } else {
                throw new PropagationException (PropagationException.PATTERN_NOT_SUPPORTED);
            }

        }

        public double GetESR (Obstruction o)
        {
            return GetESR(o.Obstacle ());
        }

        public double GetESR (Obstacle o)
        {
            var refinements = _model.ObstacleRefinements (r => r.ParentObstacleIdentifier == o.Identifier);
            if (refinements.Count () > 0) {
                o.EPS = 1 - refinements.Aggregate (1d, (x, y) => x * (1 - GetESR (y)));
            }
            return o.EPS;
        }

        public double GetESR (ObstacleRefinement o)
        {
            return o.SubObstacles ().Aggregate (1d, (x, y) => x * GetESR (y))
                    * o.DomainProperties ().Aggregate (1d, (x, y) => x * GetESR (y))
                    * o.DomainHypotheses ().Aggregate (1d, (x, y) => x * GetESR (y));
        }

        public double GetESR (DomainProperty o)
        {
            return o.EPS;
        }

        public double GetESR (DomainHypothesis o)
        {
            return o.EPS;
        }
    }
}
