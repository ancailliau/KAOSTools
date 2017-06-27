using System;
using System.Collections.Generic;
using System.Linq;
using UCLouvain.KAOSTools.Parsing.Parsers.Exceptions;

namespace UCLouvain.KAOSTools.Parsing.Parsers.Attributes
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
                throw new InvalidParameterAttributeException (identifier,
                                                              InvalidParameterAttributeException.NO_PARAM);

            if (!(value is NParsedAttributeAtomic))
                throw new InvalidAttributeValueException (identifier,
                                                          InvalidAttributeValueException.ATOMIC_ONLY);

			var v = ((NParsedAttributeAtomic)value).Value;

            if (!(v is ParsedBool))
                throw new InvalidAttributeValueException (identifier,
                                                          InvalidAttributeValueException.BOOL);

			return new DefaultValueAttribute() { Value = ((ParsedBool)((NParsedAttributeAtomic)value).Value).Value };
        }
	}
    
}
