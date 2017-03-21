using System;
using KAOSTools.Core;
using System.Collections.Generic;
using System.Linq;
using UCLouvain.KAOSTools.Core.Agents;

namespace KAOSTools.Parsing
{
    /// <summary>
    /// First stage builder.
    /// </summary>
    public class FirstStageBuilder : Builder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:FirstStageBuilder"/> class.
        /// </summary>
        /// <param name="model">Model.</param>
        /// <param name="relativePath">Relative path.</param>
        public FirstStageBuilder (KAOSModel model, 
                                  Uri relativePath)
            : base (model, relativePath)
        {}

        public void BuildElementWithKeys (ParsedElements elements)
		{
            // Build the model attributes
			foreach (var element in elements.Values.OfType<ParsedModelAttribute>())
			{
				BuildModelAttributes(element);
			}

            // Build the "level-0" elements 
            foreach (var element in elements.Values.OfType<ParsedDeclare>())
            {
                BuildDeclare (element);
            }
        }

        /// <summary>
        /// Parse the model attributes (author, name, version, etc.)
        /// </summary>
        /// <param name="element">Element.</param>
        public void BuildModelAttributes (ParsedModelAttribute element)
        {
            if (model.Parameters.ContainsKey(element.Name))
            {
                throw new BuilderException(string.Format("'{0}' is already defined", element.Name),
                    element.Filename, element.Line, element.Col);
            }

            model.Parameters.Add(element.Name, element.Value);
        }

        /// <summary>
        /// Build the elements corresponding to the parsed declare. This does not handle the attributes, which
        /// will be handled by the second stage builder.
        /// </summary>
        /// <returns>The element corresponding to the declared element.</returns>
        /// <param name="element">The parsed declare element.</param>
        public KAOSCoreElement BuildDeclare (ParsedDeclare element)
        {
            if (element is ParsedGoal)
                return BuildGoal((ParsedGoal)element);

            if (element is ParsedSoftGoal)
                return BuildSoftGoal ((ParsedSoftGoal)element);

            if (element is ParsedDomainProperty)
                return BuildDomainProperty ((ParsedDomainProperty)element);

            if (element is ParsedDomainHypothesis)
                return BuildDomainHypothesis ((ParsedDomainHypothesis)element);

            if (element is ParsedObstacle)
                return BuildObstacle ((ParsedObstacle)element);

            if (element is ParsedAgent)
                return BuildAgent ((ParsedAgent)element);

            if (element is ParsedPredicate)
                return BuildPredicate ((ParsedPredicate)element);

            if (element is ParsedEntity)
                return BuildEntity ((ParsedEntity)element);

            if (element is ParsedGivenType)
				return BuildGivenType ((ParsedGivenType)element);

            if (element is ParsedAssociation)
				return BuildRelation ((ParsedAssociation)element);

            if (element is ParsedExpert)
                return BuildExpert ((ParsedExpert)element);

            if (element is ParsedCalibration)
				return BuildCalibration ((ParsedCalibration)element);
            
            if (element is ParsedCostVariable)
				return BuildCostVariable ((ParsedCostVariable)element);

            if (element is ParsedConstraint)
				return BuildConstraint((ParsedConstraint)element);
            
            throw new BuilderException (string.Format ("Parsed element '{0}' is not supported", element.GetType ()),
                                        element.Filename, element.Line, element.Col);
		}

		public Goal BuildGoal(ParsedGoal parsedElement)
        {
            Goal g = model.goalRepository.GetGoal(parsedElement.Identifier);
            if (g == null)
            {
                g = new Goal(model, parsedElement.Identifier);
                model.goalRepository.Add(g);
            }

            return g;
		}

		public SoftGoal BuildSoftGoal(ParsedSoftGoal parsedElement)
		{
			SoftGoal g = model.goalRepository.GetSoftGoal(parsedElement.Identifier);
			if (g == null)
			{
				g = new SoftGoal(model, parsedElement.Identifier);
				model.goalRepository.Add(g);
			}

			return g;
		}

        public DomainProperty BuildDomainProperty (ParsedDomainProperty parsedElement)
		{
            DomainProperty g = model.domainRepository.GetDomainProperty(parsedElement.Identifier);
			if (g == null)
			{
				g = new DomainProperty(model, parsedElement.Identifier);
				model.domainRepository.Add(g);
			}

			return g;
		}

        public DomainHypothesis BuildDomainHypothesis (ParsedDomainHypothesis parsedElement)
		{
            DomainHypothesis g = model.domainRepository.GetDomainHypothesis(parsedElement.Identifier);
			if (g == null)
			{
				g = new DomainHypothesis(model, parsedElement.Identifier);
				model.domainRepository.Add(g);
			}

			return g;
		}

        public Obstacle BuildObstacle(ParsedObstacle parsedElement)
		{
            Obstacle g = model.obstacleRepository.GetObstacle(parsedElement.Identifier);
			if (g == null)
			{
				g = new Obstacle(model, parsedElement.Identifier);
				model.obstacleRepository.Add(g);
			}

			return g;
		}

        public Agent BuildAgent (ParsedAgent parsedElement)
		{
            Agent g = model.agentRepository.GetAgent (parsedElement.Identifier);
			if (g == null)
			{
				g = new Agent(model, parsedElement.Identifier);
				model.agentRepository.Add(g);
			}

			return g;
		}

        public Predicate BuildPredicate(ParsedPredicate parsedElement)
		{
            Predicate g = model.formalSpecRepository.GetPredicate (parsedElement.Identifier);
			if (g == null)
			{
				g = new Predicate(model, parsedElement.Identifier);
				model.formalSpecRepository.Add(g);
			}

			return g;
		}

        public Entity BuildEntity(ParsedEntity parsedElement)
		{
			Entity g = model.entityRepository.GetEntity(parsedElement.Identifier);
			if (g == null)
			{
				g = new Entity(model, parsedElement.Identifier);
				model.entityRepository.Add(g);
			}

			return g;
		}

        public GivenType BuildGivenType(ParsedGivenType parsedElement)
		{
			GivenType g = model.entityRepository.GetGivenType(parsedElement.Identifier);
			if (g == null)
			{
				g = new GivenType(model, parsedElement.Identifier);
				model.entityRepository.Add(g);
			}

			return g;
		}

		public Relation BuildRelation(ParsedAssociation parsedElement)
		{
            Relation g = model.entityRepository.GetRelation(parsedElement.Identifier);
			if (g == null)
			{
				g = new Relation(model, parsedElement.Identifier);
				model.entityRepository.Add(g);
			}

			return g;
		}

        public Expert BuildExpert (ParsedExpert parsedElement)
		{
            Expert g = model.modelMetadataRepository.GetExpert(parsedElement.Identifier);
			if (g == null)
			{
				g = new Expert(model, parsedElement.Identifier);
				model.modelMetadataRepository.Add(g);
			}

			return g;
		}

        public Calibration BuildCalibration(ParsedCalibration parsedElement)
		{
            Calibration g = model.modelMetadataRepository.GetCalibration(parsedElement.Identifier);
			if (g == null)
			{
				g = new Calibration(model, parsedElement.Identifier);
				model.modelMetadataRepository.Add(g);
			}

			return g;
		}

        public CostVariable BuildCostVariable(ParsedCostVariable parsedElement)
		{
            CostVariable g = model.modelMetadataRepository.GetCostVariable(parsedElement.Identifier);
			if (g == null)
			{
				g = new CostVariable(model, parsedElement.Identifier);
				model.modelMetadataRepository.Add(g);
			}

			return g;
		}

        public Constraint BuildConstraint(ParsedConstraint parsedElement)
		{
            Constraint g = model.modelMetadataRepository.GetConstraint(parsedElement.Identifier);
			if (g == null)
			{
				g = new Constraint(model, parsedElement.Identifier);
				model.modelMetadataRepository.Add(g);
			}

			return g;
		}
    }
}
