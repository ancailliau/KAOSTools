using System;
using System.Text.RegularExpressions;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;
using UCLouvain.KAOSTools.Parsing.Parsers;

namespace UCLouvain.KAOSTools.Parsing.Builders.Attributes
{
	public class LinkAttributeBuilder : AttributeBuilder<Relation, ParsedLinkAttribute> 
    {
		public LinkAttributeBuilder()
        {
        }

        public override void Handle(Relation element, ParsedLinkAttribute attribute, KAOSModel model)
		{
			Entity entity;
			if (attribute.Target is IdentifierExpression)
			{
				string id = ((IdentifierExpression)attribute.Target).Value;
                if ((entity = model.entityRepository.GetEntity(id)) == null) {
                    entity = new Entity(model, id) { Implicit = true };
                    model.entityRepository.Add(entity);
                }

                element.Links.Add(new Link(model) { Target = entity, Multiplicity = attribute.Multiplicity });
            }
			else
			{
                throw new UnsupportedValue(element, attribute, attribute.Target);
			}
        }
    }
}
