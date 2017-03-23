using System;
using System.Collections.Generic;
using System.Linq;

namespace KAOSTools.Parsing.Parsers.Attributes
{

	public class LinkAttributeParser : IParserAttribute
    {
		public string GetIdentifier()
        {
            return "link";
        }

        public ParsedElement ParsedAttribute(string identifier, NParsedAttributeValue parameters, NParsedAttributeValue value)
        {
			if (!(value is NParsedAttributeAtomic))
				throw new NotImplementedException("Attribute '" + identifier + "' only accept a single atomic value");

			var v = ((NParsedAttributeAtomic)value).Value;

            if (!(v is IdentifierExpression))
				throw new NotImplementedException("Attribute '" + identifier + "' only accept identifier value");

            string multiplicity = "";
            if (parameters != null)
            {
                if (parameters is NParsedAttributeAtomic)
                {
                    var parameter = ((NParsedAttributeAtomic)parameters).Value;
					if (parameter is IdentifierExpression)
                    {
						multiplicity = ((IdentifierExpression)parameter).Value;
                    }
					else if (parameter is ParsedInteger)
					{
                        multiplicity = ((ParsedInteger)parameter).Value.ToString ();
					}
					else
					{
						throw new NotImplementedException("Attribute '" + identifier + "' only accept a single 'M', 'N', '*' or an integer.");
					}
				}
				else if (parameters is NParsedAttributeList)
				{
                    if (((NParsedAttributeList)parameters).Values.Count != 2)
					{
                        throw new NotImplementedException("Attribute '" + identifier + "' only accept a single or a pair of parameters (Received: "+((NParsedAttributeList)parameters).Values.Count+").");
                    }

					var firstParameter = ((NParsedAttributeList)parameters).Values[0];
					var secondParameter = ((NParsedAttributeList)parameters).Values[1];

                    if (firstParameter is NParsedAttributeAtomic)
					{
                        var firstParameter1 = ((NParsedAttributeAtomic)firstParameter).Value;
						if (firstParameter1 is IdentifierExpression)
                        {
							multiplicity = ((IdentifierExpression)firstParameter1).Value;
                        }
						else if (firstParameter1 is ParsedInteger)
                        {
                            multiplicity = ((ParsedInteger)firstParameter1).Value.ToString();
                        }
                        else
                        {
							throw new NotImplementedException("Attribute '" + identifier + "' only accept 'M', 'N', '*' or integer as first parameter (Received: " + firstParameter1.GetType() + ").");
                        }
					}
					else
					{
						throw new NotImplementedException("Attribute '" + identifier + "' only accept atomic value as first parameter (Received: " + firstParameter.GetType() + ").");
					}

					if (secondParameter is NParsedAttributeAtomic)
					{
						var secondParameter1 = ((NParsedAttributeAtomic)secondParameter).Value;
						if (secondParameter1 is IdentifierExpression)
    					{
							multiplicity += ".." + ((IdentifierExpression)secondParameter1).Value;
    					}
						else if (secondParameter1 is ParsedInteger)
    					{
							multiplicity += ".." + ((ParsedInteger)secondParameter1).Value.ToString();
    					}
    					else
    					{
							throw new NotImplementedException("Attribute '" + identifier + "' only accept 'M', 'N', '*' or integer as second parameter (Received: " + secondParameter1.GetType() + ").");
						}
					}
					else
					{
						throw new NotImplementedException("Attribute '" + identifier + "' only accept atomic value as second parameter (Received: " + secondParameter.GetType() + ").");
					}
				}
				else
				{
                    throw new NotImplementedException("Attribute '" + identifier + "' only accept a single or a pair of 'M', 'N', '*' or integers.");
				}
            }

            return new ParsedLinkAttribute() { Target = ((NParsedAttributeAtomic)value).Value, Multiplicity = multiplicity };
        }
	}
    
}
