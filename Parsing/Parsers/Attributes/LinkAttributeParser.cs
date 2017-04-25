using System;
using System.Collections.Generic;
using System.Linq;
using UCLouvain.KAOSTools.Parsing.Parsers.Exceptions;

namespace KAOSTools.Parsing.Parsers.Attributes
{

    public class LinkAttributeParser : IParserAttribute
    {
        public string GetIdentifier ()
        {
            return "link";
        }

        public ParsedElement ParsedAttribute (string identifier, NParsedAttributeValue parameters, NParsedAttributeValue value)
        {
            if (!(value is NParsedAttributeAtomic))
                throw new InvalidAttributeValueException (identifier,
                                                          InvalidAttributeValueException.ATOMIC_ONLY);

            var v = ((NParsedAttributeAtomic)value).Value;

            if (!(v is IdentifierExpression))
                throw new InvalidAttributeValueException (identifier,
                                                          InvalidAttributeValueException.IDENTIFIER);

            string multiplicity = "";
            if (parameters != null) {
                if (parameters is NParsedAttributeAtomic) {
                    multiplicity = ParseMultiplicity (identifier, parameters);

                } else if (parameters is NParsedAttributeList) {
                    if (((NParsedAttributeList)parameters).Values.Count != 2) {
                        throw new InvalidParameterAttributeException (identifier,
                                                                  InvalidParameterAttributeException.INVALID_VALUE);
                    }

                    var firstParameter = ((NParsedAttributeList)parameters).Values [0];
                    var secondParameter = ((NParsedAttributeList)parameters).Values [1];
                    string m1 = ParseMultiplicity (identifier, firstParameter);
                    string m2 = ParseMultiplicity (identifier, secondParameter);

                    multiplicity = m1 + ".." + m2;

                } else {
                    throw new InvalidParameterAttributeException (identifier,
                                                              InvalidParameterAttributeException.ATOMIC_OR_LIST);
                }
            }

            return new ParsedLinkAttribute () {
                Target = ((NParsedAttributeAtomic)value).Value,
                Multiplicity = multiplicity
            };
        }

        public static string ParseMultiplicity (string identifier, ParsedElement secondParameter)
        {
            if (secondParameter is NParsedAttributeAtomic) 
                return ParseMultiplicity (identifier, (NParsedAttributeAtomic)secondParameter);
            
            throw new InvalidParameterAttributeException (identifier,
                                        InvalidParameterAttributeException.ATOMIC_ONLY);
        }

        public static string ParseMultiplicity (string identifier, NParsedAttributeValue parameters)
        {
            var parameter = ((NParsedAttributeAtomic)parameters).Value;

            if (parameter is IdentifierExpression) {
                string value = ((IdentifierExpression)parameter).Value;
                if (string.IsNullOrWhiteSpace (value))
                    throw new InvalidParameterAttributeException (identifier,
                                                InvalidParameterAttributeException.INVALID_VALUE);
                return value;
            }

            if (parameter is StarExpression) {
                return "*";
            }

            if (parameter is ParsedInteger) {
                int value = ((ParsedInteger)parameter).Value;
                if (value < 0)
                    throw new InvalidParameterAttributeException (identifier,
                                                InvalidParameterAttributeException.INVALID_VALUE);
                return value.ToString ();
            }

            throw new InvalidParameterAttributeException (identifier,
                                        InvalidParameterAttributeException.INVALID_VALUE);
        }
    }

}
