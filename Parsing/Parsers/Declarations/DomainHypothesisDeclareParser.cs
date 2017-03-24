using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using KAOSTools.Parsing.Parsers.Attributes;
using KAOSTools.Parsing.Parsers;

namespace KAOSTools.Parsing.Plugins
{
	public class DomainHypothesisDeclareParser : DeclareParser
	{
		public DomainHypothesisDeclareParser()
		{
			Add(new NameAttributeParser());
			Add(new DefinitionAttributeParser());
			Add(new FormalSpecAttributeParser());
			Add(new CustomAttributeParser());
		}

		public override ParsedElement ParsedDeclare(string identifier, List<dynamic> attributes)
		{
            return new ParsedDomainHypothesis(identifier)
			{
				Attributes = attributes
			};
		}

        public override string GetIdentifier()
		{
			return "(domainhypothesis|domhyp)";
		}
	}
}
