using System;
using System.Collections.Generic;
using UCLouvain.KAOSTools.Core;
using System.Linq;
using UCLouvain.KAOSTools.Core.Agents;
using UCLouvain.KAOSTools.Parsing.Parsers;

namespace UCLouvain.KAOSTools.Parsing.Builders.Attributes
{
    public class ArgumentAttributeBuilder : AttributeBuilder<Predicate, ParsedPredicateArgumentAttribute>
	{
		IDictionary<Predicate, int> predicateArgumentCurrentPosition;

        public ArgumentAttributeBuilder()
        {
			this.predicateArgumentCurrentPosition = new Dictionary<Predicate, int> ();
        }

        public override void Handle(Predicate element, ParsedPredicateArgumentAttribute attribute, KAOSModel model)
        {
			var arg_name = attribute.Name;
			Entity arg_type = null;
			if (attribute.Type != null)
			{
				if (attribute.Type is IdentifierExpression)
				{
					var id = ((IdentifierExpression)attribute.Type).Value;
					arg_type = model.entityRepository.GetEntity(id);
                    if (arg_type == null) {
                        arg_type = new Entity(model, id) { Implicit = true };
						model.entityRepository.Add(arg_type);
                    }

                } else {
                    throw new UnsupportedValue(element, attribute, attribute.Type);
                }
            }

            var currentPosition = 0;
            if (!predicateArgumentCurrentPosition.ContainsKey(element))
			{
				predicateArgumentCurrentPosition.Add(element, 0);
			}
			else
			{
				currentPosition = predicateArgumentCurrentPosition[element];
			}

			// No argument with the same name is already declared
			if (element.Arguments.Count(w => w.Name == arg_name) == 0)
			{
				element.Arguments.Add(new PredicateArgument() { Name = arg_name, Type = arg_type });
			}

			// Otherwise, it shall be the same to the one already declared
			else
			{
				// if no type is already declared, use the new one (may be not declared)
				if (element.Arguments[currentPosition].Type == null)
				{
					element.Arguments[currentPosition].Type = arg_type;
				}

				// if a type was already declared, it shall be the same (or an ancestor)
				else if (!arg_type.Ancestors().Contains(element.Arguments[currentPosition].Type))
				{
					throw new BuilderException(
                        string.Format("Argument at index {0} does not match. Actual has identifier '{1}' and name " +
                                      "'{2}' but expected has identifier '{3}' and name '{4}'. Check that you don't " +
                                      "mix name and identifier references on implicit declarations.",
															   currentPosition,
															   element.Arguments[currentPosition].Type.Identifier,
															   element.Arguments[currentPosition].Type.Name,
															   arg_type.Identifier,
															   arg_type.Name),
												attribute.Filename, attribute.Line, attribute.Col);
				}

				// if no name is already declared, use the new one (may be not declared)
				if (element.Arguments[currentPosition].Name == null)
				{
					element.Arguments[currentPosition].Name = arg_name;
				}

				// if a name was already defined, it shall be the same
				else if (element.Arguments[currentPosition].Name != arg_name)
				{
					throw new BuilderException(
                        string.Format("Argument at index {0} shall be named '{1}' but is '{2}'",
                                      currentPosition, element.Arguments[currentPosition].Name, arg_name),
                        attribute.Filename, attribute.Line, attribute.Col);
				}
			}

			predicateArgumentCurrentPosition[element]++;
        }
    }
}
