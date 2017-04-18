using System;
using System.Collections.Generic;
using System.Linq;
using UCLouvain.KAOSTools.Parsing.Parsers.Exceptions;

namespace KAOSTools.Parsing.Parsers.Attributes
{

	public class ResolvedByAttributeParser : IParserAttribute
    {
        public string GetIdentifier()
        {
            return "resolved[bB]y";
        }

        public ParsedElement ParsedAttribute(string identifier, NParsedAttributeValue parameters, NParsedAttributeValue value)
        {
            ParsedResolutionPattern pattern = null;
            if (parameters != null) {
                if (parameters is NParsedAttributeAtomic) {
                    var parameter = ((NParsedAttributeAtomic)parameters).Value;
                    if (parameter is IdentifierExpression) {
                        var patternId = ((IdentifierExpression)parameter).Value;
                        pattern = new ParsedResolutionPattern(patternId);
                    } else {
                        throw new InvalidParameterAttributeException (identifier, 
                                                                      InvalidParameterAttributeException.IDENTIFIER);
                    }
                } else {
                    throw new InvalidParameterAttributeException(identifier,
                                                                 InvalidParameterAttributeException.ATOMIC_ONLY);
                }
            }

            if (!(value is NParsedAttributeAtomic))
                throw new InvalidAttributeValueException(identifier,
                                                         InvalidAttributeValueException.ATOMIC_ONLY);

			var v = ((NParsedAttributeAtomic)value).Value;

            if (!(v is IdentifierExpression))
                throw new InvalidAttributeValueException(identifier,
                                                         InvalidAttributeValueException.IDENTIFIER);

            return new ParsedResolvedByAttribute() { 
                Value = ((NParsedAttributeAtomic)value).Value, 
                Pattern = pattern
            };
        }
	}
    
}
