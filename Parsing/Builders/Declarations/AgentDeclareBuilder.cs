using System;
using KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;

namespace KAOSTools.Parsing.Builders.Declarations
{
    public class AgentDeclareBuilder : DeclareBuilder
    {
        public AgentDeclareBuilder()
        {
        }

        public override void BuildDeclare(ParsedDeclare parsedElement, KAOSModel model)
        {
			Agent g = model.agentRepository.GetAgent(parsedElement.Identifier);
			if (g == null)
			{
				g = new Agent(model, parsedElement.Identifier);
				model.agentRepository.Add(g);
			}
        }

        public override KAOSCoreElement GetBuiltElement(Parsing.ParsedDeclare parsedElement, KAOSModel model)
        {
			return model.agentRepository.GetAgent(parsedElement.Identifier);
        }

        public override bool IsBuildable(ParsedDeclare element)
		{
			return element is ParsedAgent;
        }
    }
}
