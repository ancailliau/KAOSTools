using System;
using System.Collections.Generic;
using System.Linq;
using UCLouvain.KAOSTools.Parsing.Parsers.Exceptions;

namespace KAOSTools.Parsing.Parsers.Attributes
{

    public class AssignedToAttributeParser : IParserAttribute
    {
        public string GetIdentifier()
        {
            return "assigned[tT]o";
        }

        public ParsedElement ParsedAttribute(string identifier,
                                             NParsedAttributeValue parameters,
                                             NParsedAttributeValue value)
        {
            if (parameters != null)
                throw new InvalidParameterAttributeException (identifier,
                                                              InvalidParameterAttributeException.NO_PARAM);

            List<ParsedElement> v = new List<ParsedElement>();
            if (value is NParsedAttributeAtomic) {
                AddValueToList (identifier, value, v);

            } else if (value is NParsedAttributeList) {
                var list = ((NParsedAttributeList)value);

                foreach (var item in list.Values) {
                    AddValueToList (identifier, item, v);
                }

            } else {
                throw new InvalidAttributeValueException (identifier,
                                                          InvalidAttributeValueException.ATOMIC_OR_LIST);
            }

            // TODO Remove casting and toList
            return new ParsedAssignedToAttribute() { Values = v.Cast<dynamic>().ToList() };
        }

        private static void AddValueToList (string identifier, ParsedElement value, List<ParsedElement> v)
        {
            if (!(value is NParsedAttributeAtomic))
                throw new InvalidAttributeValueException (identifier,
                                                          InvalidAttributeValueException.ATOMIC_ONLY);
                
            var atomic = ((NParsedAttributeAtomic)value);
            if (atomic.Value is IdentifierExpression)
                v.Add (atomic.Value);
            else
                throw new InvalidAttributeValueException (identifier,
                                                          InvalidAttributeValueException.IDENTIFIER);
        }
    }

}
