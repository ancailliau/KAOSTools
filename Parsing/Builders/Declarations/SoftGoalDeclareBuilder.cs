﻿using System;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Parsing.Parsers;
namespace UCLouvain.KAOSTools.Parsing.Builders.Declarations
{
    public class SoftGoalDeclareBuilder : DeclareBuilder
    {
        public SoftGoalDeclareBuilder()
        {
        }

        public override void BuildDeclare(ParsedDeclare parsedElement, KAOSModel model)
        {
            SoftGoal g = model.goalRepository.GetSoftGoal(parsedElement.Identifier);
            if (g == null) {
                g = new SoftGoal(model, parsedElement.Identifier);
                model.goalRepository.Add(g);
            } else if (!parsedElement.Override) {
                throw new BuilderException("Cannot declare twice the same element. Use override instead.", parsedElement);
            }
        }

        public override KAOSCoreElement GetBuiltElement(ParsedDeclare parsedElement, KAOSModel model)
        {
            return model.goalRepository.GetSoftGoal(parsedElement.Identifier);
        }

        public override bool IsBuildable(ParsedDeclare element)
        {
            return element is ParsedSoftGoal;
        }
    }
}
