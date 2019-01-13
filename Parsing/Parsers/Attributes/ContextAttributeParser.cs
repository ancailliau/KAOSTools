using System;
using System.Collections.Generic;
using System.Linq;
using UCLouvain.KAOSTools.Parsing.Parsers.Exceptions;

namespace UCLouvain.KAOSTools.Parsing.Parsers.Attributes
{

	public class ContextAttributeParser : IParserAttribute
    {
        public string GetIdentifier()
        {
            return "context";
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


            return new ParsedContextAttribute() { Value = ((NParsedAttributeAtomic)value).Value };
				

        }
	}
    
}
