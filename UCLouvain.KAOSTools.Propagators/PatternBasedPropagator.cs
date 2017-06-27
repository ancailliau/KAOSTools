using System;
using System.Collections.Generic;
using System.Linq;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Core.SatisfactionRates;
namespace UCLouvain.KAOSTools.Propagators
{
    public class PatternBasedPropagator : IPropagator
    {
        KAOSModel _model;

        public PatternBasedPropagator (KAOSModel model)
        {
            _model = model;
        }

        public ISatisfactionRate GetESR (Goal g)
        {
            ISatisfactionRate cps = null;
            var refinements = _model.GoalRefinements (r => r.ParentGoalIdentifier == g.Identifier);
            if (refinements.Count () > 1)
                throw new PropagationException (PropagationException.MULTIPLE_REFINEMENTS);

            if (refinements.Count () == 1) {
                cps = GetESR (refinements.Single ());
            }

            if (refinements.Count () == 0) {
                var obstructions = _model.Obstructions (r => r.ObstructedGoalIdentifier == g.Identifier);
                if (obstructions.Count () == 0) {
                    cps = DoubleSatisfactionRate.ONE;
                } else {
                    cps = obstructions.Select (x => GetESR (x).OneMinus ()).Aggregate (DoubleSatisfactionRate.ONE, (x, y) => (DoubleSatisfactionRate) x.Product(y));
                }
            }

            _model.satisfactionRateRepository.AddGoalSatisfactionRate (g.Identifier, cps);
            return cps;
        }

        ISatisfactionRate GetESR (GoalRefinement goalRefinement)
        {
            if (goalRefinement.RefinementPattern == RefinementPattern.Milestone
	            | goalRefinement.RefinementPattern == RefinementPattern.IntroduceGuard
	            | goalRefinement.RefinementPattern == RefinementPattern.DivideAndConquer
	            | goalRefinement.RefinementPattern == RefinementPattern.Unmonitorability
	            | goalRefinement.RefinementPattern == RefinementPattern.Uncontrollability) {
                var subGoalSR = ComputeProductRefineeSatisfactionRate (goalRefinement.SubGoalIdentifiers,
                                                                       x => GetESR (_model.Goal (x)));
                var domPropSR = ComputeProductRefineeSatisfactionRate (goalRefinement.DomainPropertyIdentifiers, 
                                                                       x => GetESR(_model.DomainProperty (x)));
                var domHypSR = ComputeProductRefineeSatisfactionRate (goalRefinement.DomainHypothesisIdentifiers, 
                                                                      x => GetESR(_model.DomainHypothesis (x)));
                return subGoalSR.Product (domPropSR).Product (domHypSR);

            } else if (goalRefinement.RefinementPattern == RefinementPattern.Case) {
                var subGoalSR = ComputeRefineeSatisfactionRate (goalRefinement.SubGoalIdentifiers, 
                                                                x => GetESR(_model.Goal (x)));
                var domPropSR = ComputeRefineeSatisfactionRate (goalRefinement.DomainPropertyIdentifiers,
                                                                x => GetESR(_model.DomainProperty (x)));
                var domHypSR = ComputeRefineeSatisfactionRate (goalRefinement.DomainHypothesisIdentifiers, 
                                                               x => GetESR(_model.DomainHypothesis (x)));
                return (DoubleSatisfactionRate)subGoalSR.Sum (domPropSR).Sum (domHypSR);

            } else {
                throw new PropagationException (PropagationException.PATTERN_NOT_SUPPORTED + $" ({goalRefinement.RefinementPattern})");
            }

        }

        DoubleSatisfactionRate ComputeProductRefineeSatisfactionRate (IEnumerable<GoalRefinee> refinees, Func<string, ISatisfactionRate> get)
        {
            return refinees.Aggregate (DoubleSatisfactionRate.ONE, (x, y) => (DoubleSatisfactionRate)x.Product (get (y.Identifier)));
        }

        DoubleSatisfactionRate ComputeRefineeSatisfactionRate (IEnumerable<GoalRefinee> refinees, Func<string, ISatisfactionRate> get)
        {
            return refinees.Aggregate (
                                DoubleSatisfactionRate.ZERO,
                                (x, y) => {
                                    IRefineeParameter parameter = y.Parameters;
                                    if (!(parameter is PrimitiveRefineeParameter<double>))
                                        throw new PropagationException (PropagationException.MISSING_PARAMETER);
                                    var doubleParam = ((PrimitiveRefineeParameter<double>)parameter).Value;
                                    ISatisfactionRate sr = get(y.Identifier).Product (doubleParam);
                                    return (DoubleSatisfactionRate)x.Sum (sr);
                                }
                            );
        }

        ISatisfactionRate GetESR (Obstruction o)
        {
            return GetESR(o.Obstacle ());
        }

        public ISatisfactionRate GetESR (Obstacle o)
        {
            ISatisfactionRate esr;
            var refinements = _model.ObstacleRefinements (r => r.ParentObstacleIdentifier == o.Identifier);
            if (refinements.Count () > 0) {
                esr = refinements.Aggregate (DoubleSatisfactionRate.ONE, (x, y) => (DoubleSatisfactionRate) x.Product ((GetESR (y).OneMinus ()))).OneMinus();
                _model.satisfactionRateRepository.AddObstacleSatisfactionRate (o.Identifier, esr);
            } else {
                esr = o.LatestEPS ();
            }

            return esr;
        }

        ISatisfactionRate GetESR (ObstacleRefinement o)
        {
            return o.SubObstacles ().Aggregate (DoubleSatisfactionRate.ONE, (x, y) => (DoubleSatisfactionRate) x.Product (GetESR (y)))
                    .Product (o.DomainProperties ().Aggregate (DoubleSatisfactionRate.ONE, (x, y) => (DoubleSatisfactionRate) x.Product(GetESR (y))))
                    .Product (o.DomainHypotheses ().Aggregate (DoubleSatisfactionRate.ONE, (x, y) => (DoubleSatisfactionRate) x.Product(GetESR (y))));
        }

        ISatisfactionRate GetESR (DomainProperty o)
        {
            return o.LatestEPS ();
        }

        ISatisfactionRate GetESR (DomainHypothesis o)
        {
            return o.LatestEPS ();
        }

        public ISatisfactionRate GetESR (Obstacle obstacle, IEnumerable<Resolution> activeResolutions)
        {
            throw new NotImplementedException ();
        }

        public ISatisfactionRate GetESR (Goal goal, IEnumerable<Resolution> activeResolutions)
        {
            throw new NotImplementedException ();
        }
    }
}
