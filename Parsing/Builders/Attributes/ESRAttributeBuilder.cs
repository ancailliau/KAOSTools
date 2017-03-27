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
            if (element is Obstacle) {
                model.satisfactionRateRepository.AddObstacleSatisfactionRate(element.Identifier, new DoubleSatisfactionRate(attribute.Value));
            } else if (element is DomainProperty) {
                model.satisfactionRateRepository.AddDomPropSatisfactionRate(element.Identifier, new DoubleSatisfactionRate(attribute.Value));
            }
        }
    }
}
