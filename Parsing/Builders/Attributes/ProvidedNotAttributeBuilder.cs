using System;
using System.Collections.Generic;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;
using UCLouvain.KAOSTools.Parsing.Parsers;
using UCLouvain.KAOSTools.Core.Goals;

namespace UCLouvain.KAOSTools.Parsing.Builders.Attributes
{
	public class ProvidedNotAttributeBuilder : AttributeBuilder<Goal, ParsedProvidedNotAttribute>
	{
        FormulaBuilder fb;

        public ProvidedNotAttributeBuilder(FormulaBuilder fb)
        {
            this.fb = fb;
        }

        public override void Handle(Goal element, ParsedProvidedNotAttribute attribute, KAOSModel model)
        {
        	var providedNot = new GoalProvidedNot (model,
        										   element.Identifier, 
        										   attribute.ObstacleIdentifier, 
        										   fb.BuildFormula(attribute.Formula));
			model.Add(providedNot);
        }
    }
}
