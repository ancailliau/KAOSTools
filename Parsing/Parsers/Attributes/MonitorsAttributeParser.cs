using System;
using System.Collections.Generic;
using System.Linq;

namespace UCLouvain.KAOSTools.Parsing.Parsers.Attributes
{

	public class MonitorsAttributeParser : IParserAttribute
	{
		public string GetIdentifier()
		{
			return "monitors";
		}

		public ParsedElement ParsedAttribute(string identifier, NParsedAttributeValue parameters, NParsedAttributeValue value)
		{
			if (parameters != null)
			{
				throw new NotImplementedException();
			}

			var parsedPredicates = new List<string>();
			if (value is NParsedAttributeAtomic)
			{
				AddChild((NParsedAttributeAtomic)value, parsedPredicates);

			}
			else if (value is NParsedAttributeList)
			{
				foreach (var item in ((NParsedAttributeList)value).Values)
				{
					if (item is NParsedAttributeAtomic)
					{
						AddChild((NParsedAttributeAtomic)item, parsedPredicates);

					}

					else
					{
						throw new NotImplementedException("Attribute '" + identifier + "' only accept a list of identifiers. (Received: " +
														  string.Join(",", parsedPredicates.Select(x => x.GetType().ToString())) + ")");
					}
				}
			}
			else
			{
				throw new NotImplementedException("Attribute '" + identifier + "' only accept an atomic value or a list of atomic values.");
			}

			return new ParsedMonitorsAttribute()
			{
				ParsedPredicates = parsedPredicates
			};
		}

		static void AddChild(NParsedAttributeAtomic value, List<string> v)
		{
			ParsedElement child = value.Value;
			if (child is IdentifierExpression)
			{
				string identifier = ((IdentifierExpression)child).Value;
				v.Add(identifier);
			}
			else
			{
				throw new ParserException();
			}
		}
	}
}
