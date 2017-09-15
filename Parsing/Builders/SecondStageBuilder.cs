using UCLouvain.KAOSTools.Core;
using System.Collections.Generic;
using System;
using System.Linq;
using UCLouvain.KAOSTools.Core.Agents;
using UCLouvain.KAOSTools.Parsing.Builders.Declarations;
using UCLouvain.KAOSTools.Parsing.Builders.Attributes;
using System.Reflection;

namespace UCLouvain.KAOSTools.Parsing
{
    public class SecondStageBuilder : AttributeStageBuilder
	{
		public SecondStageBuilder (KAOSModel model, Uri relativePath)
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

			attributeBuilders.Add(new AgentTypeAttributeBuilder());
            attributeBuilders.Add(new AssignedToAttributeBuilder());
            attributeBuilders.Add(new AttributeAttributeBuilder());
            attributeBuilders.Add(new CustomAttributeBuilder());
            attributeBuilders.Add(new DefaultValueAttributeBuilder());
            attributeBuilders.Add(new DefinitionAttributeBuilder());
            attributeBuilders.Add(new EntityTypeAttributeBuilder());
			attributeBuilders.Add(new IsAAttributeBuilder());
            attributeBuilders.Add(new LinkAttributeBuilder());
            attributeBuilders.Add(new NameAttributeBuilder());
            attributeBuilders.Add(new ObstructedByAttributeBuilder());
			attributeBuilders.Add(new RefinedByGoalAttributeBuilder());
			attributeBuilders.Add(new RefinedByObstacleAttributeBuilder());
            attributeBuilders.Add(new ResolvedByAttributeBuilder());
			attributeBuilders.Add(new RSRAttributeBuilder());

            attributeBuilders.Add(new ESRAttributeBuilder());
            attributeBuilders.Add(new ESRUncertainAttributeBuilder());
            
			attributeBuilders.Add(new ExceptAttributeBuilder());
			attributeBuilders.Add(new ReplacesAttributeBuilder());
        }
    }
}

