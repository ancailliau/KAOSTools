using System;
using KAOSTools.Core;
namespace KAOSTools.Parsing.Builders.Attributes
{
	public abstract class AttributeBuilder<T, V> : IAttributeBuilder
        where T : KAOSCoreElement
        where V : ParsedElement
	{
		public abstract void Handle(T element, V attribute, KAOSModel model);

		protected void Handle<T>(KAOSCoreElement element,
							   T value,
							   string propertyName)
		{
			var definitionProperty = element.GetType().GetProperty(propertyName);
			if (definitionProperty == null)
				throw new InvalidOperationException(string.Format("'{0}' has no property '{1}'",
																	element.GetType(), propertyName));

			if (definitionProperty.PropertyType != typeof(T))
				throw new InvalidOperationException(string.Format("Type of property '{1}' in '{0}' is not '{2}'",
																	element.GetType(), propertyName, typeof(T).Name));

			definitionProperty.SetValue(element, value, null);
		}
    }
}
