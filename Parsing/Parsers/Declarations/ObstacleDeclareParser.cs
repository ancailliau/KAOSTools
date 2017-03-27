using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using KAOSTools.Parsing.Parsers.Attributes;
using KAOSTools.Parsing.Parsers;

namespace KAOSTools.Parsing.Parsers.Declarations
{
	public class ObstacleDeclareParser : DeclareParser
	{
		public ObstacleDeclareParser()
		{
			Add(new NameAttributeParser());
			Add(new DefinitionAttributeParser());
            Add(new RefinedByAttributeParser());
			Add(new FormalSpecAttributeParser());
			Add(new ResolvedByAttributeParser());
            Add(new ESRAttributeParser());
			Add(new CustomAttributeParser());
		}

		public override ParsedElement ParsedDeclare(string identifier, List<dynamic> attributes)
		{
			return new ParsedObstacle(identifier)
			{
				Attributes = attributes
			};
		}

        public override string GetIdentifier()
		{
			return "obstacle";
		}
	}
}
