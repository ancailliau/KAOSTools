using System;
using System.Collections.Generic;
using System.Linq;

namespace KAOSTools.Parsing.Parsers.Attributes
{

	public class AgentTypeAttributeParser : IParserAttribute
    {
        public string GetIdentifier()
        {
            return "type";
        }

        public ParsedElement ParsedAttribute(string identifier, NParsedAttributeValue parameters, NParsedAttributeValue value)
        {
			if (parameters != null)
                throw new NotImplementedException("Attribute '"+identifier+"' does not accept parameters.");

			if (!(value is NParsedAttributeAtomic))
				throw new NotImplementedException("Attribute '"+identifier+"' only accept a single atomic value");

			var v = ((NParsedAttributeAtomic)value).Value;

            if (!(v is IdentifierExpression))
				throw new NotImplementedException("Attribute '" + identifier + "' only accept 'software', 'environment' and 'malicious' as value.");

            var v2 = (IdentifierExpression)v;

            switch (v2.Value)
            {
				case "software":
                    return new ParsedAgentTypeAttribute() { Value = ParsedAgentType.Software };

				case "environment":
                    return new ParsedAgentTypeAttribute() { Value = ParsedAgentType.Environment };

				case "malicious":
                    return new ParsedAgentTypeAttribute() { Value = ParsedAgentType.Malicious };

                default:
					throw new NotImplementedException("Attribute '" + identifier + "' only accept 'software', 'environment' and 'malicious' as value.");
            }
				

        }
	}
    
}
