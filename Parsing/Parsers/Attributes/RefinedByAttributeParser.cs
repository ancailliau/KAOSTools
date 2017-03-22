using System;
using System.Collections.Generic;
using System.Linq;

namespace KAOSTools.Parsing.Parsers.Attributes
{

    public class RefinedByAttributeParser : IParserAttribute
	{
        public string GetIdentifier()
        {
            return "refined[bB]y";
        }

		public ParsedElement ParsedAttribute(string identifier, NParsedAttributeValue parameters, NParsedAttributeValue value)
        {
			if (parameters != null)
				throw new NotImplementedException("Attribute '" + identifier + "' does not accept parameters.");

			List<ParsedElement> v = new List<ParsedElement>();
            if (value is NParsedAttributeAtomic)
            {
                v.Add(((NParsedAttributeAtomic)value).Value);

            }
            else if (value is NParsedAttributeList)
            {
                v = ((NParsedAttributeList)value).Values;

                if (!(v.All(x => x is IdentifierExpression)))
                    throw new NotImplementedException("Attribute '" + identifier + "' only accept a list of identifiers.");
            }
            else
            {
                throw new NotImplementedException("Attribute '" + identifier + "' only accept an atomic value or a list of atomic values.");
            }

            // TODO Remove casting and toList
            return new ParsedRefinedByAttribute() { Values = v.Cast<dynamic>().ToList() };
		}
	}
}
