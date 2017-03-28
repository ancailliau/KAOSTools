using System;
using KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;
using KAOSTools.Parsing.Parsers;

namespace KAOSTools.Parsing.Builders.Attributes
{
	public class AttributeAttributeBuilder : AttributeBuilder<Entity, ParsedAttributeAttribute>
    {
		public AttributeAttributeBuilder()
        {
        }

        public override void Handle(Entity element, ParsedAttributeAttribute attribute, KAOSModel model)
		{
			var a = new EntityAttribute(model);
			a.Identifier = attribute.Identifier;

            GivenType givenType;
            if (attribute.Type == null) {
                // No given type was specified
                givenType = null;

            } else if (attribute.Type is IdentifierExpression) {
                var identifier = ((IdentifierExpression)attribute.Type).Value;
                if ((givenType = model.entityRepository.GetGivenType(identifier)) == null) {
                    givenType = new GivenType(model, identifier) { Implicit = true };
                    model.entityRepository.Add(givenType);
                }

            } else {
                throw new UnsupportedValue(element, attribute, attribute.Type);
            }

            a.TypeIdentifier = givenType?.Identifier;
            a.EntityIdentifier = element.Identifier;

			model.Add(a);
        }
    }
}
