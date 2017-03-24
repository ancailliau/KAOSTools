using System;
using KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;
using KAOSTools.Parsing.Parsers;

namespace KAOSTools.Parsing.Builders.Attributes
{
	public class AttributeAttributeBuilder : AttributeBuilder<Goal, ParsedAttributeAttribute>
    {
		public AttributeAttributeBuilder()
        {
        }

        public override void Handle(Goal element, ParsedAttributeAttribute attribute, KAOSModel model)
		{
			var a = new EntityAttribute(model);
            a.Name = attribute.Name;

            GivenType givenType;
            if ((givenType = model.entityRepository.GetGivenType(attribute.Type)) != null)
            {
                a.TypeIdentifier = givenType.Identifier;
            }
            else
            {
				throw new BuilderException("Type '" + attribute.Type + "' is not defined.",
												   attribute.Filename,
												   attribute.Line,
												   attribute.Col);
            }

            a.EntityIdentifier = element.Identifier;

			model.Add(a);
        }
    }
}
