using System;
using KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;

namespace KAOSTools.Parsing.Builders.Attributes
{
	public class CustomAttributeBuilder : AttributeBuilder<KAOSCoreElement, ParsedCustomAttribute>
    {
		public CustomAttributeBuilder()
        {
        }

        public override void Handle(KAOSCoreElement element, ParsedCustomAttribute attribute, KAOSModel model)
		{
            element.CustomData.Add (attribute.Key, attribute.Value);
        }
    }
}
