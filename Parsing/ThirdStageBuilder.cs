using KAOSTools.Core;
using System.Collections.Generic;
using System;
using System.Linq;

namespace KAOSTools.Parsing
{
    public class ThirdStageBuilder : Builder
    {
        FirstStageBuilder fsb;
        SecondStageBuilder ssb;
        FormulaBuilder fb;
        IDictionary<Predicate, int> predicateArgumentCurrentPosition;

        public ThirdStageBuilder (KAOSModel model, 
                                   IDictionary<KAOSCoreElement, IList<Declaration>> declarations,
                                   FirstStageBuilder fsb,
                                   SecondStageBuilder ssb,
                                  FormulaBuilder fb,
                                  Uri relativePath)
            : base (model, declarations, relativePath)
        {
            this.fsb = fsb;
            this.fb = fb;
            this.ssb = ssb;
            this.predicateArgumentCurrentPosition = new Dictionary<Predicate, int> ();
        }

        public void BuildElement (ParsedElements elements)
        {
            foreach (dynamic element in elements.Values) {
                BuildElement (element);
            }
        }

        public void BuildElement (ParsedElement element)
        {
        }

        public void BuildElement (ParsedModelAttribute element)
        {
        }

		#region

        // Only elements with a formal spec attribute are concerned by the third stage.

		public void BuildElement(ParsedGoal goal)
		{
			var e = model.goalRepository.GetGoal(goal.Identifier);
			if (e == null)
				throw new InvalidOperationException(string.Format("Goal '{0}' was not pre-built.", goal.Identifier));

			BuildElement(goal, e);
		}

		public void BuildElement(ParsedObstacle obstacle)
		{
			var e = model.obstacleRepository.GetObstacle(obstacle.Identifier);
			if (e == null)
				throw new InvalidOperationException(string.Format("Obstacle '{0}' was not pre-built.", obstacle.Identifier));

			BuildElement(obstacle, e);
		}

		public void BuildElement(ParsedDomainProperty domprop)
		{
			var e = model.domainRepository.GetDomainProperty(domprop.Identifier);
			if (e == null)
				throw new InvalidOperationException(string.Format("Domain property '{0}' was not pre-built.", domprop.Identifier));

			BuildElement(domprop, e);
		}

		public void BuildElement(ParsedDomainHypothesis domhyp)
		{
			var e = model.domainRepository.GetDomainHypothesis(domhyp.Identifier);
			if (e == null)
				throw new InvalidOperationException(string.Format("Domain hypothesis '{0}' was not pre-built.", domhyp.Identifier));

			BuildElement(domhyp, e);
		}

		#endregion

		public void BuildElement (ParsedElementWithAttributes element, dynamic e)
        {
            foreach (dynamic attribute in element.Attributes) {
                Handle (e, attribute);
            }
        }
        
        public void Handle (KAOSCoreElement element, object attribute)
        {
            // Ignore all but defined
        }

        public void Handle (Predicate predicate, ParsedPredicateArgumentAttribute ppa)
        {
            var arg_name = ppa.Name;
            Entity arg_type = null;
            if (ppa.Type != null) {
                if (ppa.Type is IdentifierExpression | ppa.Type is NameExpression) {
                    if (!Get<Entity>(ppa.Type, out arg_type)) {
                        arg_type = Create<Entity> (ppa.Type);
                    }
                } else if (ppa.Type is ParsedEntity) {
                    arg_type = fsb.BuildElementWithKeys (ppa.Type);
                    BuildElement (ppa.Type);

                } else {
                    throw new NotImplementedException (string.Format ("'{0}' is not supported in '{1}' on '{2}'", 
                                                                      ppa.Type.GetType().Name,
                                                                      ppa.GetType().Name,
                                                                      predicate.GetType().Name));
                }
            }

            var currentPosition = 0;
            if (!predicateArgumentCurrentPosition.ContainsKey(predicate)) {
                predicateArgumentCurrentPosition.Add (predicate, 0);
            } else {
                currentPosition = predicateArgumentCurrentPosition[predicate];
            }

            // No argument with the same name is already declared
            if (predicate.Arguments.Count (w => w.Name == arg_name) == 0) {
                predicate.Arguments.Add (new PredicateArgument() { Name = arg_name, Type = arg_type });
            }

            // Otherwise, it shall be the same to the one already declared
            else {
                // if no type is already declared, use the new one (may be not declared)
                if (predicate.Arguments[currentPosition].Type == null) {
                    predicate.Arguments[currentPosition].Type = arg_type;
                } 

                // if a type was already declared, it shall be the same (or an ancestor)
                else if (!arg_type.Ancestors().Contains(predicate.Arguments[currentPosition].Type)) {
                    throw new BuilderException (string.Format ("Argument at index {0} does not match. Actual has identifier '{1}' and name '{2}' but expected has identifier '{3}' and name '{4}'. Check that you don't mix name and identifier references on implicit declarations.", 
                                                               currentPosition, 
                                                               predicate.Arguments [currentPosition].Type.Identifier, 
                                                               predicate.Arguments [currentPosition].Type.Name, 
                                                               arg_type.Identifier, 
                                                               arg_type.Name),
                                                ppa.Filename, ppa.Line, ppa.Col);
                }

                // if no name is already declared, use the new one (may be not declared)
                if (predicate.Arguments[currentPosition].Name == null) {
                    predicate.Arguments[currentPosition].Name = arg_name;
                }

                // if a name was already defined, it shall be the same
                else if (predicate.Arguments[currentPosition].Name != arg_name) {
                    throw new BuilderException (string.Format ("Argument at index {0} shall be named '{1}' but is '{2}'", 
                                                               currentPosition, predicate.Arguments [currentPosition].Name, arg_name),
                                                ppa.Filename, ppa.Line, ppa.Col);
                }
            }

            predicateArgumentCurrentPosition[predicate]++;
        }

        public void Handle (Predicate element, ParsedFormalSpecAttribute formalSpec)
        {
            Handle (element, fb.BuildPredicateFormula (element, formalSpec.Value), "FormalSpec");
        }


        public void Handle (KAOSCoreElement element, ParsedFormalSpecAttribute formalSpec)
        {
            // Third stage is required because formula might need information build
            // only on stage 2. For example, ancestors of entities during predicates resolution.
            Handle (element, fb.BuildFormula (formalSpec.Value), "FormalSpec");
        }

        public void Handle<T> (KAOSCoreElement element, 
                               T value, 
                               string propertyName) {
            var definitionProperty = element.GetType ().GetProperty (propertyName);
            if (definitionProperty == null)
                throw new InvalidOperationException (string.Format ("'{0}' has not property '{1}'", 
                                                                    element.GetType (), propertyName));

            if (definitionProperty.PropertyType != typeof(T))
                throw new InvalidOperationException (string.Format ("Type of property '{1}' in '{0}' is not '{2}'", 
                                                                    element.GetType (), propertyName, typeof(T).Name));

            definitionProperty.SetValue (element, value, null);
        }
    }
}

