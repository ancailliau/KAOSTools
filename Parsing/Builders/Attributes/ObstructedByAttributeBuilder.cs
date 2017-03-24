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
				if ((obstacle = model.obstacleRepository.GetObstacle(id)) != null)
				{
					obstruction.SetObstacle(obstacle);
				}
				else
				{
					throw new BuilderException("Obstacle '" + id + "' is not defined", 
                                               attribute.Filename, attribute.Line, attribute.Col);
				}
			}
			else
			{
				throw new NotImplementedException(string.Format("'{0}' is not supported in '{1}' on '{2}'", 
                                                                attribute.Value.GetType().Name, 
                                                                attribute.GetType().Name, 
                                                                element.GetType().Name));
			}

			model.Add(obstruction);
        }
    }
}
