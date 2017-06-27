using System;
using System.Collections.Generic;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Core.Agents;
using UCLouvain.KAOSTools.Parsing.Parsers;

namespace UCLouvain.KAOSTools.Parsing.Builders.Attributes
{
	public class FormalSpecAttributeBuilder : AttributeBuilder<KAOSCoreElement, ParsedFormalSpecAttribute>
	{
        FormulaBuilder fb;

        public FormalSpecAttributeBuilder(FormulaBuilder fb)
        {
            this.fb = fb;
        }

        public override void Handle(KAOSCoreElement element, ParsedFormalSpecAttribute attribute, KAOSModel model)
        {
            if (element is Predicate)
			    Handle(element, fb.BuildPredicateFormula((Predicate)element, attribute.Value), "FormalSpec");
            else
				Handle(element, fb.BuildFormula(attribute.Value), "FormalSpec");
        }
    }
}
