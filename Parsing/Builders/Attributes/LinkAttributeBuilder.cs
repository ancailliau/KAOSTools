using System;
using System.Text.RegularExpressions;
using KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;

namespace KAOSTools.Parsing.Builders.Attributes
{
	public class LinkAttributeBuilder : AttributeBuilder<Relation, ParsedLinkAttribute> 
    {
		public LinkAttributeBuilder()
        {
        }

        public override void Handle(Relation element, ParsedLinkAttribute attribute, KAOSModel model)
		{
			Entity entity;
			if (attribute.Target is IdentifierExpression)
			{
				string id = ((IdentifierExpression)attribute.Target).Value;
                if ((entity = model.entityRepository.GetEntity(id)) != null) {
					element.Links.Add(new Link(model) { Target = entity, Multiplicity = attribute.Multiplicity });
				}
                else
				{
					throw new BuilderException("Entity '" + id + "' is not defined.",
													   attribute.Filename,
													   attribute.Line,
													   attribute.Col);
				}
			}
			else
			{
				throw new NotImplementedException(string.Format("'{0}' is not supported in '{1}' on '{2}'",
																  attribute.Target.GetType().Name,
																  attribute.GetType().Name,
																  element.GetType().Name));
			}
        }
    }
}
