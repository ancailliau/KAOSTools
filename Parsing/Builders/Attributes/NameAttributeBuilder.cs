﻿using System;
using System.Text.RegularExpressions;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;
using UCLouvain.KAOSTools.Parsing.Parsers;

namespace UCLouvain.KAOSTools.Parsing.Builders.Attributes
{
	public class NameAttributeBuilder : AttributeBuilder<KAOSCoreElement, ParsedNameAttribute>
    {
		public NameAttributeBuilder()
        {
        }

		public override void Handle(KAOSCoreElement element, ParsedNameAttribute attribute, KAOSModel model)
		{
            Handle(element, Sanitize(attribute.Value), "Name");
		}

		protected string Sanitize(string text)
		{
			var t = Regex.Replace(text, @"\s+", " ", RegexOptions.Multiline).Trim();
			t = Regex.Replace(t, "\"\"", "\"", RegexOptions.Multiline);
			return t;
		}
    }
}
