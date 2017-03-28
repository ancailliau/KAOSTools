using System;
using KAOSTools.Core;
using KAOSTools.Parsing.Parsers;
namespace KAOSTools.Parsing.Builders.Declarations
{
	public class PredicateDeclareBuilder : DeclareBuilder
    {
		public PredicateDeclareBuilder()
        {
        }

        public override void BuildDeclare(ParsedDeclare parsedElement, KAOSModel model)
        {
			Predicate g = model.formalSpecRepository.GetPredicate(parsedElement.Identifier);
			if (g == null)
			{
				g = new Predicate(model, parsedElement.Identifier);
				model.formalSpecRepository.Add(g);
			} else if (!parsedElement.Override) {
				throw new BuilderException("Cannot declare twice the same element. Use override instead.", parsedElement);
			}
		}

		public override KAOSCoreElement GetBuiltElement(ParsedDeclare parsedElement, KAOSModel model)
		{
			return model.formalSpecRepository.GetPredicate(parsedElement.Identifier);
		}

		public override bool IsBuildable(ParsedDeclare element)
		{
            return element is ParsedPredicate;
		}
    }
}
