using System;
using System.Collections.Generic;
using System.Linq;
using UCLouvain.KAOSTools.Parsing.Parsers.Exceptions;

namespace UCLouvain.KAOSTools.Parsing.Parsers.Attributes
{

	public class ArgumentAttributeParser : IParserAttribute
    {
        public string GetIdentifier()
        {
            return "argument";
        }

        public ParsedElement ParsedAttribute(string identifier, NParsedAttributeValue parameters, NParsedAttributeValue value)
        {
            if (parameters != null)
                throw new InvalidParameterAttributeException (identifier,
                                                              InvalidParameterAttributeException.NO_PARAM);

			if (value is NParsedAttributeColon)
            {
                var colonValue = ((NParsedAttributeColon)value);
				var left = colonValue.Left;
				var right = colonValue.Right;

                if (!(left is IdentifierExpression) | !(right is IdentifierExpression))
                    throw new InvalidAttributeValueException (identifier,
                                                              InvalidAttributeValueException.IDENTIFIER);

				var leftIdentifier = ((IdentifierExpression)left).Value;

				return new ParsedPredicateArgumentAttribute(leftIdentifier, right);
            }

            throw new InvalidAttributeValueException (identifier,
                                                      InvalidAttributeValueException.COLON_ONLY);
        }
   }
    
}
