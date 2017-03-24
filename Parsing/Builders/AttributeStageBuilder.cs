using System;
using System.Collections.Generic;
using System.Reflection;
using KAOSTools.Core;
using KAOSTools.Parsing.Builders.Attributes;
using KAOSTools.Parsing.Builders.Declarations;
using KAOSTools.Parsing.Parsers;

namespace KAOSTools.Parsing
{
    public abstract class AttributeStageBuilder : Builder
	{
		protected List<DeclareBuilder> declareBuilders = new List<DeclareBuilder>();

		protected List<IAttributeBuilder> attributeBuilders = new List<IAttributeBuilder>();

		public AttributeStageBuilder(KAOSModel model, Uri relativePath)
            : base (model, relativePath)
        {
		}

		public void BuildElement(ParsedElements elements)
		{
			foreach (dynamic element in elements.Values)
			{
				BuildElement(element);
			}
		}

		public void BuildElement(ParsedModelAttribute element)
		{
		}

		public void BuildElement(ParsedDeclare element)
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

			throw new NotImplementedException(string.Format("'{0}' is not yet supported",
															 element.GetType().Name));
		}

		public void BuildElement(ParsedDeclare element, dynamic e)
		{
			foreach (dynamic attribute in element.Attributes)
			{
				Handle(e, attribute);
			}
		}

		public void Handle(KAOSCoreElement element, ParsedElement attribute)
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
