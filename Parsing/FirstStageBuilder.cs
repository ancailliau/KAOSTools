using System;
using KAOSTools.MetaModel;
using System.Collections.Generic;
using System.Linq;

namespace KAOSTools.Parsing
{

    /// <summary>
    /// First stage builder.
    /// </summary>
    public class FirstStageBuilder : Builder
    {

        public FirstStageBuilder (KAOSModel model, 
                                  IDictionary<KAOSMetaModelElement, 
                                  IList<Declaration>> declarations,
                                  Uri relativePath)
            : base (model, declarations, relativePath)
        {}

        public void BuildElementWithKeys (ParsedElements elements)
        {
            foreach (var element in elements.Values.Where (x => x is ParsedElementWithAttributes).Cast<ParsedElementWithAttributes>()) {
                BuildElementWithKeys (element);
            }
        }

        public KAOSMetaModelElement BuildElementWithKeys (ParsedElementWithAttributes element)
        {
            if (element is ParsedGoal)
                return BuildKAOSElement<Goal> (element);

            if (element is ParsedAntiGoal)
                return BuildKAOSElement<AntiGoal> (element);

            if (element is ParsedDomainProperty)
                return BuildKAOSElement<DomainProperty> (element);

            if (element is ParsedDomainHypothesis)
                return BuildKAOSElement<DomainHypothesis> (element);

            if (element is ParsedObstacle)
                return BuildKAOSElement<Obstacle> (element);

            if (element is ParsedAgent)
                return BuildKAOSElement<Agent> (element);

            if (element is ParsedPredicate)
                return BuildKAOSElement<Predicate> (element);

            if (element is ParsedSystem)
                return BuildKAOSElement<AlternativeSystem> (element);

            if (element is ParsedEntity)
                return BuildKAOSElement<Entity> (element);

            if (element is ParsedGivenType)
                return BuildKAOSElement<GivenType> (element);

            if (element is ParsedAssociation)
                return BuildKAOSElement<Relation> (element);

            throw new BuilderException (string.Format ("'{0}' not supported", element.GetType ()),
                                        element.Filename, element.Line, element.Col);
        }

        public T BuildKAOSElement<T> (ParsedElementWithAttributes parsedElement) 
            where T: KAOSMetaModelElement
        {
            var element = (T) Activator.CreateInstance (typeof(T), new object[] { model });

            string name, identifier, signature;
            var hasName = GetName (parsedElement, out name);
            var hasIdentifier = GetIdentifier (parsedElement, out identifier);
            var hasSignature = GetSignature (parsedElement, out signature);

            Func<KAOSMetaModelElement, bool> predicate = e => e.GetType () == typeof(T) && e.Identifier == identifier;
            if (hasIdentifier 
                && model.Elements.Any (predicate)) {
                if (parsedElement.Override)
                    return model.Elements.Single (predicate).OverrideKeys (element) as T;
                
                throw new BuilderException (string.Format ("Identifier '{0}' is declared multiple times", 
                                                           identifier),
                                            parsedElement.Filename, parsedElement.Line, parsedElement.Col);
            }
            
            var hasNameProperty = typeof(T).GetProperty ("Name") != null;
            predicate = e => e.GetType () == typeof(T) 
                && typeof(T).GetProperty("Name").GetValue(e, null) as string == name;

            if (hasName & !hasNameProperty)
                throw new InvalidOperationException("Attempt to set a name to '" + element.GetType () + "'.");

            if (!hasIdentifier
                && hasNameProperty 
                && hasName 
                && model.Elements.Any(predicate)) {
                if (parsedElement.Override)
                    return model.Elements.Single (predicate).OverrideKeys (element) as T;
                
                throw new BuilderException (string.Format ("Name '{0}' is declared multiple times", 
                                                           name),
                                            parsedElement.Filename, parsedElement.Line, parsedElement.Col);
            }

            if (hasIdentifier) 
                typeof(T).GetProperty ("Identifier").SetValue (element, identifier, null);
            
            if (hasNameProperty & hasName) 
                typeof(T).GetProperty ("Name").SetValue (element, name, null);

            if (!declarations.ContainsKey(element)) {
                declarations.Add (element, new List<Declaration> {
                    new Declaration (parsedElement.Line, parsedElement.Col, parsedElement.Filename, relativePath, DeclarationType.Declaration)
                });
            } else {
                declarations[element].Add(new Declaration (parsedElement.Line, parsedElement.Col, parsedElement.Filename, relativePath, DeclarationType.Override));
            }

            model.Add (element);

            return element;
        }
    }
}

