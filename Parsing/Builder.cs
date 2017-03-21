using System;
using System.Text.RegularExpressions;
using System.Linq;
using KAOSTools.Core;
using System.Collections.Generic;

namespace KAOSTools.Parsing
{
    public class Builder
    {
        protected KAOSModel model;
        protected IDictionary<KAOSCoreElement, IList<Declaration>> declarations;
        protected Uri relativePath;
    
        public Builder (KAOSModel model, 
                        IDictionary<KAOSCoreElement, IList<Declaration>> declarations,
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
            
            name = Sanitize((expr as ParsedNameAttribute).Value);
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
        
        protected dynamic GetByIdentifier (ParsedElementWithAttributes element, string identifier)
        {
            return model.Elements.SingleOrDefault (e => e.Identifier == identifier);
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
            return model.Elements.SingleOrDefault (e => { 
                var property = e.GetType ().GetProperty (propertyName);
                if (property == null)
                    return false;
                return ((string) property.GetValue(e, null)) == propertyValue;
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
            where T : KAOSCoreElement
        {
            Func<T, bool> predicate = e => (string) e.GetType ().GetProperty (property).GetValue (e, null) == value;
            element = model.Elements.Where (x => x is T).Cast<T>().SingleOrDefault (predicate);

            if (element != null)
                return true;

            return false;
        }

        protected bool Get<T> (IdentifierExpression identifier, out T element)
            where T : KAOSCoreElement
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
            where T : KAOSCoreElement
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
            where T : KAOSCoreElement
        {
            dynamic t = (T) Activator.CreateInstance (typeof(T), new Object[] { model });

            t.Implicit = true;
            t.Identifier = identifier.Value;
            
            model.Add (t);
            declarations.Add (t, new List<Declaration> {
                new Declaration (identifier.Line, identifier.Col, identifier.Filename, relativePath, DeclarationType.Reference)
            });
            
            return t;
        }

        protected T Create<T> (NameExpression name)
            where T : KAOSCoreElement
        {
			dynamic t = (T) Activator.CreateInstance (typeof(T), new Object[] { model });
            t.Implicit = true;

            if (typeof(T).GetProperty ("Name") == null)
                throw new InvalidOperationException (string.Format ("'{0}' has no property 'Name'", typeof(T).Name));

            typeof(T).GetProperty ("Name").SetValue (t, name.Value, null);

            model.Add (t);
            declarations.Add (t, new List<Declaration> {
                new Declaration (name.Line, name.Col, name.Filename, relativePath, DeclarationType.Reference)
            });
            
            return t;
        }

        #endregion
    }
}

