using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace KAOSTools.Core
{
    #region Object Model

    public class Entity : KAOSCoreElement {
        public string Name { get; set; }

        public override string FriendlyName {
            get {
                return string.IsNullOrEmpty(Name) ? Identifier : Name;
            }
        }

        public string Definition { get; set; }
        public EntityType Type { get; set; }

        public ISet<string> ParentIdentifiers { get; set; }

        public Entity (KAOSModel model) : base (model)
        {
            ParentIdentifiers    = new HashSet<string> ();
        }

        public void AddParent (Entity parent)
        {
            ParentIdentifiers.Add (parent.Identifier);
        }

        public override KAOSCoreElement Copy ()
        {
            return new Entity (model) {
                Identifier = Identifier,
                Implicit = Implicit,
                Name = Name,
                Definition = Definition,
                Type = Type,
                ParentIdentifiers = new HashSet<string> (ParentIdentifiers)
            };
        }
    }

    public class Attribute : KAOSCoreElement {
        public bool Derived { get; set; }
        public string Name { get; set; }
        public string Definition { get; set; }

        public string EntityIdentifier {
            get;
            set;
        }

        public override string FriendlyName {
            get {
                return string.IsNullOrEmpty(Name) ? Identifier : Name;
            }
        }

        public string TypeIdentifier { get; set; }

        public Attribute (KAOSModel model) : base (model)
        {
            Derived = false;
        }
        
        public void SetEntity (Entity entity)
        {
            EntityIdentifier = entity.Identifier;
        }

        public void SetType (GivenType givenType)
        {
            TypeIdentifier = givenType.Identifier;
        }

        public override KAOSCoreElement Copy ()
        {
            return new Attribute (model) {
                Identifier = Identifier,
                Implicit = Implicit,
                Derived = Derived,
                Name = Name,
                Definition = Definition,
                TypeIdentifier = TypeIdentifier
            };
        }
    }

    public class GivenType : KAOSCoreElement {
        public string Name { get; set; }

        public override string FriendlyName {
            get {
                return string.IsNullOrEmpty(Name) ? Identifier : Name;
            }
        }

        public string Definition { get; set; }

        public GivenType  (KAOSModel model) : base (model)
        {
            
        }

        public override KAOSCoreElement Copy ()
        {
            throw new NotImplementedException ();
        }
    }

    public class Relation : Entity {

        public ISet<Link> Links { get; set; } 
        public Relation (KAOSModel model) : base (model)
        {
            Links = new HashSet<Link> ();
        }
    }

    public class Link : KAOSCoreElement {
        public Entity Target { get; set; }
        public string Role { get; set; }
        public string Multiplicity { get; set; }
        public Link  (KAOSModel model) : base (model)
        {}

        public override KAOSCoreElement Copy ()
        {
            throw new NotImplementedException ();
        }
    }

    public enum EntityType {
        None, Software, Environment, Shared
    }

    #endregion

    public class Predicate : KAOSCoreElement
    {
        public string Name { get; set; }

        public bool DefaultValue {
            get;
            set;
        }

        public override string FriendlyName {
            get {
                return string.IsNullOrEmpty(Name) ? Identifier : Name;
            }
        }

        public IList<PredicateArgument> Arguments { get; set; }

        public string Definition { get; set; }

        public Formula FormalSpec { get; set; }

        public Predicate  (KAOSModel model) : base (model)
        {
            Arguments = new List<PredicateArgument> ();
            DefaultValue = false;
        }

        public override KAOSCoreElement Copy ()
        {
			return new Predicate(model) {
				Identifier = Identifier,
				Implicit = Implicit,
				CustomData = CustomData,
				Name = Name,
				DefaultValue = DefaultValue,
				Arguments = new List<PredicateArgument>(Arguments.Select(x => x.Copy())),
				Definition = Definition,
				FormalSpec = FormalSpec
			};

        }
    }

    public class PredicateArgument {

        public string Name { get; set; }

        public Entity Type { get; set; }

		public PredicateArgument Copy()
		{
			return new PredicateArgument() {
				Name = Name,
				Type = (Entity)Type.Copy()
			};
		}

    }



    

    

    

    

    

    

    

    

    

    

    


}
