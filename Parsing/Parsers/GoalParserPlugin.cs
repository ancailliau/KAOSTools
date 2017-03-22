using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using KAOSTools.Parsing.Parsers.Attributes;

namespace KAOSTools.Parsing.Plugins
{
    public class GoalParserPlugin : ParserPlugin
    {
		string name = "goal";

        public GoalParserPlugin()
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

		protected override string GetName()
		{
			return "goal";
		}
    }

    

    
}
