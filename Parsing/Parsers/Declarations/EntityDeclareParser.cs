using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using KAOSTools.Parsing.Parsers.Attributes;
using KAOSTools.Parsing.Parsers;

namespace KAOSTools.Parsing.Parsers.Declarations
{
	public class EntityDeclareParser : DeclareParser
	{
		public EntityDeclareParser()
		{
			Add(new NameAttributeParser());
			Add(new DefinitionAttributeParser());
            Add(new EntityTypeAttributeParser());
            Add(new IsAAttributeParser());
            Add(new AttributeAttributeParser());
			Add(new CustomAttributeParser());
		}

		public override ParsedElement ParsedDeclare(string identifier, List<dynamic> attributes)
		{
            return new ParsedEntity(identifier)
			{
				Attributes = attributes
			};
		}

        public override string GetIdentifier()
		{
			return "entity";
		}
	}
}
