using System;
using KAOSTools.Core;
namespace KAOSTools.Parsing.Builders.Declarations
{
	public class GoalDeclareBuilder : DeclareBuilder
    {
		public GoalDeclareBuilder()
        {
        }

		public override void BuildDeclare(ParsedDeclare parsedElement, KAOSModel model)
        {
			Goal g = model.goalRepository.GetGoal(parsedElement.Identifier);
			if (g == null)
			{
				g = new Goal(model, parsedElement.Identifier);
				model.goalRepository.Add(g);
			}
		}

		public override KAOSCoreElement GetBuiltElement(Parsing.ParsedDeclare parsedElement, KAOSModel model)
		{
			return model.goalRepository.GetGoal(parsedElement.Identifier);
		}

		public override bool IsBuildable(ParsedDeclare element)
		{
            return element is ParsedGoal;
		}
    }
}
