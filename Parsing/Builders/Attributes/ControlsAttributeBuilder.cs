using System;
using System.Text.RegularExpressions;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;
using UCLouvain.KAOSTools.Parsing.Parsers;
using UCLouvain.KAOSTools.Core.Model;

namespace UCLouvain.KAOSTools.Parsing.Builders.Attributes
{
	public class ControlsAttributeBuilder : AttributeBuilder<Agent, ParsedControlsAttribute>
	{
		public ControlsAttributeBuilder()
		{
		}

		public override void Handle(Agent element, ParsedControlsAttribute attribute, KAOSModel model)
		{
			foreach (var id in attribute.ParsedPredicates)
			{
				Predicate p;
				if ((p = model.formalSpecRepository.GetPredicate(id)) == null)
				{
					p = new Predicate(model, id) { Implicit = true };
					model.Add(p);
				}


				var monitoringLink = new AgentControlLink(model)
				{
					AgentIdentifier = element.Identifier,
					PredicateIdentifier = p.Identifier
				};

				model.Add(monitoringLink);
			}

		}
	}
}
