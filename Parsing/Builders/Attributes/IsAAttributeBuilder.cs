using System;
using System.Text.RegularExpressions;
using KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;

namespace KAOSTools.Parsing.Builders.Attributes
{
	public class IsAAttributeBuilder : AttributeBuilder<Entity, ParsedIsAAttribute>
    {
		public IsAAttributeBuilder()
        {
        }

	    public override void Handle(Entity element, ParsedIsAAttribute attribute, KAOSModel model)
		{
			if (attribute.Value is IdentifierExpression)
			{
                string id = ((IdentifierExpression)attribute.Value).Value;
				
                Entity entity;
                if ((entity = model.entityRepository.GetEntity(id)) != null)
                {
                    element.AddParent(entity);
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
																  attribute.Value.GetType().Name,
																  attribute.GetType().Name,
																  element.GetType().Name));
			}
        }
    }
}
