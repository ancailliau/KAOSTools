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
    }

    public enum AgentType
    {
        None,
        Software,
        Environment,
        Malicious
    }

    public abstract class AgentAssignment : KAOSMetaModelElement
    {
        public AlternativeSystem SystemReference { get; set; }

        public IList<Agent> Agents { get; set; }

        public bool IsEmpty {
            get {
                return Agents.Count == 0;
            }
        }

        public AgentAssignment (KAOSModel model) : base (model)
        {
            Agents = new List<Agent> ();
        }
    }

    public class GoalAgentAssignment : AgentAssignment {
        public Goal Goal { get; set ; }
        public GoalAgentAssignment  (KAOSModel model) : base (model) {}
    }

    public class ObstacleAgentAssignment : AgentAssignment {
        public Obstacle Obstacle { get; set ; }
        public ObstacleAgentAssignment  (KAOSModel model) : base (model) {}
    }

    public class AntiGoalAgentAssignment : AgentAssignment {
        public AntiGoal AntiGoal { get; set ; }
        public AntiGoalAgentAssignment  (KAOSModel model) : base (model) {}
    }

    public class GoalRefinement : KAOSMetaModelElement
    {
        public Goal ParentGoal { get; set; }

        public AlternativeSystem SystemReference { get; set; }

        public IList<Goal> Subgoals { get; set; }

        public IList<DomainProperty> DomainProperties { get; set; }

        public IList<DomainHypothesis> DomainHypotheses { get; set; }

        public bool IsEmpty {
            get {
                return Subgoals.Count + DomainProperties.Count + DomainHypotheses.Count == 0;
            }
        }

        public RefinementPattern RefinementPattern { get; set; }
        public List<dynamic> Parameters { get; set; }

        public GoalRefinement (KAOSModel model) : base (model)
        {
            SystemReference = null;
            Subgoals = new List<Goal> ();
            DomainProperties = new List<DomainProperty> ();
            DomainHypotheses = new List<DomainHypothesis> ();
            Parameters = new List<dynamic> ();
        }

        public GoalRefinement (KAOSModel model, Goal goal) : this (model)
        {
            Subgoals.Add (goal);
        }

        public GoalRefinement (KAOSModel model, params Goal[] goals) : this (model)
        {
            foreach (var goal in goals)
                Subgoals.Add (goal);
        }
    }


    public class AntiGoalRefinement : KAOSMetaModelElement
    {
        public AntiGoal ParentAntiGoal { get; set; }
        public AlternativeSystem SystemReference { get; set; }
        public IList<AntiGoal> SubAntiGoals { get; set; }
        public IList<Obstacle> Obstacles { get; set; }

        public IList<DomainProperty> DomainProperties { get; set; }
        public IList<DomainHypothesis> DomainHypotheses { get; set; }

        public bool IsEmpty {
            get {
                return SubAntiGoals.Count + Obstacles.Count 
                    + DomainProperties.Count + DomainHypotheses.Count == 0;
            }
        }

        public AntiGoalRefinement (KAOSModel model) : base (model)
        {
            SubAntiGoals = new List<AntiGoal> ();
            Obstacles = new List<Obstacle> ();
            DomainProperties = new List<DomainProperty> ();
            DomainHypotheses = new List<DomainHypothesis> ();
        }
    }

    public class ObstacleRefinement : KAOSMetaModelElement
    {
        public Obstacle ParentObstacle { get; set; }
        public IList<Obstacle> Subobstacles { get; set; }

        public IList<DomainProperty> DomainProperties { get; set; }

        public IList<DomainHypothesis> DomainHypotheses { get; set; }

        public bool IsEmpty {
            get {
                return Subobstacles.Count + DomainProperties.Count + DomainHypotheses.Count == 0;
            }
        }
        public ObstacleRefinement (KAOSModel model) : base (model)
        {
            Subobstacles = new List<Obstacle> ();
            DomainProperties = new List<DomainProperty> ();
            DomainHypotheses = new List<DomainHypothesis> ();
        }

        public ObstacleRefinement (KAOSModel model, Obstacle obstacle) : this (model)
        {
            Subobstacles.Add (obstacle);
        }

        public ObstacleRefinement (KAOSModel model, params Obstacle[] obstacles) : this (model)
        {
            foreach (var obstacle in obstacles)
                Subobstacles.Add (obstacle);
        }
    }

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

    public class Obstruction : KAOSMetaModelElement {
        public Goal ObstructedGoal { get; set; }
        public Obstacle Obstacle { get; set; }
        public Obstruction (KAOSModel model) : base (model)
        {}
    }

    public class Resolution : KAOSMetaModelElement {
        public Obstacle Obstacle { get; set; }
        public Goal ResolvingGoal { get; set; }
        public List<dynamic> Parameters { get; set; }
        public ResolutionPattern ResolutionPattern { get; set; }
        public Resolution (KAOSModel model) : base (model)
        {
            ResolutionPattern = ResolutionPattern.None;
            Parameters = new List<dynamic> ();
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
    }

    public class PredicateArgument {

        public string Name { get; set; }

        public Entity Type { get; set; }
    }
}
