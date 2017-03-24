﻿using System;
using System.Text.RegularExpressions;
using KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;
using KAOSTools.Parsing.Parsers;

namespace KAOSTools.Parsing.Builders.Attributes
{
	public class EntityTypeAttributeBuilder : AttributeBuilder<Entity, ParsedEntityTypeAttribute>
    {
		public EntityTypeAttributeBuilder()
        {
        }

		public override void Handle(Entity element, ParsedEntityTypeAttribute attribute, KAOSModel model)
		{
			EntityType type;
			if (attribute.Value == ParsedEntityType.Software)
			{
				type = EntityType.Software;
			}
			else if (attribute.Value == ParsedEntityType.Environment)
			{
				type = EntityType.Environment;
			}
			else if (attribute.Value == ParsedEntityType.Shared)
			{
				type = EntityType.Shared;
			}
			else
			{
				type = EntityType.None;
			}

			Handle(element, type, "Type");
        }
    }
}