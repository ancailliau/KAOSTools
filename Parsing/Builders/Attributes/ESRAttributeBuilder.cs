using System;
using System.Text.RegularExpressions;
using KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;
using KAOSTools.Parsing.Parsers;
using UCLouvain.KAOSTools.Core.SatisfactionRates;

namespace KAOSTools.Parsing.Builders.Attributes
{
    public class ESRAttributeBuilder : AttributeBuilder<KAOSCoreElement, ParsedProbabilityAttribute>
    {
		public ESRAttributeBuilder()
        {
        }

		public override void Handle(KAOSCoreElement element, ParsedProbabilityAttribute attribute, KAOSModel model)
		{
            var doubleSatisfactionRate = new DoubleSatisfactionRate(attribute.Value);

            if (attribute.ExpertIdentifier != null) {
                if (model.modelMetadataRepository.ExpertExists(attribute.ExpertIdentifier)) {
                    doubleSatisfactionRate.ExpertIdentifier = attribute.ExpertIdentifier;

                } else {
                    throw new BuilderException("Expert '"+attribute.ExpertIdentifier+"' is not defined.", attribute);
                }
            }

            if (element is Obstacle) {
                model.satisfactionRateRepository.AddObstacleSatisfactionRate(element.Identifier, doubleSatisfactionRate);
            } else if (element is DomainProperty) {
                model.satisfactionRateRepository.AddDomPropSatisfactionRate(element.Identifier, doubleSatisfactionRate);
            }
        }
    }
}
