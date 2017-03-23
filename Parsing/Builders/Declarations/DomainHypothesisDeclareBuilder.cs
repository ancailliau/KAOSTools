using System;
using KAOSTools.Core;
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
			}
		}

		public override KAOSCoreElement GetBuiltElement(Parsing.ParsedDeclare parsedElement, KAOSModel model)
		{
			return model.domainRepository.GetDomainHypothesis(parsedElement.Identifier);
		}

		public override bool IsBuildable(ParsedDeclare element)
		{
            return element is ParsedDomainHypothesis;
		}
    }
}
