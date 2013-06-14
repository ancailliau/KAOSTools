using System.Collections.Generic;
using System;

namespace KAOSTools.MetaModel
{
    /// <summary>
    /// Represents an element of the KAOS meta-model.
    /// </summary>
    public abstract class KAOSMetaModelElement
    {
        string _identifier = Guid.NewGuid ().ToString ();

        /// <summary>
        /// Gets or sets the identifier of the element.
        /// </summary>
        /// <value>The unique identifier.</value>
        public string Identifier { 
            get {
                return _identifier; 
            }
            set { 
                _identifier = value;
            }
        }

        /// <summary>
        /// Gets the friendly name of the concept. For instance, it will return the name if a name is defined, 
        /// otherwise the identifier.
        /// </summary>
        /// <value>The name.</value>
        public virtual string FriendlyName {
            get {
                return Identifier;
            }
        }

        /// <summary>
        /// Gets the name of the concept. For instance, 'goal' or 'domain property'.
        /// </summary>
        /// <value>The name of the concept.</value>
        public virtual string ConceptName {
            get { return "KAOSMetaModelElement"; }
        }

        /// <summary>
        /// Gets or sets whether this <see cref="KAOSTools.MetaModel.KAOSMetaModelElement"/>
        /// is implicitly declared or explicitely declared.
        /// </summary>
        /// <value><c>true</c> if implicit; otherwise, <c>false</c>.</value>
        public bool Implicit { get; set; }

        /// <summary>
        /// Gets or sets the systems the element is in.
        /// </summary>
        /// <value>The systems the element is referenced in.</value>
        public ISet<AlternativeSystem> InSystems { get; set; }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="KAOSTools.MetaModel.KAOSMetaModelElement"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="KAOSTools.MetaModel.KAOSMetaModelElement"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
        /// <see cref="KAOSTools.MetaModel.KAOSMetaModelElement"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(KAOSMetaModelElement))
                return false;
            KAOSMetaModelElement other = (KAOSMetaModelElement)obj;
            return _identifier == other._identifier;
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="KAOSTools.MetaModel.KAOSMetaModelElement"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode ()
        {
            unchecked {
                return (_identifier != null ? _identifier.GetHashCode () : 0);
            }
        }
        
    }

    #region Goal Model

    /// <summary>
    /// Represents a goal
    /// </summary>
    public class Goal : KAOSMetaModelElement
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name of the goal.</value>
        public string Name { get; set; }

        public override string FriendlyName {
            get {
                return string.IsNullOrEmpty(Name) ? Identifier : Name;
            }
        }

        public override string ConceptName {
            get {
                return "Goal";
            }
        }

        /// <summary>
        /// Gets or sets the definition.
        /// </summary>
        /// <value>The definition of the goal.</value>
        public string Definition { get; set; }

        /// <summary>
        /// Gets or sets the formal specification.
        /// </summary>
        /// <value>The formal specification of the goal.</value>
        public Formula FormalSpec { get; set; }

        /// <summary>
        /// Gets or sets the computed probablity of satisfaction
        /// </summary>
        /// <value>The probability of satisfaction computed by propagation.</value>
        public double CPS { get; set; }

        /// <summary>
        /// Gets or sets the required degree of satisfaction.
        /// </summary>
        /// <value>The required degree of satisfaction.</value>
        public double RDS { get; set; }

        /// <summary>
        /// Gets or sets the refinements for the goal.
        /// </summary>
        /// <value>The refinements.</value>
        public ISet<GoalRefinement> Refinements { get; set; }

        /// <summary>
        /// Gets or sets the obstructions of the goal.
        /// </summary>
        /// <value>The obstruction.</value>
        public ISet<Obstacle> Obstructions { get; set; }

        /// <summary>
        /// Gets or sets the agents assignment for the goal.
        /// </summary>
        /// <value>The agents assignments.</value>
        public ISet<AgentAssignment> AgentAssignments { get; set; }

        public ISet<GoalException> Exceptions { get; set; }
        public ISet<Assumption> Assumptions { get; set; }

        /// <summary>
        /// Initializes a new goal.
        /// </summary>
        public Goal ()
        {
            Refinements = new HashSet<GoalRefinement> ();
            Obstructions = new HashSet<Obstacle> ();
            AgentAssignments = new HashSet<AgentAssignment> ();
            InSystems = new HashSet<AlternativeSystem>();
            Exceptions = new HashSet<GoalException> ();
            Assumptions = new HashSet<Assumption> ();
        }

        /// <summary>
        /// Initializes a new goal with the specified name.
        /// </summary>
        /// <param name="name">Name.</param>
        public Goal (string name) : this()
        {
            Name = name;
        }

        /// <summary>
        /// Initializes a new goal with the specified identifier and name.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <param name="name">Name.</param>
        public Goal (string identifier, string name) : this(name)
        {
            Identifier = identifier;
        }
    }
    
    /// <summary>
    /// Represents a goal
    /// </summary>
    public class AntiGoal : KAOSMetaModelElement
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name of the goal.</value>
        public string Name { get; set; }

        public override string FriendlyName {
            get {
                return string.IsNullOrEmpty(Name) ? Identifier : Name;
            }
        }

        public override string ConceptName {
            get {
                return "AntiGoal";
            }
        }

        /// <summary>
        /// Gets or sets the definition.
        /// </summary>
        /// <value>The definition of the goal.</value>
        public string Definition { get; set; }

        /// <summary>
        /// Gets or sets the formal specification.
        /// </summary>
        /// <value>The formal specification of the goal.</value>
        public Formula FormalSpec { get; set; }

        /// <summary>
        /// Gets or sets the refinements for the goal.
        /// </summary>
        /// <value>The refinements.</value>
        public ISet<AntiGoalRefinement> Refinements { get; set; }

        /// <summary>
        /// Gets or sets the agents assignment for the goal.
        /// </summary>
        /// <value>The agents assignments.</value>
        public ISet<AgentAssignment> AgentAssignments { get; set; }

        /// <summary>
        /// Initializes a new goal.
        /// </summary>
        public AntiGoal ()
        {
            Refinements = new HashSet<AntiGoalRefinement> ();
            AgentAssignments = new HashSet<AgentAssignment> ();
            InSystems = new HashSet<AlternativeSystem>();
        }

        /// <summary>
        /// Initializes a new goal with the specified name.
        /// </summary>
        /// <param name="name">Name.</param>
        public AntiGoal (string name) : this()
        {
            Name = name;
        }

        /// <summary>
        /// Initializes a new goal with the specified identifier and name.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <param name="name">Name.</param>
        public AntiGoal (string identifier, string name) : this(name)
        {
            Identifier = identifier;
        }
    }

    /// <summary>
    /// Represents an obstacle.
    /// </summary>
    public class Obstacle : KAOSMetaModelElement
    {
        /// <summary>
        /// Gets or sets the name of the obstacle.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        
        public override string FriendlyName {
            get {
                return string.IsNullOrEmpty(Name) ? Identifier : Name;
            }
        }
        
        public override string ConceptName {
            get {
                return "Obstacle";
            }
        }

        /// <summary>
        /// Gets or sets the definition of the obstacle.
        /// </summary>
        /// <value>The definition.</value>
        public string Definition { get; set; }

        /// <summary>
        /// Gets or sets the formal specification of the obstacle.
        /// </summary>
        /// <value>The formal specification.</value>
        public Formula FormalSpec { get; set; }

        /// <summary>
        /// Gets or sets the estimated probability of satisfaction.
        /// </summary>
        /// <value>The estimated probability of satisfaction.</value>
        public double EPS { get; set; }

        /// <summary>
        /// Gets or sets the computed probability of satisfaction.
        /// </summary>
        /// <value>The computed probability of satisfaction.</value>
        public double CPS { get; set; }

        /// <summary>
        /// Gets or sets the refinements of the obstacle.
        /// </summary>
        /// <value>The refinements.</value>
        public IList<ObstacleRefinement> Refinements { get; set; }

        /// <summary>
        /// Gets or sets the resolutions for the obstacle.
        /// </summary>
        /// <value>The resolutions.</value>
        public IList<Resolution> Resolutions { get; set; }

        public ISet<Assumption> Assumptions { get; set; }

        
        public ISet<AgentAssignment> AgentAssignments { get; set; }

        /// <summary>
        /// Initializes a new obstacle.
        /// </summary>
        public Obstacle ()
        {
            Refinements = new List<ObstacleRefinement> ();
            Resolutions = new List<Resolution> ();
            Assumptions = new HashSet<Assumption> ();
            AgentAssignments = new HashSet<AgentAssignment> ();
        }

        /// <summary>
        /// Initialize a new obstacle with the specified name.
        /// </summary>
        /// <param name="name">Name.</param>
        public Obstacle (string name) : this ()
        {
            Name = name;
        }

        /// <summary>
        /// Initializes a new obstacle with the specified identifier and name.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <param name="name">Name.</param>
        public Obstacle (string identifier, string name) : this (name)
        {
            Identifier = identifier;
        }
    }

    /// <summary>
    /// Representes a domain hypothesis.
    /// </summary>
    public class DomainHypothesis : KAOSMetaModelElement
    { 
        /// <summary>
        /// Gets or sets the name of the domain hypothesis.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        
        public override string FriendlyName {
            get {
                return string.IsNullOrEmpty(Name) ? Identifier : Name;
            }
        }
        
        public override string ConceptName {
            get {
                return "Domain hypothesis";
            }
        }

        /// <summary>
        /// Gets or sets the definition of the domain hypothesis.
        /// </summary>
        /// <value>The definition.</value>
        public string Definition { get; set; }
        
        /// <summary>
        /// Gets or sets the formal specification for the domain property.
        /// </summary>
        /// <value>The formal specification.</value>
        public Formula FormalSpec { get; set; }
        
        /// <summary>
        /// Gets or sets the estimated probability of satisfaction.
        /// </summary>
        /// <value>The estimated probability of satisfaction.</value>
        public double EPS { get; set; }
    }

    /// <summary>
    /// Represents a domain property.
    /// </summary>
    public class DomainProperty : KAOSMetaModelElement
    { 
        /// <summary>
        /// Gets or sets the name of the domain property.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        
        public override string FriendlyName {
            get {
                return string.IsNullOrEmpty(Name) ? Identifier : Name;
            }
        }
        
        public override string ConceptName {
            get {
                return "Domain property";
            }
        }

        /// <summary>
        /// Gets or sets the definition of the domain property.
        /// </summary>
        /// <value>The definition.</value>
        public string Definition { get; set; }

        /// <summary>
        /// Gets or sets the formal specification for the domain property.
        /// </summary>
        /// <value>The formal specification.</value>
        public Formula FormalSpec { get; set; }

        /// <summary>
        /// Gets or sets the estimated probability of satisfaction.
        /// </summary>
        /// <value>The estimated probability of satisfaction.</value>
        public double EPS { get; set; }
    }

    /// <summary>
    /// Represents an agent.
    /// </summary>
    public class Agent : KAOSMetaModelElement
    {
        /// <summary>
        /// Gets or sets the name of the agent.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        
        public override string FriendlyName {
            get {
                return string.IsNullOrEmpty(Name) ? Identifier : Name;
            }
        }
        
        public override string ConceptName {
            get {
                return "Agent";
            }
        }

        /// <summary>
        /// Gets or sets the definition of the agent.
        /// </summary>
        /// <value>The definition.</value>
        public string Definition { get; set; }

        /// <summary>
        /// Gets or sets the type of the agent. The type is either <c>None</c>, 
        /// <c>Software</c>, <c>Environment</c>. 
        /// 
        /// See <see cref="KAOSTools.MetaModel.AgentType"/> for more details.
        /// </summary>
        /// <value>The type.</value>
        public AgentType Type { get; set; }

        public ISet<AlternativeSystem> InSystems { get; set; }

        /// <summary>
        /// Initializes a new agent
        /// </summary>
        public Agent ()
        {
            Type = AgentType.None;
        }

        /// <summary>
        /// Initializes a new agent with the specified name.
        /// </summary>
        /// <param name="name">Name.</param>
        public Agent (string name) : this ()
        {
            Name = name;
        }

        /// <summary>
        /// Initializes a new agent of specified type with the specified name.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="type">Type.</param>
        public Agent (string name, AgentType type) : this (name)
        {
            Type = type;
        }

        /// <summary>
        /// Initializes a new agent with the specified identifier and name.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <param name="name">Name.</param>
        public Agent (string identifier, string name) : this (name)
        {
            Identifier = identifier;
        }

        /// <summary>
        /// Initializes a new agent of the specified type with the specified identifier and name.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <param name="name">Name.</param>
        /// <param name="type">Type.</param>
        public Agent (string identifier, string name, AgentType type) : this (identifier, name)
        {
            Type = type;
        }
    }

    /// <summary>
    /// Represents the type of an agent.
    /// </summary>
    public enum AgentType
    {
        None,
        Software,
        Environment
    }

    /// <summary>
    /// Represents the assignment of agent(s) to a goal.
    /// </summary>
    public class AgentAssignment : KAOSMetaModelElement
    {
        /// <summary>
        /// Gets or sets the reference to the considered system. This is used to model
        /// alternative assignement.
        /// </summary>
        /// <value>The system.</value>
        public AlternativeSystem SystemReference { get; set; }

        /// <summary>
        /// Gets or sets the agents involved with the assignements.
        /// </summary>
        /// <value>The agents.</value>
        public IList<Agent> Agents { get; set; }

        /// <summary>
        /// Gets a value indicating whether this assignement contains agents.
        /// </summary>
        /// <value><c>true</c> if this assignement is empty; otherwise, <c>false</c>.</value>
        public bool IsEmpty {
            get {
                return Agents.Count == 0;
            }
        }

        /// <summary>
        /// Initializes a new assignment.
        /// </summary>
        public AgentAssignment ()
        {
            Agents = new List<Agent> ();
        }

        /// <summary>
        /// Initializes a new assignement for the specified agent.
        /// </summary>
        /// <param name="agent">The agent.</param>
        public AgentAssignment (Agent agent) : this ()
        {
            Agents.Add (agent);
        }

        /// <summary>
        /// Initializes a new assignment for the agents.
        /// </summary>
        /// <param name="agents">Agents.</param>
        public AgentAssignment (params Agent[] agents)
        {
            foreach (var a in agents) {
                Agents.Add (a);
            }
        }
    }

    /// <summary>
    /// Represents a goal refinement
    /// </summary>
    public class GoalRefinement : KAOSMetaModelElement
    {
        /// <summary>
        /// Gets or sets the reference to the considered system. This is used to model
        /// alternative refinement.
        /// </summary>
        /// <value>The system reference.</value>
        public AlternativeSystem SystemReference { get; set; }

        /// <summary>
        /// Gets or sets the sub-goals for the refinement.
        /// </summary>
        /// <value>The sub-goals.</value>
        public IList<Goal> Subgoals { get; set; }

        /// <summary>
        /// Gets or sets the domain properties explicitely involved in the refinement.
        /// </summary>
        /// <value>The domain properties.</value>
        public IList<DomainProperty> DomainProperties { get; set; }

        /// <summary>
        /// Gets or sets the domain hypotheses explicitely involved in the refinement.
        /// </summary>
        /// <value>The domain hypotheses.</value>
        public IList<DomainHypothesis> DomainHypotheses { get; set; }

        /// <summary>
        /// Gets a value indicating whether this refinement contains sub-goals, domain properties or domain hypothesis.
        /// </summary>
        /// <value><c>true</c> if this refinement is empty; otherwise, <c>false</c>.</value>
        public bool IsEmpty {
            get {
                return Subgoals.Count + DomainProperties.Count + DomainHypotheses.Count == 0;
            }
        }

        public RefinementPattern RefinementPattern { get; set; }
        public List<dynamic> Parameters { get; set; }

        /// <summary>
        /// Initializes a new goal refinement.
        /// </summary>
        public GoalRefinement ()
        {
            SystemReference = null;
            Subgoals = new List<Goal> ();
            DomainProperties = new List<DomainProperty> ();
            DomainHypotheses = new List<DomainHypothesis> ();
            Parameters = new List<dynamic> ();
        }

        /// <summary>
        /// Initializes a new goal refinement with one sub-goal.
        /// </summary>
        /// <param name="goal">The sub-goal.</param>
        public GoalRefinement (Goal goal) : this ()
        {
            Subgoals.Add (goal);
        }

        /// <summary>
        /// Initializes a new goal refinement with the specified sub-goals.
        /// </summary>
        /// <param name="goals">The sub-goals.</param>
        public GoalRefinement (params Goal[] goals) : this ()
        {
            foreach (var goal in goals)
                Subgoals.Add (goal);
        }
    }
    

    /// <summary>
    /// Represents a anti-goal refinement
    /// </summary>
    public class AntiGoalRefinement : KAOSMetaModelElement
    {
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

        /// <summary>
        /// Initializes a new goal refinement.
        /// </summary>
        public AntiGoalRefinement ()
        {
            SubAntiGoals = new List<AntiGoal> ();
            Obstacles = new List<Obstacle> ();
            DomainProperties = new List<DomainProperty> ();
            DomainHypotheses = new List<DomainHypothesis> ();
        }
    }

    /// <summary>
    /// Represents an obstacle refinement.
    /// </summary>
    public class ObstacleRefinement : KAOSMetaModelElement
    {
        /// <summary>
        /// Gets or sets the sub-obstacles.
        /// </summary>
        /// <value>The sub-obstacles.</value>
        public IList<Obstacle> Subobstacles { get; set; }

        /// <summary>
        /// Gets or sets the domain properties involved in the obstacle refinement.
        /// </summary>
        /// <value>The domain properties.</value>
        public IList<DomainProperty> DomainProperties { get; set; }

        /// <summary>
        /// Gets or sets the domain hypotheses explicitely involved in the refinement.
        /// </summary>
        /// <value>The domain hypotheses.</value>
        public IList<DomainHypothesis> DomainHypotheses { get; set; }
        
        /// <summary>
        /// Gets a value indicating whether this refinement contains sub-obstacles, domain properties or domain hypothesis.
        /// </summary>
        /// <value><c>true</c> if this refinement is empty; otherwise, <c>false</c>.</value>
        public bool IsEmpty {
            get {
                return Subobstacles.Count + DomainProperties.Count + DomainHypotheses.Count == 0;
            }
        }
        /// <summary>
        /// Initializes a new obstacle refinement.
        /// </summary>
        public ObstacleRefinement ()
        {
            Subobstacles = new List<Obstacle> ();
            DomainProperties = new List<DomainProperty> ();
            DomainHypotheses = new List<DomainHypothesis> ();
        }

        /// <summary>
        /// Initializes a new obstacle refinement with one sub-obstacle.
        /// </summary>
        /// <param name="obstacle">The sub-obstacle.</param>
        public ObstacleRefinement (Obstacle obstacle) : this ()
        {
            Subobstacles.Add (obstacle);
        }

        /// <summary>
        /// Initializes a new obstacle refinement with specified sub-obstacles.
        /// </summary>
        /// <param name="obstacles">The sub-obstacles.</param>
        public ObstacleRefinement (params Obstacle[] obstacles) : this ()
        {
            foreach (var obstacle in obstacles)
                Subobstacles.Add (obstacle);
        }
    }

    public class GoalException : KAOSMetaModelElement {
        public Obstacle ResolvedObstacle { get; set; }
        public Goal ResolvingGoal { get; set; }
    }

    public abstract class Assumption : KAOSMetaModelElement {
        public dynamic Assumed { get; set; }
    }

    public class GoalAssumption : Assumption {}
    public class DomainHypothesisAssumption : Assumption {}
    public class ObstacleNegativeAssumption : Assumption {}

    public class Resolution : KAOSMetaModelElement {
        public Goal ResolvingGoal { get; set; }
        public List<dynamic> Parameters { get; set; }
        public ResolutionPattern ResolutionPattern { get; set; }
        public Resolution ()
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

        public override string ConceptName {
            get {
                return "Object";
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

        public Entity ()
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

        public Attribute ()
        {
            Derived = false;
        }

        public Attribute (string name, GivenType type)
        {
            this.Name = name; 
            this.Type = type;
        }
    }

    public class GivenType : KAOSMetaModelElement {
        public string Name { get; set; }
        
        public override string FriendlyName {
            get {
                return string.IsNullOrEmpty(Name) ? Identifier : Name;
            }
        }
        

        public override string ConceptName {
            get {
                return "Type";
            }
        }

        public string Definition { get; set; }
    }

    public class Relation : Entity {

        public override string ConceptName {
            get {
                return "Association";
            }
        }

        public ISet<Link> Links { get; set; } 
        public Relation ()
        {
            Links = new HashSet<Link> ();
        }
    }

    public class Link : KAOSMetaModelElement {
        public Entity Target { get; set; }
        public string Role { get; set; }
        public string Multiplicity { get; set; }
        public Link (Entity target, string multiplicty = null)
        {
            Target = target; 
            Multiplicity = multiplicty;
        }
    }

    public enum EntityType {
        None, Software, Environment, Shared
    }

    #endregion

    /// <summary>
    /// Represents an alternative system.
    /// </summary>
    public class AlternativeSystem : KAOSMetaModelElement
    {
        /// <summary>
        /// Gets or sets the name of the alternative system.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        
        public override string FriendlyName {
            get {
                return string.IsNullOrEmpty(Name) ? Identifier : Name;
            }
        }

        public override string ConceptName {
            get {
                return "System";
            }
        }

        /// <summary>
        /// Gets or sets the definition of the alternative system.
        /// </summary>
        /// <value>The description.</value>
        public string Definition { get; set; }

        /// <summary>
        /// Gets or sets the alternatives to the system.
        /// </summary>
        /// <value>The alternatives.</value>
        public ISet<AlternativeSystem> Alternatives { get; set; }
 
        /// <summary>
        /// Initializes a new system.
        /// </summary>
        public AlternativeSystem ()
        {
            Alternatives = new HashSet<AlternativeSystem> ();
        }
    }

    /// <summary>
    /// Represents a predicate. 
    /// </summary>
    public class Predicate : KAOSMetaModelElement
    {
        /// <summary>
        /// Gets or sets the name of the predicate.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        
        public override string FriendlyName {
            get {
                return string.IsNullOrEmpty(Name) ? Identifier : Name;
            }
        }

        public override string ConceptName {
            get {
                return "Predicate";
            }
        }

        /// <summary>
        /// Gets or sets the arguments of the predicate.
        /// </summary>
        /// <value>The arguments.</value>
        public IList<PredicateArgument> Arguments { get; set; }

        /// <summary>
        /// Gets or sets the definition of the predicate.
        /// </summary>
        /// <value>The definition.</value>
        public string Definition { get; set; }
 
        /// <summary>
        /// Gets or sets the formal specification of the predicate.
        /// </summary>
        /// <value>The formal specification.</value>
        public Formula FormalSpec { get; set; }

        /// <summary>
        /// Initializes a new predicate.
        /// </summary>
        public Predicate ()
        {
            Arguments = new List<PredicateArgument> ();
        }
    }

    /// <summary>
    /// Represents an argument of a predicate
    /// </summary>
    public class PredicateArgument {

        /// <summary>
        /// Gets or sets the name of the argument
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of the argument
        /// </summary>
        /// <value>The type.</value>
        public Entity Type { get; set; }
    }
}
