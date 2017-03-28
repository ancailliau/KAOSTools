using System;
using System.Text.RegularExpressions;
using KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;
using KAOSTools.Parsing.Parsers;

namespace KAOSTools.Parsing.Builders.Attributes
{
	public class ObstructedByAttributeBuilder : AttributeBuilder<Goal, ParsedObstructedByAttribute>
    {
		public ObstructedByAttributeBuilder()
        {
        }

        public override void Handle(Goal element, ParsedObstructedByAttribute attribute, KAOSModel model)
		{
            var obstruction = new Obstruction(model);
            obstruction.SetObstructedGoal(element);

			if (attribute.Value is IdentifierExpression)
			{
				var id = ((IdentifierExpression)attribute.Value).Value;

				Obstacle obstacle;
                if ((obstacle = model.obstacleRepository.GetObstacle(id)) == null) {
                    obstacle = new Obstacle(model, id) { Implicit = true };
                    model.obstacleRepository.Add(obstacle);
                }

                obstruction.SetObstacle(obstacle);
            }
			else
			{
                throw new UnsupportedValue(element, attribute, attribute.Value);
			}

			model.Add(obstruction);
        }
    }
}
