using System;
using System.Collections.Generic;
using System.Linq;
using UCLouvain.KAOSTools.Parsing.Parsers.Exceptions;

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
                throw new InvalidParameterAttributeException (identifier,
                                                              InvalidParameterAttributeException.NO_PARAM);

            if (!(value is NParsedAttributeAtomic))
                throw new InvalidAttributeValueException (identifier,
                                                          InvalidAttributeValueException.ATOMIC_ONLY);

			var v = ((NParsedAttributeAtomic)value).Value;

            if (!(v is IdentifierExpression))
                throw new InvalidAttributeValueException (identifier,
                                                          InvalidAttributeValueException.IDENTIFIER);

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
                throw new InvalidAttributeValueException (identifier,
                                                          InvalidAttributeValueException.INVALID_VALUE);
            }
				

        }
	}
    
}
