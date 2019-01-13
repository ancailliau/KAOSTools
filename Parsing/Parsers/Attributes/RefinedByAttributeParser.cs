using System;
using System.Collections.Generic;
using System.Linq;

namespace UCLouvain.KAOSTools.Parsing.Parsers.Attributes
{

	public class RefinedByAttributeParser : IParserAttribute
	{
		public string GetIdentifier()
		{
			return "refined[bB]y";
		}

		ParsedRefinementPattern parsedRefinementPattern;
		string contextIdentifier;

		public ParsedElement ParsedAttribute(string identifier, NParsedAttributeValue parameters, NParsedAttributeValue value)
		{
			parsedRefinementPattern = null;
			contextIdentifier = null;

			if (parameters != null)
			{
				ParseParameter(identifier, parameters);
			}

			var parsedRefinees = new List<ParsedRefinee>();
			if (value is NParsedAttributeAtomic)
			{
				AddChild((NParsedAttributeAtomic)value, parsedRefinees);

			}
			else if (value is NParsedAttributeList)
			{
				foreach (var item in ((NParsedAttributeList)value).Values)
				{
					if (item is NParsedAttributeAtomic)
					{
						AddChild((NParsedAttributeAtomic)item, parsedRefinees);

					}
					else if (item is NParsedAttributeBracket)
					{
						var child = ((NParsedAttributeBracket)item).Item;
						var parameters_child = ((NParsedAttributeBracket)item).Parameter;

						var refinee = new ParsedRefinee();

						if (child is IdentifierExpression)
						{
							refinee.Identifier = ((IdentifierExpression)child).Value;
						}
						else
						{
							throw new NotImplementedException();
						}

						// TODO fix this
						if (parameters_child is NParsedAttributeAtomic)
						{
							var parameter_child = ((NParsedAttributeAtomic)parameters_child).Value;
							if (parameter_child is ParsedFloat)
							{
								refinee.Parameters = new ParsedPrimitiveRefineeParameter<double>(((ParsedFloat)parameter_child).Value);

							}
							else if (parameter_child is ParsedInteger)
							{
								refinee.Parameters = new ParsedPrimitiveRefineeParameter<double>(((ParsedInteger)parameter_child).Value);

							}
							else
							{
								throw new NotImplementedException(parameter_child.GetType().ToString() + " " + parameter_child.ToString());
							}

						}
						else
						{
							throw new NotImplementedException();
						}

						parsedRefinees.Add(refinee);

					}
					else
					{
						throw new NotImplementedException("Attribute '" + identifier + "' only accept a list of identifiers. (Received: " +
														  string.Join(",", parsedRefinees.Select(x => x.GetType().ToString())) + ")");
					}
				}
			}
			else
			{
				throw new NotImplementedException("Attribute '" + identifier + "' only accept an atomic value or a list of atomic values.");
			}

			// TODO Remove casting and toList
			return new ParsedRefinedByAttribute()
			{
				ParsedRefinees = parsedRefinees,
				RefinementPattern = parsedRefinementPattern,
				ContextIdentifier = contextIdentifier,
			};
		}

		void ParseParameter(string identifier, ParsedElement parameters)
		{
			if (parameters is NParsedAttributeAtomic)
			{
				var parameter = ((NParsedAttributeAtomic)parameters).Value;
				ParseRefinementPattern(parameter);
			}
			else if (parameters is NParsedAttributeEqual)
			{
				var left = ((NParsedAttributeEqual)parameters).Left;
				var right = ((NParsedAttributeEqual)parameters).Right;
				if (left is IdentifierExpression ie) {
					if (ie.Value == "context" & (right is IdentifierExpression)) {
						var ie2 = (IdentifierExpression) right;
						contextIdentifier = ie2.Value;
					} else if (ie.Value == "pattern") {
						ParseRefinementPattern(right);
						
					} else {
						throw new NotImplementedException($"Parameter '{ie.Value}' was not recognized.");
					}
				} else {
					throw new NotImplementedException($"Parameter '{left}' was not recognized.");
				}
			}
			else if (parameters is NParsedAttributeList l) {
				foreach (var item in l.Values) {
					ParseParameter(identifier, item);
				}
			}
			else
			{
				throw new NotImplementedException("Attribute '" + identifier + "' unrecognized parameter.");
			}
		}
		
		void ParseRefinementPattern(ParsedElement parameter)
		{
			if (parameter is IdentifierExpression)
				{
					var patternId = ((IdentifierExpression)parameter).Value;

					switch (patternId)
					{
						case "case":
							parsedRefinementPattern = new ParsedRefinementPattern { Name = ParsedRefinementPatternName.Case };
							break;
						case "milestone":
							parsedRefinementPattern = new ParsedRefinementPattern { Name = ParsedRefinementPatternName.Milestone };
							break;
						case "introduce_guard":
							parsedRefinementPattern = new ParsedRefinementPattern { Name = ParsedRefinementPatternName.IntroduceGuard };
							break;
						case "divide_and_conquer":
							parsedRefinementPattern = new ParsedRefinementPattern { Name = ParsedRefinementPatternName.DivideAndConquer };
							break;
						case "unmonitorability":
							parsedRefinementPattern = new ParsedRefinementPattern { Name = ParsedRefinementPatternName.Unmonitorability };
							break;
						case "uncontrollability":
							parsedRefinementPattern = new ParsedRefinementPattern { Name = ParsedRefinementPatternName.Uncontrollability };
							break;

						default:
							throw new NotImplementedException("Refinement pattern '" + patternId + "' is not defined.");
					}
				}
		}

		private static void AddChild(NParsedAttributeAtomic value, List<ParsedRefinee> v)
		{
			ParsedElement child = value.Value;
			if (child is IdentifierExpression)
			{
				string identifier = ((IdentifierExpression)child).Value;
				v.Add(new ParsedRefinee(identifier));
			}
			else
			{
				throw new ParserException();
			}
		}
	}
}
