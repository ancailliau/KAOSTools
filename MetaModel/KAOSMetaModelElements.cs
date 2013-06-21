using System.Collections.Generic;
using System;
using System.Linq;

namespace KAOSTools.MetaModel
{
    public abstract class KAOSMetaModelElement
    {
        public string Identifier { get; set; }

        public virtual string FriendlyName {
            get {
                return Identifier;
            }
        }

        public bool Implicit { get; set; }

        public ISet<AlternativeSystem> InSystems { get; set; }

        public KAOSModel model;

        public KAOSMetaModelElement (KAOSModel model)
        {
            this.Identifier = Guid.NewGuid ().ToString ();
            this.model = model;
        }

        public abstract KAOSMetaModelElement Copy ();

        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(KAOSMetaModelElement))
                return false;
            KAOSMetaModelElement other = (KAOSMetaModelElement)obj;
            return Identifier == other.Identifier;
        }

        public override int GetHashCode ()
        {
            unchecked {
                return (Identifier != null ? Identifier.GetHashCode () : 0);
            }
        }

    }

    #region Goal Model

    #region Meta entities

    public class Goal : KAOSMetaModelElement
    {
        public string Name { get; set; }

        public override string FriendlyName {
            get {
                return string.IsNullOrEmpty(Name) ? Identifier : Name;
            }
        }

        public string Definition { get; set; }

        public Formula FormalSpec { get; set; }

        public double CPS { get; set; }

        public double RDS { get; set; }

        public ISet<GoalException> Exceptions { get; set; }
        public ISet<Assumption> Assumptions { get; set; }

        public Goal (KAOSModel model) : base(model)
        {
            InSystems = new HashSet<AlternativeSystem>();
            Exceptions = new HashSet<GoalException> ();
            Assumptions = new HashSet<Assumption> ();
        }

        public override KAOSMetaModelElement Copy ()
        {
            return new Goal (null) {
                Identifier = Identifier,
                Implicit = Implicit,
                Name = Name,
                Definition = Definition,
                FormalSpec = FormalSpec,
                CPS = CPS,
                RDS = RDS,
                Exceptions = new HashSet<GoalException> (Exceptions),
                Assumptions = new HashSet<Assumption> (Assumptions)
            };
        }
    }

    public class AntiGoal : KAOSMetaModelElement
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
            InSystems = new HashSet<AlternativeSystem>();
        }

        public override KAOSMetaModelElement Copy ()
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

    public class Obstacle : KAOSMetaModelElement
    {
        public string Name { get; set; }

        public override string FriendlyName {
            get {
                return string.IsNullOrEmpty(Name) ? Identifier : Name;
            }
        }

        public string Definition { get; set; }

        public Formula FormalSpec { get; set; }

        public double EPS { get; set; }

        public double CPS { get; set; }

        public ISet<Assumption> Assumptions { get; set; }

        public Obstacle (KAOSModel model) : base(model)
        {
            Assumptions = new HashSet<Assumption> ();
        }

        public override KAOSMetaModelElement Copy ()
        {
            return new Obstacle (null) {
                Identifier = Identifier,
                Implicit = Implicit,
                Name = Name,
                Definition = Definition,
                FormalSpec = FormalSpec,
                CPS = CPS,
                EPS = EPS,
                Assumptions = new HashSet<Assumption> (Assumptions)
            };
        }
    }

    public class DomainHypothesis : KAOSMetaModelElement
    { 
        public string Name { get; set; }

        public override string FriendlyName {
            get {
                return string.IsNullOrEmpty(Name) ? Identifier : Name;
            }
        }

        public string Definition { get; set; }

        public Formula FormalSpec { get; set; }

        public double EPS { get; set; }

        public DomainHypothesis (KAOSModel model) : base(model)
        {

        }

        public override KAOSMetaModelElement Copy ()
        {
            return new DomainHypothesis (null) {
                Identifier = Identifier,
                Implicit = Implicit,
                Name = Name,
                Definition = Definition,
                FormalSpec = FormalSpec,
                EPS = EPS
            };
        }
    }

    public class DomainProperty : KAOSMetaModelElement
    { 
        public string Name { get; set; }

        public override string FriendlyName {
            get {
                return string.IsNullOrEmpty(Name) ? Identifier : Name;
            }
        }

        public string Definition { get; set; }

        public Formula FormalSpec { get; set; }

        public double EPS { get; set; }

        public DomainProperty (KAOSModel model) : base (model) {}

        public override KAOSMetaModelElement Copy ()
        {
            return new DomainProperty (null) {
                Identifier = Identifier,
                Implicit = Implicit,
                Name = Name,
                Definition = Definition,
                FormalSpec = FormalSpec,
                EPS = EPS
            };
        }
    }

    public class Agent : KAOSMetaModelElement
    {
        public string Name { get; set; }

        public override string FriendlyName {
            get {
                return string.IsNullOrEmpty(Name) ? Identifier : Name;
            }
        }

        public string Definition { get; set; }

        public AgentType Type { get; set; }

        public Agent (KAOSModel model) : base(model)
        {
            Type = AgentType.None;
        }

        public override KAOSMetaModelElement Copy ()
        {
            return new Agent(null) {
                Identifier = Identifier,
                Implicit = Implicit,
                Name = Name,
                Definition = Definition,
                Type = Type
            };
        }
    }

    public enum AgentType
    {
        None,
        Software,
        Environment,
        Malicious
    }

    #endregion

    #region Assignements

    public abstract class AgentAssignment : KAOSMetaModelElement
    {
        public AlternativeSystem SystemReference { get; set; }

        public ISet<string> AgentIdentifiers { get; set; }

        public bool IsEmpty {
            get {
                return AgentIdentifiers.Count == 0;
            }
        }

        public AgentAssignment (KAOSModel model) : base (model)
        {
            AgentIdentifiers = new HashSet<string> ();
        }

        public void Add (Agent agent)
        {
            this.AgentIdentifiers.Add (agent.Identifier);
        }
    }

    public class GoalAgentAssignment : AgentAssignment {
        public string GoalIdentifier { get; set ; }
        public GoalAgentAssignment  (KAOSModel model) : base (model) {}

        public override KAOSMetaModelElement Copy ()
        {
            return new GoalAgentAssignment (null) {
                Identifier = Identifier,
                Implicit = Implicit,
                GoalIdentifier = GoalIdentifier,
                AgentIdentifiers = new HashSet<string> (AgentIdentifiers)
            };
        }
    }

    public class ObstacleAgentAssignment : AgentAssignment {
        public string ObstacleIdentifier { get; set ; }
        public ObstacleAgentAssignment  (KAOSModel model) : base (model) {}

        public override KAOSMetaModelElement Copy ()
        {
            return new ObstacleAgentAssignment (null) {
                Identifier = Identifier,
                Implicit = Implicit,
                ObstacleIdentifier = ObstacleIdentifier,
                AgentIdentifiers = new HashSet<string> (AgentIdentifiers)
            };
        }
    }

    public class AntiGoalAgentAssignment : AgentAssignment {
        public string AntiGoalIdentifier { get; set ; }
        public AntiGoalAgentAssignment  (KAOSModel model) : base (model) {}

        public override KAOSMetaModelElement Copy ()
        {
            return new AntiGoalAgentAssignment (null) {
                Identifier = Identifier,
                Implicit = Implicit,
                AntiGoalIdentifier = AntiGoalIdentifier,
                AgentIdentifiers = new HashSet<string> (AgentIdentifiers)
            };
        }
    }

    #endregion

    #region Refinements

    public class GoalRefinement : KAOSMetaModelElement
    {
        public string ParentGoalIdentifier { get; set; }

        public string SystemReferenceIdentifier { get; set; }

        public ISet<string> SubGoalIdentifiers { get; set; }
        public ISet<string> DomainPropertyIdentifiers { get; set; }
        public ISet<string> DomainHypothesisIdentifiers { get; set; }

        public bool IsEmpty {
            get {
                return SubGoalIdentifiers.Count 
                    + DomainPropertyIdentifiers.Count 
                    + DomainHypothesisIdentifiers.Count == 0;
            }
        }

        public RefinementPattern RefinementPattern { get; set; }
        public List<dynamic> Parameters { get; set; }

        public GoalRefinement (KAOSModel model) : base (model)
        {
            SubGoalIdentifiers = new HashSet<string> ();
            DomainPropertyIdentifiers = new HashSet<string> ();
            DomainHypothesisIdentifiers = new HashSet<string> ();

            Parameters = new List<dynamic> ();
        }

        public GoalRefinement (KAOSModel model, Goal goal) : this (model)
        {
            SubGoalIdentifiers.Add (goal.Identifier);
        }

        public GoalRefinement (KAOSModel model, params Goal[] goals) : this (model)
        {
            foreach (var goal in goals)
                SubGoalIdentifiers.Add (goal.Identifier);
        }

        public void SetParentGoal (Goal element)
        {
            this.ParentGoalIdentifier = element.Identifier;
        }

        public void SetSystemReference (AlternativeSystem system)
        {
            this.SystemReferenceIdentifier = system.Identifier;
        }

        public void Add (Goal goal)
        {
            this.SubGoalIdentifiers.Add (goal.Identifier);
        }
        
        public void Add (DomainProperty domProp)
        {
            this.DomainPropertyIdentifiers.Add (domProp.Identifier);
        }
        
        public void Add (DomainHypothesis domHyp)
        {
            this.DomainHypothesisIdentifiers.Add (domHyp.Identifier);
        }

        public override KAOSMetaModelElement Copy ()
        {
            return new GoalRefinement (null) {
                Identifier = Identifier,
                Implicit = Implicit,
                ParentGoalIdentifier = ParentGoalIdentifier,
                SystemReferenceIdentifier = SystemReferenceIdentifier,
                SubGoalIdentifiers = new HashSet<string> (SubGoalIdentifiers),
                DomainPropertyIdentifiers = new HashSet<string> (DomainPropertyIdentifiers),
                DomainHypothesisIdentifiers = new HashSet<string> (DomainHypothesisIdentifiers),
                RefinementPattern = RefinementPattern,
                Parameters = new List<dynamic> (Parameters)
            };
        }
    }

    public class AntiGoalRefinement : KAOSMetaModelElement
    {
        public string ParentAntiGoalIdentifier { get; set; }
        public string SystemReferenceIdentifier { get; set; }

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

        public void SetSystemReference (AlternativeSystem system)
        {
            this.SystemReferenceIdentifier = system.Identifier;
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

        public override KAOSMetaModelElement Copy ()
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

    public class ObstacleRefinement : KAOSMetaModelElement
    {
        public string ParentObstacleIdentifier { get; set; }

        public ISet<string> SubobstacleIdentifiers { get; set; }
        public ISet<string> DomainPropertyIdentifiers { get; set; }
        public ISet<string> DomainHypothesisIdentifiers { get; set; }

        public bool IsEmpty {
            get {
                return SubobstacleIdentifiers.Count + DomainPropertyIdentifiers.Count + DomainHypothesisIdentifiers.Count == 0;
            }
        }
        public ObstacleRefinement (KAOSModel model) : base (model)
        {
            SubobstacleIdentifiers = new HashSet<string> ();
            DomainPropertyIdentifiers = new HashSet<string> ();
            DomainHypothesisIdentifiers = new HashSet<string> ();
        }

        public ObstacleRefinement (KAOSModel model, Obstacle obstacle) : this (model)
        {
            SubobstacleIdentifiers.Add (obstacle.Identifier);
        }

        public ObstacleRefinement (KAOSModel model, params Obstacle[] obstacles) : this (model)
        {
            foreach (var obstacle in obstacles)
                SubobstacleIdentifiers.Add (obstacle.Identifier);
        }

        public void SetParentObstacle (Obstacle element)
        {
            this.ParentObstacleIdentifier = element.Identifier;
        }

        public void Add (Obstacle obstacle)
        {
            this.SubobstacleIdentifiers.Add (obstacle.Identifier);
        }

        public void Add (DomainProperty domProp)
        {
            this.DomainPropertyIdentifiers.Add (domProp.Identifier);
        }

        public void Add (DomainHypothesis domHyp)
        {
            this.DomainHypothesisIdentifiers.Add (domHyp.Identifier);
        }

        public override KAOSMetaModelElement Copy ()
        {
            return new ObstacleRefinement (null) {
                Identifier = Identifier,
                Implicit = Implicit,
                ParentObstacleIdentifier = ParentObstacleIdentifier,
                SubobstacleIdentifiers = new HashSet<string> (SubobstacleIdentifiers),
                DomainPropertyIdentifiers = new HashSet<string> (DomainPropertyIdentifiers),
                DomainHypothesisIdentifiers = new HashSet<string> (DomainHypothesisIdentifiers)
            };
        }
    }

    #endregion

    #region Obstructions and resolutions

    public class Obstruction : KAOSMetaModelElement {
        public string ObstructedGoalIdentifier { get; set; }
        public string ObstacleIdentifier { get; set; }

        public Obstruction (KAOSModel model) : base (model) {}

        public void SetObstructedGoal (Goal goal)
        {
            this.ObstructedGoalIdentifier = goal.Identifier;
        }

        public void SetObstacle (Obstacle obstacle)
        {
            this.ObstacleIdentifier = obstacle.Identifier;
        }

        public override KAOSMetaModelElement Copy ()
        {
            return new Obstruction (null) {
                ObstructedGoalIdentifier = ObstructedGoalIdentifier,
                ObstacleIdentifier = ObstacleIdentifier
            };
        }
    }

    public class Resolution : KAOSMetaModelElement {
        public string ObstacleIdentifier { get; set; }
        public string ResolvingGoalIdentifier { get; set; }

        public ResolutionPattern ResolutionPattern { get; set; }
        public List<dynamic> Parameters { get; set; }

        public Resolution (KAOSModel model) : base (model)
        {
            ResolutionPattern = ResolutionPattern.None;
            Parameters = new List<dynamic> ();
        }

        public void SetResolvingGoal (Goal goal)
        {
            this.ResolvingGoalIdentifier = goal.Identifier;
        }

        public void SetObstacle (Obstacle obstacle)
        {
            this.ObstacleIdentifier = obstacle.Identifier;
        }

        public override KAOSMetaModelElement Copy ()
        {
            return new Resolution (null) {
                ResolvingGoalIdentifier = ResolvingGoalIdentifier,
                ObstacleIdentifier = ObstacleIdentifier,
                ResolutionPattern = ResolutionPattern,
                Parameters = new List<dynamic> (Parameters)
            };
        }
    }

    public enum ResolutionPattern {
        None, 
        GoalSubstitution,
        ObstaclePrevention,
        ObstacleReduction,
        GoalRestoration,
        GoalWeakening,
        ObstacleWeakMitigation,
        ObstacleStrongMitigation
    }

    public enum RefinementPattern {
        None, 
        Milestone, 
        Case, 
        IntroduceGuard, 
        DivideAndConquer, 
        Unmonitorability, 
        Uncontrollability
    }

    #endregion

    #region Exceptions and assumptions

    public class GoalException {
        public bool Implicit { get; set; }
        public Obstacle ResolvedObstacle { get; set; }
        public Goal ResolvingGoal { get; set; }

    }

    public abstract class Assumption {
        public bool Implicit { get; set; }
        public dynamic Assumed { get; set; }

    }

    public class GoalAssumption : Assumption {}
    public class DomainHypothesisAssumption : Assumption {}
    public class ObstacleNegativeAssumption : Assumption {}

    #endregion

    #endregion

    #region Object Model

    public class Entity : KAOSMetaModelElement {
        public string Name { get; set; }

        public override string FriendlyName {
            get {
                return string.IsNullOrEmpty(Name) ? Identifier : Name;
            }
        }

        public string Definition { get; set; }
        public ISet<Attribute> Attributes { get; set; }
        public EntityType Type { get; set; }

        public ISet<Entity> Parents { get; set; }

        public ISet<Entity> Ancestors
        {
            get
            {
                var ancestors = new HashSet<Entity>();
                ancestors.Add(this);
                foreach (var parent in Parents) {
                    foreach (var a in parent.Ancestors) {
                        ancestors.Add (a);
                    }
                }
                return ancestors;
            }
        }

        public Entity (KAOSModel model) : base (model)
        {
            Attributes = new HashSet<Attribute> ();
            Type = EntityType.None;
            Parents = new HashSet<Entity> ();
        }

        public override KAOSMetaModelElement Copy ()
        {
            throw new NotImplementedException ();
        }
    }

    public class Attribute : KAOSMetaModelElement {
        public bool Derived { get; set; }
        public string Name { get; set; }
        public string Definition { get; set; }

        public override string FriendlyName {
            get {
                return string.IsNullOrEmpty(Name) ? Identifier : Name;
            }
        }

        public GivenType Type { get; set; }

        public Attribute (KAOSModel model) : base (model)
        {
            Derived = false;
        }

        public override KAOSMetaModelElement Copy ()
        {
            throw new NotImplementedException ();
        }
    }

    public class GivenType : KAOSMetaModelElement {
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

        public override KAOSMetaModelElement Copy ()
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

    public class Link : KAOSMetaModelElement {
        public Entity Target { get; set; }
        public string Role { get; set; }
        public string Multiplicity { get; set; }
        public Link  (KAOSModel model) : base (model)
        {}

        public override KAOSMetaModelElement Copy ()
        {
            throw new NotImplementedException ();
        }
    }

    public enum EntityType {
        None, Software, Environment, Shared
    }

    #endregion

    public class AlternativeSystem : KAOSMetaModelElement
    {
        public string Name { get; set; }

        public override string FriendlyName {
            get {
                return string.IsNullOrEmpty(Name) ? Identifier : Name;
            }
        }

        public string Definition { get; set; }

        public ISet<AlternativeSystem> Alternatives { get; set; }

        public AlternativeSystem  (KAOSModel model) : base (model)
        {
            Alternatives = new HashSet<AlternativeSystem> ();
        }

        public override KAOSMetaModelElement Copy ()
        {
            throw new NotImplementedException ();
        }
    }

    public class Predicate : KAOSMetaModelElement
    {
        public string Name { get; set; }

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
        }

        public override KAOSMetaModelElement Copy ()
        {
            throw new NotImplementedException ();
        }
    }

    public class PredicateArgument {

        public string Name { get; set; }

        public Entity Type { get; set; }
    }
}
