using System;
using System.Collections.Generic;
using System.Linq;
using UCLouvain.KAOSTools.Parsing.Parsers.Exceptions;

namespace KAOSTools.Parsing.Parsers.Attributes
{

	public class ObstructedByAttributeParser : IParserAttribute
    {
        public string GetIdentifier()
        {
            return "obstructed[bB]y";
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

            return new ParsedObstructedByAttribute() { Value = ((NParsedAttributeAtomic)value).Value };
        }
	}
    
}
