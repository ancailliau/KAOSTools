using System;
using System.Collections.Generic;
using System.Linq;
using UCLouvain.KAOSTools.Parsing.Parsers.Exceptions;

namespace UCLouvain.KAOSTools.Parsing.Parsers.Attributes
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
            string anchorId = null;
            
            if (parameters != null) {
                if (parameters is NParsedAttributeColon) {
                    var p = (NParsedAttributeColon)parameters;
                
                    var left = p.Left;
                    if (left is IdentifierExpression) {
                        var patternId = ((IdentifierExpression)left).Value;
                        pattern = new ParsedResolutionPattern(patternId);
                    } else {
                        throw new InvalidParameterAttributeException (identifier, 
                                                                      InvalidParameterAttributeException.IDENTIFIER);
                    }
                    
                    var right = p.Right;
                    if (right is IdentifierExpression) {
                        anchorId = ((IdentifierExpression)right).Value;
                    } else {
                        throw new InvalidParameterAttributeException (identifier, 
                                                                      InvalidParameterAttributeException.IDENTIFIER);
                    }
                    
                } else {
                    throw new InvalidParameterAttributeException(identifier,
                                                                 InvalidParameterAttributeException.COLON_ONLY);
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
                Pattern = pattern,
                AnchorId = anchorId
            };
        }
	}
    
}
