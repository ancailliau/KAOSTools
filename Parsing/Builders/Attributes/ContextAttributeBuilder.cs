using System;
using System.Text.RegularExpressions;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;
using UCLouvain.KAOSTools.Parsing.Parsers;
using UCLouvain.KAOSTools.Core.Model;

namespace UCLouvain.KAOSTools.Parsing.Builders.Attributes
{
	public class ContextAttributeBuilder : AttributeBuilder<KAOSCoreElement, ParsedContextAttribute>
    {
		public ContextAttributeBuilder()
        {
        }

        public override void Handle(KAOSCoreElement element, ParsedContextAttribute attribute, KAOSModel model)
		{
            if (attribute.Value is IdentifierExpression)
			{
				var id = ((IdentifierExpression)attribute.Value).Value;

				Context context;
                if ((context = model.modelMetadataRepository.GetContext(id)) == null) {
                    context = new Context(model, id) { Implicit = true };
                    model.modelMetadataRepository.Add(context);
                }

				if (element is DomainProperty dp)
					dp.ContextIdentifier = context.Identifier;
				else if (element is DomainHypothesis dh)
					dh.ContextIdentifier = context.Identifier;
				else
					throw new NotImplementedException();
            }
			else
			{
                throw new UnsupportedValue(element, attribute, attribute.Value);
			}
        }
    }
}
