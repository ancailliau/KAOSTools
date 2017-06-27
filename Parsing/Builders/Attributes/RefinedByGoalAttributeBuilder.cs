using System;
using System.Text.RegularExpressions;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;
using UCLouvain.KAOSTools.Parsing.Parsers;

namespace UCLouvain.KAOSTools.Parsing.Builders.Attributes
{
	public class RefinedByGoalAttributeBuilder : AttributeBuilder<Goal, ParsedRefinedByAttribute>
    {
		public RefinedByGoalAttributeBuilder()
        {
        }

        public override void Handle(Goal element, ParsedRefinedByAttribute attribute, KAOSModel model)
		{
            var refinement = new GoalRefinement(model);
            refinement.SetParentGoal(element);

            // Parse the reference to children
            foreach (var child in attribute.ParsedRefinees) {
                var id = child.Identifier;
                var param = child.Parameters;
                IRefineeParameter refineeParameter = null;

                if (param != null) {
                    if (param is ParsedPrimitiveRefineeParameter<double>) {
                        var cast = ((ParsedPrimitiveRefineeParameter<double>)param);
                        refineeParameter = new PrimitiveRefineeParameter<double> (cast.Value);

                    } else {
                        throw new NotImplementedException ();
                    }
                }

                Goal refinee;
                DomainProperty domprop;
                DomainHypothesis domhyp;

                if ((refinee = model.goalRepository.GetGoal (id)) != null) {
                    refinement.Add (refinee, refineeParameter);
                } else if ((domprop = model.domainRepository.GetDomainProperty (id)) != null) {
                    refinement.Add (domprop, refineeParameter);
                } else if ((domhyp = model.domainRepository.GetDomainHypothesis (id)) != null) {
                    refinement.Add (domhyp, refineeParameter);
                } else {
                    refinee = new Goal (model, id) { Implicit = true };
                    model.goalRepository.Add (refinee);
                    refinement.Add (refinee);
                }
            }

			// Parse the refinement pattern provided
			if (attribute.RefinementPattern != null)
			{
				if (attribute.RefinementPattern.Name == ParsedRefinementPatternName.Milestone)
				{
					refinement.RefinementPattern = RefinementPattern.Milestone;
				}

				else if (attribute.RefinementPattern.Name == ParsedRefinementPatternName.Case)
				{
					refinement.RefinementPattern = RefinementPattern.Case;
				}

				else if (attribute.RefinementPattern.Name == ParsedRefinementPatternName.IntroduceGuard)
				{
					refinement.RefinementPattern = RefinementPattern.IntroduceGuard;
				}

				else if (attribute.RefinementPattern.Name == ParsedRefinementPatternName.DivideAndConquer)
				{
					refinement.RefinementPattern = RefinementPattern.DivideAndConquer;
				}

				else if (attribute.RefinementPattern.Name == ParsedRefinementPatternName.Uncontrollability)
				{
					refinement.RefinementPattern = RefinementPattern.Uncontrollability;
				}

				else if (attribute.RefinementPattern.Name == ParsedRefinementPatternName.Unmonitorability)
				{
					refinement.RefinementPattern = RefinementPattern.Unmonitorability;
				}
                
				else
				{
                    throw new BuilderException ("Unsupported pattern " + refinement.RefinementPattern, attribute);
				}
			}

			if (!refinement.IsEmpty)
				model.Add(refinement);
        }
    }
}
