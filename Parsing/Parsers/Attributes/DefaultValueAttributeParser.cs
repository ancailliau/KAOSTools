using System;
using System.Collections.Generic;
using System.Linq;

namespace KAOSTools.Parsing.Parsers.Attributes
{

    public class DefaultValueAttributeParser : IParserAttribute
    {
        public string GetIdentifier()
        {
            return "default";
        }

        public ParsedElement ParsedAttribute(string identifier, NParsedAttributeValue parameters, NParsedAttributeValue value)
        {
			if (parameters != null)
                throw new NotImplementedException("Attribute '"+identifier+"' does not accept parameters.");

			if (!(value is NParsedAttributeAtomic))
				throw new NotImplementedException("Attribute '"+identifier+"' only accept a single atomic value");

			var v = ((NParsedAttributeAtomic)value).Value;

            if (!(v is ParsedBool))
				throw new NotImplementedException("Attribute '"+identifier+"' only accept boolean value");

			return new DefaultValueAttribute() { Value = ((ParsedBool)((NParsedAttributeAtomic)value).Value).Value };
        }
	}
    
}
