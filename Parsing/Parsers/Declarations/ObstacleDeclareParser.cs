using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UCLouvain.KAOSTools.Parsing.Parsers.Attributes;
using UCLouvain.KAOSTools.Parsing.Parsers;

namespace UCLouvain.KAOSTools.Parsing.Parsers.Declarations
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

		public override ParsedElement ParsedDeclare(string identifier, List<dynamic> attributes, bool @override)
		{
			return new ParsedObstacle(identifier)
			{
				Attributes = attributes,
                Override = @override
			};
		}

        public override string GetIdentifier()
		{
			return "obstacle";
		}
	}
}
