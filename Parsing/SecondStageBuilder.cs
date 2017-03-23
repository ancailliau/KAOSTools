using KAOSTools.Core;
using System.Collections.Generic;
using System;
using System.Linq;
using UCLouvain.KAOSTools.Core.Agents;
using KAOSTools.Parsing.Builders.Declarations;
using KAOSTools.Parsing.Builders.Attributes;
using System.Reflection;

namespace KAOSTools.Parsing
{
    public class SecondStageBuilder : Builder
	{
		List<DeclareBuilder> declareBuilders = new List<DeclareBuilder>();

		List<IAttributeBuilder> attributeBuilders = new List<IAttributeBuilder>();

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
			declareBuilders.Add(new SoftGoalDeclareParser());
			declareBuilders.Add(new TypeDeclareBuilder());

			attributeBuilders.Add(new AgentTypeAttributeBuilder());
            attributeBuilders.Add(new ArgumentAttributeBuilder());
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
            attributeBuilders.Add(new RefinedByAttributeBuilder());
            attributeBuilders.Add(new ResolvedByAttributeBuilder());
            attributeBuilders.Add(new RSRAttributeBuilder());
        }

        public void BuildElement (ParsedElements elements)
        {
            foreach (dynamic element in elements.Values) {
                BuildElement (element);
            }
        }

        public void BuildElement (ParsedModelAttribute element)
        {
        }

        public void BuildElement (ParsedDeclare element)
        {
			foreach (var builder in declareBuilders)
			{
				if (builder.IsBuildable(element))
				{
                    var e = builder.GetBuiltElement(element, model);
					if (e == null)
						throw new InvalidOperationException(string.Format("Element '{0}' was not pre-built during first stage.", e.Identifier));

                    BuildElement(element, e);
					return;
				}
			}

            throw new NotImplementedException (string.Format("'{0}' is not yet supported", 
                                                             element.GetType().Name));
        }

        public void BuildElement (ParsedDeclare element, dynamic e)
        {
            foreach (dynamic attribute in element.Attributes) {
				Handle  (e, attribute);
            }
        }

        public void Handle (KAOSCoreElement element, ParsedElement attribute)
        {
            foreach (var builder in attributeBuilders)
            {
                var genericArguments = builder.GetType().BaseType.GetGenericArguments();

				if (genericArguments[0].IsAssignableFrom(element.GetType())
					&& genericArguments[1].IsAssignableFrom(attribute.GetType()))
                {
                    var method = builder.GetType().GetMethod("Handle", new[] { genericArguments[0], genericArguments[1], typeof(KAOSModel) });
                    if (method == null)
                    {
                        throw new Exception("Cannot find method Handle with generic parameters.");
                    }
                    Console.WriteLine("Calling " + builder.GetType() + "." + method.ToString());
                    try
                    {
                        method.Invoke(builder, new object[] { element, attribute, model });
                    }
					catch (TargetInvocationException e)
                    {
                        throw e.InnerException;
                    }
                }
            }
        }

    }
}

