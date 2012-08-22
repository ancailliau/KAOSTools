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

    public class Goal : Element
    {
        public List<Attribute> Attributes { get; set; }
        public Goal () { Attributes = new List<Attribute>(); }
    }

    public class DomainProperty : Element
    {
        public List<Attribute> Attributes { get; set; }
        public DomainProperty () { Attributes = new List<Attribute>(); }
    }

    public class Obstacle : Element
    {
        public List<Attribute> Attributes { get; set; }
        public Obstacle () { Attributes = new List<Attribute>(); }
    }

    public interface Attribute : Element {}

    public class Attributes : Element {
        public List<Attribute> Values     { get; set; }
        public Attributes () {
            Values = new List<Attribute>();
        }
    }

    public class ObstructedByList : Attribute
    {
        public List<IdentifierOrName> Values    { get; set; }

        public ObstructedByList ()
        {
            Values = new List<IdentifierOrName> ();
        }
    }

    public class RefinedByList : Attribute
    {
        public List<IdentifierOrName> Values    { get; set; }

        public RefinedByList ()
        {
            Values = new List<IdentifierOrName> ();
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
}

