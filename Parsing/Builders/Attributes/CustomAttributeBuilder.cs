using System;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;
using UCLouvain.KAOSTools.Parsing.Parsers;

namespace UCLouvain.KAOSTools.Parsing.Builders.Attributes
{
	public class CustomAttributeBuilder : AttributeBuilder<KAOSCoreElement, ParsedCustomAttribute>
    {
		public CustomAttributeBuilder()
        {
        }

        public override void Handle(KAOSCoreElement element, ParsedCustomAttribute attribute, KAOSModel model)
		{
            element.CustomData.Add (attribute.Key.Substring (1), attribute.Value);
        }
    }
}
