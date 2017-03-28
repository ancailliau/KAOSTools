using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using KAOSTools.Parsing.Parsers.Attributes;
using KAOSTools.Parsing.Parsers;

namespace KAOSTools.Parsing.Parsers.Declarations
{
	public class GoalDeclareParser : DeclareParser
    {
		public GoalDeclareParser()
        {
			Add(new NameAttributeParser());
			Add(new DefinitionAttributeParser());
            Add(new RefinedByAttributeParser());
			Add(new RSRAttributeParser());
            Add(new ObstructedByAttributeParser());
			Add(new AssignedToAttributeParser());
            Add(new FormalSpecAttributeParser());
            Add(new CustomAttributeParser());
        }

		public override ParsedElement ParsedDeclare(string identifier, List<dynamic> attributes, bool @override)
		{
			return new ParsedGoal(identifier)
			{
				Attributes = attributes,
                Override = @override
			};
		}

        public override string GetIdentifier()
		{
			return "goal";
		}
    }

    

    
}
