using System;
using System.Collections.Generic;
using System.Linq;

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
				throw new NotImplementedException("Attribute '" + identifier + "' does not accept parameters.");


            if (value is NParsedAttributeColon) {
                var colonValue = ((NParsedAttributeColon)value);
                var left = colonValue.Left;
                var right = colonValue.Right;

				if (!(left is IdentifierExpression) | !(right is IdentifierExpression))
					throw new NotImplementedException(
                        string.Format("Attribute '{0}' only accept a single identifier or a pair identifiers:identifier.", 
                                      identifier));

                var leftIdentifier = ((IdentifierExpression)left).Value;

                return new ParsedAttributeAttribute(leftIdentifier, right);
            } else if (value is NParsedAttributeAtomic) {
                var leftIdentifier = ((NParsedAttributeAtomic)value).Value;
                if (leftIdentifier is IdentifierExpression) {
                    return new ParsedAttributeAttribute(((IdentifierExpression)leftIdentifier).Value, null);
				}

                    throw new NotImplementedException(
                        string.Format("Attribute '{0}' only accept a single identifier or a pair identifiers:identifier.", 
                                      identifier));
            }

			throw new NotImplementedException(
				string.Format("Attribute '{0}' only accept a single identifier or a pair identifiers:identifier.",
							  identifier));
        }
   }
    
}
