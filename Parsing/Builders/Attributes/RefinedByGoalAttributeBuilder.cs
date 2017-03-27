using System;
using System.Text.RegularExpressions;
using KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;
using KAOSTools.Parsing.Parsers;

namespace KAOSTools.Parsing.Builders.Attributes
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
			foreach (var child in attribute.Values)
			{
                if (child is IdentifierExpression)
                {
                    var id = ((IdentifierExpression)child).Value;

                    Goal refinee;
                    DomainProperty domprop;
                    DomainHypothesis domhyp;

                    if ((refinee = model.goalRepository.GetGoal(id)) != null)
                    {
                        refinement.Add(refinee);
                    }
                    else if ((domprop = model.domainRepository.GetDomainProperty(id)) != null)
                    {
                        refinement.Add(domprop);
                    }
                    else if ((domhyp = model.domainRepository.GetDomainHypothesis(id)) != null)
                    {
                        refinement.Add(domhyp);
                    }
                    else
                    {
                        throw new BuilderException("Goal, domain property or domain hypothesis '" + id + "' not defined",
                                                   child.Filename, child.Line, child.Col);
                    }
                }
                else
                {
                    throw new NotImplementedException(string.Format("'{0}' is not supported in '{1}' on '{2}'", 
                                                                    child.GetType().Name, 
                                                                    attribute.GetType().Name, 
                                                                    element.GetType().Name));
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
					// TODO Refactor that allowing to specify how much the subgoal contribute (alone) to the parent goal.
					foreach (var p in attribute.RefinementPattern.Parameters)
					{
						refinement.Parameters.Add((p as ParsedFloat).Value);
					}
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

				else if (attribute.RefinementPattern.Name == ParsedRefinementPatternName.Redundant)
				{
					refinement.RefinementPattern = RefinementPattern.Redundant;
				}


				else
				{
					throw new NotImplementedException();
				}
			}

			if (!refinement.IsEmpty)
				model.Add(refinement);
        }
    }
}
