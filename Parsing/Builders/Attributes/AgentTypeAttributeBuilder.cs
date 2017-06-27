using System;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Parsing.Parsers;
using UCLouvain.KAOSTools.Core.Agents;

namespace UCLouvain.KAOSTools.Parsing.Builders.Attributes
{
    public class AgentTypeAttributeBuilder : AttributeBuilder<Agent, ParsedAgentTypeAttribute>
    {
        public AgentTypeAttributeBuilder()
        {
        }

		public override void Handle(Agent element, ParsedAgentTypeAttribute attribute, KAOSModel model)
        {
            if (attribute.Value == ParsedAgentType.Environment)
				Handle(element, AgentType.Environment, "Type");

			else if (attribute.Value == ParsedAgentType.Software)
				Handle(element, AgentType.Software, "Type");

			else if (attribute.Value == ParsedAgentType.Malicious)
				Handle(element, AgentType.Malicious, "Type");

			else
				throw new NotImplementedException();
        }
    }
}
