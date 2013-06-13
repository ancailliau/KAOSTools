using System;
using System.Text.RegularExpressions;
using System.Linq;
using KAOSTools.MetaModel;
using System.Collections.Generic;

namespace KAOSTools.Parsing
{
    public class Builder
    {
        protected KAOSModel model;
        protected IDictionary<KAOSMetaModelElement, IList<Declaration>> declarations;
        protected Uri relativePath;
    
        public Builder (KAOSModel model, 
                        IDictionary<KAOSMetaModelElement, IList<Declaration>> declarations,
                        Uri relativePath)
        {
            this.model = model;
            this.declarations = declarations;
            this.relativePath = relativePath;
        }

        protected string Sanitize (string text) 
        {
            var t = Regex.Replace(text, @"\s+", " ", RegexOptions.Multiline).Trim ();
            t = Regex.Replace (t, "\"\"", "\"", RegexOptions.Multiline);
            return t;
        }

        protected bool GetName (ParsedElementWithAttributes parsedElement, out string name) {
            if (parsedElement == null)
                throw new ArgumentNullException ("parsedElement");
            
            if (parsedElement.Attributes.Count (x => x.GetType () == typeof(ParsedSignatureAttribute)) > 1)
                throw new InvalidOperationException ("Attribute 'name' is defined multiple times.");

            var expr = parsedElement.Attributes.SingleOrDefault (x => x.GetType () == typeof(ParsedNameAttribute));
            if (expr == null) {
                name = null;
                return false;
            }
            
            name = (expr as ParsedNameAttribute).Value;
            return true;
        }
        
        protected bool GetSignature (ParsedElementWithAttributes parsedElement, out string signature) {
            if (parsedElement == null)
                throw new ArgumentNullException ("parsedElement");

            if (parsedElement.Attributes.Count (x => x.GetType () == typeof(ParsedSignatureAttribute)) > 1)
                throw new InvalidOperationException ("Attribute 'signature' is defined multiple times.");
            
            var expr = parsedElement.Attributes.SingleOrDefault (x => x.GetType () == typeof(ParsedSignatureAttribute));
            if (expr == null) {
                signature = null;
                return false;
            }
            
            signature = (expr as ParsedSignatureAttribute).Value;
            return true;
        }
        
        protected bool GetIdentifier (ParsedElementWithAttributes parsedElement, out string identifier) {
            if (parsedElement == null)
                throw new ArgumentNullException ("parsedElement");
            
            var expr = parsedElement.Attributes.SingleOrDefault (x => x.GetType () == typeof(ParsedIdentifierAttribute));
            if (expr == null) {
                identifier = null;
                return false;
            }
            
            identifier = (expr as ParsedIdentifierAttribute).Value;
            return true;
        }

        protected IEnumerable<dynamic> GetCollection (ParsedElementWithAttributes element)
        {
            if (element is ParsedPredicate)
                return model.Predicates;
            
            if (element is ParsedSystem)
                return model.GoalModel.Systems;
            
            if (element is ParsedGoal)
                return model.GoalModel.Goals;
            
            if (element is ParsedDomainProperty)
                return model.GoalModel.DomainProperties;
            
            if (element is ParsedDomainHypothesis)
                return model.GoalModel.DomainHypotheses;
            
            if (element is ParsedObstacle)
                return model.GoalModel.Obstacles;
            
            if (element is ParsedAssociation)
                return model.Relations;
            
            if (element is ParsedGivenType)
                return model.GivenTypes;
            
            if (element is ParsedAgent)
                return model.GoalModel.Agents;
            
            if (element is ParsedEntity)
                return model.Entities;
            
            throw new NotImplementedException (string.Format ("Collection inference not implemented for '{0}'",
                                                              element.GetType ()));
        }

        protected ISet<T> GetCollection<T> ()
        {
            if (typeof(T) == typeof(Predicate))
                return (ISet<T>) model.Predicates;
            
            if (typeof(T) == typeof(AlternativeSystem))
                return (ISet<T>) model.GoalModel.Systems;
            
            if (typeof(T) == typeof(Goal))
                return (ISet<T>) model.GoalModel.Goals;
            
            if (typeof(T) == typeof(DomainProperty))
                return (ISet<T>) model.GoalModel.DomainProperties;
            
            if (typeof(T) == typeof(DomainHypothesis))
                return (ISet<T>) model.GoalModel.DomainHypotheses;
            
            if (typeof(T) == typeof(Obstacle))
                return (ISet<T>) model.GoalModel.Obstacles;
            
            if (typeof(T) == typeof(Relation))
                return (ISet<T>) model.Relations;
            
            if (typeof(T) == typeof(GivenType))
                return (ISet<T>) model.GivenTypes;
            
            if (typeof(T) == typeof(Agent))
                return (ISet<T>) model.GoalModel.Agents;
            
            if (typeof(T) == typeof(Entity))
                return (ISet<T>) model.Entities;
            
            throw new NotImplementedException (string.Format ("Collection inference not implemented for '{0}'",
                                                              typeof(T).Name));
        }
        
        protected dynamic GetByIdentifier (ParsedElementWithAttributes element, string identifier)
        {
            return GetCollection (element).SingleOrDefault (e => e.Identifier == identifier);
        }
        
        protected dynamic GetByName (ParsedElementWithAttributes element, string name)
        {
            return GetByProperty (element, "Name", name);
        }
        
        protected dynamic GetBySignature (ParsedElementWithAttributes element, string signature)
        {
            return GetByProperty (element, "Signature", signature);
        }
        
        protected dynamic GetByProperty (ParsedElementWithAttributes element, 
                               string propertyName, 
                               string propertyValue)
        {
            return GetCollection (element).SingleOrDefault (e => { 
                var property = e.GetType ().GetProperty (propertyName);
                if (property == null)
                    throw new InvalidOperationException (string.Format ("'{0}' has no property '{1}'",
                                                                        e.GetType (), propertyName));
                return property.GetValue(e, null) == propertyValue;
            });
        }
        
        protected dynamic GetElement (ParsedElementWithAttributes element)
        {
            string name, identifier, signature;
            
            if (GetIdentifier (element, out identifier))
                return GetByIdentifier (element, identifier);
            
            if (GetName (element, out name))
                return GetByName (element, name);
            
            if (GetSignature (element, out signature))
                return GetBySignature (element, signature);
            
            throw new InvalidOperationException (string.Format (
                "Element '{0}' has no identifier, no name, no signature.", element));
        }


        #region Get helpers

        bool Get<T> (string value, out T element, string property)
            where T : KAOSMetaModelElement
        {
            Func<T, bool> predicate = e => (string) e.GetType ().GetProperty (property).GetValue (e, null) == value;
            if (GetCollection<T> ().Any (predicate)) {
                element = GetCollection<T> ().Single (predicate);
                return true;
            }
            
            element = null;
            return false;
        }

        protected bool Get<T> (IdentifierExpression identifier, out T element)
            where T : KAOSMetaModelElement
        {
            var val = Get (identifier.Value, out element, "Identifier");
            if (val) {
                if (!declarations.ContainsKey(element)) {
                    declarations.Add (element, new List<Declaration> ());
                }
                declarations[element].Add (new Declaration (identifier.Line, identifier.Col, identifier.Filename, relativePath, DeclarationType.Reference));
            }

            return val;
        }

        protected bool Get<T> (NameExpression name, out T element)
            where T : KAOSMetaModelElement
        {
            var val = Get (name.Value, out element, "Name");
            if (val) {
                if (!declarations.ContainsKey(element)) {
                    declarations.Add (element, new List<Declaration> ());
                }
                declarations[element].Add (new Declaration (name.Line, name.Col, name.Filename, relativePath, DeclarationType.Reference));
            }

            return val;
        }

        #endregion

        #region Create helpers

        protected T Create<T> (IdentifierExpression identifier)
            where T : KAOSMetaModelElement, new() 
        {
            var system = new T () {
                Implicit = true,
                Identifier = identifier.Value
            };
            
            GetCollection<T> ().Add (system);
            declarations.Add (system, new List<Declaration> {
                new Declaration (identifier.Line, identifier.Col, identifier.Filename, relativePath, DeclarationType.Reference)
            });
            
            return system;
        }

        protected T Create<T> (NameExpression name)
            where T : KAOSMetaModelElement, new() 
        {
            var t = new T () {
                Implicit = true
            };

            if (typeof(T).GetProperty ("Name") == null)
                throw new InvalidOperationException (string.Format ("'{0}' has no property 'Name'", typeof(T).Name));

            typeof(T).GetProperty ("Name").SetValue (t, name.Value, null);

            GetCollection<T> ().Add (t);
            declarations.Add (t, new List<Declaration> {
                new Declaration (name.Line, name.Col, name.Filename, relativePath, DeclarationType.Reference)
            });
            
            return t;
        }

        #endregion
    }
}

