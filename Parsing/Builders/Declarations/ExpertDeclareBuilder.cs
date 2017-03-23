using System;
using KAOSTools.Core;
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
			}
		}

		public override KAOSCoreElement GetBuiltElement(Parsing.ParsedDeclare parsedElement, KAOSModel model)
		{
			return model.modelMetadataRepository.GetExpert(parsedElement.Identifier);
		}

		public override bool IsBuildable(ParsedDeclare element)
		{
            return element is ParsedExpert;
		}
    }
}
