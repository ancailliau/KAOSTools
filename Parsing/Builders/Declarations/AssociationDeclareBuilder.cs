using System;
using KAOSTools.Core;
using KAOSTools.Parsing.Parsers;
namespace KAOSTools.Parsing.Builders.Declarations
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
