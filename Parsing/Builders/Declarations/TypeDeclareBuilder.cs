using System;
using KAOSTools.Core;
namespace KAOSTools.Parsing.Builders.Declarations
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
			}
		}

		public override KAOSCoreElement GetBuiltElement(Parsing.ParsedDeclare parsedElement, KAOSModel model)
		{
			return model.entityRepository.GetGivenType(parsedElement.Identifier);
		}

		public override bool IsBuildable(ParsedDeclare element)
		{
            return element is ParsedGivenType;
		}
    }
}
