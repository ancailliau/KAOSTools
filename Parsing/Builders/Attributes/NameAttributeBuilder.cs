using System;
using System.Text.RegularExpressions;
using KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;

namespace KAOSTools.Parsing.Builders.Attributes
{
	public class NameAttributeBuilder : AttributeBuilder<KAOSCoreElement, ParsedNameAttribute>
    {
		public NameAttributeBuilder()
        {
        }

		public override void Handle(KAOSCoreElement element, ParsedNameAttribute attribute, KAOSModel model)
		{
			Handle(element, attribute.Value, "Name");
        }
    }
}
