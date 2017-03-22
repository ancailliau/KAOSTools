using System;
using System.Collections.Generic;
using System.Linq;

namespace KAOSTools.Parsing.Parsers.Attributes
{

    public class DefinitionAttributeParser : IParserAttribute
	{
        public string GetIdentifier()
        {
            return "definition";
        }

		public ParsedElement ParsedAttribute(string identifier, NParsedAttributeValue parameters, NParsedAttributeValue value)
        {
            if (parameters != null)
				throw new NotImplementedException("Attribute 'definition' does not accept parameters.");

            if (!(value is NParsedAttributeAtomic))
				throw new NotImplementedException("Attribute 'definition' only accept a single atomic value");

            var v = ((NParsedAttributeAtomic)value).Value;

            if (!(v is ParsedString))
				throw new NotImplementedException("Attribute 'definition' only accept string value");

            return new ParsedDefinitionAttribute() { Value = ((ParsedString)((NParsedAttributeAtomic)value).Value) };
		}
	}
}
