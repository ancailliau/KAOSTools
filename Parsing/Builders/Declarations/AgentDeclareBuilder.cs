using System;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;
using UCLouvain.KAOSTools.Parsing.Parsers;

namespace UCLouvain.KAOSTools.Parsing.Builders.Declarations
{
    public class AgentDeclareBuilder : DeclareBuilder
    {
        public AgentDeclareBuilder()
        {
        }

        public override void BuildDeclare(ParsedDeclare parsedElement, KAOSModel model)
        {
			Agent g = model.agentRepository.GetAgent(parsedElement.Identifier);
            if (g == null) {
                g = new Agent(model, parsedElement.Identifier);
                model.agentRepository.Add(g);
            } else if (!parsedElement.Override) {
                throw new BuilderException("Cannot declare twice the same element. Use override instead.", parsedElement);
            }
        }

        public override KAOSCoreElement GetBuiltElement(ParsedDeclare parsedElement, KAOSModel model)
        {
			return model.agentRepository.GetAgent(parsedElement.Identifier);
        }

        public override bool IsBuildable(ParsedDeclare element)
		{
			return element is ParsedAgent;
        }
    }
}
