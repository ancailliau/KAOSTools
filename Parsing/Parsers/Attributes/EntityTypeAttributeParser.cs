using System;
using System.Collections.Generic;
using System.Linq;
using UCLouvain.KAOSTools.Parsing.Parsers.Exceptions;

namespace UCLouvain.KAOSTools.Parsing.Parsers.Attributes
{

	public class EntityTypeAttributeParser : IParserAttribute
    {
        public string GetIdentifier()
        {
            return "type";
        }

        public ParsedElement ParsedAttribute(string identifier, NParsedAttributeValue parameters, NParsedAttributeValue value)
        {
            if (parameters != null)
                throw new InvalidParameterAttributeException (identifier,
                                                              InvalidParameterAttributeException.NO_PARAM);

            if (!(value is NParsedAttributeAtomic))
                throw new InvalidAttributeValueException (identifier,
                                                          InvalidAttributeValueException.ATOMIC_ONLY);

			var v = ((NParsedAttributeAtomic)value).Value;

            if (!(v is IdentifierExpression))
                throw new InvalidAttributeValueException (identifier,
                                                          InvalidAttributeValueException.IDENTIFIER);

            var v2 = (IdentifierExpression)v;

            switch (v2.Value)
            {
				case "software":
					return new ParsedEntityTypeAttribute() { Value = ParsedEntityType.Software };

				case "environment":
					return new ParsedEntityTypeAttribute() { Value = ParsedEntityType.Environment };

				case "shared":
                    return new ParsedEntityTypeAttribute() { Value = ParsedEntityType.Shared };

            default:
                throw new InvalidAttributeValueException (identifier,
                                                          InvalidAttributeValueException.INVALID_VALUE);
            }
				

        }
	}
    
}
