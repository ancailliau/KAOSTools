using System;
using System.Text.RegularExpressions;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;
using UCLouvain.KAOSTools.Parsing.Parsers;
using UCLouvain.KAOSTools.Core.SatisfactionRates;
using System.Linq;

namespace UCLouvain.KAOSTools.Parsing.Builders.Attributes
{
	public class ESRUncertainAttributeBuilder : AttributeBuilder<KAOSCoreElement, ParsedUDistribution>
    {
		public ESRUncertainAttributeBuilder()
        {
        }

		public override void Handle(KAOSCoreElement element, ParsedUDistribution attribute, KAOSModel model)
		{
            ISatisfactionRate satRate = null;

            if (attribute is ParsedUniformDistribution) {
                var ud = ((ParsedUniformDistribution)attribute);
                satRate = new UniformSatisfactionRate(ud.LowerBound, ud.UpperBound);

            } else if (attribute is ParsedBetaDistribution) {
				var ud = ((ParsedBetaDistribution)attribute);
                satRate = new BetaSatisfactionRate(ud.Alpha, ud.Beta);

			} else if (attribute is ParsedTriangularDistribution) {
				var ud = ((ParsedTriangularDistribution)attribute);
				satRate = new TriangularSatisfactionRate(ud.Min, ud.Mode, ud.Max);

            } else if (attribute is ParsedPertDistribution) {
				var ud = ((ParsedPertDistribution)attribute);
                satRate = new PERTSatisfactionRate(ud.Min, ud.Mode, ud.Max);

            } else if (attribute is ParsedQuantileDistribution pqd) {
            	if (!model.Parameters.ContainsKey("experts.quantiles"))
					throw new InvalidProgramException("Please specify the quantiles.");
	
				// Get the string and removes first and last characters (parenthesis)
				string quantile_string = model.Parameters["experts.quantiles"];
				quantile_string = quantile_string.Remove(quantile_string.Length - 1).Substring(1);
	
				// Build the list of quantiles
				var quantiles = quantile_string.Split(',').Select(x => {
					var y = x.Trim();
					if (y.EndsWith("%", StringComparison.Ordinal)) {
						return double.Parse(y.Remove(y.Length - 1));
					} else {
						return double.Parse(y);
					}
				}).ToArray();

                satRate = new QuantileList(pqd.Quantiles);

            } else
                throw new NotImplementedException();

            // Fill the expert if provided
            if (attribute.ExpertIdentifier != null) {
                if (model.modelMetadataRepository.ExpertExists(attribute.ExpertIdentifier)) {
					satRate.ExpertIdentifier = attribute.ExpertIdentifier;

                } else {
                    throw new BuilderException("Expert '"+attribute.ExpertIdentifier+"' is not defined.", attribute);
                }
            }

            // Adds to the correct collection
            if (element is Obstacle) {
				model.satisfactionRateRepository.AddObstacleSatisfactionRate(element.Identifier, satRate);

            } else if (element is DomainProperty) {
				model.satisfactionRateRepository.AddDomPropSatisfactionRate(element.Identifier, satRate);
				
            } else if (element is Calibration) {
				model.satisfactionRateRepository.AddCalibrationSatisfactionRate(element.Identifier, satRate);
				
            } else {
				throw new NotImplementedException();
            }
        }
    }
}
