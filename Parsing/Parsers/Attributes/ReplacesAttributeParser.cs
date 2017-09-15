using System;
using System.Collections.Generic;
using System.Linq;
using UCLouvain.KAOSTools.Parsing.Parsers.Exceptions;

namespace UCLouvain.KAOSTools.Parsing.Parsers.Attributes
{

    public class ReplacesAttributeParser : IParserAttribute
    {
        public string GetIdentifier()
        {
			return @"replaces";
        }

        public ParsedElement ParsedAttribute(string identifier, NParsedAttributeValue parameters, NParsedAttributeValue value)
        {
            if (parameters == null)
                throw new InvalidParameterAttributeException (identifier,
                                                              InvalidParameterAttributeException.NO_PARAM);

            if (!(parameters is NParsedAttributeAtomic))
                throw new InvalidParameterAttributeException (identifier,
                                                              InvalidParameterAttributeException.ATOMIC_ONLY);
			
            if (!(value is NParsedAttributeAtomic))
                throw new InvalidAttributeValueException (identifier,
                                                          InvalidAttributeValueException.ATOMIC_ONLY);

			var p = ((NParsedAttributeAtomic)parameters).Value;
			var v = ((NParsedAttributeAtomic)value).Value;

            if (!(p is IdentifierExpression))
                throw new InvalidParameterAttributeException (identifier,
                                                          InvalidAttributeValueException.IDENTIFIER);
            var pValue = (IdentifierExpression)p;

            if (!(v is IdentifierExpression))
                throw new InvalidAttributeValueException (identifier,
                                                          InvalidAttributeValueException.IDENTIFIER);
            var stringValue = (IdentifierExpression)v;

            return new ParsedReplacesAttribute() {
				ObstacleIdentifier = pValue.Value,
                ReplacedGoalIdentifier = stringValue.Value
            };
        }
	}
    
}
