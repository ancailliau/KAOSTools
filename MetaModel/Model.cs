using LtlSharp;
using System.Collections.Generic;
using System.Linq;
using System;

namespace KAOSTools.MetaModel
{
    /// <summary>
    /// Represents an element of the KAOS meta-model.
    /// </summary>
    public abstract class KAOSMetaModelElement
    {
        private string _identifier = Guid.NewGuid ().ToString ();

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
        /// Gets or sets whether this <see cref="KAOSTools.MetaModel.KAOSMetaModelElement"/>
        /// is implicitly declared or explicitely declared.
        /// </summary>
        /// <value><c>true</c> if implicit; otherwise, <c>false</c>.</value>
        public bool Implicit { get; set; }

        
        /// <summary>
        /// Gets or sets the systems the element is in.
        /// </summary>
        /// <value>The systems the element is referenced in.</value>
        public ISet<System> InSystems { get; set; }
    }

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

        /// <summary>
        /// Gets or sets the definition.
        /// </summary>
        /// <value>The definition of the goal.</value>
        public string Definition { get; set; }

        /// <summary>
        /// Gets or sets the formal specification.
        /// </summary>
        /// <value>The formal specification of the goal.</value>
        public LTLFormula FormalSpec { get; set; }

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

        /// <summary>
        /// Initializes a new goal.
        /// </summary>
        public Goal ()
        {
            Refinements = new HashSet<GoalRefinement> ();
            Obstructions = new HashSet<Obstacle> ();
            AgentAssignments = new HashSet<AgentAssignment> ();
            InSystems = new HashSet<System>();
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
    /// Represents an obstacle.
    /// </summary>
    public class Obstacle : KAOSMetaModelElement
    {
        /// <summary>
        /// Gets or sets the name of the obstacle.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the definition of the obstacle.
        /// </summary>
        /// <value>The definition.</value>
        public string Definition { get; set; }

        /// <summary>
        /// Gets or sets the formal specification of the obstacle.
        /// </summary>
        /// <value>The formal specification.</value>
        public LTLFormula FormalSpec { get; set; }

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
        public IList<Goal> Resolutions { get; set; }

        /// <summary>
        /// Initializes a new obstacle.
        /// </summary>
        public Obstacle ()
        {
            Refinements = new List<ObstacleRefinement> ();
            Resolutions = new List<Goal> ();
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

        /// <summary>
        /// Gets or sets the definition of the domain hypothesis.
        /// </summary>
        /// <value>The definition.</value>
        public string Definition { get; set; }
        
        /// <summary>
        /// Gets or sets the formal specification for the domain property.
        /// </summary>
        /// <value>The formal specification.</value>
        public LTLFormula FormalSpec { get; set; }
        
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

        /// <summary>
        /// Gets or sets the definition of the domain property.
        /// </summary>
        /// <value>The definition.</value>
        public string Definition { get; set; }

        /// <summary>
        /// Gets or sets the formal specification for the domain property.
        /// </summary>
        /// <value>The formal specification.</value>
        public LTLFormula FormalSpec { get; set; }

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

        /// <summary>
        /// Gets or sets the description of the agent.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the type of the agent. The type is either <c>None</c>, 
        /// <c>Software</c>, <c>Environment</c>. 
        /// 
        /// See <see cref="KAOSTools.MetaModel.AgentType"/> for more details.
        /// </summary>
        /// <value>The type.</value>
        public AgentType Type { get; set; }

        public ISet<System> InSystems { get; set; }

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
        public System SystemReference { get; set; }

        /// <summary>
        /// Gets or sets the agents involved with the assignements.
        /// </summary>
        /// <value>The agents.</value>
        public IList<Agent> Agents { get; set; }

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
        public System SystemReference { get; set; }

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
        /// Initializes a new goal refinement.
        /// </summary>
        public GoalRefinement ()
        {
            SystemReference = null;
            Subgoals = new List<Goal> ();
            DomainProperties = new List<DomainProperty> ();
            DomainHypotheses = new List<DomainHypothesis> ();
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
        /// Initializes a new obstacle refinement.
        /// </summary>
        public ObstacleRefinement ()
        {
            Subobstacles = new List<Obstacle> ();
            DomainProperties = new List<DomainProperty> ();
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

    /// <summary>
    /// Represents an alternative system.
    /// </summary>
    public class System : KAOSMetaModelElement
    {
        /// <summary>
        /// Gets or sets the name of the alternative system.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the alternative system.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the alternatives to the system.
        /// </summary>
        /// <value>The alternatives.</value>
        public ISet<System> Alternatives { get; set; }
 
        /// <summary>
        /// Initializes a new system.
        /// </summary>
        public System ()
        {
            Alternatives = new HashSet<System> ();
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
 
        /// <summary>
        /// Gets or sets the definition of the predicate.
        /// </summary>
        /// <value>The definition.</value>
        public string Definition { get; set; }
 
        /// <summary>
        /// Gets or sets the signature of the predicate.
        /// </summary>
        /// <value>The signature.</value>
        public string Signature { get; set; }
 
        /// <summary>
        /// Gets or sets the formal specification of the predicate.
        /// </summary>
        /// <value>The formal specification.</value>
        public string FormalSpec { get; set; }
    }
}
