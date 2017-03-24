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


			if (value is NParsedAttributeColon)
            {
                var colonValue = ((NParsedAttributeColon)value);
				var left = colonValue.Left;
				var right = colonValue.Right;

				if (!(left is IdentifierExpression) | !(right is IdentifierExpression))
					throw new NotImplementedException("Attribute '" + identifier + "' only accept a single identifiers:identifier.");

				var leftIdentifier = ((IdentifierExpression)left).Value;

                return new ParsedAttributeAttribute(leftIdentifier, right);
            }

            throw new NotImplementedException("Attribute '" + identifier + "' only accept a single identifiers:identifier.");
        }
   }
    
}
