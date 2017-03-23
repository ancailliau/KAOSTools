using System;
using KAOSTools.Core;
namespace KAOSTools.Parsing.Builders.Declarations
{
	public class SoftGoalDeclareParser : DeclareBuilder
    {
		public SoftGoalDeclareParser()
        {
        }

        public override void BuildDeclare(ParsedDeclare parsedElement, KAOSModel model)
        {
			SoftGoal g = model.goalRepository.GetSoftGoal(parsedElement.Identifier);
			if (g == null)
			{
				g = new SoftGoal(model, parsedElement.Identifier);
				model.goalRepository.Add(g);
			}
		}

		public override KAOSCoreElement GetBuiltElement(Parsing.ParsedDeclare parsedElement, KAOSModel model)
		{
			return model.goalRepository.GetSoftGoal(parsedElement.Identifier);
		}

		public override bool IsBuildable(ParsedDeclare element)
		{
            return element is ParsedSoftGoal;
		}
    }
}
