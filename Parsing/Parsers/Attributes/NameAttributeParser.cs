using System;
using System.Collections.Generic;
using System.Linq;

namespace KAOSTools.Parsing.Parsers.Attributes
{

    public class NameAttributeParser : IParserAttribute
    {
        public string GetIdentifier()
        {
            return "name";
        }

        public ParsedElement ParsedAttribute(string identifier, NParsedAttributeValue parameters, NParsedAttributeValue value)
        {
			if (parameters != null)
				throw new NotImplementedException("Attribute 'name' does not accept parameters.");

			if (!(value is NParsedAttributeAtomic))
				throw new NotImplementedException("Attribute 'name' only accept a single atomic value");

			var v = ((NParsedAttributeAtomic)value).Value;

			if (!(v is ParsedString))
				throw new NotImplementedException("Attribute 'name' only accept string value");

			return new ParsedNameAttribute() { Value = ((ParsedString)((NParsedAttributeAtomic)value).Value).Value };
        }
	}
    
}
