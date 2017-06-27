using System;
using System.Text.RegularExpressions;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;
using UCLouvain.KAOSTools.Parsing.Parsers;

namespace UCLouvain.KAOSTools.Parsing.Builders.Attributes
{
	public class IsAAttributeBuilder : AttributeBuilder<Entity, ParsedIsAAttribute>
    {
		public IsAAttributeBuilder()
        {
        }

	    public override void Handle(Entity element, ParsedIsAAttribute attribute, KAOSModel model)
		{
			if (attribute.Value is IdentifierExpression)
			{
                string identifier = ((IdentifierExpression)attribute.Value).Value;
				
                Entity entity;
                if ((entity = model.entityRepository.GetEntity(identifier)) == null) {
                    entity = new Entity(model, identifier) { Implicit = true };
                    model.entityRepository.Add(entity);
                }

                element.AddParent(entity);
            }
			else
			{
                throw new UnsupportedValue(element, attribute, attribute.Value);
			}
        }
    }
}
