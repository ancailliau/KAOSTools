using System;
using System.Collections.Generic;
using System.Linq;

namespace KAOSTools.Parsing.Parsers.Attributes
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
				throw new NotImplementedException("Attribute '" + identifier + "' does not accept parameters.");


			var v = new List<ParsedPredicateArgumentAttribute>();
            if (value is NParsedAttributeColon)
            {
                var colonValue = ((NParsedAttributeColon)value);
                AddValue(identifier, v, colonValue);
            }
            else
			{
				throw new NotImplementedException("Attribute '" + identifier + "' only accept a single or a list of identifiers:identifier.");
			}

            // TODO Remove casting and toList
            var arg = v.Single();
            return arg;
        }

        static void AddValue(string identifier, List<ParsedPredicateArgumentAttribute> v, NParsedAttributeColon colonValue)
        {
            var left = colonValue.Left;
            var right = colonValue.Right;

            if (!(left is IdentifierExpression) | !(right is IdentifierExpression))
                throw new NotImplementedException("Attribute '" + identifier + "' only accept a list of identifiers:identifier.");

            var leftIdentifier = ((IdentifierExpression)left).Value;

            v.Add(new ParsedPredicateArgumentAttribute(leftIdentifier, right));
        }
   }
    
}
