using System;
using System.Text.RegularExpressions;
using KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;

namespace KAOSTools.Parsing.Builders.Attributes
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

				goal = model.goalRepository.GetGoal(id);

			}
			else
			{
				throw new NotImplementedException(string.Format("'{0}' is not supported in '{1}' on '{2}'",
																  attribute.Value.GetType().Name,
																  attribute.GetType().Name,
																  element.GetType().Name));
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

				// TODO WTF is that doing???
				//foreach (var parameter in resolvedBy.Pattern.Parameters) {
				//    DomainHypothesis hypothesis;
				//    if (!Get (parameter, out hypothesis)) {
				//        Goal goalAsParameter;
				//        if (!Get (parameter, out goalAsParameter)) {
				//            goalAsParameter = Create<Goal> (parameter);
				//        }
				//        resolution.Parameters.Add (goalAsParameter);
				//    } else {
				//        resolution.Parameters.Add (hypothesis);
				//    }
				//}
			}

			model.Add(resolution);
        }
    }
}
