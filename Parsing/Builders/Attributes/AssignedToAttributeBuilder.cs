﻿using System;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;
using UCLouvain.KAOSTools.Parsing.Parsers;

namespace UCLouvain.KAOSTools.Parsing.Builders.Attributes
{
    public class AssignedToAttributeBuilder : AttributeBuilder<Goal, ParsedAssignedToAttribute>
    {
		public AssignedToAttributeBuilder()
        {
        }

        public override void Handle(Goal element, ParsedAssignedToAttribute attribute, KAOSModel model)
		{
			var assignment = new GoalAgentAssignment(model);
			assignment.GoalIdentifier = element.Identifier;

			foreach (var child in attribute.Values)
			{
                if (child is IdentifierExpression)
                {
                    var identifier = ((IdentifierExpression)child).Value;

                    Agent agent;
                    if ((agent = model.agentRepository.GetAgent(identifier)) == null)
                    {
                        agent = new Agent(model, identifier) { Implicit = true };
                        model.agentRepository.Add(agent);
                    }

                    assignment.Add(agent);
                }
                else
                {
                    throw new NotImplementedException(string.Format("'{0}' is not supported in '{1}' on '{2}'. ({3},{4})", 
                                                                    child.GetType().Name, 
                                                                    attribute.GetType().Name, 
                                                                    element.GetType().Name, attribute.Line, attribute.Col));
                }
			}

			if (!assignment.IsEmpty)
				model.Add(assignment);
        }
    }
}
