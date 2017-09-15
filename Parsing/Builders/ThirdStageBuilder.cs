using UCLouvain.KAOSTools.Core;
using System.Collections.Generic;
using System;
using System.Linq;
using UCLouvain.KAOSTools.Parsing.Builders.Attributes;
using UCLouvain.KAOSTools.Parsing.Builders.Declarations;
using System.Reflection;

namespace UCLouvain.KAOSTools.Parsing
{
    public class ThirdStageBuilder : AttributeStageBuilder
    {
		public ThirdStageBuilder (KAOSModel model, 
                                  FormulaBuilder fb,
                                  Uri relativePath)
            : base (model, relativePath)
        {
        	declareBuilders.Add(new AgentDeclareBuilder());
			declareBuilders.Add(new AssociationDeclareBuilder());
			declareBuilders.Add(new CalibrationDeclareBuilder());
			declareBuilders.Add(new DomainHypothesisDeclareBuilder());
			declareBuilders.Add(new DomainPropertyDeclareBuilder());
			declareBuilders.Add(new EntityDeclareBuilder());
			declareBuilders.Add(new ExpertDeclareBuilder());
			declareBuilders.Add(new GoalDeclareBuilder());
			declareBuilders.Add(new SoftGoalDeclareBuilder());
			declareBuilders.Add(new TypeDeclareBuilder());
			declareBuilders.Add(new ObstacleDeclareBuilder());
            declareBuilders.Add(new PredicateDeclareBuilder());

            attributeBuilders.Add(new ArgumentAttributeBuilder());
            attributeBuilders.Add(new FormalSpecAttributeBuilder(fb));
            
            attributeBuilders.Add(new ProvidedNotAttributeBuilder(fb));
		}
    }
}

