using System;
using KAOSTools.Core;
using KAOSTools.Parsing.Parsers;
namespace KAOSTools.Parsing.Builders.Declarations
{
	public class ExpertDeclareBuilder : DeclareBuilder
    {
		public ExpertDeclareBuilder()
        {
        }

        public override void BuildDeclare(ParsedDeclare parsedElement, KAOSModel model)
        {
			Expert g = model.modelMetadataRepository.GetExpert(parsedElement.Identifier);
			if (g == null)
			{
				g = new Expert(model, parsedElement.Identifier);
				model.modelMetadataRepository.Add(g);
			} else if (!parsedElement.Override) {
				throw new BuilderException("Cannot declare twice the same element. Use override instead.", parsedElement);
			}
		}

		public override KAOSCoreElement GetBuiltElement(ParsedDeclare parsedElement, KAOSModel model)
		{
			return model.modelMetadataRepository.GetExpert(parsedElement.Identifier);
		}

		public override bool IsBuildable(ParsedDeclare element)
		{
            return element is ParsedExpert;
		}
    }
}
