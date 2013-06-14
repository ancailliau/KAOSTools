using System;
using System.Collections.Generic;

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
        public List<dynamic> Attributes { get; set; }
        public ParsedElementWithAttributes ()
        {
            Attributes = new List<dynamic>();
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

    public class ParsedExceptionAttribute : ParsedAttribute {
        public dynamic ResolvedObstacle { get; set ; }
        public dynamic ResolvingGoal { get; set ; }
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

    public class ParsedPredicate            : ParsedElementWithAttributes {}
    public class ParsedSystem               : ParsedElementWithAttributes {}
    public class ParsedGoal                 : ParsedElementWithAttributes {}
    public class ParsedAntiGoal             : ParsedElementWithAttributes {}
    public class ParsedDomainProperty       : ParsedElementWithAttributes {}
    public class ParsedDomainHypothesis     : ParsedElementWithAttributes {}
    public class ParsedObstacle             : ParsedElementWithAttributes {}
    public class ParsedAssociation          : ParsedElementWithAttributes {}
    public class ParsedGivenType            : ParsedElementWithAttributes {}
    public class ParsedAgent                : ParsedElementWithAttributes {}
    public class ParsedAttributeDeclaration : ParsedElementWithAttributes {}

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

    public class ParsedRefinedByAttribute  : ParsedAttributeWithElementsAndSystemIdentifier {
        public ParsedRefinementPattern RefinementPattern { get; set; }
    }
    
    public class ParsedRefinedByAntiGoalAttribute  : ParsedAttributeWithElementsAndSystemIdentifier {
        public ParsedRefinementPattern RefinementPattern { get; set; }
    }

    public class ParsedAssignedToAttribute : ParsedAttributeWithElementsAndSystemIdentifier {}

    public class ParsedIsAAttribute                  : ParsedAttributeWithValue<dynamic> {}
    public class ParsedObstructedByAttribute         : ParsedAttributeWithValue<dynamic> {}
    public class ParsedAlternativeAttribute          : ParsedAttributeWithValue<dynamic> {}
    public class ParsedAssumptionAttribute           : ParsedAttributeWithValue<dynamic> {}
    public class ParsedNegativeAssumptionAttribute   : ParsedAttributeWithValue<dynamic> {}
    public class ParsedAttributeEntityTypeAttribute  : ParsedAttributeWithValue<dynamic> {}
    public class ParsedDerivedAttribute              : ParsedAttributeWithValue<dynamic> {}

    public class ParsedAgentTypeAttribute    : ParsedAttributeWithValue<ParsedAgentType>  {}
    public class ParsedEntityTypeAttribute   : ParsedAttributeWithValue<ParsedEntityType> {}

    public class ParsedNameAttribute         : ParsedAttributeWithValue<string> {}
    public class ParsedIdentifierAttribute   : ParsedAttributeWithValue<string> {}
    public class ParsedDefinitionAttribute   : ParsedAttributeWithValue<string> {}
    public class ParsedSignatureAttribute    : ParsedAttributeWithValue<string> {}

    public class ParsedProbabilityAttribute  : ParsedAttributeWithValue<double> {}
    public class ParsedRDSAttribute          : ParsedAttributeWithValue<double> {}

    public class ParsedFormalSpecAttribute   : ParsedAttributeWithValue<ParsedElement> {}

    
    public class ParsedResolvedByAttribute   : ParsedAttributeWithValue<dynamic> {
        public ParsedResolutionPattern Pattern { get; set; }
    }

    public class ParsedLinkAttribute : ParsedAttribute
    {
        public string Multiplicity { get; set; }
        public dynamic Target { get; set; }
    }


    public class ParsedResolutionPattern : ParsedElement
    {
        public string Name { get; set; }
        public List<dynamic> Parameters { get; set; }
        public ParsedResolutionPattern ()
        {
            Parameters = new List<dynamic> ();
        }
    }

    public class ParsedRefinementPattern : ParsedElement
    {
        public ParsedRefinementPatternName Name { get; set; }
        public List<dynamic> Parameters { get; set; }
        public ParsedRefinementPattern ()
        {
            Parameters = new List<dynamic> ();
        }
    }

    public enum ParsedRefinementPatternName {
        None, Milestone, Case, IntroduceGuard, DivideAndConquer, Unmonitorability, Uncontrollability
    }




    public class ParsedPredicateArgumentAttribute : ParsedAttribute {
        public string Name { get; set; }
        public dynamic Type { get; set; }
        
        public ParsedPredicateArgumentAttribute (string name, dynamic type)
        {
            Name = name; 
            Type = type;
        }
    }

    public class ParsedAttributeAttribute : ParsedAttribute {
        public string Identifier { get; set; }
        public string Name { get; set; }
        public string Definition { get; set; }
        public dynamic Type { get; set; }

        public ParsedAttributeAttribute ()
        {
        }

        public ParsedAttributeAttribute (string name, dynamic type)
        {
            Name = name; 
            Type = type;
        }
    }

    public class ParsedVariableReference : ParsedElement
    {
        public string Value { get; set; }
        public ParsedVariableReference (string value) {
            Value = value;
        }
    }

    #endregion

    #region Expressions

    public class ParsedSystemReference : ParsedElement {
        public dynamic Name { get; set; }
        public List<dynamic> Values { get; set; }
        public ParsedSystemReference ()
        {
            Values = new List<dynamic>();
        }
    }


    public class IdentifierExpression : ParsedAttribute
    {
        public string Value { get; set; }
        public IdentifierExpression (string value) {
            Value = value;
        }
    }
    
    public class NameExpression : ParsedAttribute
    {
        public string Value { get; set; }
        public NameExpression (string value) {
            Value = value;
        }
    }
    
    public class ParsedFloat : ParsedElement
    {
        public double Value { get; set; }
    }
    
    public class ParsedString : ParsedElement
    {
        public string Value { get; set; }
    }

    
    public class MultiplictyExpression : ParsedAttribute
    {
        public string Value { get; set; }
    }

    public class ParsedVariableDeclaration {
        public string VariableName;
        public dynamic Type;
        public ParsedVariableDeclaration (string name, dynamic type)
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
   

    public class ParsedEventuallyBeforeExpression : ParsedBinaryExpression {
        public ParsedTimeBound TimeBound;
    }


    public class ParsedUnaryExpression : ParsedElement {
        public ParsedElement Enclosed;
    }

    public class ParsedNotExpression : ParsedUnaryExpression {}
    public class ParsedNextExpression : ParsedUnaryExpression {}
    public class ParsedEventuallyExpression : ParsedUnaryExpression {
        public ParsedTimeBound TimeBound;
    }
    public class ParsedGloballyExpression : ParsedUnaryExpression {
        public ParsedTimeBound TimeBound;
    }


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
        public dynamic Relation;
        public IList<string> Variables;
        public ParsedInRelationExpression ()
        {
            Variables = new List<string> ();
        }
    }

    public class ParsedPredicateReferenceExpression : ParsedElement {
        public dynamic PredicateSignature;
        public IList<string> ActualArguments;
        public ParsedPredicateReferenceExpression (dynamic nameoridentifier)
        {
            PredicateSignature = nameoridentifier;
            ActualArguments = new List<string>();
        }
    }

    public class ParsedAttributeReferenceExpression : ParsedElement {
        public string Variable;
        public dynamic AttributeSignature;
        public ParsedAttributeReferenceExpression (string variable, dynamic attribute)
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

    public enum ParsedTimeUnit {
        day, hour, minute, second, milisecond
    }
    
    public enum ParsedTimeComparator {
        less, strictly_less, greater, strictly_greater, equal
    }

    public class ParsedTime : ParsedElement {
        public IList<ParsedAtomicTime> Constraints { get; set; }
        public ParsedTime ()
        {
            Constraints = new List<ParsedAtomicTime> ();
        }
    }

    public class ParsedAtomicTime {
        public int Duration { get; set; }
        public ParsedTimeUnit Unit { get; set; }
    }

    public class ParsedTimeBound : ParsedElement {
        public ParsedTimeComparator Comparator;
        public ParsedTime Bound;
    }

}

