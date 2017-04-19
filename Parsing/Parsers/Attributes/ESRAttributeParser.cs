using System;
using System.Collections.Generic;
using System.Linq;
using UCLouvain.KAOSTools.Parsing.Parsers.Exceptions;

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
                        throw new InvalidParameterAttributeException (identifier,
                                                                  InvalidParameterAttributeException.IDENTIFIER);
                    }

                } else {
                    throw new InvalidParameterAttributeException (identifier,
                                                              InvalidParameterAttributeException.ATOMIC_ONLY);
                }
            }

            if (value is NParsedAttributeAtomic) {
                return ParseAtomic (identifier, value, expert_id);

            } else if (value is NParsedAttributeBracket) {
                return ParseBracket (identifier, value);

            } else {
                throw new InvalidAttributeValueException (identifier,
                                                          InvalidAttributeValueException.ATOMIC_OR_BRACKET);
            }
        }

        private static ParsedElement ParseAtomic (string identifier, NParsedAttributeValue value, string expert_id)
        {
            var v = ((NParsedAttributeAtomic)value).Value;
            double esr = -1;

            if (v is ParsedFloat) {
                esr = ((ParsedFloat)v).Value;

            } else if (v is ParsedInteger) {
                esr = ((ParsedInteger)v).Value;

            } else if (v is ParsedPercentage) {
                esr = ((ParsedPercentage)v).Value / 100d;
            } else {
                throw new InvalidAttributeValueException (identifier,
                                                          InvalidAttributeValueException.FLOAT_INTEGER_PERCENTAGE_ONLY);
            }

            if (esr < 0 | esr > 1) // ntm: ignore 00de (Or => Xor)
                throw new InvalidAttributeValueException (identifier,
                                                          InvalidAttributeValueException.PROBABILITY_EXPECTED);

            return new ParsedProbabilityAttribute () { Value = esr, ExpertIdentifier = expert_id };
        }

        private static ParsedElement ParseBracket (string identifier, NParsedAttributeValue value)
        {
            var item = ((NParsedAttributeBracket)value).Item;
            var param = ((NParsedAttributeBracket)value).Parameter;

            // Parse the name of the distribution
            string distribution_name = ParseDistributionName (identifier, item);

            // Parse the parameters of the distribution
            List<double> distribution_parameters = ParseDistributionParameter (identifier, param);

            // Build the parsed element
            switch (distribution_name) {
            case "beta":
                return ParseBeta (identifier, distribution_parameters);

            case "uniform":
                return ParseUniform (identifier, distribution_parameters);

            case "triangular":
                return ParseTriangular (identifier, distribution_parameters);

            case "pert":
            case "PERT":
                return ParsePERT (identifier, distribution_parameters);

            default:
                throw new InvalidAttributeValueException (identifier,
                                                          InvalidAttributeValueException.INVALID_VALUE);
            }
        }

        private static string ParseDistributionName (string identifier, ParsedElement item)
        {
            string distribution_name = null;
            if (item is IdentifierExpression) {
                distribution_name = ((IdentifierExpression)item).Value;
            } else {
                throw new InvalidAttributeValueException (identifier,
                                                          InvalidAttributeValueException.IDENTIFIER);
            }

            return distribution_name;
        }

        private static List<double> ParseDistributionParameter (string identifier, ParsedElement param)
        {
            List<double> distribution_parameters = new List<double> ();
            if (param is NParsedAttributeAtomic) {
                ParseDistributionParameter (identifier, distribution_parameters, ((NParsedAttributeAtomic)param).Value);

            } else if (param is NParsedAttributeList) {
                foreach (var p in ((NParsedAttributeList)param).Values) {
                    ParseDistributionParameter (identifier, distribution_parameters, p);
                }

            } else {
                throw new InvalidAttributeValueException (identifier,
                                                          InvalidAttributeValueException.ATOMIC_OR_LIST);
            }

            return distribution_parameters;
        }

        private static ParsedElement ParseBeta (string identifier, List<double> distribution_parameters)
        {
            if (distribution_parameters.Count != 2)
                throw new InvalidAttributeValueException (identifier,
                                                          InvalidAttributeValueException.INVALID_VALUE);

            double alpha = distribution_parameters [0];
            double beta = distribution_parameters [1];
            if (alpha < 0 | beta < 0)
                throw new InvalidAttributeValueException (identifier,
                                                          InvalidAttributeValueException.INVALID_VALUE);

            return new ParsedBetaDistribution () {
                Alpha = alpha,
                Beta = beta
            };
        }

        private static ParsedElement ParseUniform (string identifier, List<double> distribution_parameters)
        {
            if (distribution_parameters.Count != 2)
                throw new InvalidAttributeValueException (identifier,
                                                          InvalidAttributeValueException.INVALID_VALUE);

            double lower = distribution_parameters [0];
            double upper = distribution_parameters [1];
            if (lower > upper | lower < 0 | upper > 1)
                throw new InvalidAttributeValueException (identifier,
                                                          InvalidAttributeValueException.INVALID_VALUE);
            return new ParsedUniformDistribution () {
                LowerBound = lower,
                UpperBound = upper
            };
        }

        private static ParsedElement ParseTriangular (string identifier, List<double> distribution_parameters)
        {
            if (distribution_parameters.Count != 3)
                throw new InvalidAttributeValueException (identifier,
                                                          InvalidAttributeValueException.INVALID_VALUE);

            double min = distribution_parameters [0];
            double mode = distribution_parameters [1];
            double max = distribution_parameters [2];
            if (min > mode | mode > max | min < 0 | max > 1)
                throw new InvalidAttributeValueException (identifier,
                                                          InvalidAttributeValueException.INVALID_VALUE);

            return new ParsedTriangularDistribution () {
                Min = min,
                Mode = mode,
                Max = max
            };
        }

        private static ParsedElement ParsePERT (string identifier, List<double> distribution_parameters)
        {
            if (distribution_parameters.Count != 3)
                throw new InvalidAttributeValueException (identifier,
                                                          InvalidAttributeValueException.INVALID_VALUE);

            double min = distribution_parameters [0];
            double mode = distribution_parameters [1];
            double max = distribution_parameters [2];
            if (min > mode | mode > max | min < 0 | max > 1)
                throw new InvalidAttributeValueException (identifier,
                                                          InvalidAttributeValueException.INVALID_VALUE);

            return new ParsedPertDistribution () {
                Min = min,
                Mode = mode,
                Max = max
            };
        }

        static void ParseDistributionParameter(string identifier, List<double> distribution_parameters, ParsedElement p)
        {
            if (p is NParsedAttributeAtomic) {
                var pp = ((NParsedAttributeAtomic)p).Value;

                if (pp is ParsedInteger) {
                    distribution_parameters.Add(((ParsedInteger)pp).Value);
                } else if (pp is ParsedFloat) {
                    distribution_parameters.Add(((ParsedFloat)pp).Value);
                } else if (pp is ParsedPercentage) {
                    distribution_parameters.Add(((ParsedPercentage)pp).Value / 100d);
                } else {
                    throw new InvalidAttributeValueException (identifier,
                                                              InvalidAttributeValueException.INVALID_VALUE);
                }

            } else {
                throw new InvalidAttributeValueException (identifier,
                                                          InvalidAttributeValueException.ATOMIC_ONLY);
                
            }
        }
   }
    
}
