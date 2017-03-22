using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using KAOSTools.Parsing.Parsers.Attributes;

namespace KAOSTools.Parsing.Plugins
{
	public class AgentParserPlugin : ParserPlugin
	{
		public AgentParserPlugin()
		{
			Add(new NameAttributeParser());
			Add(new DefinitionAttributeParser());
			Add(new CustomAttributeParser());
		}

		public override ParsedElement ParsedDeclare(string identifier, List<dynamic> attributes)
		{
			return new ParsedAgent(identifier)
			{
				Attributes = attributes
			};
		}

		protected override string GetName()
		{
			return "agent";
		}
	}
}
