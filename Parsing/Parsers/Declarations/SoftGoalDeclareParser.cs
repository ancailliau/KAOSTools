using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using KAOSTools.Parsing.Parsers.Attributes;
using KAOSTools.Parsing.Parsers;

namespace KAOSTools.Parsing.Parsers.Declarations
{
    public class SoftGoalDeclareParser : DeclareParser
    {
		public SoftGoalDeclareParser()
        {
			Add(new NameAttributeParser());
			Add(new DefinitionAttributeParser());
            Add(new CustomAttributeParser());
        }

		public override ParsedElement ParsedDeclare(string identifier, List<dynamic> attributes)
		{
            return new ParsedSoftGoal(identifier)
			{
				Attributes = attributes
			};
		}

        public override string GetIdentifier()
		{
			return "softgoal";
		}
    }

    

    
}
