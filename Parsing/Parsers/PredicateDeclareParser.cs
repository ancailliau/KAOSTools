using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using KAOSTools.Parsing.Parsers.Attributes;

namespace KAOSTools.Parsing.Plugins
{
	public class PredicateDeclareParser : DeclareParser
    {
		public PredicateDeclareParser()
        {
			Add(new NameAttributeParser());
			Add(new DefinitionAttributeParser());
			Add(new FormalSpecAttributeParser());
            Add(new ArgumentAttributeParser());
            Add(new DefaultValueAttributeParser());
            Add(new CustomAttributeParser());
        }

		public override ParsedElement ParsedDeclare(string identifier, List<dynamic> attributes)
		{
            return new ParsedPredicate(identifier)
			{
				Attributes = attributes
			};
		}

        public override string GetIdentifier()
		{
			return "predicate";
		}
    }

    

    
}
