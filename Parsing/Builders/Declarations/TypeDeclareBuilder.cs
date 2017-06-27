using System;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Parsing.Parsers;
namespace UCLouvain.KAOSTools.Parsing.Builders.Declarations
{
	public class TypeDeclareBuilder : DeclareBuilder
    {
		public TypeDeclareBuilder()
        {
        }

        public override void BuildDeclare(ParsedDeclare parsedElement, KAOSModel model)
        {
			GivenType g = model.entityRepository.GetGivenType(parsedElement.Identifier);
			if (g == null)
			{
				g = new GivenType(model, parsedElement.Identifier);
				model.entityRepository.Add(g);
			} else if (!parsedElement.Override) {
				throw new BuilderException("Cannot declare twice the same element. Use override instead.", parsedElement);
			}
		}

		public override KAOSCoreElement GetBuiltElement(ParsedDeclare parsedElement, KAOSModel model)
		{
			return model.entityRepository.GetGivenType(parsedElement.Identifier);
		}

		public override bool IsBuildable(ParsedDeclare element)
		{
            return element is ParsedGivenType;
		}
    }
}
