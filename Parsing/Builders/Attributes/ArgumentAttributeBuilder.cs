using System;
using KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;

namespace KAOSTools.Parsing.Builders.Attributes
{
	public class ArgumentAttributeBuilder : AttributeBuilder<KAOSCoreElement, ParsedPredicateArgumentAttribute>
    {
		public ArgumentAttributeBuilder()
        {
        }

        public override void Handle(KAOSCoreElement element, ParsedPredicateArgumentAttribute parsedElement, KAOSModel model)
        {
            // Ignore
            // TODO is it then required?
        }
    }
}
