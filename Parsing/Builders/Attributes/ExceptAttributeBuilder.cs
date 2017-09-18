using System;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;
using UCLouvain.KAOSTools.Parsing.Parsers;

namespace UCLouvain.KAOSTools.Parsing.Builders.Attributes
{
	public class ExceptAttributeBuilder : AttributeBuilder<Goal, ParsedExceptAttribute>
    {
		public ExceptAttributeBuilder()
        {
        }

        public override void Handle(Goal element, ParsedExceptAttribute attribute, KAOSModel model)
		{
			var exception = new GoalException(model);
			exception.AnchorGoalIdentifier = element.Identifier;
			exception.ResolvingGoalIdentifier = attribute.CountermeasureIdentifier;
			exception.ResolvedObstacleIdentifier = attribute.ObstacleIdentifier;

			if (!model.goalRepository.GoalExists(attribute.CountermeasureIdentifier)) {
				model.Add(new Goal(model) {
					Identifier = attribute.CountermeasureIdentifier,
					Implicit = true
				});
			}
			
			if (!model.obstacleRepository.ObstacleExists(attribute.ObstacleIdentifier)) {
				model.Add(new Obstacle(model) {
					Identifier = attribute.ObstacleIdentifier,
					Implicit = true
				});
			}
			
			model.Add(exception);
        }
    }
}
