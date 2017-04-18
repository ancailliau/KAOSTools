using System;
using System.Collections.Generic;
using System.Linq;
using UCLouvain.KAOSTools.Parsing.Parsers.Exceptions;

namespace KAOSTools.Parsing.Parsers.Attributes
{

    public class RSRAttributeParser : IParserAttribute
    {
        public string GetIdentifier()
        {
            return "rsr";
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

            double rds = 1;
            if (v is ParsedFloat) {
                rds = ((ParsedFloat)v).Value;
            } else if (v is ParsedInteger) {
                rds = ((ParsedInteger)v).Value * 1d; // ntm: ignore 008b (Mul => Div)
            } else if (v is ParsedPercentage) {
                rds = ((ParsedPercentage)v).Value / 100;
            } else {
                throw new InvalidAttributeValueException (identifier,
                                                          InvalidAttributeValueException.FLOAT_INTEGER_PERCENTAGE_ONLY);
            }

            if (rds < 0 | rds > 1) // ntm: ignore 00de (Or => Xor)
                throw new InvalidAttributeValueException (identifier,
                                                          InvalidAttributeValueException.PROBABILITY_EXPECTED);

            return new ParsedRDSAttribute () { Value = rds };
        }
	}
    
}
