using System;
using System.Collections.Generic;
using System.Linq;
using UCLouvain.KAOSTools.Parsing.Parsers.Exceptions;

namespace KAOSTools.Parsing.Parsers.Attributes
{

	public class AttributeAttributeParser : IParserAttribute
    {
        public string GetIdentifier()
        {
            return "attribute";
        }

        public ParsedElement ParsedAttribute(string identifier, NParsedAttributeValue parameters, NParsedAttributeValue value)
        {
            if (parameters != null)
                throw new InvalidParameterAttributeException (identifier,
                                                              InvalidParameterAttributeException.NO_PARAM);

            string leftIdentifier = null;
            dynamic type = null;

            if (value is NParsedAttributeColon) {
                var colonValue = ((NParsedAttributeColon)value);
                var left = colonValue.Left;
                var right = colonValue.Right;

                if (!(left is IdentifierExpression) | !(right is IdentifierExpression))
                    throw new InvalidAttributeValueException (identifier,
                                                              InvalidAttributeValueException.IDENTIFIER);

                leftIdentifier = ((IdentifierExpression)left).Value;
                type = right;

            } else if (value is NParsedAttributeAtomic) {
                var atomicValue = ((NParsedAttributeAtomic)value).Value;
                if (atomicValue is IdentifierExpression) {
                    leftIdentifier = ((IdentifierExpression)atomicValue).Value;

                } else {
                    throw new InvalidAttributeValueException (identifier,
                                                              InvalidAttributeValueException.IDENTIFIER);
                }

            } else {
                throw new InvalidAttributeValueException (identifier,
                                                          InvalidAttributeValueException.ATOMIC_OR_COLON);
                
            }

            return new ParsedAttributeAttribute (leftIdentifier, type);
        }
   }
    
}
