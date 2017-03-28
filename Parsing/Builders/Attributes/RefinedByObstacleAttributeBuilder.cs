using System;
using System.Text.RegularExpressions;
using KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;
using KAOSTools.Parsing.Parsers;

namespace KAOSTools.Parsing.Builders.Attributes
{
    public class RefinedByObstacleAttributeBuilder : AttributeBuilder<Obstacle, ParsedRefinedByAttribute>
    {
		public RefinedByObstacleAttributeBuilder()
        {
        }

		public override void Handle(Obstacle element, ParsedRefinedByAttribute attribute, KAOSModel model)
		{
            var refinement = new ObstacleRefinement(model);
            refinement.SetParentObstacle(element);

			// Parse the reference to children
			foreach (var child in attribute.Values)
			{
                if (child is IdentifierExpression)
                {
                    var id = ((IdentifierExpression)child).Value;

                    Obstacle refinee;
                    DomainProperty domprop;
                    DomainHypothesis domhyp;

                    if ((refinee = model.obstacleRepository.GetObstacle(id)) != null)
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
                    else {
                        refinee = new Obstacle(model, id) { Implicit = true };
                        model.obstacleRepository.Add(refinee);
                        refinement.Add(refinee);
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

			if (!refinement.IsEmpty)
				model.Add(refinement);
        }
    }
}
