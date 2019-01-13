using System;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;
using UCLouvain.KAOSTools.Parsing.Parsers;
using UCLouvain.KAOSTools.Core.Model;

namespace UCLouvain.KAOSTools.Parsing.Builders.Declarations
{
    public class ContextDeclareBuilder : DeclareBuilder
    {
        public ContextDeclareBuilder()
        {
        }

        public override void BuildDeclare(ParsedDeclare parsedElement, KAOSModel model)
        {
			Context context = model.modelMetadataRepository.GetContext(parsedElement.Identifier);
            if (context == null) {
                context = new Context(model, parsedElement.Identifier);
                model.modelMetadataRepository.Add(context);
                
            } else if (!parsedElement.Override) {
                throw new BuilderException("Cannot declare twice the same element. Use override instead.", parsedElement);
            }
        }

        public override KAOSCoreElement GetBuiltElement(ParsedDeclare parsedElement, KAOSModel model)
        {
			return model.modelMetadataRepository.GetContext(parsedElement.Identifier);
        }

        public override bool IsBuildable(ParsedDeclare element)
		{
			return element is ParsedContext;
        }
    }
}
