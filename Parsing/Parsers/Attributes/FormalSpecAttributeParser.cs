using System;
using System.Collections.Generic;
using System.Linq;

namespace KAOSTools.Parsing.Parsers.Attributes
{

	public class FormalSpecAttributeParser : IParserAttribute
    {
        public string GetIdentifier()
        {
            return "formal[sS]pec";
        }

        public ParsedElement ParsedAttribute(string identifier, NParsedAttributeValue parameters, NParsedAttributeValue value)
        {
			if (parameters != null)
				throw new NotImplementedException("Attribute '" + identifier + "' does not accept parameters.");

			if (!(value is NParsedAttributeAtomic))
				throw new NotImplementedException("Attribute '" + identifier + "' only accept a single atomic value");

			var v = ((NParsedAttributeAtomic)value).Value;

            // TODO this shall be stronger
			if (!(v is ParsedElement))
				throw new NotImplementedException("Attribute '" + identifier + "' only accept formula values");

            return new ParsedFormalSpecAttribute() { Value = ((NParsedAttributeAtomic)value).Value };
        }
	}
    
}
