﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace UCLouvain.KAOSTools.Parsing.Parsers
{
    public class ParsedElement {
        public int Line { get; set; }
        public int Col { get; set; }
        public string Filename { get; set; }
    }

    public class ParsedModelAttribute  : ParsedElement {
        public string Name { get; set; }
        public string Value { get; set; }
    }
        
    public class ParsedElements : ParsedElement {
        public List<ParsedElement> Values     { get; set; }
        public ParsedElements () {
            Values = new List<ParsedElement>();
        }
    }

    public class ParsedDeclare : ParsedElement {
        public bool Override { get; set; }
        public readonly string Identifier;
        public string DeclaredItem;

		public List<dynamic> Attributes { get; set; }   
		
        public ParsedDeclare (string identifier)
        {
            Identifier = identifier;
            Attributes = new List<dynamic>();
            Override = false;
        }
	}

	public class NParsedAttribute : ParsedElement {
		public string AttributeName { get; set; }
		public NParsedAttributeValue Parameters { get; set; }
		public NParsedAttributeValue AttributeValue { get; set; }
    }

	public abstract class NParsedAttributeValue : ParsedElement {
    }

	public class NParsedAttributeAtomic : NParsedAttributeValue {
        public ParsedElement Value { get; set; }
        public NParsedAttributeAtomic ()
        {
        }
        public NParsedAttributeAtomic(ParsedElement value)
        {
            Value = value;   
        }
    }

	public class NParsedAttributeColon : NParsedAttributeValue
	{
        public ParsedElement Left { get; set; }
		public ParsedElement Right { get; set; }
        public NParsedAttributeColon ()
        {
            
        }
        public NParsedAttributeColon (ParsedElement left, ParsedElement right)
        {
            Left = left;
            Right = right;
        }
    }

	public class NParsedAttributeBracket : NParsedAttributeValue
	{
		public ParsedElement Item { get; set; }
		public ParsedElement Parameter { get; set; }
        public NParsedAttributeBracket ()
        {
            
        }
        public NParsedAttributeBracket (ParsedElement item, ParsedElement parameter)
        {
            Item = item;
            Parameter = parameter;
        }
	}

	public class NParsedAttributeEqual : NParsedAttributeValue
	{
		public ParsedElement Left { get; set; }
		public ParsedElement Right { get; set; }
        public NParsedAttributeEqual ()
        {
            
        }
        public NParsedAttributeEqual (ParsedElement left, ParsedElement right)
        {
            Left = left;
            Right = right;
        }
	}
	
	
    
	public class NParsedAttributeList : NParsedAttributeValue
	{
        public List<ParsedElement> Values { get; set; }
        public NParsedAttributeList(IEnumerable<ParsedElement> values)
		{
            Values = new List<ParsedElement>(values);
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
        None, Software, Environment, Malicious
    }

    public enum ParsedEntityType {
        None, Software, Environment, Shared
    }

    #endregion

    #region First class

    public class ParsedPredicate : ParsedDeclare { public ParsedPredicate (string identifier) : base(identifier) {} }
	public class ParsedSystem : ParsedDeclare { public ParsedSystem (string identifier) : base(identifier) {} }
	public class ParsedGoal : ParsedDeclare { public ParsedGoal (string identifier) : base(identifier) {} }
	public class ParsedSoftGoal : ParsedDeclare { public ParsedSoftGoal (string identifier) : base(identifier) {} }
	public class ParsedDomainProperty : ParsedDeclare { public ParsedDomainProperty (string identifier) : base(identifier) {} }
	public class ParsedDomainHypothesis : ParsedDeclare { public ParsedDomainHypothesis (string identifier) : base(identifier) {} }
	public class ParsedObstacle : ParsedDeclare { public ParsedObstacle (string identifier) : base(identifier) {} }
	public class ParsedAssociation : ParsedDeclare { public ParsedAssociation (string identifier) : base(identifier) {} }
	public class ParsedGivenType : ParsedDeclare { public ParsedGivenType (string identifier) : base(identifier) {} }
	public class ParsedAgent : ParsedDeclare { public ParsedAgent (string identifier) : base(identifier) {} }
	public class ParsedAttributeDeclaration : ParsedDeclare { public ParsedAttributeDeclaration (string identifier) : base(identifier) {} }
	public class ParsedGoalRefinement : ParsedDeclare { public ParsedGoalRefinement (string identifier) : base(identifier) {} }
                                                                            
    public class ParsedExpert : ParsedDeclare { public ParsedExpert (string identifier) : base(identifier) {} }
	public class ParsedCalibration : ParsedDeclare { public ParsedCalibration (string identifier) : base(identifier) {} }
	public class ParsedCostVariable : ParsedDeclare { public ParsedCostVariable (string identifier) : base(identifier) {} }
                                                                            
    public class ParsedConstraint : ParsedDeclare { public ParsedConstraint (string identifier) : base(identifier) {} }

	public class ParsedContext : ParsedDeclare { public ParsedContext (string identifier) : base(identifier) {} }


    public class ParsedEntity : ParsedDeclare
    {
        public ParsedEntityType EntityType { get; set; }
        public ParsedEntity (string identifier) : base (identifier) {
            EntityType = ParsedEntityType.None;
        }
    }

    #endregion

    #region Attributes

    public class ParsedConflictAttribute : ParsedAttributeWithElements {}
    public class ParsedOrCstAttribute    : ParsedAttributeWithElements {}

    public abstract class ParsedAttributeWithElementsAndSystemIdentifier : ParsedAttributeWithElements {
        public dynamic SystemIdentifier { get; set; }
        public ParsedAttributeWithElementsAndSystemIdentifier () : base ()
        {
            SystemIdentifier = null;
        }
    }

    public class ParsedRefinedByAttribute  : ParsedElement {
        public List<ParsedRefinee> ParsedRefinees {
            get;
            set;
        }
        public ParsedRefinementPattern RefinementPattern { get; set; }
        public string ContextIdentifier
		{
			get;
			set;
		}
    }
    
    public class ParsedRefinedByAntiGoalAttribute : ParsedElement {
        public List<ParsedRefinee> ParsedRefinee {
            get;
            set;
        }
        public ParsedRefinementPattern RefinementPattern { get; set; }
    }

    public class ParsedRefinee : ParsedElement
    {
        public string Identifier {
            get;
            set;
        }
        public IParsedRefineeParameter Parameters {
            get;
            set;
        }
        public ParsedRefinee ()
        {
        }
        public ParsedRefinee (string identifier)
        {
            Identifier = identifier;
        }
        public ParsedRefinee (string identifier, IParsedRefineeParameter parameters)
        {
            Identifier = identifier;
            Parameters = parameters;
        }
    }

    public interface IParsedRefineeParameter {}
    public class ParsedPrimitiveRefineeParameter<T> : IParsedRefineeParameter
    {
        public T Value {
            get;
            set;
        }
        public ParsedPrimitiveRefineeParameter (T value)
        {
            Value = value;
        }
    }

    public class ParsedSoftGoalContributionAttribute  : ParsedAttributeWithElements {}
    public class ParsedSoftGoalContribution {
        public dynamic SoftGoal { get; set; }
        public ParsedContribution Contribution { get; set; }
    }
    public enum ParsedContribution { Positive, Negative }

    public class ParsedAssignedToAttribute : ParsedAttributeWithElementsAndSystemIdentifier {}

    public class ParsedGoalRefinementChildrenAttribute : ParsedAttributeWithElements {}

    public class ParsedIsAAttribute                  : ParsedAttributeWithValue<dynamic> {}
    public class ParsedObstructedByAttribute         : ParsedAttributeWithValue<dynamic> {}
    public class ParsedAlternativeAttribute          : ParsedAttributeWithValue<dynamic> {}
    public class ParsedAssumptionAttribute           : ParsedAttributeWithValue<dynamic> {}
    public class ParsedNegativeAssumptionAttribute   : ParsedAttributeWithValue<dynamic> {}
    public class ParsedAttributeEntityTypeAttribute  : ParsedAttributeWithValue<dynamic> {}
    public class ParsedDerivedAttribute              : ParsedAttributeWithValue<dynamic> {}
    public class ParsedSysRefAttribute               : ParsedAttributeWithValue<dynamic> {}
    public class ParsedPatternAttribute              : ParsedAttributeWithValue<ParsedRefinementPattern> {}
    public class ParsedIsComplete                    : ParsedAttributeWithValue<bool> {}

    public class ParsedAgentTypeAttribute    : ParsedAttributeWithValue<ParsedAgentType>  {}
    public class ParsedEntityTypeAttribute   : ParsedAttributeWithValue<ParsedEntityType> {}

    public class ParsedNameAttribute         : ParsedAttributeWithValue<string> {}
    public class ParsedIdentifierAttribute   : ParsedAttributeWithValue<string> {}
    public class ParsedDefinitionAttribute   : ParsedAttributeWithValue<ParsedString> {}
    public class ParsedSignatureAttribute    : ParsedAttributeWithValue<string> {}
    public class DefaultValueAttribute    : ParsedAttributeWithValue<bool> {}
    
    public class ParsedContextAttribute              : ParsedAttributeWithValue<dynamic> {}


    public class ParsedCostAttribute         : ParsedAttributeWithValue<ParsedFloat> {
        public dynamic CostVariable { get; set; }
    }

    public class ParsedCustomAttribute         : ParsedAttributeWithValue<string> {
        public string Key { get; set; }
    }

    public class ParsedExceptAttribute          : ParsedAttribute {
    	public string ObstacleIdentifier
		{
			get;
			set;
		}
		public string CountermeasureIdentifier
		{
			get;
			set;
		}
	}

    public class ParsedReplacesAttribute          : ParsedAttribute {
    	public string ObstacleIdentifier
		{
			get;
			set;
		}
		public string ReplacedGoalIdentifier
		{
			get;
			set;
		}
	}

    public class ParsedProvidedNotAttribute          : ParsedAttribute {
    	public string ObstacleIdentifier
		{
			get;
			set;
		}
		public ParsedElement Formula
		{
			get;
			set;
		}
	}

    public class ParsedProvidedAttribute          : ParsedAttribute {
    	public string ObstacleIdentifier
		{
			get;
			set;
		}
		public ParsedElement Formula
		{
			get;
			set;
		}
	}

    public class ParsedRelaxedToAttribute          : ParsedAttribute {
    	public string ObstacleIdentifier
		{
			get;
			set;
		}
		public ParsedElement Formula
		{
			get;
			set;
		}
	}

	public class ParsedRDSAttribute          : ParsedAttributeWithValue<double> {}

    public class ParsedFormalSpecAttribute   : ParsedAttributeWithValue<ParsedElement> {}

    public class ParsedResolvedByAttribute   : ParsedAttributeWithValue<dynamic> {
        public ParsedResolutionPattern Pattern { get; set; }
        public string AnchorId { get; set; }
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

        public ParsedResolutionPattern(string name) : base ()
        {
            Name = name;
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
        None, Milestone, Case, IntroduceGuard, DivideAndConquer, Unmonitorability, Uncontrollability, Redundant
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
        public string Definition { get; set; }
        public dynamic Type { get; set; }

        public ParsedAttributeAttribute ()
        {
        }

        public ParsedAttributeAttribute (string identifier, dynamic type)
        {
            Identifier = identifier; 
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
        public override string ToString()
        {
            return string.Format("[IdentifierExpression: Value={0}]", Value);
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
        public ParsedFloat ()
        {
        }
        public ParsedFloat (double value)
        {
            Value = value;
        }
	}
    
    public class ParsedDecimal : ParsedElement
    {
        public Decimal Value { get; set; }
        public ParsedDecimal ()
        {
        }
        public ParsedDecimal (Decimal value)
        {
            Value = value;
        }
	}

	public class ParsedInteger : ParsedElement
	{
		public int Value { get; set; }
        public ParsedInteger ()
        {
        }
        public ParsedInteger (int value)
        {
            Value = value;
        }
	}

	public class ParsedPercentage : ParsedElement
	{
		public double Value { get; set; }
        public ParsedPercentage ()
        {
        }
        public ParsedPercentage (double value)
        {
            Value = value;
        }
	}
    
    public class ParsedString : ParsedElement
    {
        public bool Verbatim { get; set; }
        public string Value { get; set; }
        public ParsedString ()
        {

        }
        public ParsedString (string value)
        {
            Value = value;
        }
	}

	public class ParsedBool : ParsedElement
	{
		public bool Value { get; set; }
        public ParsedBool ()
        {

        }
        public ParsedBool (bool value)
        {
            Value = value;
        }
    }

    public class StarExpression : ParsedAttribute
    {
        public StarExpression ()
        {
        }
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
    public class ParsedUntilExpression : ParsedBinaryExpression {
		public ParsedTimeBound TimeBound;
	}
    public class ParsedReleaseExpression : ParsedBinaryExpression {}
    public class ParsedUnlessExpression : ParsedBinaryExpression {
		public ParsedTimeBound TimeBound;
	}
    public class ParsedAndExpression : ParsedBinaryExpression {}
    public class ParsedOrExpression : ParsedBinaryExpression {}
   
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





	public class ParsedExpertProbabilityAttribute : ParsedAttribute
	{
		public dynamic IdOrNAme { get; set; }
		public dynamic Estimate { get; set; }
	}

	public class ParsedProbabilityAttribute : ParsedAttributeWithValue<double>
	{
		public string ExpertIdentifier { get; set; }

	}

	public class ParsedUDistribution : ParsedElement
	{
		public string ExpertIdentifier { get; set; }
    }

    public class ParsedUniformDistribution : ParsedUDistribution {
        public double LowerBound;
		public double UpperBound;
    }

    public class ParsedTriangularDistribution : ParsedUDistribution {
        public double Min;
        public double Max;
        public double Mode;
    }

    public class ParsedPertDistribution : ParsedUDistribution {
        public double Min;
        public double Max;
        public double Mode;
    }

    public class ParsedBetaDistribution : ParsedUDistribution {
        public double Alpha;
        public double Beta;
    }

    public class ParsedQuantileDistribution : ParsedUDistribution {
        public List<double> Quantiles;
        public ParsedQuantileDistribution ()
        {
            Quantiles = new List<double> ();
        }
        public ParsedQuantileDistribution(List<double> quantiles)
		{
			Quantiles = quantiles;
		}
    }

	

    public class ParsedMonitorsAttribute  : ParsedElement {
        public List<string> ParsedPredicates {
            get;
            set;
        }
    }

    public class ParsedControlsAttribute  : ParsedElement {
        public List<string> ParsedPredicates {
            get;
            set;
        }
    }
}

