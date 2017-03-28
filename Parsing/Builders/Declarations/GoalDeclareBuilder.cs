﻿using System;
using KAOSTools.Core;
using KAOSTools.Parsing.Parsers;
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
			} else if (!parsedElement.Override) {
				throw new BuilderException("Cannot declare twice the same element. Use override instead.", parsedElement);
			}
		}

		public override KAOSCoreElement GetBuiltElement(ParsedDeclare parsedElement, KAOSModel model)
		{
			return model.goalRepository.GetGoal(parsedElement.Identifier);
		}

		public override bool IsBuildable(ParsedDeclare element)
		{
            return element is ParsedGoal;
		}
    }
}
