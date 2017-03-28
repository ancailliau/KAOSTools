using System;
using KAOSTools.Core;
using KAOSTools.Parsing.Parsers;
namespace KAOSTools.Parsing.Builders.Declarations
{
	public class DomainHypothesisDeclareBuilder : DeclareBuilder
    {
		public DomainHypothesisDeclareBuilder()
        {
        }

        public override void BuildDeclare(ParsedDeclare parsedElement, KAOSModel model)
        {
			DomainHypothesis g = model.domainRepository.GetDomainHypothesis(parsedElement.Identifier);
			if (g == null)
			{
				g = new DomainHypothesis(model, parsedElement.Identifier);
				model.domainRepository.Add(g);
			} else if (!parsedElement.Override) {
				throw new BuilderException("Cannot declare twice the same element. Use override instead.", parsedElement);
			}
		}

		public override KAOSCoreElement GetBuiltElement(ParsedDeclare parsedElement, KAOSModel model)
		{
			return model.domainRepository.GetDomainHypothesis(parsedElement.Identifier);
		}

		public override bool IsBuildable(ParsedDeclare element)
		{
            return element is ParsedDomainHypothesis;
		}
    }
}
