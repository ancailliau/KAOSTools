using System;
using System.Text.RegularExpressions;
using KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;

namespace KAOSTools.Parsing.Builders.Attributes
{
	public class DefinitionAttributeBuilder : AttributeBuilder<KAOSCoreElement, ParsedDefinitionAttribute>
    {
		public DefinitionAttributeBuilder()
        {
        }

		public override void Handle(KAOSCoreElement element, ParsedDefinitionAttribute attribute, KAOSModel model)
		{
			Handle(element, attribute.Value.Verbatim ? attribute.Value.Value : Sanitize(attribute.Value.Value), "Definition");
        }

		protected string Sanitize(string text)
		{
			var t = Regex.Replace(text, @"\s+", " ", RegexOptions.Multiline).Trim();
			t = Regex.Replace(t, "\"\"", "\"", RegexOptions.Multiline);
			return t;
		}
    }
}
