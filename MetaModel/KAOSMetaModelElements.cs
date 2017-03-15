using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace KAOSTools.Core
{
    [DataContract]
    public abstract class KAOSCoreElement
    {
        [DataMember]
        public string Identifier { get; set; }

        public virtual string FriendlyName {
            get {
                return Identifier;
            }
        }

        [DataMember]
        public bool Implicit { get; set; }

        public ISet<AlternativeSystem> InSystems { get; set; }

        public KAOSModel model;

        public IDictionary<string,string> CustomData {
            get;
            set;
        }

        public KAOSCoreElement (KAOSModel model)
        {
            this.Identifier = Guid.NewGuid ().ToString ();
            this.model = model;
            this.CustomData = new Dictionary<string,string> ();
        }

        public abstract KAOSCoreElement Copy ();

        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(KAOSCoreElement))
                return false;
            KAOSCoreElement other = (KAOSCoreElement)obj;
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

    [DataContract]
    public class Goal : KAOSCoreElement
    {
        [DataMember]
        public string Name { get; set; }

        public override string FriendlyName {
            get {
                return string.IsNullOrEmpty(Name) ? Identifier : Name;
            }
        }

        [DataMember]
        public string Definition { get; set; }

        public Formula FormalSpec { get; set; }

        public double CPS { get; set; }

        public double RDS { get; set; }

        public IDictionary<CostVariable, double> Costs {
            get;
            set;
        }

        public Goal (KAOSModel model) : base(model)
        {
            InSystems = new HashSet<AlternativeSystem>();
            Costs = new Dictionary<CostVariable, double> ();
        }

        public override KAOSCoreElement Copy ()
        {
            return new Goal (null) {
                Identifier = Identifier,
                Implicit = Implicit,
                Name = Name,
                Definition = Definition,
                FormalSpec = FormalSpec,
                CPS = CPS,
                RDS = RDS,
            };
        }
    }

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
            InSystems = new HashSet<AlternativeSystem>();
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

    public class SoftGoal : KAOSCoreElement
    {
        public string Name { get; set; }

        public override string FriendlyName {
            get {
                return string.IsNullOrEmpty(Name) ? Identifier : Name;
            }
        }

        public string Definition { get; set; }

        public SoftGoal (KAOSModel model) : base(model)
        {
            InSystems = new HashSet<AlternativeSystem>();
        }

        public override KAOSCoreElement Copy ()
        {
            return new SoftGoal (null) {
                Identifier = Identifier,
                Implicit = Implicit,
                Name = Name,
                Definition = Definition
            };
        }
    }

    [DataContract]
    public class Obstacle : KAOSCoreElement
    {
        [DataMember]
        public string Name { get; set; }

        public override string FriendlyName {
            get {
                return string.IsNullOrEmpty(Name) ? Identifier : Name;
            }
        }

        [DataMember]
        public string Definition { get; set; }

        public Formula FormalSpec { get; set; }

        public double EPS { get; set; }

        public double CPS { get; set; }

        public UncertaintyDistribution SatisfactionUncertainty { get; set; }

        public Dictionary<Expert, QuantileList> ExpertEstimates { get; set; }

        public Obstacle (KAOSModel model) : base(model)
        {
            ExpertEstimates = new Dictionary<Expert, QuantileList> ();
        }

        public override KAOSCoreElement Copy ()
        {
            return new Obstacle (null) {
                Identifier = Identifier,
                Implicit = Implicit,
                Name = Name,
                Definition = Definition,
                FormalSpec = FormalSpec,
                CPS = CPS,
                EPS = EPS
            };
        }
    }

    [DataContract]
    public class DomainHypothesis : KAOSCoreElement
    { 
        [DataMember]
        public string Name { get; set; }

        public override string FriendlyName {
            get {
                return string.IsNullOrEmpty(Name) ? Identifier : Name;
            }
        }

        [DataMember]
        public string Definition { get; set; }

        public Formula FormalSpec { get; set; }

        public double EPS { get; set; }

        public UncertaintyDistribution SatisfactionUncertainty { get; set; }

        public DomainHypothesis (KAOSModel model) : base(model)
        {

        }

        public override KAOSCoreElement Copy ()
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

    [DataContract]
    public class DomainProperty : KAOSCoreElement
    { 
        [DataMember]
        public string Name { get; set; }

        public override string FriendlyName {
            get {
                return string.IsNullOrEmpty(Name) ? Identifier : Name;
            }
        }

        [DataMember]
        public string Definition { get; set; }

        public Formula FormalSpec { get; set; }

        public double EPS { get; set; }

        public DomainProperty (KAOSModel model) : base (model) {}

        public override KAOSCoreElement Copy ()
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

    [DataContract]
    public class Agent : KAOSCoreElement
    {
        [DataMember]
        public string Name { get; set; }

        public override string FriendlyName {
            get {
                return string.IsNullOrEmpty(Name) ? Identifier : Name;
            }
        }

        [DataMember]
        public string Definition { get; set; }

        [DataMember]
        public AgentType Type { get; set; }

        public Agent (KAOSModel model) : base(model)
        {
            Type = AgentType.None;
        }

        public override KAOSCoreElement Copy ()
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

    [DataContract]
    public abstract class AgentAssignment : KAOSCoreElement
    {
        public AlternativeSystem SystemReference { get; set; }

        [DataMember]
        public IList<string> AgentIdentifiers { get; set; }

        public bool IsEmpty {
            get {
                return AgentIdentifiers.Count == 0;
            }
        }

        public AgentAssignment (KAOSModel model) : base (model)
        {
            AgentIdentifiers = new List<string> ();
        }

        public void Add (Agent agent)
        {
            this.AgentIdentifiers.Add (agent.Identifier);
        }
    }

    [DataContract]
    public class GoalAgentAssignment : AgentAssignment {
        
        [DataMember]
        public string GoalIdentifier { get; set ; }
        public GoalAgentAssignment  (KAOSModel model) : base (model) {}

        public override KAOSCoreElement Copy ()
        {
            return new GoalAgentAssignment (null) {
                Identifier = Identifier,
                Implicit = Implicit,
                GoalIdentifier = GoalIdentifier,
                AgentIdentifiers = new List<string> (AgentIdentifiers)
            };
        }
    }

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

    [DataContract]
    public class GoalRefinement : KAOSCoreElement
    {
        
        [DataMember]
        public string ParentGoalIdentifier { get; set; }

        public string SystemReferenceIdentifier { get; set; }

        public bool IsComplete {
            get;
            set;
        }

        public ISet<string> PositiveSoftGoalsIdentifiers { get; set; }
        public ISet<string> NegativeSoftGoalsIdentifiers { get; set; }

        [DataMember]
        public IList<string> SubGoalIdentifiers { get; set; }
        
        [DataMember]
        public ISet<string> DomainPropertyIdentifiers { get; set; }
        
        [DataMember]
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
            SubGoalIdentifiers = new List<string> ();
            DomainPropertyIdentifiers = new HashSet<string> ();
            DomainHypothesisIdentifiers = new HashSet<string> ();

            PositiveSoftGoalsIdentifiers = new HashSet<string> ();
            NegativeSoftGoalsIdentifiers = new HashSet<string> ();

            IsComplete = false;

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

        public void Remove (Goal goal)
        {
            this.SubGoalIdentifiers.Remove (goal.Identifier);
        }

        public void Remove (DomainProperty domProp)
        {
            this.DomainPropertyIdentifiers.Remove (domProp.Identifier);
        }

        public void Remove (DomainHypothesis domHyp)
        {
            this.DomainHypothesisIdentifiers.Remove (domHyp.Identifier);
        }

        public void AddNegativeSoftGoal (SoftGoal goal)
        {
            this.NegativeSoftGoalsIdentifiers.Add (goal.Identifier);
        }

        public void AddPositiveSoftGoal (SoftGoal goal)
        {
            this.PositiveSoftGoalsIdentifiers.Add (goal.Identifier);
        }

        public override KAOSCoreElement Copy ()
        {
            return new GoalRefinement (null) {
                Identifier = Identifier,
                Implicit = Implicit,
                ParentGoalIdentifier = ParentGoalIdentifier,
                SystemReferenceIdentifier = SystemReferenceIdentifier,
                SubGoalIdentifiers = new List<string> (SubGoalIdentifiers),
                DomainPropertyIdentifiers = new HashSet<string> (DomainPropertyIdentifiers),
                DomainHypothesisIdentifiers = new HashSet<string> (DomainHypothesisIdentifiers),
                RefinementPattern = RefinementPattern,
                Parameters = new List<dynamic> (Parameters)
            };
        }
    }

    public class AntiGoalRefinement : KAOSCoreElement
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

    public class ObstacleRefinement : KAOSCoreElement
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

        public override KAOSCoreElement Copy ()
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

    [DataContract]
    public class Obstruction : KAOSCoreElement {
        
        [DataMember]
        public string ObstructedGoalIdentifier { get; set; }
        
        [DataMember]
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

        public override KAOSCoreElement Copy ()
        {
            return new Obstruction (null) {
                Identifier = Identifier,
                Implicit = Implicit,
                ObstructedGoalIdentifier = ObstructedGoalIdentifier,
                ObstacleIdentifier = ObstacleIdentifier
            };
        }
    }

    [DataContract]
    public class Resolution : KAOSCoreElement {
        
        [DataMember]
        public string ObstacleIdentifier { get; set; }
        
        [DataMember]
        public string ResolvingGoalIdentifier { get; set; }

        [DataMember]
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

        public override KAOSCoreElement Copy ()
        {
            return new Resolution (null) {
                Identifier = Identifier,
                Implicit = Implicit,
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
        ObstacleMitigation,
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
        Uncontrollability,
        Redundant
    }

    #endregion

    #region Exceptions and assumptions

    [DataContract]
    public class GoalException : KAOSCoreElement {
        public GoalException (KAOSModel model) : base (model)
        {
        }
        
        [DataMember]
        public string AnchorGoalIdentifier { get; set; }
        
        [DataMember]
        public string ResolvedObstacleIdentifier { get; set; }
        
        [DataMember]
        public string ResolvingGoalIdentifier { get; set; }

        public void SetAnchorGoal (Goal goal)
        {
            AnchorGoalIdentifier = goal.Identifier;
        }

        public void SetResolvingGoal (Goal goal)
        {
            ResolvingGoalIdentifier = goal.Identifier;
        }

        public void SetObstacle (Obstacle obstacle)
        {
            ResolvedObstacleIdentifier = obstacle.Identifier;
        }

        public override KAOSCoreElement Copy ()
        {
            return new GoalException (model) {
                AnchorGoalIdentifier = AnchorGoalIdentifier,
                ResolvingGoalIdentifier = ResolvingGoalIdentifier,
                ResolvedObstacleIdentifier = ResolvedObstacleIdentifier,
                Implicit = Implicit,
                Identifier = Identifier
            };
        }
    }

    public class GoalReplacement : KAOSCoreElement {
        public GoalReplacement (KAOSModel model) : base (model)
        {
        }
        public string AnchorGoalIdentifier { get; set; }
        public string ResolvedObstacleIdentifier { get; set; }
        public string ResolvingGoalIdentifier { get; set; }

        public void SetAnchorGoal (Goal goal)
        {
            AnchorGoalIdentifier = goal.Identifier;
        }

		public void SetAnchorGoal(string goal)
		{
			AnchorGoalIdentifier = goal;
		}

        public void SetResolvingGoal (Goal goal)
        {
            ResolvingGoalIdentifier = goal.Identifier;
        }

        public void SetObstacle (Obstacle obstacle)
        {
            ResolvedObstacleIdentifier = obstacle.Identifier;
        }

        public override KAOSCoreElement Copy ()
        {
            return new GoalReplacement (model) {
                AnchorGoalIdentifier = AnchorGoalIdentifier,
                ResolvingGoalIdentifier = ResolvingGoalIdentifier,
                ResolvedObstacleIdentifier = ResolvedObstacleIdentifier,
                Implicit = Implicit,
                Identifier = Identifier
            };
        }

    }


    public class ObstacleAssumption : KAOSCoreElement {
        public ObstacleAssumption (KAOSModel model) : base (model)
        {
        }
        public string AnchorGoalIdentifier { get; set; }
        public string ResolvedObstacleIdentifier { get; set; }

        public void SetAnchorGoal (Goal goal)
        {
            AnchorGoalIdentifier = goal.Identifier;
        }

        public void SetObstacle (Obstacle obstacle)
        {
            ResolvedObstacleIdentifier = obstacle.Identifier;
        }

        public override KAOSCoreElement Copy ()
        {
            return new ObstacleAssumption (model) {
                AnchorGoalIdentifier = AnchorGoalIdentifier,
                ResolvedObstacleIdentifier = ResolvedObstacleIdentifier,
                Implicit = Implicit,
                Identifier = Identifier
            };
        }

    }

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

    public class AlternativeSystem : KAOSCoreElement
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

        public override KAOSCoreElement Copy ()
        {
			return new AlternativeSystem(model) {
				Identifier = Identifier,
				Implicit = Implicit,
				InSystems = InSystems,
				CustomData = CustomData,
				Name = Name,
				Definition = Definition,
				Alternatives = new HashSet<AlternativeSystem> (Alternatives.Select (x => (AlternativeSystem) x.Copy ()))
			};
        }
    }

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
				InSystems = InSystems,
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



    public abstract class UncertaintyDistribution {
        public abstract double Sample (Random r);
    }

    public class UniformDistribution : UncertaintyDistribution {
        public float LowerBound;
        public float UpperBound;

        public override double Sample (Random r) {
            return MathNet.Numerics.Distributions.ContinuousUniform.Sample (r, LowerBound, UpperBound);
        }
    }

    public class TriangularDistribution : UncertaintyDistribution {
        public float Min;
        public float Max;
        public float Mode;

        public override double Sample (Random r) {
            return MathNet.Numerics.Distributions.Triangular.Sample (r, Min, Max, Mode);
        }
    }

    public class PERTDistribution : UncertaintyDistribution {
        public float Min;
        public float Max;
        public float Mode;

        public override double Sample (Random r) {
            var mean = (Min + 4 * Mode + Max) / 6;
            var alpha = 6 * ((mean - Min) / (Max - Min));
            var beta = 6 * ((Max - mean) / (Max - Min));
            // Console.WriteLine ("beta={0}, alpha={1}, mean={2}", beta, alpha, mean);
            
            var s = r.NextDouble ();
            return (alglib.invincompletebeta (alpha, beta, s) * (Max - Min)) + Min;
        }
    }

    public class BetaDistribution : UncertaintyDistribution {
        public float Alpha;
        public float Beta;

        public override double Sample (Random r) {
            return MathNet.Numerics.Distributions.Beta.Sample(r, Alpha, Beta);
        }
    }

    public class QuantileDistribution : UncertaintyDistribution
    {
        readonly double[] probabilities;
        readonly double[] quantiles;

        public QuantileDistribution (double[] probabilities, double[] quantiles)
        {
            this.probabilities = probabilities;
            this.quantiles = quantiles;
        }

        public override double Sample (Random _random)
        {
            double s = _random.NextDouble ();

            int i;
            for (i = 1; i < probabilities.Length - 1; i++) {
                if (s < probabilities [i]) {
                    break;
                }
            }

            var ss = (s - probabilities [i-1]) / (probabilities [i] - probabilities [i - 1]);
            var ss2 = ss * (quantiles [i] - quantiles [i - 1]) + quantiles[i - 1];
            return ss2;
        }

        public double LowerBound {
            get {
                return quantiles.Min ();
            }
        }

        public double UpperBound {
            get {
                return quantiles.Max ();
            }
        }

        public override string ToString ()
        {
            return string.Format ("[QuantileDistribution: {0}]", 
                string.Join (" ", Enumerable.Range (0, quantiles.Length).Select (i => quantiles[i] + ":" + probabilities[i]))
            );
        }

    }

    public class MixtureDistribution : UncertaintyDistribution
    {
        readonly double[] cummulativeWeight;
        readonly QuantileDistribution[] distributions;

        public MixtureDistribution (double[] cummulativeWeight, QuantileDistribution[] distributions)
        {
            this.cummulativeWeight = cummulativeWeight;
            this.distributions = distributions;
        }

        public override double Sample (Random _random)
        {
            double s = _random.NextDouble ();

            int i;
            for (i = 1; i < cummulativeWeight.Length - 1; i++) {
                if (s < cummulativeWeight [i]) {
                    break;
                }
            }
            return distributions[i-1].Sample (_random);
        }

        public double LowerBound {
            get {
                return distributions.Min (x => x.LowerBound);
            }
        }

        public double UpperBound {
            get {
                return distributions.Max (x => x.UpperBound);
            }
        }
    }

    public class QuantileList {
        public List<double> Quantiles;
        public QuantileList ()
        {
            Quantiles = new List<double> ();
        }
    }

    public class Expert : KAOSCoreElement
    {
        public string Name { get; set; }

        public override string FriendlyName {
            get {
                return string.IsNullOrEmpty(Name) ? Identifier : Name;
            }
        }

        public Expert  (KAOSModel model) : base (model)
        {
        }

        public override KAOSCoreElement Copy ()
        {
            throw new NotImplementedException ();
        }
    }

    public class Calibration : KAOSCoreElement
    {
        public string Name { get; set; }

        public double EPS { get; set; }

        public Dictionary<Expert, QuantileList> ExpertEstimates { get; set; }

        public override string FriendlyName {
            get {
                return string.IsNullOrEmpty(Name) ? Identifier : Name;
            }
        }

        public Calibration  (KAOSModel model) : base (model)
        {
        }

        public override KAOSCoreElement Copy ()
        {
            throw new NotImplementedException ();
        }
    }

    public class CostVariable : KAOSCoreElement
    {
        public string Name { get; set; }

        public CostVariable  (KAOSModel model) : base (model)
        {
        }

        public override KAOSCoreElement Copy ()
        {
            throw new NotImplementedException ();
        }
    }

    public class Constraint : KAOSCoreElement
    {
        public string Name { get; set; }
        public string Definition { get; set; }
        public List<string> Conflict  { get; set; }
        public List<string> Or  { get; set; }

        public Constraint  (KAOSModel model) : base (model)
        {
            Conflict = new List<string> ();
            Or = new List<string> ();
        }

        public override KAOSCoreElement Copy ()
        {
			return new Constraint(model) {
				Identifier = Identifier,
				Implicit = Implicit,
				InSystems = InSystems,
				CustomData = CustomData,
				Name = Name,
				Definition = Definition,
				Conflict = new List<string>(Conflict),
				Or = new List<string>(Or)
			};
        }
    }

    public class ReqOpSpecification
    {
        public Goal Goal;
        public Formula Specification;
        public ReqOpSpecification (Goal goal, Formula specification)
        {
            this.Goal = goal;
            this.Specification = specification;
        }
        
    }

    public class Operation : KAOSCoreElement
    {
        public string Name { get; set; }
        public Formula DomPre { get; set; }
        public Formula DomPost { get; set; }

        public IList<ReqOpSpecification> ReqTrig { get; set; }
        public IList<ReqOpSpecification> ReqPre { get; set; }
        public IList<ReqOpSpecification> ReqPost { get; set; }

        public Operation  (KAOSModel model) : base (model)
        {
            ReqTrig = new List<ReqOpSpecification> ();
            ReqPre = new List<ReqOpSpecification> ();
            ReqPost = new List<ReqOpSpecification> ();
        }

        public override KAOSCoreElement Copy ()
        {
            throw new NotImplementedException ();
        }
    }
}
