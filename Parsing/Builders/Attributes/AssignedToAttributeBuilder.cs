using System;
using KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;

namespace KAOSTools.Parsing.Builders.Attributes
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
                    if ((agent = model.agentRepository.GetAgent(identifier)) != null)
                    {
                        assignment.Add(agent);
                    }
                    else
                    {
                        throw new BuilderException("Agent '" + identifier + "' is not defined", 
                                                   attribute.Filename, 
                                                   attribute.Line, 
                                                   attribute.Col);
                    }
                }
                else
                {
					throw new NotImplementedException(string.Format("'{0}' is not supported in '{1}' on '{2}'", 
                                                                    child.GetType().Name, 
                                                                    attribute.GetType().Name, 
                                                                    element.GetType().Name));
                }
			}

			if (!assignment.IsEmpty)
				model.Add(assignment);
        }
    }
}
