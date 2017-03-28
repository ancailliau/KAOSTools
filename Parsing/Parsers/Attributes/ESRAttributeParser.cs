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
            string expert_id = null;
            if (parameters != null) {
                if (parameters is NParsedAttributeAtomic) {
                    var parameter = ((NParsedAttributeAtomic)parameters).Value;
                    if (parameter is IdentifierExpression) {
                        expert_id = ((IdentifierExpression)parameter).Value;

                    } else {
                        throw new NotImplementedException("Attribute '" + identifier + "' only accept an indentifier parameter.");
                    }

                } else {
                    throw new NotImplementedException("Attribute '" + identifier + "' only accept an atomic parameter.");
                }
            }

            if (value is NParsedAttributeAtomic) {
                var v = ((NParsedAttributeAtomic)value).Value;

                if (v is ParsedFloat) {
                    double esr = ((ParsedFloat)v).Value;
                    return new ParsedProbabilityAttribute() { Value = esr, ExpertIdentifier = expert_id };

                }

                if (v is ParsedInteger) {
                    double esr = ((ParsedInteger)v).Value * 1d;
                    return new ParsedProbabilityAttribute() { Value = esr, ExpertIdentifier = expert_id };

                }

                if (v is ParsedPercentage) {
                    double esr = ((ParsedPercentage)v).Value / 100d;
                    return new ParsedProbabilityAttribute() { Value = esr, ExpertIdentifier = expert_id };
                }

            }

            if (value is NParsedAttributeBracket) {
                var item = ((NParsedAttributeBracket)value).Item;
                var param = ((NParsedAttributeBracket)value).Parameter;

                // Parse the name of the distribution
                string distribution_name = null;
                if (item is IdentifierExpression) {
                    distribution_name = ((IdentifierExpression)item).Value;
                }

                // Parse the parameters of the distribution
                List<double> distribution_parameters = new List<double> ();
				if (param is NParsedAttributeAtomic) {
                    ParseDistributionParameter(distribution_parameters, ((NParsedAttributeAtomic)param).Value);

                } else if (param is NParsedAttributeList) {
                    foreach (var p in ((NParsedAttributeList)param).Values) {
                        ParseDistributionParameter(distribution_parameters, p);
                    }

                } else {
					throw new NotImplementedException();
				}

                // Build the parsed element
                switch (distribution_name) {
					case "beta":
                        return new ParsedBetaDistribution() {
                            Alpha = distribution_parameters[0],
                            Beta = distribution_parameters[1]
						};

                    case "uniform":
                        return new ParsedUniformDistribution() { 
                            LowerBound = distribution_parameters[0], 
                            UpperBound = distribution_parameters[1]
                        };

                    case "triangular":
						return new ParsedTriangularDistribution() { 
                            Min = distribution_parameters[0], 
                            Mode = distribution_parameters[1], 
                            Max = distribution_parameters[2]
                        };

					case "pert":
					case "PERT":
                        return new ParsedPertDistribution() { 
                            Min = distribution_parameters[0], 
                            Mode = distribution_parameters[1], 
                            Max = distribution_parameters[2]
                        };

                    default:
                        throw new NotImplementedException();
                }
            }

            throw new NotImplementedException("Attribute '" + identifier + "' received an invalid value.");
        }

        static void ParseDistributionParameter(List<double> distribution_parameters, ParsedElement p)
        {
            if (p is NParsedAttributeAtomic) {
                var pp = ((NParsedAttributeAtomic)p).Value;
                if (pp is ParsedInteger) {
                    distribution_parameters.Add(((ParsedInteger)pp).Value);
                } else if (pp is ParsedFloat) {
                    distribution_parameters.Add(((ParsedFloat)pp).Value);
                } else if (pp is ParsedPercentage) {
                    distribution_parameters.Add(((ParsedPercentage)pp).Value / 100d);
                }

            } else {
                throw new NotImplementedException();
            }
        }
   }
    
}
