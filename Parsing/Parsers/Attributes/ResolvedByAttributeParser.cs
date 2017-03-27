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
            ParsedResolutionPattern pattern = null;
            if (parameters != null) {
                if (parameters is NParsedAttributeAtomic) {
                    var parameter = ((NParsedAttributeAtomic)parameters).Value;
                    if (parameter is IdentifierExpression) {
                        var patternId = ((IdentifierExpression)parameter).Value;
                        pattern = new ParsedResolutionPattern(patternId);
                    }
                } else {
                    throw new NotImplementedException("Attribute '" + identifier + "' only accept a single parameter.");
                }
            }

			if (!(value is NParsedAttributeAtomic))
				throw new NotImplementedException("Attribute '" + identifier + "' only accept a single atomic value");

			var v = ((NParsedAttributeAtomic)value).Value;

            if (!(v is IdentifierExpression))
				throw new NotImplementedException("Attribute '" + identifier + "' only accept identifier value");

            return new ParsedResolvedByAttribute() { Value = ((NParsedAttributeAtomic)value).Value, Pattern = pattern };
        }
	}
    
}
