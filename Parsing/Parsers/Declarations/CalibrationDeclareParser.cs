using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using KAOSTools.Parsing.Parsers.Attributes;

namespace KAOSTools.Parsing.Plugins
{
	public class CalibrationDeclareParser : DeclareParser
	{
		public CalibrationDeclareParser()
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
			return "calibration";
		}
	}
}
