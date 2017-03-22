using System;
using System.Collections.Generic;
using System.Linq;

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
				throw new NotImplementedException("Attribute 'rsr' does not accept parameters.");

			if (!(value is NParsedAttributeAtomic))
				throw new NotImplementedException("Attribute 'rsr' only accept a single atomic value");

			var v = ((NParsedAttributeAtomic)value).Value;

            double rds = 1;
            if (v is ParsedFloat)
                rds = ((ParsedFloat)v).Value;
            else if (v is ParsedPercentage)
                rds = ((ParsedPercentage)v).Value / 100;
			else
				throw new NotImplementedException("Attribute 'rsr' only accept float or percentage value");

            return new ParsedRDSAttribute() { Value = rds };
        }
	}
    
}
