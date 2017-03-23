using System;
using KAOSTools.Core;
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
			}
		}

		public override KAOSCoreElement GetBuiltElement(Parsing.ParsedDeclare parsedElement, KAOSModel model)
		{
			return model.entityRepository.GetEntity(parsedElement.Identifier);
		}

		public override bool IsBuildable(ParsedDeclare element)
		{
            return element is ParsedEntity;
		}
    }
}
