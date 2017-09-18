using System;
using System.Collections.Generic;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;
using UCLouvain.KAOSTools.Parsing.Parsers;
using UCLouvain.KAOSTools.Core.Goals;

namespace UCLouvain.KAOSTools.Parsing.Builders.Attributes
{
	public class RelaxedToAttributeBuilder : AttributeBuilder<Goal, ParsedRelaxedToAttribute>
	{
        FormulaBuilder fb;

        public RelaxedToAttributeBuilder(FormulaBuilder fb)
        {
            this.fb = fb;
        }

        public override void Handle(Goal element, ParsedRelaxedToAttribute attribute, KAOSModel model)
        {
        	var relaxedTo = new GoalRelaxedTo (model,
        										   element.Identifier, 
        										   attribute.ObstacleIdentifier, 
        										   fb.BuildFormula(attribute.Formula));
			
			if (!model.obstacleRepository.ObstacleExists(attribute.ObstacleIdentifier)) {
				model.Add(new Obstacle(model) {
					Identifier = attribute.ObstacleIdentifier,
					Implicit = true
				});
			}
			model.Add(relaxedTo);
        }
    }
}
