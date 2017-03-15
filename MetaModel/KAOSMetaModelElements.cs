using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace KAOSTools.Core
{
    

    #region Goal Model

    #region Meta entities

    

    public class AntiGoal : KAOSCoreElement
    {
        public string Name { get; set; }

        public override string FriendlyName {
            get {
                return string.IsNullOrEmpty(Name) ? Identifier : Name;
            }
        }

        public string Definition { get; set; }

        public Formula FormalSpec { get; set; }


        public AntiGoal (KAOSModel model) : base(model)
        {
        }

        public override KAOSCoreElement Copy ()
        {
            return new AntiGoal (null) {
                Identifier = Identifier,
                Implicit = Implicit,
                Name = Name,
                Definition = Definition,
                FormalSpec = FormalSpec
            };
        }
    }

    

    

    

    

    

    

    #endregion

    #region Assignements

    

    

    [DataContract]
    public class OperationAgentPerformance : AgentAssignment {

        [DataMember]
        public string OperationIdentifier { get; set ; }
        public OperationAgentPerformance  (KAOSModel model) : base (model) {}

        public override KAOSCoreElement Copy ()
        {
            return new OperationAgentPerformance (null) {
                Identifier = Identifier,
                Implicit = Implicit,
                OperationIdentifier = OperationIdentifier,
                AgentIdentifiers = new List<string> (AgentIdentifiers)
            };
        }
    }

    public class ObstacleAgentAssignment : AgentAssignment {
        public string ObstacleIdentifier { get; set ; }
        public ObstacleAgentAssignment  (KAOSModel model) : base (model) {}

        public override KAOSCoreElement Copy ()
        {
            return new ObstacleAgentAssignment (null) {
                Identifier = Identifier,
                Implicit = Implicit,
                ObstacleIdentifier = ObstacleIdentifier,
                AgentIdentifiers = new List<string> (AgentIdentifiers)
            };
        }
    }

    public class AntiGoalAgentAssignment : AgentAssignment {
        public string AntiGoalIdentifier { get; set ; }
        public AntiGoalAgentAssignment  (KAOSModel model) : base (model) {}

        public override KAOSCoreElement Copy ()
        {
            return new AntiGoalAgentAssignment (null) {
                Identifier = Identifier,
                Implicit = Implicit,
                AntiGoalIdentifier = AntiGoalIdentifier,
                AgentIdentifiers = new List<string> (AgentIdentifiers)
            };
        }
    }

    #endregion

    #region Refinements

    

    public class AntiGoalRefinement : KAOSCoreElement
    {
        public string ParentAntiGoalIdentifier { get; set; }
        public string SystemReferenceIdentifier { get; set; } // TODO remove

        public ISet<string> SubAntiGoalIdentifiers { get; set; }
        public ISet<string> ObstacleIdentifiers { get; set; }
        public ISet<string> DomainPropertyIdentifiers { get; set; }
        public ISet<string> DomainHypothesisIdentifiers { get; set; }

        public bool IsEmpty {
            get {
                return SubAntiGoalIdentifiers.Count + ObstacleIdentifiers.Count 
                    + DomainPropertyIdentifiers.Count + DomainHypothesisIdentifiers.Count == 0;
            }
        }

        public AntiGoalRefinement (KAOSModel model) : base (model)
        {
            SubAntiGoalIdentifiers = new HashSet<string> ();
            ObstacleIdentifiers = new HashSet<string> ();
            DomainPropertyIdentifiers = new HashSet<string> ();
            DomainHypothesisIdentifiers = new HashSet<string> ();
        }

        public void SetParentAntiGoal (AntiGoal element)
        {
            this.ParentAntiGoalIdentifier = element.Identifier;
        }

        public void Add (Obstacle obstacle)
        {
            this.ObstacleIdentifiers.Add (obstacle.Identifier);
        }

        public void Add (AntiGoal antiGoal)
        {
            this.SubAntiGoalIdentifiers.Add (antiGoal.Identifier);
        }

        public void Add (DomainProperty domProp)
        {
            this.DomainPropertyIdentifiers.Add (domProp.Identifier);
        }

        public void Add (DomainHypothesis domHyp)
        {
            this.DomainHypothesisIdentifiers.Add (domHyp.Identifier);
        }

        public override KAOSCoreElement Copy ()
        {
            return new AntiGoalRefinement (null) {
                Identifier = Identifier,
                Implicit = Implicit,
                ParentAntiGoalIdentifier = ParentAntiGoalIdentifier,
                SystemReferenceIdentifier = SystemReferenceIdentifier,
                SubAntiGoalIdentifiers = new HashSet<string> (SubAntiGoalIdentifiers),
                ObstacleIdentifiers = new HashSet<string> (ObstacleIdentifiers),
                DomainPropertyIdentifiers = new HashSet<string> (DomainPropertyIdentifiers),
                DomainHypothesisIdentifiers = new HashSet<string> (DomainHypothesisIdentifiers)
            };
        }
    }

    

    #endregion

    #region Obstructions and resolutions

    

    

    

    

    #endregion

    #region Exceptions and assumptions

    

    


    

    #endregion

    #endregion

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
