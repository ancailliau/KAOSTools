using KAOSTools.Core;
using System.Collections.Generic;
using System;
using System.Linq;
using KAOSTools.Parsing.Builders.Attributes;
using KAOSTools.Parsing.Builders.Declarations;
using System.Reflection;

namespace KAOSTools.Parsing
{
    public class ThirdStageBuilder : AttributeStageBuilder
    {
		FormulaBuilder fb;

        public ThirdStageBuilder (KAOSModel model, 
                                  FormulaBuilder fb,
                                  Uri relativePath)
            : base (model, relativePath)
        {
            this.fb = fb;
			declareBuilders.Add(new AgentDeclareBuilder());
			declareBuilders.Add(new AssociationDeclareBuilder());
			declareBuilders.Add(new CalibrationDeclareBuilder());
			declareBuilders.Add(new DomainHypothesisDeclareBuilder());
			declareBuilders.Add(new DomainPropertyDeclareBuilder());
			declareBuilders.Add(new EntityDeclareBuilder());
			declareBuilders.Add(new ExpertDeclareBuilder());
			declareBuilders.Add(new GoalDeclareBuilder());
			declareBuilders.Add(new SoftGoalDeclareParser());
			declareBuilders.Add(new TypeDeclareBuilder());

            attributeBuilders.Add(new ArgumentAttributeBuilder());
            attributeBuilders.Add(new FormalSpecAttributeBuilder(fb));
		}
    }
}

