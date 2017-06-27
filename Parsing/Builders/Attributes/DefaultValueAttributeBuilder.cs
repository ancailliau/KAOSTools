using System;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;
using UCLouvain.KAOSTools.Parsing.Parsers;

namespace UCLouvain.KAOSTools.Parsing.Builders.Attributes
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
