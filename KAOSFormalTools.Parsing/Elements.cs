using System;
using System.Collections.Generic;
using LtlSharp;

namespace KAOSFormalTools.Parsing
{
    public interface Element {}
    
    public class Elements : Element {
        public List<Element> Values     { get; set; }
        public Elements () {
            Values = new List<Element>();
        }
    }
    
    public class Predicate : Element
    {
        public List<Attribute> Attributes { get; set; }
        public Predicate () { Attributes = new List<Attribute>(); }
    }
    
    public class Alternative : Element
    {
        public List<Attribute> Attributes { get; set; }
        public Alternative () { Attributes = new List<Attribute>(); }
    }

    public class Goal : Element
    {
        public List<Attribute> Attributes { get; set; }
        public bool Override { get; set; }
        public Goal () { Attributes = new List<Attribute>(); Override = false; }
    }

    public class DomainProperty : Element
    {
        public List<Attribute> Attributes { get; set; }
        public DomainProperty () { Attributes = new List<Attribute>(); }
    }

    public class DomainHypothesis : Element
    {
        public List<Attribute> Attributes { get; set; }
        public DomainHypothesis () { Attributes = new List<Attribute>(); }
    }

    public class Obstacle : Element
    {
        public List<Attribute> Attributes { get; set; }
        public Obstacle () { Attributes = new List<Attribute>(); }
    }

    public class Agent : Element
    {
        public List<Attribute> Attributes { get; set; }
        public AgentType       Type       { get; set; }
        public Agent () { Attributes = new List<Attribute>();  Type = AgentType.None; }
    }

    public enum AgentType { None, Software, Environment }

    public interface Attribute : Element {}

    public class Attributes : Element {
        public List<Attribute> Values     { get; set; }
        public Attributes () {
            Values = new List<Attribute>();
        }
    }

    public class ObstructedByList : Attribute
    {
        public List<Element> Values    { get; set; }

        public ObstructedByList ()
        {
            Values = new List<Element> ();
        }
    }

    public class RefinedByList : Attribute
    {
        public IdentifierOrName AlternativeIdentifier { get; set; }
        public List<Element> Values    { get; set; }

        public RefinedByList ()
        {
            AlternativeIdentifier = null;
            Values = new List<Element> ();
        }
    }

    public class ResolvedByList : Attribute
    {
        public List<Element> Values    { get; set; }
        
        public ResolvedByList ()
        {
            Values = new List<Element> ();
        }
    }

    public class AssignedToList : Attribute
    {
        public IdentifierOrName AlternativeIdentifier { get; set; }
        public List<Element> Values    { get; set; }

        public AssignedToList ()
        {
            AlternativeIdentifier = null;
            Values = new List<Element> ();
        }
    }

    public class IdentifierOrName : Attribute {
        public string Value               { get; set; }
    }

    public class Identifier : IdentifierOrName
    {
        public Identifier (string value)
        {
            Value = value;
        }
    }

    public class Name : IdentifierOrName
    {
        public Name (string value)
        {
            Value = value;
        }
    }

    public class Definition : Attribute
    {
        public string Value { get ; set ; }
        public Definition (string value)
        {
            Value = value;
        }
    }
    
    public class Description : Attribute
    {
        public string Value { get ; set ; }
        public Description (string value)
        {
            Value = value;
        }
    }
    
    public class Probability : Attribute
    {
        public double Value { get ; set ; }
        public Probability (double value)
        {
            Value = value;
        }
    }
    
    public class RDS : Attribute
    {
        public double Value { get ; set ; }
        public RDS (double value)
        {
            Value = value;
        }
    }

    public class FormalSpec : Attribute
    {
        public LTLFormula Value               { get; set; }

        public FormalSpec (string value)
        {
            Value = LtlSharp.Parser.Parse (value);

            if (Value == null) {
                throw new ParsingException (string.Format ("Could not parse '{0}'", value));
            }
        }
    }

    public class StringFormalSpec : Attribute
    {
        public string Value { get; set; }
        public StringFormalSpec (string value)
        {
            Value = value;
        }
    }

    public class Signature : Attribute
    {
        public string Value { get; set; }
        public Signature (string value)
        {
            Value = value;
        }
    }
}

