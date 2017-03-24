using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using KAOSTools.Parsing.Parsers.Attributes;
using KAOSTools.Parsing.Parsers;

namespace KAOSTools.Parsing.Parsers.Declarations
{
	public class ExpertDeclareParser : DeclareParser
	{
		public ExpertDeclareParser()
		{
			Add(new NameAttributeParser());
			Add(new CustomAttributeParser());
		}

		public override ParsedElement ParsedDeclare(string identifier, List<dynamic> attributes)
		{
            return new ParsedExpert(identifier)
			{
				Attributes = attributes
			};
		}

        public override string GetIdentifier()
		{
			return "expert";
		}
	}
}