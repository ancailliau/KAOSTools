using System;
using KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;
using KAOSTools.Parsing.Parsers;

namespace KAOSTools.Parsing.Builders.Attributes
{
	public class DefaultValueAttributeBuilder : AttributeBuilder<Predicate, DefaultValueAttribute>
    {
		public DefaultValueAttributeBuilder()
        {
        }

		public override void Handle(Predicate predicate, DefaultValueAttribute attribute, KAOSModel model)
		{
			predicate.DefaultValue = attribute.Value;
        }
    }
}
