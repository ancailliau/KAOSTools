using System;
using System.Collections.Generic;

namespace KAOSTools.MetaModel
{
    public abstract class Formula {}

    public class Forall : Formula {
        public IList<ArgumentDeclaration> Declarations;
        public Formula Enclosed;
        public Forall ()
        {
            Declarations = new List<ArgumentDeclaration> ();
        }
    }

    public class Exists : Formula {
        public IList<ArgumentDeclaration> Declarations;
        public Formula Enclosed;
        public Exists ()
        {
            Declarations = new List<ArgumentDeclaration> ();
        }
    }

    public class ArgumentDeclaration {
        public string Name { get; set; }
        public Entity Type { get; set; }
    }

    public class StrongImply : Formula {
        public Formula Left;
        public Formula Right;
    }
    
    public class Imply : Formula {
        public Formula Left;
        public Formula Right;
    }
    
    public class Equivalence : Formula {
        public Formula Left;
        public Formula Right;
    }
    
    public class Until : Formula {
        public Formula Left;
        public Formula Right;
    }
    
    public class Release : Formula {
        public Formula Left;
        public Formula Right;
    }
    
    public class Unless : Formula {
        public Formula Left;
        public Formula Right;
    }
    
    public class And : Formula {
        public Formula Left;
        public Formula Right;
    }
    
    public class Or : Formula {
        public Formula Left;
        public Formula Right;
    }
    
    public class Not : Formula {
        public Formula Enclosed;
    }
    
    public class Next : Formula {
        public Formula Enclosed;
    }
    
    public class Eventually : Formula {
        public Formula Enclosed;
        public TimeBound TimeBound;
    }
    
    public class EventuallyBefore : Formula {
        public Formula Left;
        public Formula Right;
        public TimeBound TimeBound;
    }

    public class Globally : Formula {
        public Formula Enclosed;
        public TimeBound TimeBound;
    }

    public class PredicateReference : Formula {
        public Predicate Predicate;
        public IList<string> ActualArguments;
        public PredicateReference ()
        {
            ActualArguments = new List<string>();
        }
    }
    
    public class RelationReference : Formula {
        public Relation Relation;
        public IList<string> ActualArguments;
        public RelationReference ()
        {
            ActualArguments = new List<string>();
        }
    }
    
    public class VariableReference : Formula {
        public string Name { get; set; }
        public VariableReference ()
        {}
        public VariableReference (string name)
        {
            this.Name = name;
        }
    }

    public class AttributeReference : Formula {
        public string Variable { get; set; }
        public Entity Entity { get; set; }
        public Attribute Attribute { get; set; }

        public AttributeReference ()
        {}

        public AttributeReference (string variable, Entity entity, Attribute attribute)
        {
            this.Variable = variable;
            this.Entity = entity;
            this.Attribute = attribute;
        }
    }

    public class ComparisonPredicate : Formula {
        public ComparisonCriteria Criteria { get; set; }
        public Formula Left { get; set; }
        public Formula Right { get; set; }
    }

    public enum ComparisonCriteria {
        Equals, NotEquals, BiggerThanOrEquals, LessThanOrEquals, LessThan, BiggerThan
    }

    public class StringConstant : Formula {
        public string Value { get; set; }
    }

    public class NumericConstant : Formula {
        public double Value { get; set; }
    }

    public class BoolConstant : Formula {
        public bool Value { get; set; }
    }
    #region Time bound

    public enum TimeComparator {
        less, strictly_less, greater, strictly_greater, equal
    }

    public class TimeBound {
        public TimeComparator Comparator;
        public TimeSpan Bound;
    }

    #endregion
}

