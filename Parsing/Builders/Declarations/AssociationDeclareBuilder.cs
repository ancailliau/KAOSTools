using System;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Parsing.Parsers;
namespace UCLouvain.KAOSTools.Parsing.Builders.Declarations
{
	public class AssociationDeclareBuilder : DeclareBuilder
    {
		public AssociationDeclareBuilder()
        {
        }

        public override void BuildDeclare(ParsedDeclare parsedElement, KAOSModel model)
        {
			Relation g = model.entityRepository.GetRelation(parsedElement.Identifier);
			if (g == null)
			{
				g = new Relation(model, parsedElement.Identifier);
				model.entityRepository.Add(g);
			} else if (!parsedElement.Override) {
				throw new BuilderException("Cannot declare twice the same element. Use override instead.", parsedElement);
			}
		}

		public override KAOSCoreElement GetBuiltElement(ParsedDeclare parsedElement, KAOSModel model)
		{
			return model.entityRepository.GetRelation(parsedElement.Identifier);
		}

		public override bool IsBuildable(ParsedDeclare element)
		{
            return element is ParsedAssociation;
		}
    }
}
