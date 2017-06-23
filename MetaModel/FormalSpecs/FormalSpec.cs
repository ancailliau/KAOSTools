using System;
using System.Collections.Generic;
using System.Linq;

namespace KAOSTools.Core
{
	public abstract class Formula
	{
		public abstract IEnumerable<PredicateReference> PredicateReferences { get; }

        public abstract Formula Copy ();
    }

	public class Forall : Formula {
        public HashSet<ArgumentDeclaration> Declarations;
        public Formula Enclosed;
        public Forall ()
        {
            Declarations = new HashSet<ArgumentDeclaration> ();
        }

		public override IEnumerable<PredicateReference> PredicateReferences {
			get {
				return Enclosed.PredicateReferences;
			}
		}
        
        public override bool Equals (object obj)
        {
            if (obj is Forall f) {
                return f.Declarations.SetEquals (Declarations) & f.Enclosed.Equals (Enclosed);
            }
            return false;
        }
        
        public override string ToString ()
        {
            return string.Format ("forall {0} . {1}", string.Join (",", Declarations.Select (x=>x.ToString ())), Enclosed);
        }
        
        public override Formula Copy ()
        {
            return new Forall () {
                Declarations = new HashSet<ArgumentDeclaration> (Declarations),
                Enclosed = Enclosed.Copy ()
            };
        }
	}

    public class Exists : Formula {
        public HashSet<ArgumentDeclaration> Declarations;
        public Formula Enclosed;
        public Exists ()
        {
            Declarations = new HashSet<ArgumentDeclaration> ();
        }

		public override IEnumerable<PredicateReference> PredicateReferences {
			get {
				return Enclosed.PredicateReferences;
			}
		}
        
        public override bool Equals (object obj)
        {
            if (obj is Exists f) {
                return f.Declarations.SetEquals (Declarations) & f.Enclosed.Equals (Enclosed);
            }
            return false;
        }
        
        public override string ToString ()
        {
            return string.Format ("exists {0} . {1}", string.Join (",", Declarations.Select (x=>x.ToString ())), Enclosed);
        }
        
        public override Formula Copy ()
        {
            return new Exists () {
                Declarations = new HashSet<ArgumentDeclaration> (Declarations),
                Enclosed = Enclosed.Copy ()
            };
        }
    }

    public class ArgumentDeclaration {
        public string Name { get; set; }
        
        public string Type { get; set; }
        
        public override bool Equals (object obj)
        {
            if (obj is ArgumentDeclaration ad) {
                return ad.Name.Equals (Name) && ad.Type.Equals (Type);
            }
            return false;
        }

        public override int GetHashCode ()
        {
            return 17 + Name.GetHashCode () + 23 * Type.GetHashCode ();
        }
        
        public override string ToString ()
        {
            return string.Format ("{0}: {1}", Name, Type);
        }
        
        public ArgumentDeclaration Copy ()
        {
            return new ArgumentDeclaration () {
                Name = Name,
                Type = Type
            };
        }
    }

    public class StrongImply : Formula {
        public Formula Left;
        public Formula Right;

		public override IEnumerable<PredicateReference> PredicateReferences {
			get {
				return Left.PredicateReferences.Union (Right.PredicateReferences);
			}
		}
        
        public override bool Equals (object obj)
        {
            if (obj is StrongImply f) {
                return f.Left.Equals (Left) & f.Right.Equals (Right);
            }
            return false;
        }
        
        public override string ToString ()
        {
            var left = Left.ToString ();
            if (!(Left is PredicateReference
                  | Left is RelationReference
                  | Left is AttributeReference
                  | Left is ComparisonPredicate
                  | Left is Not
                  | Left is Next
                  | Left is Eventually
                  | Left is Globally
                  | Left is And
                  | Left is Or
                  | Left is Release
                  | Left is Until
                  | Left is Unless
                  | Left is Imply
                  | Left is Equivalence)) {
                left = "(" + left + ")";
            }
            return string.Format ("when {0} then {1}", Left, Right);
        }
        
        public override Formula Copy ()
        {
            return new StrongImply () {
                Left = Left.Copy (),
                Right = Right.Copy ()
            };
        }
    }
    
    public class Imply : Formula {
        public Formula Left;
        public Formula Right;
        public Imply ()
        {
            
        }
        public Imply (Formula left, Formula right)
        {
            this.Left = left;
            this.Right = right;
        }

		public override IEnumerable<PredicateReference> PredicateReferences {
			get {
				return Left.PredicateReferences.Union(Right.PredicateReferences);
			}
		}
        
        public override bool Equals (object obj)
        {
            if (obj is Imply f) {
                return f.Left.Equals (Left) & f.Right.Equals (Right);
            }
            return false;
        }
        
        public override string ToString ()
        {
            var left = Left.ToString ();
            if (!(Left is PredicateReference 
                  | Left is RelationReference 
                  | Left is AttributeReference 
                  | Left is ComparisonPredicate
                  | Left is Not
                  | Left is Next
                  | Left is Eventually
                  | Left is Globally
                  | Left is And
                  | Left is Or
                  | Left is Release
                  | Left is Until
                  | Left is Unless)) {
                left = "(" + left + ")";
            }
            return string.Format ("if {0} then {1}", Left, Right);
        }
        
        public override Formula Copy ()
        {
            return new Imply () {
                Left = Left.Copy (),
                Right = Right.Copy ()
            };
        }
    }
    
    public class Equivalence : Formula {
        public Formula Left;
        public Formula Right;

		public override IEnumerable<PredicateReference> PredicateReferences {
			get {
				return Left.PredicateReferences.Union(Right.PredicateReferences);
			}
		}
        
        public override bool Equals (object obj)
        {
            if (obj is Equivalence f) {
                return f.Left.Equals (Left) & f.Right.Equals (Right);
            }
            return false;
        }
        
        public override string ToString ()
        {
            var left = Left.ToString ();
            if (!(Left is PredicateReference
                  | Left is RelationReference
                  | Left is AttributeReference
                  | Left is ComparisonPredicate
                  | Left is Not
                  | Left is Next
                  | Left is Eventually
                  | Left is Globally
                  | Left is And
                  | Left is Or
                  | Left is Release
                  | Left is Until
                  | Left is Unless)) {
                left = "(" + left + ")";
            }
            return string.Format ("{0} iff {1}", left, Right);
        }
        
        public override Formula Copy ()
        {
            return new Equivalence () {
                Left = Left.Copy (),
                Right = Right.Copy ()
            };
        }
    }
    
    public class Until : Formula {
        public Formula Left;
        public Formula Right;
        public Until ()
        {
            
        }
        public Until (Formula left, Formula right)
        {
            this.Left = left;
            this.Right = right;
        }

		public override IEnumerable<PredicateReference> PredicateReferences {
			get {
				return Left.PredicateReferences.Union(Right.PredicateReferences);
			}
		}
        
        public override bool Equals (object obj)
        {
            if (obj is Until f) {
                return f.Left.Equals (Left) & f.Right.Equals (Right);
            }
            return false;
        }
        
        public override string ToString ()
        {
            var left = Left.ToString ();
            if (!(Left is PredicateReference 
                  | Left is RelationReference 
                  | Left is AttributeReference 
                  | Left is ComparisonPredicate
                  | Left is Not
                  | Left is Next
                  | Left is Eventually
                  | Left is Globally
                  | Left is And
                  | Left is Or)) {
                left = "(" + left + ")";
            }
            return string.Format ("{0} until {1}", left, Right);
        }
        
        public override Formula Copy ()
        {
            return new Until () {
                Left = Left.Copy (),
                Right = Right.Copy ()
            };
        }
    }
    
    public class Release : Formula {
        public Formula Left;
        public Formula Right;

		public override IEnumerable<PredicateReference> PredicateReferences {
			get {
				return Left.PredicateReferences.Union(Right.PredicateReferences);
			}
		}
        
        public override bool Equals (object obj)
        {
            if (obj is Release f) {
                return f.Left.Equals (Left) & f.Right.Equals (Right);
            }
            return false;
        }
        
        public override string ToString ()
        {
            var left = Left.ToString ();
            if (!(Left is PredicateReference 
                  | Left is RelationReference 
                  | Left is AttributeReference 
                  | Left is ComparisonPredicate
                  | Left is Not
                  | Left is Next
                  | Left is Eventually
                  | Left is Globally
                  | Left is And
                  | Left is Or)) {
                left = "(" + left + ")";
            }
            return string.Format ("{0} release {1}", left, Right);
        }
        
        public override Formula Copy ()
        {
            return new Release () {
                Left = Left.Copy (),
                Right = Right.Copy ()
            };
        }
    }
    
    public class Unless : Formula {
        public Formula Left;
        public Formula Right;

		public override IEnumerable<PredicateReference> PredicateReferences {
			get {
				return Left.PredicateReferences.Union(Right.PredicateReferences);
			}
		}
        
        public override bool Equals (object obj)
        {
            if (obj is Unless f) {
                return f.Left.Equals (Left) & f.Right.Equals (Right);
            }
            return false;
        }
        
        public override string ToString ()
        {
            var left = Left.ToString ();
            if (!(Left is PredicateReference
                  | Left is RelationReference
                  | Left is AttributeReference
                  | Left is ComparisonPredicate
                  | Left is Not
                  | Left is Next
                  | Left is Eventually
                  | Left is Globally
                  | Left is And
                  | Left is Or)) {
                left = "(" + left + ")";
            }
            return string.Format ("{0} unless {1}", left, Right);
        }
        
        public override Formula Copy ()
        {
            return new Unless () {
                Left = Left.Copy (),
                Right = Right.Copy ()
            };
        }
    }
    
    public class And : Formula {
        public Formula Left;
        public Formula Right;
        public And ()
        {
            
        }
        public And (Formula left, Formula right)
        {
            this.Left = left;
            this.Right = right;
        }

		public override IEnumerable<PredicateReference> PredicateReferences {
			get {
				return Left.PredicateReferences.Union(Right.PredicateReferences);
			}
		}
        
        public override bool Equals (object obj)
        {
            if (obj is And f) {
                return f.Left.Equals (Left) & f.Right.Equals (Right);
            }
            return false;
        }
        
        public override string ToString ()
        {
            var left = Left.ToString ();
            if (!(Left is PredicateReference 
                  | Left is RelationReference 
                  | Left is AttributeReference 
                  | Left is ComparisonPredicate
                  | Left is Not
                  | Left is Next
                  | Left is Eventually
                  | Left is Globally)) {
                left = "(" + left + ")";
            }
            return string.Format ("{0} and {1}", left, Right);
        }
        
        public override Formula Copy ()
        {
            return new And () {
                Left = Left.Copy (),
                Right = Right.Copy ()
            };
        }
    }
    
    public class Or : Formula {
        public Formula Left;
        public Formula Right;
        public Or ()
        {

        }
        public Or (Formula left, Formula right)
        {
            this.Left = left;
            this.Right = right;
        }

		public override IEnumerable<PredicateReference> PredicateReferences {
			get {
				return Left.PredicateReferences.Union(Right.PredicateReferences);
			}
		}
        
        public override bool Equals (object obj)
        {
            if (obj is Or f) {
                return f.Left.Equals (Left) & f.Right.Equals (Right);
            }
            return false;
        }
        
        public override string ToString ()
        {
            var left = Left.ToString ();
            if (!(Left is PredicateReference 
                  | Left is RelationReference 
                  | Left is AttributeReference 
                  | Left is ComparisonPredicate
                  | Left is Not
                  | Left is Next
                  | Left is Eventually
                  | Left is Globally)) {
                left = "(" + left + ")";
            }
            return string.Format ("{0} or {1}", left, Right);
        }
        
        public override Formula Copy ()
        {
            return new Or () {
                Left = Left.Copy (),
                Right = Right.Copy ()
            };
        }
    }
    
    public class Not : Formula {
        public Formula Enclosed;
        public Not ()
        {
            
        }
        public Not (Formula enclosed)
        {
            this.Enclosed = enclosed;
        }

		public override IEnumerable<PredicateReference> PredicateReferences {
			get {
				return Enclosed.PredicateReferences;
			}
		}
        
        public override bool Equals (object obj)
        {
            if (obj is Not f) {
                return f.Enclosed.Equals (Enclosed);
            }
            return false;
        }
        
        public override string ToString ()
        {
        
            var enclosed = Enclosed.ToString ();
            if (!(Enclosed is PredicateReference 
                  | Enclosed is RelationReference 
                  | Enclosed is AttributeReference 
                  | Enclosed is ComparisonPredicate
                  | Enclosed is Not
                  | Enclosed is Next
                  | Enclosed is Eventually
                  | Enclosed is Globally)) {
                enclosed = "(" + enclosed + ")";
            }
            return string.Format ("not {0}", enclosed);
        }
        
        public override Formula Copy ()
        {
            return new Not () {
                Enclosed = Enclosed.Copy ()
            };
        }
    }
    
    public class Next : Formula {
        public Formula Enclosed;
        public Next ()
        {
            
        }
        public Next (Formula enclosed)
        {
            this.Enclosed = enclosed;
        }

		public override IEnumerable<PredicateReference> PredicateReferences {
			get {
				return Enclosed.PredicateReferences;
			}
		}
        
        public override bool Equals (object obj)
        {
            if (obj is Next f) {
                return f.Enclosed.Equals (Enclosed);
            }
            return false;
        }
        
        public override string ToString ()
        {
            var enclosed = Enclosed.ToString ();
            if (!(Enclosed is PredicateReference 
                  | Enclosed is RelationReference 
                  | Enclosed is AttributeReference 
                  | Enclosed is ComparisonPredicate
                  | Enclosed is Not
                  | Enclosed is Next
                  | Enclosed is Eventually
                  | Enclosed is Globally)) {
                enclosed = "(" + enclosed + ")";
            }
            return string.Format ("next {0}", enclosed);
        }
        
        public override Formula Copy ()
        {
            return new Next () {
                Enclosed = Enclosed.Copy ()
            };
        }
    }
    
    public class Eventually : Formula {
        public Formula Enclosed;
        public TimeBound TimeBound;

		public override IEnumerable<PredicateReference> PredicateReferences {
			get {
				return Enclosed.PredicateReferences;
			}
		}
        
        public override bool Equals (object obj)
        {
            if (obj is Eventually f) {
                return f.Enclosed.Equals (Enclosed) & f.TimeBound.Equals (TimeBound);
            }
            return false;
        }
        
        string ToStringTimeBound ()
        {
            if (TimeBound == null)
                return default(string);
            
            string bound = TimeBound.Bound.ToString (@"%d'd%m'm%s's%FFFFFF'ms");
            
            switch (TimeBound.Comparator) {
            case TimeComparator.less: return ", before " + bound + ",";
            case TimeComparator.equal: return ", in " + bound + ",";
            case TimeComparator.greater: return ", after " + bound + ",";
            case TimeComparator.strictly_less: return ", strictly before " + bound + ",";
            case TimeComparator.strictly_greater: return ", strictly after " + bound + ",";

            default:
                throw new NotImplementedException ();
            }
        }

        public override string ToString ()
        {
            var enclosed = Enclosed.ToString ();
            if (!(Enclosed is PredicateReference 
                  | Enclosed is RelationReference 
                  | Enclosed is AttributeReference 
                  | Enclosed is ComparisonPredicate
                  | Enclosed is Not
                  | Enclosed is Next
                  | Enclosed is Eventually
                  | Enclosed is Globally)) {
                enclosed = "(" + enclosed + ")";
            }
            return string.Format ("sooner-or-later{1} {0}", enclosed, ToStringTimeBound());
        }
        
        public override Formula Copy ()
        {
            return new Eventually () {
                Enclosed = Enclosed.Copy (),
                TimeBound = TimeBound?.Copy ()
            };
        }
    }
    
    public class Globally : Formula {
        public Formula Enclosed;
        public TimeBound TimeBound;
        public Globally ()
        {
            
        }
        public Globally (Formula enclosed)
        {
            this.Enclosed = enclosed;
        }

		public override IEnumerable<PredicateReference> PredicateReferences {
			get {
				return Enclosed.PredicateReferences;
			}
		}
        
        public override bool Equals (object obj)
        {
            if (obj is Globally f) {
                return f.Enclosed.Equals (Enclosed) & f.TimeBound.Equals (TimeBound);
            }
            return false;
        }
        
        string ToStringTimeBound ()
        {
            if (TimeBound == null)
                return default(string);
            
            string bound = TimeBound.Bound.ToString (@"%d'd%m'm%s's%FFFFFF'ms");
            switch (TimeBound.Comparator) {
            case TimeComparator.less: return ", for less than " + bound + ",";
            case TimeComparator.equal: return ", for " + bound + ",";
            case TimeComparator.greater: return ", for more than " + bound + ",";
            case TimeComparator.strictly_less: return ", for strictly less than " + bound + ",";
            case TimeComparator.strictly_greater: return ", for strictly more than " + bound + ",";

            default:
                throw new NotImplementedException ();
            }
        }

        public override string ToString ()
        {
            var enclosed = Enclosed.ToString ();
            if (!(Enclosed is PredicateReference 
                  | Enclosed is RelationReference 
                  | Enclosed is AttributeReference 
                  | Enclosed is ComparisonPredicate
                  | Enclosed is Not
                  | Enclosed is Next
                  | Enclosed is Eventually
                  | Enclosed is Globally)) {
                enclosed = "(" + enclosed + ")";
            }
            return string.Format ("always{1} {0}", enclosed, ToStringTimeBound());
        }
        
        public override Formula Copy ()
        {
            return new Globally () {
                Enclosed = Enclosed.Copy (),
                TimeBound = TimeBound?.Copy ()
            };
        }
    }

    public class PredicateReference : Formula {
        public string Predicate;
        public IList<string> ActualArguments;

		public PredicateReference()
		{
			ActualArguments = new List<string>();
		}

		public PredicateReference(string predicate)
		{
			this.Predicate = predicate;
		}

		public override IEnumerable<PredicateReference> PredicateReferences {
			get {
				return new HashSet<PredicateReference> (new[] { this });
			}
		}
        
        public override bool Equals (object obj)
        {
            if (obj is PredicateReference f) {
                return f.Predicate.Equals (Predicate) & Enumerable.SequenceEqual (f.ActualArguments, ActualArguments);
            }
            return false;
        }

        public override string ToString ()
        {
            return string.Format ("{0}({1})", Predicate, string.Join (",", ActualArguments));
        }
        
        public override Formula Copy ()
        {
            return new PredicateReference () {
                Predicate = Predicate,
                ActualArguments = new List<string>(ActualArguments)
            };
        }
    }
    
    public class RelationReference : Formula {
        public string Relation;
        public IList<string> ActualArguments;
        public RelationReference ()
        {
            ActualArguments = new List<string>();
        }

		public override IEnumerable<PredicateReference> PredicateReferences {
			get {
				return Enumerable.Empty<PredicateReference>();
			}
		}
        
        public override bool Equals (object obj)
        {
            if (obj is RelationReference f) {
                return f.Relation.Equals (Relation) & Enumerable.SequenceEqual (f.ActualArguments, ActualArguments);
            }
            return false;
        }

        public override string ToString ()
        {
            return string.Format ("({1}) in {0}", Relation, string.Join (",", ActualArguments));
        }
        
        public override Formula Copy ()
        {
            return new RelationReference () {
                Relation = Relation,
                ActualArguments = new List<string>(ActualArguments)
            };
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

		public override IEnumerable<PredicateReference> PredicateReferences {
			get {
				return Enumerable.Empty<PredicateReference>();
			}
		}
        
        public override bool Equals (object obj)
        {
            if (obj is VariableReference f) {
                return f.Name.Equals (Name);
            }
            return false;
        }

        public override string ToString ()
        {
            return string.Format ("{0}", Name);
        }
        
        public override Formula Copy ()
        {
            return new VariableReference () {
                Name = Name,
            };
        }
    }

    public class AttributeReference : Formula {
        public string Variable { get; set; }
        public string Entity { get; set; }
        public string Attribute { get; set; }

        public AttributeReference ()
        {}

        public AttributeReference (string variable, string entity, string attribute)
        {
            this.Variable = variable;
            this.Entity = entity;
            this.Attribute = attribute;
        }

		public override IEnumerable<PredicateReference> PredicateReferences {
			get {
				return Enumerable.Empty<PredicateReference>();
			}
		}
        
        public override bool Equals (object obj)
        {
            if (obj is AttributeReference f) {
                return f.Variable.Equals (Variable) & f.Entity.Equals (Entity) & f.Attribute.Equals (Attribute);
            }
            return false;
        }

        public override string ToString ()
        {
            return string.Format ("{0}.{1}", Variable, Attribute);
        }
        
        public override Formula Copy ()
        {
            return new AttributeReference () {
                Variable = Variable,
                Entity = Entity,
                Attribute = Attribute,
            };
        }
    }

    public class ComparisonPredicate : Formula {
        public ComparisonCriteria Criteria { get; set; }
        public Formula Left { get; set; }
        public Formula Right { get; set; }

		public override IEnumerable<PredicateReference> PredicateReferences {
			get {
				return Left.PredicateReferences.Union(Right.PredicateReferences);
			}
		}
        
        public override bool Equals (object obj)
        {
            if (obj is ComparisonPredicate f) {
                return f.Criteria.Equals (Criteria) & f.Left.Equals (Left) & f.Right.Equals (Right);
            }
            return false;
        }
        
        string CriteriaToString () 
        {
            switch (Criteria) {
            case ComparisonCriteria.BiggerThan: return ">";
            case ComparisonCriteria.BiggerThanOrEquals: return ">=";
            case ComparisonCriteria.Equals: return "==";
            case ComparisonCriteria.LessThan: return "<";
            case ComparisonCriteria.LessThanOrEquals: return "<=";
            case ComparisonCriteria.NotEquals: return "!=";
            default:
                throw new NotImplementedException ();
            }
        }

        public override string ToString ()
        {
            return string.Format ("{0} {1} {2}", Left, CriteriaToString (), Right);
        }
        
        public override Formula Copy ()
        {
            return new ComparisonPredicate () {
                Left = Left,
                Criteria = Criteria,
                Right = Right,
            };
        }
    }

    public enum ComparisonCriteria {
        Equals, NotEquals, BiggerThanOrEquals, LessThanOrEquals, LessThan, BiggerThan
    }

    public class StringConstant : Formula {
        public string Value { get; set; }

		public override IEnumerable<PredicateReference> PredicateReferences {
			get {
				return Enumerable.Empty<PredicateReference>();
			}
		}
        
        public override bool Equals (object obj)
        {
            if (obj is StringConstant f) {
                return f.Value.Equals (Value);
            }
            return false;
        }

        public override string ToString ()
        {
            return string.Format ("\"{0}\"", Value);
        }
        
        public override Formula Copy ()
        {
            return new StringConstant () {
                Value = Value,
            };
        }
    }

    public class NumericConstant : Formula {
        public double Value { get; set; }

		public override IEnumerable<PredicateReference> PredicateReferences {
			get {
				return Enumerable.Empty<PredicateReference>();
			}
		}
        
        public override bool Equals (object obj)
        {
            if (obj is NumericConstant f) {
                return f.Value.Equals (Value);
            }
            return false;
        }

        public override string ToString ()
        {
            return string.Format ("{0}", Value);
        }
        
        public override Formula Copy ()
        {
            return new NumericConstant () {
                Value = Value,
            };
        }
    }

    public class BoolConstant : Formula {
        public bool Value { get; set; }

		public override IEnumerable<PredicateReference> PredicateReferences {
			get {
				return Enumerable.Empty<PredicateReference>();
			}
		}
        
        public override bool Equals (object obj)
        {
            if (obj is BoolConstant f) {
                return f.Value.Equals (Value);
            }
            return false;
        }

        public override string ToString ()
        {
            return string.Format ("{0}", Value ? "true" : "false");
        }
        
        public override Formula Copy ()
        {
            return new BoolConstant () {
                Value = Value,
            };
        }
    }

    #region Time bound

    public enum TimeComparator {
        less, strictly_less, greater, strictly_greater, equal
    }

    public class TimeBound {
        public TimeComparator Comparator;
        public TimeSpan Bound;
        
        public override bool Equals (object obj)
        {
            if (obj is TimeBound f) {
                return f.Comparator.Equals (Comparator) & f.Bound.Equals (Bound);
            }
            return false;
        }
        
        public TimeBound Copy ()
        {
            return new TimeBound () {
                Comparator = Comparator,
                Bound = Bound,
            };
        }
    }

    #endregion
}

