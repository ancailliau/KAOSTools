using System;
using KAOSTools.Core;
namespace KAOSTools.Parsing.Builders.Declarations
{
	public class DomainPropertyDeclareBuilder : DeclareBuilder
    {
		public DomainPropertyDeclareBuilder()
        {
        }

        public override void BuildDeclare(ParsedDeclare parsedElement, KAOSModel model)
        {
			DomainProperty g = model.domainRepository.GetDomainProperty(parsedElement.Identifier);
			if (g == null)
			{
				g = new DomainProperty(model, parsedElement.Identifier);
				model.domainRepository.Add(g);
			}
		}

		public override KAOSCoreElement GetBuiltElement(Parsing.ParsedDeclare parsedElement, KAOSModel model)
		{
			return model.domainRepository.GetDomainProperty(parsedElement.Identifier);
		}

		public override bool IsBuildable(ParsedDeclare element)
		{
            return element is ParsedDomainProperty;
		}
    }
}
