using System;
using System.Collections.Generic;
using System.Linq;

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
			if (parameters != null)
				throw new NotImplementedException("Attribute '" + identifier + "' does not accept parameters.");

			if (!(value is NParsedAttributeAtomic))
				throw new NotImplementedException("Attribute '" + identifier + "' only accept a single atomic value");

			var v = ((NParsedAttributeAtomic)value).Value;

            if (!(v is IdentifierExpression))
				throw new NotImplementedException("Attribute '" + identifier + "' only accept identifier value");

            return new ParsedResolvedByAttribute() { Value = ((NParsedAttributeAtomic)value).Value };
        }
	}
    
}