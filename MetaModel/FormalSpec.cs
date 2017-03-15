using System;
using System.Collections.Generic;
using System.Linq;

namespace KAOSTools.Core
{
	public abstract class Formula
	{
		public abstract IEnumerable<PredicateReference> PredicateReferences { get; }
	}

	public class Forall : Formula {
        public IList<ArgumentDeclaration> Declarations;
        public Formula Enclosed;
        public Forall ()
        {
            Declarations = new List<ArgumentDeclaration> ();
        }

		public override IEnumerable<PredicateReference> PredicateReferences {
			get {
				return Enclosed.PredicateReferences;
			}
		}
	}

    public class Exists : Formula {
        public IList<ArgumentDeclaration> Declarations;
        public Formula Enclosed;
        public Exists ()
        {
            Declarations = new List<ArgumentDeclaration> ();
        }

		public override IEnumerable<PredicateReference> PredicateReferences {
			get {
				return Enclosed.PredicateReferences;
			}
		}
    }

    public class ArgumentDeclaration {
        public string Name { get; set; }
        public Entity Type { get; set; }
    }

    public class StrongImply : Formula {
        public Formula Left;
        public Formula Right;

		public override IEnumerable<PredicateReference> PredicateReferences {
			get {
				return Left.PredicateReferences.Union (Right.PredicateReferences);
			}
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
    }
    
    public class Equivalence : Formula {
        public Formula Left;
        public Formula Right;

		public override IEnumerable<PredicateReference> PredicateReferences {
			get {
				return Left.PredicateReferences.Union(Right.PredicateReferences);
			}
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
    }
    
    public class Release : Formula {
        public Formula Left;
        public Formula Right;

		public override IEnumerable<PredicateReference> PredicateReferences {
			get {
				return Left.PredicateReferences.Union(Right.PredicateReferences);
			}
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
    }
    
    public class Eventually : Formula {
        public Formula Enclosed;
        public TimeBound TimeBound;

		public override IEnumerable<PredicateReference> PredicateReferences {
			get {
				return Enclosed.PredicateReferences;
			}
		}
    }
    
	// Until
    public class EventuallyBefore : Formula {
        public Formula Left;
		public Formula Right;
        public TimeBound TimeBound;

		public override IEnumerable<PredicateReference> PredicateReferences {
			get {
				return Left.PredicateReferences.Union (Right.PredicateReferences);
			}
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
    }

    public class PredicateReference : Formula {
        public Predicate Predicate;
        public IList<string> ActualArguments;

		public PredicateReference()
		{
			ActualArguments = new List<string>();
		}

		public PredicateReference(Predicate predicate)
		{
			this.Predicate = predicate;
		}

		public override IEnumerable<PredicateReference> PredicateReferences {
			get {
				return new HashSet<PredicateReference> (new[] { this });
			}
		}
    }
    
    public class RelationReference : Formula {
        public Relation Relation;
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

		public override IEnumerable<PredicateReference> PredicateReferences {
			get {
				return Enumerable.Empty<PredicateReference>();
			}
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
    }

    public class NumericConstant : Formula {
        public double Value { get; set; }

		public override IEnumerable<PredicateReference> PredicateReferences {
			get {
				return Enumerable.Empty<PredicateReference>();
			}
		}
    }

    public class BoolConstant : Formula {
        public bool Value { get; set; }

		public override IEnumerable<PredicateReference> PredicateReferences {
			get {
				return Enumerable.Empty<PredicateReference>();
			}
		}
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

