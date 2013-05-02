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
        private Uri relativePath;

        public FirstStageBuilder (KAOSModel model, 
                                  IDictionary<KAOSMetaModelElement, 
                                  IList<Declaration>> declarations,
                                  Uri relativePath)
            : base (model, declarations)
        {
            this.relativePath = relativePath;
        }

        public void BuildElementWithKeys (ParsedElements elements)
        {
            foreach (var element in elements.Values) {
                BuildElementWithKeys (element);
            }
        }

        public KAOSMetaModelElement BuildElementWithKeys (ParsedElement element)
        {
            if (element is ParsedGoal)
                return BuildKAOSElement<Goal> (element as ParsedGoal, 
                                               model.GoalModel.Goals);

            if (element is ParsedDomainProperty)
                return BuildKAOSElement<DomainProperty> (element as ParsedDomainProperty, 
                                                         model.GoalModel.DomainProperties);

            if (element is ParsedDomainHypothesis)
                return BuildKAOSElement<DomainHypothesis> (element as ParsedDomainHypothesis, 
                                                           model.GoalModel.DomainHypotheses);

            if (element is ParsedObstacle)
                return BuildKAOSElement<Obstacle> (element as ParsedObstacle, 
                                                   model.GoalModel.Obstacles);

            if (element is ParsedAgent)
                return BuildKAOSElement<Agent> (element as ParsedAgent, 
                                                model.GoalModel.Agents);

            if (element is ParsedPredicate)
                return BuildKAOSElement<Predicate> (element as ParsedPredicate, 
                                                    model.Predicates);

            if (element is ParsedSystem)
                return BuildKAOSElement<AlternativeSystem> (element as ParsedSystem, 
                                                            model.GoalModel.Systems);

            if (element is ParsedEntity)
                return BuildKAOSElement<Entity> (element as ParsedEntity, 
                                                 model.Entities);

            if (element is ParsedGivenType)
                return BuildKAOSElement<GivenType> (element as ParsedGivenType, 
                                                    model.GivenTypes);

            if (element is ParsedAssociation)
                return BuildKAOSElement<Entity, Relation> (element as ParsedAssociation, 
                                                           model.Entities);

            throw new BuilderException (string.Format ("'{0}' not supported", element.GetType ()),
                                        element.Filename, element.Line, element.Col);
        }

        public T BuildKAOSElement<T> (ParsedElementWithAttributes parsedElement,
                                      ISet<T> collection) 
            where T: KAOSMetaModelElement, new()
        {
            return BuildKAOSElement<T,T> (parsedElement, collection);
        }

        /// <summary>
        /// Builds a KAOS element <c>TOut</c> and add it to the specified collection. The element created contains
        /// <c>Identifier</c>, <c>Name</c> and <c>Signature</c> if an attribute defines it.
        /// </summary>
        /// <returns>The KAOS element.</returns>
        /// <param name="parsedElement">Parsed element.</param>
        /// <param name="collection">Collection.</param>
        /// <typeparam name="TIn">The type of the collection to write to.</typeparam>
        /// <typeparam name="TOut">The type of the element to create.</typeparam>
        public TOut BuildKAOSElement<TIn,TOut> (ParsedElementWithAttributes parsedElement,
                                                ISet<TIn> collection) 
            where TOut: TIn, KAOSMetaModelElement, new()
            where TIn: KAOSMetaModelElement
        {
            var element = new TOut ();

            string name, identifier, signature;
            var hasName = GetName (parsedElement, out name);
            var hasIdentifier = GetIdentifier (parsedElement, out identifier);
            var hasSignature = GetSignature (parsedElement, out signature);

            Func<TIn, bool> predicate = e => e.GetType () == typeof(TOut) && e.Identifier == identifier;
            if (hasIdentifier 
                && collection.Any (predicate)) {
                if (parsedElement.Override)
                    return collection.Single (predicate).OverrideKeys (element) as TOut;
                
                throw new BuilderException (string.Format ("Identifier '{0}' is declared multiple times", 
                                                           identifier),
                                            parsedElement.Filename, parsedElement.Line, parsedElement.Col);
            }
            
            var hasNameProperty = typeof(TOut).GetProperty ("Name") != null;
            predicate = e => e.GetType () == typeof(TOut) 
                && typeof(TOut).GetProperty("Name").GetValue(e, null) as string == name;

            if (hasName & !hasNameProperty)
                throw new InvalidOperationException("Attempt to set a name to '" + element.GetType () + "'.");

            if (hasNameProperty 
                && hasName 
                && collection.Any(predicate)) {
                if (parsedElement.Override)
                    return collection.Single (predicate).OverrideKeys (element) as TOut;
                
                throw new BuilderException (string.Format ("Name '{0}' is declared multiple times", 
                                                           name),
                                            parsedElement.Filename, parsedElement.Line, parsedElement.Col);
            }

            var hasSignatureProperty = typeof(TOut).GetProperty ("Signature") != null;
            predicate = e => e.GetType () == typeof(TOut) 
                && typeof(TOut).GetProperty("Signature").GetValue(e, null) as string == signature;
            
            if (hasSignature & !hasSignatureProperty)
                throw new InvalidOperationException("Attempt to set a signature to '" + element.GetType () + "'.");

            if (hasSignatureProperty 
                && hasSignature 
                && collection.Any(predicate)) {
                if (parsedElement.Override)
                    return collection.Single (predicate).OverrideKeys (element) as TOut;
                
                throw new BuilderException (string.Format ("Signature '{0}' is declared multiple times", 
                                                           signature),
                                            parsedElement.Filename, parsedElement.Line, parsedElement.Col);
            }

            if (hasIdentifier) 
                typeof(TOut).GetProperty ("Identifier").SetValue (element, identifier, null);
            
            if (hasNameProperty & hasName) 
                typeof(TOut).GetProperty ("Name").SetValue (element, name, null);

            if (hasSignatureProperty & hasSignature) 
                typeof(TOut).GetProperty ("Signature").SetValue (element, signature, null);

            declarations.Add (element, new List<Declaration> {
                new Declaration (parsedElement.Line, 
                                 parsedElement.Col, 
                                 parsedElement.Filename, relativePath)
            });

            collection.Add (element);

            return element;
        }
    }
}

