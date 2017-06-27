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

			var parsedRefinees = new List<ParsedRefinee>();
            if (value is NParsedAttributeAtomic) {
                AddChild ((NParsedAttributeAtomic)value, parsedRefinees);
                
            } else if (value is NParsedAttributeList)
            {
				foreach (var item in ((NParsedAttributeList)value).Values) {
                    if (item is NParsedAttributeAtomic) {
                        AddChild ((NParsedAttributeAtomic)item, parsedRefinees);
                        
                    } else if (item is NParsedAttributeBracket) {
						var child = ((NParsedAttributeBracket)item).Item;
                        var parameters_child = ((NParsedAttributeBracket)item).Parameter;

                        var refinee = new ParsedRefinee ();

						if (child is IdentifierExpression) {
                            refinee.Identifier = ((IdentifierExpression)child).Value;
                        } else {
                            throw new NotImplementedException();
                        }
						
                        // TODO fix this
                        if (parameters_child is NParsedAttributeAtomic) {
							var parameter_child = ((NParsedAttributeAtomic)parameters_child).Value;
                            if (parameter_child is ParsedFloat) {
                                refinee.Parameters = new ParsedPrimitiveRefineeParameter<double> (((ParsedFloat)parameter_child).Value);
                                
							} else if (parameter_child is ParsedInteger) {
                                refinee.Parameters = new ParsedPrimitiveRefineeParameter<double> (((ParsedInteger)parameter_child).Value);
                                
                            } else {
                                throw new NotImplementedException (parameter_child.GetType ().ToString () + " " + parameter_child.ToString ());
                            }

                        } else {
                            throw new NotImplementedException();
                        }
                        
                        parsedRefinees.Add (refinee);

                    } else {
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
            return new ParsedRefinedByAttribute() {
                ParsedRefinees = parsedRefinees, 
                RefinementPattern = parsedRefinementPattern
            };
		}

        private static void AddChild (NParsedAttributeAtomic value, List<ParsedRefinee> v)
        {
            ParsedElement child = value.Value;
            if (child is IdentifierExpression) {
                string identifier = ((IdentifierExpression)child).Value;
                v.Add (new ParsedRefinee (identifier));
            } else {
                throw new ParserException ();
            }
        }
    }
}
