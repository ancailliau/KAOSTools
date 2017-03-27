using System;
using System.Collections.Generic;
using System.Linq;

namespace KAOSTools.Parsing.Parsers.Attributes
{

	public class ESRAttributeParser : IParserAttribute
    {
        public string GetIdentifier()
        {
            return "(esr|probability)";
        }

		public ParsedElement ParsedAttribute(string identifier, NParsedAttributeValue parameters, NParsedAttributeValue value)
        {
			if (parameters != null)
				throw new NotImplementedException("Attribute '" + identifier +"' does not accept parameters.");

			if (!(value is NParsedAttributeAtomic))
				throw new NotImplementedException("Attribute '" + identifier +"' only accept a single atomic value");

			var v = ((NParsedAttributeAtomic)value).Value;

            double esr = 1;
            if (v is ParsedFloat)
				esr = ((ParsedFloat)v).Value;
            else if (v is ParsedInteger)
				esr = ((ParsedInteger)v).Value * 1d;
            else if (v is ParsedPercentage)
                esr = ((ParsedPercentage)v).Value / 100;
			else
                throw new NotImplementedException("Attribute '" + identifier +"' only accept float or percentage value. (Received: "+v.GetType()+")");

			return new ParsedProbabilityAttribute() { Value = esr };
        }
	}
    
}
