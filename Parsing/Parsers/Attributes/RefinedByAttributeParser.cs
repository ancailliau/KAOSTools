using System;
using System.Collections.Generic;
using System.Linq;

namespace KAOSTools.Parsing.Parsers.Attributes
{

    public class RefinedByAttributeParser : IParserAttribute
	{
        public string GetIdentifier()
        {
            return "refined[bB]y";
        }

		public ParsedElement ParsedAttribute(string identifier, NParsedAttributeValue parameters, NParsedAttributeValue value)
        {
            ParsedRefinementPattern parsedRefinementPattern = null;
            if (parameters != null) {

                if (parameters is NParsedAttributeAtomic) {
                    var parameter = ((NParsedAttributeAtomic)parameters).Value;
                    if (parameter is IdentifierExpression) {
                        var patternId = ((IdentifierExpression)parameter).Value;

                        switch (patternId) {
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
                                throw new NotImplementedException("Refinement pattern '"+ patternId + "' is not defined.");
                        }

                    }
                } else {
                    throw new NotImplementedException("Attribute '" + identifier + "' only accept a single parameter.");
                }
            }	

			List<ParsedElement> v = new List<ParsedElement>();
            if (value is NParsedAttributeAtomic)
            {
                v.Add(((NParsedAttributeAtomic)value).Value);

            }
            else if (value is NParsedAttributeList)
            {
				foreach (var item in ((NParsedAttributeList)value).Values) {
                    if (item is NParsedAttributeAtomic) {
                        var child = ((NParsedAttributeAtomic)item).Value;
                        if (child is IdentifierExpression) {
                            v.Add(child);

                        } else {
                            throw new NotImplementedException("Attribute '" + identifier + "' only accept a list of identifiers. (Received: " +
                                                              string.Join(",", v.Select(x => x.GetType().ToString())) + ")");
                        }
                    } else if (item is NParsedAttributeBracket) {
						var child = ((NParsedAttributeBracket)item).Item;
                        var parameters_child = ((NParsedAttributeBracket)item).Parameter;

						if (child is IdentifierExpression) {
                                v.Add(child);
                            } else {
                                throw new NotImplementedException();
                            }
						
                        // TODO fix this
                        if (parameters_child is NParsedAttributeAtomic) {
							var parameter_child = ((NParsedAttributeAtomic)parameters_child).Value;
                            if (parameter_child is ParsedFloat) {
                                if (parsedRefinementPattern != null) {
                                    parsedRefinementPattern.Parameters.Add(parameter_child);
                                    v.Add(child);

                                } else {
                                    throw new NotImplementedException();
                                }
							} else {
                                throw new NotImplementedException(parameter_child.GetType ().ToString () + " " + parameter_child.ToString () );
							}

                        } else {
                            throw new NotImplementedException();
                        }

                    } else {
                        throw new NotImplementedException("Attribute '" + identifier + "' only accept a list of identifiers. (Received: " +
                                                          string.Join(",", v.Select(x => x.GetType().ToString())) + ")");
                    }
                }
            }
            else
            {
                throw new NotImplementedException("Attribute '" + identifier + "' only accept an atomic value or a list of atomic values.");
            }

            // TODO Remove casting and toList
            return new ParsedRefinedByAttribute() { Values = v.Cast<dynamic>().ToList(), RefinementPattern = parsedRefinementPattern };
		}
	}
}
