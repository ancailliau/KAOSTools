using System;
using System.Collections.Generic;
using System.Linq;
using UCLouvain.KAOSTools.Core.Agents;
using UCLouvain.KAOSTools.Core.Repositories;
using UCLouvain.KAOSTools.Core.Repositories.Memory;

namespace UCLouvain.KAOSTools.Core
{
    public class KAOSModel
    {
        public string Author
        {
            get
            {
                return Parameters["author"];
            }
        }

		public string Title
		{
			get
			{
				return Parameters["title"];
			}
		}
		public string Version
		{
			get
			{
				return Parameters["version"];
			}
		}

        public Dictionary<string,string> Parameters {
            get;
            set;
        }

        public IAgentRepository agentRepository;
        public IDomainRepository domainRepository;
        public IGoalRepository goalRepository;
        public IModelMetadataRepository modelMetadataRepository;
        public IEntityRepository entityRepository;
        public IObstacleRepository obstacleRepository;
        public IFormalSpecRepository formalSpecRepository;
        public ISatisfactionRateRepository satisfactionRateRepository;

		public KAOSModel ()
        {
            agentRepository = new AgentRepository();
			domainRepository = new DomainRepository();
			goalRepository = new GoalRepository();
			modelMetadataRepository = new ModelMetadataRepository();
			entityRepository = new EntityRepository();
			obstacleRepository = new ObstacleRepository();
            formalSpecRepository = new FormalSpecRepository ();
            satisfactionRateRepository = new SatisfactionRateRepository();

            Parameters = new Dictionary<string, string> ();
        }

        public void Add(Agent agent)
        {
			this.agentRepository.Add(agent);
		}

        public void Add(GoalAgentAssignment agentAssigment)
		{
			this.agentRepository.Add(agentAssigment);
		}

        public void Add(DomainProperty domprop)
		{
            this.domainRepository.Add(domprop);
		}

        public void Add(DomainHypothesis domhyp)
		{
			this.domainRepository.Add(domhyp);
		}

        public void Add(Goal goal)
		{
            this.goalRepository.Add(goal);
		}

		public void Add(SoftGoal softGoal)
		{
			this.goalRepository.Add(softGoal);
		}

		public void Add(GoalRefinement refinement)
		{
			this.goalRepository.Add(refinement);
		}

		public void Add(GoalException exception)
		{
			this.goalRepository.Add(exception);
		}

        public void Add(GoalReplacement replacement)
		{
			this.goalRepository.Add(replacement);
		}

        public void Add(Constraint v)
        {
            this.modelMetadataRepository.Add(v);
        }

		public void Add(CostVariable v)
		{
			this.modelMetadataRepository.Add(v);
		}

		public void Add(Expert v)
		{
			this.modelMetadataRepository.Add(v);
		}

		public void Add(Calibration v)
		{
			this.modelMetadataRepository.Add(v);
		}

		public void Add(EntityAttribute v)
		{
            this.entityRepository.Add(v);
		}

		public void Add(Entity v)
		{
			this.entityRepository.Add(v);
		}

		public void Add(GivenType v)
		{
			this.entityRepository.Add(v);
		}

		public void Add(Link v)
		{
			this.entityRepository.Add(v);
		}

		public void Add(Relation v)
		{
			this.entityRepository.Add(v);
		}

		public void Add(Obstacle v)
		{
            this.obstacleRepository.Add(v);
		}

		public void Add(ObstacleAssumption v)
		{
			this.obstacleRepository.Add(v);
		}

		public void Add(ObstacleRefinement v)
		{
			this.obstacleRepository.Add(v);
		}

		public void Add(Obstruction v)
		{
			this.obstacleRepository.Add(v);
		}

		public void Add(Resolution v)
		{
			this.obstacleRepository.Add(v);
		}

        public void Add(Predicate v)
		{
            this.formalSpecRepository.Add(v);
        }

        /*
		private void Add (KAOSCoreElement element)
        {
            var e = element;
            if (e.model != this)
                e = element.Copy ();

            if (e.model == null)
                e.model = this;

            if (this._elements.ContainsKey(e.Identifier))
                throw new InvalidOperationException ("Duplicated ID " + e.Identifier);

            this._elements.Add (e.Identifier, e);
        }
        */


		public KAOSModel Copy()
		{
			throw new NotImplementedException();
		}
    }

    public class KAOSView : KAOSModel {
    
        public KAOSModel ParentModel { get; set; }

        public KAOSView (KAOSModel model) : base()
        {
            this.ParentModel = model;
        }

    }
}

