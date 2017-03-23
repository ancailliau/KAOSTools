using System;
using System.Text.RegularExpressions;
using KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;

namespace KAOSTools.Parsing.Builders.Attributes
{
    public class RSRAttributeBuilder : AttributeBuilder<Goal, ParsedRDSAttribute>
    {
		public RSRAttributeBuilder()
        {
        }

        public override void Handle(Goal element, ParsedRDSAttribute attribute, KAOSModel model)
		{
            Handle(element, attribute.Value, "RDS");
        }
    }
}
