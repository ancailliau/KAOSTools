using System;
using System.Collections.Generic;
using LtlSharp;

namespace KAOSTools.Parsing
{
    public class ParsedElement {
        public int Line { get; set; }
        public int Col { get; set; }
        public string Filename { get; set; }
    }
    
    public class ParsedElements : ParsedElement {
        public List<ParsedElement> Values     { get; set; }
        public ParsedElements () {
            Values = new List<ParsedElement>();
        }
    }

    public class ParsedElementWithAttributes : ParsedElement {
        public bool Override { get; set; }
        public List<ParsedAttribute> Attributes { get; set; }
        public ParsedElementWithAttributes ()
        {
            Attributes = new List<ParsedAttribute>();
            Override = false;
        }
    }

    public class ParsedAttribute : ParsedElement {}
    public class ParsedAttributeWithElements : ParsedAttribute {
        public List<dynamic> Values { get; set; }
        public ParsedAttributeWithElements ()
        {
            Values = new List<dynamic>();
        }
    }

    public class ParsedAttributeWithValue<T> : ParsedAttribute {
        public T Value { get; set; }
    }

    #region Enumerations

    public enum ParsedAgentType { 
        None, Software, Environment
    }

    public enum ParsedEntityType {
        None, Software, Environment, Shared
    }

    #endregion

    #region First class

    public class ParsedPredicate : ParsedElementWithAttributes {}
    public class ParsedSystem : ParsedElementWithAttributes {}
    public class ParsedGoal : ParsedElementWithAttributes {}
    public class ParsedDomainProperty : ParsedElementWithAttributes {}
    public class ParsedDomainHypothesis : ParsedElementWithAttributes {}
    public class ParsedObstacle : ParsedElementWithAttributes {}
    public class ParsedAssociation : ParsedElementWithAttributes {}
    public class ParsedGivenType : ParsedElementWithAttributes {}
    public class ParsedAgent : ParsedElementWithAttributes {}

    public class ParsedEntity : ParsedElementWithAttributes
    {
        public ParsedEntityType EntityType { get; set; }
        public ParsedEntity () {
            EntityType = ParsedEntityType.None;
        }
    }

    #endregion

    #region Attributes


    public abstract class ParsedAttributeWithElementsAndSystemIdentifier : ParsedAttributeWithElements {
        public dynamic SystemIdentifier { get; set; }
        public ParsedAttributeWithElementsAndSystemIdentifier () : base ()
        {
            SystemIdentifier = null;
        }
    }

    public class ParsedRefinedByAttribute : ParsedAttributeWithElementsAndSystemIdentifier {}
    public class ParsedAssignedToAttribute : ParsedAttributeWithElementsAndSystemIdentifier {}

    public class ParsedLinkAttribute : ParsedAttribute
    {
        public string Multiplicity { get; set; }
        public dynamic Target { get; set; }
    }

    public class ParsedIsAAttribute : ParsedAttributeWithValue<dynamic> {}
    public class ParsedObstructedByAttribute : ParsedAttributeWithValue<dynamic> {}
    public class ParsedAlternativeAttribute : ParsedAttributeWithValue<dynamic> {}
    public class ParsedResolvedByAttribute : ParsedAttributeWithValue<dynamic> {}

    public class ParsedAgentTypeAttribute : ParsedAttributeWithValue<ParsedAgentType> {}
    public class ParsedEntityTypeAttribute : ParsedAttributeWithValue<ParsedEntityType> {}
    public class ParsedNameAttribute : ParsedAttributeWithValue<string> {}
    public class ParsedIdentifierAttribute : ParsedAttributeWithValue<string> {}
    public class ParsedDefinitionAttribute : ParsedAttributeWithValue<string> {}
    public class ParsedProbabilityAttribute : ParsedAttributeWithValue<double> {}
    public class ParsedRDSAttribute : ParsedAttributeWithValue<double> {}
    public class ParsedFormalSpecAttribute : ParsedAttributeWithValue<ParsedElement> {}
    public class ParsedSignatureAttribute : ParsedAttributeWithValue<string> {}

    public class ParsedPredicateArgumentAttribute : ParsedAttribute {
        public NameExpression Name { get; set; }
        public dynamic Type { get; set; }
        
        public ParsedPredicateArgumentAttribute (NameExpression name, IdentifierOrNameExpression type)
        {
            Name = name; Type = type;
        }
    }

    public class ParsedAttributeAttribute : ParsedAttribute {
        public string Name { get; set; }
        public dynamic Type { get; set; }
        
        public ParsedAttributeAttribute (string name, dynamic type)
        {
            Name = name; Type = type;
        }
    }

    #endregion

    #region Expressions

    [Obsolete]
    public class IdentifierOrNameExpression : ParsedAttribute {
        public string Value { get; set; }
    }
    
    public class IdentifierExpression : IdentifierOrNameExpression
    {
        public IdentifierExpression (string value)
        {
            Value = value;
        }
    }
    
    public class NameExpression : IdentifierOrNameExpression
    {
        public NameExpression (string value)
        {
            Value = value;
        }
    }
    
    public class MultiplictyExpression : ParsedAttribute
    {
        public string Value { get; set; }
    }

    public class ParsedVariableDeclaration {
        public string VariableName;
        public string Type;
        public ParsedVariableDeclaration (string name, string type)
        {
            VariableName = name; Type = type;
        }
    }

    public class ParsedForallExpression : ParsedElement {
        public IList<ParsedVariableDeclaration> arguments;
        public ParsedElement Enclosed;
        public ParsedForallExpression ()
        {
            arguments = new List<ParsedVariableDeclaration> ();
        }
    }
    
    public class ParsedExistsExpression : ParsedElement {
        public IList<ParsedVariableDeclaration> arguments;
        public ParsedElement Enclosed;
        public ParsedExistsExpression ()
        {
            arguments = new List<ParsedVariableDeclaration> ();
        }
    }

    public class ParsedBinaryExpression : ParsedElement {
        public ParsedElement Left;
        public ParsedElement Right;
    }

    public class ParsedStrongImplyExpression : ParsedBinaryExpression {}
    public class ParsedImplyExpression : ParsedBinaryExpression {}
    public class ParsedEquivalenceExpression : ParsedBinaryExpression {}
    public class ParsedUntilExpression : ParsedBinaryExpression {}
    public class ParsedReleaseExpression : ParsedBinaryExpression {}
    public class ParsedUnlessExpression : ParsedBinaryExpression {}
    public class ParsedAndExpression : ParsedBinaryExpression {}
    public class ParsedOrExpression : ParsedBinaryExpression {}
        
    public class ParsedUnaryExpression : ParsedElement {
        public ParsedElement Enclosed;
    }

    public class ParsedNotExpression : ParsedUnaryExpression {}
    public class ParsedNextExpression : ParsedUnaryExpression {}
    public class ParsedEventuallyExpression : ParsedUnaryExpression {}
    public class ParsedGloballyExpression : ParsedUnaryExpression {}


    public class ParsedComparisonExpression : ParsedBinaryExpression {
        public ParsedComparisonCriteria criteria;
        public ParsedComparisonExpression (ParsedComparisonCriteria criteria, 
                                           ParsedElement left, 
                                           ParsedElement right)
        {
            this.criteria = criteria;
            this.Left = left;
            this.Right = right;
        }
    }

    public enum ParsedComparisonCriteria {
        Equals, NotEquals, BiggerThanOrEquals, LessThanOrEquals, LessThan, BiggerThan
    }

    public class ParsedInRelationExpression : ParsedElement {
        public string Relation;
        public IList<string> Variables;
        public ParsedInRelationExpression ()
        {
            Variables = new List<string> ();
        }
    }

    public class ParsedPredicateReferenceExpression : ParsedElement {
        public string PredicateSignature;
        public IList<IdentifierExpression> ActualArguments;
        public ParsedPredicateReferenceExpression (string name)
        {
            PredicateSignature = name;
            ActualArguments = new List<IdentifierExpression>();
        }
    }

    public class ParsedAttributeReferenceExpression : ParsedElement {
        public string Variable;
        public string AttributeSignature;
        public ParsedAttributeReferenceExpression (string variable, string attribute)
        {
            this.Variable = variable;
            this.AttributeSignature = attribute;
        }
    }

    public abstract class ParsedConstantExpression<T> : ParsedElement {
        public T Value;
    }

    public class ParsedStringConstantExpression : ParsedConstantExpression<string> {
        public ParsedStringConstantExpression (string value)
        {
            this.Value = value;
        }
    }

    public class ParsedNumericConstantExpression : ParsedConstantExpression<double> {
        public ParsedNumericConstantExpression (double value)
        {
            this.Value = value;
        }
    }
    
    public class ParsedBoolConstantExpression : ParsedConstantExpression<bool> {
        public ParsedBoolConstantExpression (bool value)
        {
            this.Value = value;
        }
    }

    #endregion

}

