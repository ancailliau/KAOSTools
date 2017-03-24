using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using KAOSTools.Parsing.Parsers.Attributes;
using KAOSTools.Parsing.Parsers;

namespace KAOSTools.Parsing.Plugins
{
	public class AssociationDeclareParser : DeclareParser
	{
		public AssociationDeclareParser()
		{
			Add(new NameAttributeParser());
			Add(new DefinitionAttributeParser());
			Add(new AttributeAttributeParser());
            Add(new LinkAttributeParser());
			Add(new CustomAttributeParser());
		}

		public override ParsedElement ParsedDeclare(string identifier, List<dynamic> attributes)
		{
            return new ParsedAssociation(identifier)
			{
				Attributes = attributes
			};
		}

        public override string GetIdentifier()
		{
			return "association";
		}
	}
}
