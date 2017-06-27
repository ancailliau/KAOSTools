using System;
using System.Text.RegularExpressions;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;
using UCLouvain.KAOSTools.Parsing.Parsers;

namespace UCLouvain.KAOSTools.Parsing.Builders.Attributes
{
	public class ResolvedByAttributeBuilder : AttributeBuilder<Obstacle, ParsedResolvedByAttribute>
    {
		public ResolvedByAttributeBuilder()
        {
        }

        public override void Handle(Obstacle element, ParsedResolvedByAttribute attribute, KAOSModel model)
		{
            Goal goal;
			if (attribute.Value is IdentifierExpression)
			{
				var id = ((IdentifierExpression)attribute.Value).Value;
                if ((goal = model.goalRepository.GetGoal(id)) == null) {
                    goal = new Goal(model, id) { Implicit = true };
                    model.goalRepository.Add(goal);
                }
			}
			else
			{
                throw new UnsupportedValue(element, attribute, attribute.Value);
			}

			var resolution = new Resolution(model);
            resolution.SetObstacle(element);
			resolution.SetResolvingGoal(goal);

			if (attribute.Pattern != null)
			{
				if (attribute.Pattern.Name == "substitution")
					resolution.ResolutionPattern = ResolutionPattern.GoalSubstitution;

				else if (attribute.Pattern.Name == "prevention")
					resolution.ResolutionPattern = ResolutionPattern.ObstaclePrevention;

				else if (attribute.Pattern.Name == "obstacle_reduction")
					resolution.ResolutionPattern = ResolutionPattern.ObstacleReduction;

				else if (attribute.Pattern.Name == "restoration")
					resolution.ResolutionPattern = ResolutionPattern.GoalRestoration;

				else if (attribute.Pattern.Name == "weakening")
					resolution.ResolutionPattern = ResolutionPattern.GoalWeakening;

				else if (attribute.Pattern.Name == "mitigation")
					resolution.ResolutionPattern = ResolutionPattern.ObstacleMitigation;

				else if (attribute.Pattern.Name == "weak_mitigation")
					resolution.ResolutionPattern = ResolutionPattern.ObstacleWeakMitigation;

				else if (attribute.Pattern.Name == "strong_mitigation")
					resolution.ResolutionPattern = ResolutionPattern.ObstacleStrongMitigation;

				else
					throw new NotImplementedException();
			}

            if (attribute.AnchorId != null) {
                Goal anchor;
                if ((anchor = model.goalRepository.GetGoal(attribute.AnchorId)) == null) {
                    anchor = new Goal(model, attribute.AnchorId) { Implicit = true };
                    model.goalRepository.Add(anchor);
                }
                resolution.AnchorIdentifier = attribute.AnchorId;
            }

			model.Add(resolution);
        }
    }
}
