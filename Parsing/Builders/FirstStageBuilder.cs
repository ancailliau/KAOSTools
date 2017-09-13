using System;
using UCLouvain.KAOSTools.Core;
using System.Collections.Generic;
using System.Linq;
using UCLouvain.KAOSTools.Core.Agents;
using UCLouvain.KAOSTools.Parsing.Builders.Declarations;
using UCLouvain.KAOSTools.Parsing.Parsers;

namespace UCLouvain.KAOSTools.Parsing
{
    /// <summary>
    /// First stage builder.
    /// </summary>
    public class FirstStageBuilder : Builder
	{
		List<DeclareBuilder> declareBuilders = new List<DeclareBuilder>();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:FirstStageBuilder"/> class.
        /// </summary>
        /// <param name="model">Model.</param>
        /// <param name="relativePath">Relative path.</param>
        public FirstStageBuilder (KAOSModel model, 
                                  Uri relativePath)
            : base (model, relativePath)
        {
			declareBuilders.Add(new AgentDeclareBuilder());
			declareBuilders.Add(new AssociationDeclareBuilder());
            declareBuilders.Add(new CalibrationDeclareBuilder());
			declareBuilders.Add(new DomainHypothesisDeclareBuilder());
            declareBuilders.Add(new DomainPropertyDeclareBuilder());
            declareBuilders.Add(new EntityDeclareBuilder());
            declareBuilders.Add(new ExpertDeclareBuilder());
			declareBuilders.Add(new GoalDeclareBuilder());
            declareBuilders.Add(new SoftGoalDeclareBuilder());
			declareBuilders.Add(new TypeDeclareBuilder());
			declareBuilders.Add(new ObstacleDeclareBuilder());
            declareBuilders.Add(new PredicateDeclareBuilder());
        }

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
                throw new BuilderException(string.Format("'{0}' is already defined.", element.Name),
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
        public void BuildDeclare (ParsedDeclare element)
        {
            foreach (var builder in declareBuilders)
            {
                if (builder.IsBuildable(element)) {
                    builder.BuildDeclare(element, model);
                    return;
                }
            }

            throw new BuilderException (string.Format ("Parsed element '{0}' is not supported", element.GetType ()),
                                        element.Filename, element.Line, element.Col);
		}
    }
}
