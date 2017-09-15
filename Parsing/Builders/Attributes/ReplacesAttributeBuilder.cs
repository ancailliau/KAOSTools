using System;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;
using UCLouvain.KAOSTools.Parsing.Parsers;

namespace UCLouvain.KAOSTools.Parsing.Builders.Attributes
{
	public class ReplacesAttributeBuilder : AttributeBuilder<Goal, ParsedReplacesAttribute>
    {
		public ReplacesAttributeBuilder()
        {
        }

        public override void Handle(Goal element, ParsedReplacesAttribute attribute, KAOSModel model)
		{
			var goalReplacement = new GoalReplacement(model);
			goalReplacement.AnchorGoalIdentifier = element.Identifier;
			goalReplacement.ResolvingGoalIdentifier = attribute.ReplacedGoalIdentifier;
			goalReplacement.ResolvedObstacleIdentifier = attribute.ObstacleIdentifier;
			model.Add(goalReplacement);
        }
    }
}
