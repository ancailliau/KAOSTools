using System;
using System.Collections.Generic;
using System.Linq;

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
                throw new NotImplementedException("Attribute '" + identifier + "' does not accept parameters.");

            List<ParsedElement> v = new List<ParsedElement>();
            if (value is NParsedAttributeAtomic) {
                v.Add(((NParsedAttributeAtomic)value).Value);

            } else if (value is NParsedAttributeList) {
                v = ((NParsedAttributeList)value).Values;

                if (!(v.All(x => x is NParsedAttributeAtomic)))
                    throw new NotImplementedException(
                        string.Format("Attribute '{0}' only accept a list of atomic values. (Received: {1})",
                                      identifier, string.Join(",", v.Select(x => x.GetType().ToString()))));

                v = v.OfType<NParsedAttributeAtomic>().Select(x => x.Value).ToList();
                if (!(v.All(x => x is IdentifierExpression)))
                    throw new NotImplementedException(
                        string.Format("Attribute '{0}' only accept a list of identifiers. (Received: {1})",
                                      identifier, string.Join(",", v.Select(x => x.GetType().ToString()))));
            } else {
                throw new NotImplementedException(
                    string.Format("Attribute '{0}' only accept an atomic value or a list of atomic values.",
                                  identifier));
            }

            // TODO Remove casting and toList
            return new ParsedAssignedToAttribute() { Values = v.Cast<dynamic>().ToList() };
        }
    }

}
