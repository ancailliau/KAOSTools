using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UCLouvain.KAOSTools.Parsing.Parsers.Attributes;
using UCLouvain.KAOSTools.Parsing.Parsers;

namespace UCLouvain.KAOSTools.Parsing.Parsers.Declarations
{
	public class ContextDeclareParser : DeclareParser
	{
		public ContextDeclareParser()
		{
			Add(new NameAttributeParser());
			Add(new DefinitionAttributeParser());
			Add(new CustomAttributeParser());
		}

		public override ParsedElement ParsedDeclare(string identifier, List<dynamic> attributes, bool @override)
		{
			return new ParsedContext(identifier)
			{
				Attributes = attributes,
                Override = @override
			};
		}

        public override string GetIdentifier()
		{
			return "context";
		}
	}
}
