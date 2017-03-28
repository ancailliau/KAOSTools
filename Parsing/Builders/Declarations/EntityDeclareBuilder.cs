using System;
using KAOSTools.Core;
using KAOSTools.Parsing.Parsers;
namespace KAOSTools.Parsing.Builders.Declarations
{
	public class EntityDeclareBuilder : DeclareBuilder
    {
		public EntityDeclareBuilder()
        {
        }

        public override void BuildDeclare(ParsedDeclare parsedElement, KAOSModel model)
        {
			Entity g = model.entityRepository.GetEntity(parsedElement.Identifier);
			if (g == null)
			{
				g = new Entity(model, parsedElement.Identifier);
				model.entityRepository.Add(g);
			} else if (!parsedElement.Override) {
				throw new BuilderException("Cannot declare twice the same element. Use override instead.", parsedElement);
			}
		}

		public override KAOSCoreElement GetBuiltElement(ParsedDeclare parsedElement, KAOSModel model)
		{
			return model.entityRepository.GetEntity(parsedElement.Identifier);
		}

		public override bool IsBuildable(ParsedDeclare element)
		{
            return element is ParsedEntity;
		}
    }
}
