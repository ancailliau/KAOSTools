using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using KAOSTools.Parsing.Parsers.Attributes;

namespace KAOSTools.Parsing.Plugins
{
	public class GoalDeclareParser : DeclareParser
    {
		string name = "goal";

		public GoalDeclareParser()
        {
			Add(new NameAttributeParser());
			Add(new DefinitionAttributeParser());
            Add(new RefinedByAttributeParser());
			Add(new RSRAttributeParser());
            Add(new ObstructedByAttributeParser());
            Add(new AssignedToAttributeParser());
            Add(new CustomAttributeParser());
        }

		public override ParsedElement ParsedDeclare(string identifier, List<dynamic> attributes)
		{
			return new ParsedGoal(identifier)
			{
				Attributes = attributes
			};
		}

        public override string GetIdentifier()
		{
			return "goal";
		}
    }

    

    
}
