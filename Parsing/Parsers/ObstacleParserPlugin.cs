using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using KAOSTools.Parsing.Parsers.Attributes;

namespace KAOSTools.Parsing.Plugins
{
	public class ObstacleParserPlugin : ParserPlugin
	{
		public ObstacleParserPlugin()
		{
			Add(new NameAttributeParser());
			Add(new DefinitionAttributeParser());
			Add(new CustomAttributeParser());
		}

		public override ParsedElement ParsedDeclare(string identifier, List<dynamic> attributes)
		{
			return new ParsedObstacle(identifier)
			{
				Attributes = attributes
			};
		}

		protected override string GetName()
		{
			return "obstacle";
		}
	}
}
