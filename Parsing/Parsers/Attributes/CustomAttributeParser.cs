using System;
using System.Collections.Generic;
using System.Linq;

namespace KAOSTools.Parsing.Parsers.Attributes
{

    public class CustomAttributeParser : IParserAttribute
    {
        public string GetIdentifier()
        {
			return @"\$[a-zA-Z][a-zA-Z0-9_-]*";
        }

        public ParsedElement ParsedAttribute(string identifier, NParsedAttributeValue parameters, NParsedAttributeValue value)
        {
			if (parameters != null)
				throw new NotImplementedException("Custom attribute does not accept parameters.");

			if (!(value is NParsedAttributeAtomic))
				throw new NotImplementedException("Custom attribute only accept a single atomic value");

			var v = ((NParsedAttributeAtomic)value).Value;

			if (!(v is ParsedString))
				throw new NotImplementedException("Custom attribute only accept string value");

            return new ParsedCustomAttribute() { 
                Key = identifier,
                Value = ((ParsedString)((NParsedAttributeAtomic)value).Value).Value
            };
        }
	}
    
}
