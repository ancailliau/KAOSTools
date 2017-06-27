using System;
using System.Text.RegularExpressions;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;
using UCLouvain.KAOSTools.Parsing.Parsers;

namespace UCLouvain.KAOSTools.Parsing.Builders.Attributes
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
			foreach (var child in attribute.ParsedRefinees)
			{
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
                
                Obstacle refinee;
                DomainProperty domprop;
                DomainHypothesis domhyp;

                if ((refinee = model.obstacleRepository.GetObstacle(id)) != null)
                {
                    refinement.Add(refinee, refineeParameter);
                }
                else if ((domprop = model.domainRepository.GetDomainProperty(id)) != null)
                {
                    refinement.Add(domprop, refineeParameter);
                }
                else if ((domhyp = model.domainRepository.GetDomainHypothesis(id)) != null)
                {
                    refinement.Add(domhyp, refineeParameter);
                }
                else {
                    refinee = new Obstacle(model, id) { Implicit = true };
                    model.obstacleRepository.Add(refinee);
                    refinement.Add(refinee);
                }
			}

			if (!refinement.IsEmpty)
				model.Add(refinement);
        }
    }
}
