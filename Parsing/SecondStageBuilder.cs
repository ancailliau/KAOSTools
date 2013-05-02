using KAOSTools.MetaModel;
using System.Collections.Generic;
using System;
using System.Linq;

namespace KAOSTools.Parsing
{
    public class SecondStageBuilder : Builder
    {
        FirstStageBuilder fsb;
        FormulaBuilder fb;
        IDictionary<Predicate, int> predicateArgumentCurrentPosition;

        public SecondStageBuilder (KAOSModel model, 
                                   IDictionary<KAOSMetaModelElement, IList<Declaration>> declarations,
                                   FirstStageBuilder fsb,
                                   FormulaBuilder fb)
            : base (model, declarations)
        {
            this.fsb = fsb;
            this.fb = fb;
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
            throw new NotImplementedException (string.Format("'{0}' is not yet supported", 
                                                             element.GetType().Name));
        }

        public void BuildElement (ParsedElementWithAttributes element)
        {
            var e = GetElement (element);
            if (e == null) 
                throw new InvalidOperationException (string.Format ("Element '{0}' was not pre-built.", element));

            foreach (dynamic attribute in element.Attributes) {
                Handle (e, attribute);
            }
        }

        public void Handle (KAOSMetaModelElement element, ParsedAttribute attribute)
        {
            throw new NotImplementedException (string.Format("'{0}' is not yet supported on '{1}'", 
                                                             attribute.GetType().Name,
                                                             element.GetType ().Name));
        }

        public void Handle (Predicate predicate, ParsedPredicateArgumentAttribute ppa)
        {
            var arg_name = ppa.Name.Value;
            Entity arg_type = null;
            if (ppa.Type != null && !Get<Entity>(ppa.Type, out arg_type))
                arg_type = Create<Entity> (ppa.Type);

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
                
                // if a type was already declared, it shall be the same
                else if (predicate.Arguments[currentPosition].Type != arg_type) {
                    throw new BuilderException (string.Format ("Argument at index {0} shall be '{1}' but is '{2}'.", 
                                                               currentPosition, predicate.Arguments [currentPosition].Type.Name, arg_type.Name),
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
        
        public void Handle (Obstacle element, ParsedResolvedByAttribute resolvedBy)
        {
            if (resolvedBy.Value is IdentifierExpression | resolvedBy.Value is NameExpression) {
                Goal goal;
                if (!Get (resolvedBy.Value, out goal)) {
                    goal = Create<Goal> (resolvedBy.Value);
                }
                element.Resolutions.Add (goal);

            } else if (resolvedBy.Value is ParsedGoal) {
                element.Resolutions.Add (fsb.BuildElementWithKeys (resolvedBy.Value));
                BuildElement (resolvedBy.Value);

            } else {
                throw new NotImplementedException (string.Format ("'{0}' is not supported in '{1}' on '{2}'", 
                                                                  resolvedBy.Value.GetType().Name,
                                                                  resolvedBy.GetType().Name,
                                                                  element.GetType().Name));
            }
        }

        public void Handle (Goal element, ParsedObstructedByAttribute obstructedBy)
        {
            if (obstructedBy.Value is IdentifierExpression | obstructedBy.Value is NameExpression) {
                Obstacle obstacle;
                if (!Get (obstructedBy.Value, out obstacle)) {
                    obstacle = Create<Obstacle> (obstructedBy.Value);
                }
                element.Obstructions.Add (obstacle);
                    
            } else if (obstructedBy.Value is ParsedObstacle) {
                element.Obstructions.Add (fsb.BuildElementWithKeys (obstructedBy.Value));
                BuildElement (obstructedBy.Value);
            } else {
                
                // TODO use string.Format
                throw new NotImplementedException (
                    "'" + obstructedBy.Value.GetType().Name 
                    + "' is not supported in '" 
                    + obstructedBy.GetType().Name + "' on '" 
                    + element.GetType().Name + "'");
            }
        }

        public void Handle (AlternativeSystem element, ParsedAlternativeAttribute alternativeAttribute)
        {
            if (alternativeAttribute.Value is IdentifierExpression | alternativeAttribute.Value is NameExpression) {
                AlternativeSystem alternative;
                if (!Get (alternativeAttribute.Value, out alternative)) {
                    alternative = Create<AlternativeSystem> (alternativeAttribute.Value);
                }
                element.Alternatives.Add (alternative);
                
            } else if (alternativeAttribute.Value is ParsedSystem) {
                element.Alternatives.Add (fsb.BuildElementWithKeys (alternativeAttribute.Value));
                BuildElement (alternativeAttribute.Value);
            } else {
                
                // TODO use string.Format
                throw new NotImplementedException (
                    "'" + alternativeAttribute.Value.GetType().Name 
                    + "' is not supported in '" 
                    + alternativeAttribute.GetType().Name + "' on '" 
                    + element.GetType().Name + "'");
            }
        }

        public void Handle (Goal element, ParsedAssignedToAttribute assignedTo)
        {
            var assignment = new AgentAssignment ();
            
            if (assignedTo.SystemIdentifier != null) {
                AlternativeSystem alternative;
                if (!Get (assignedTo.SystemIdentifier, out alternative)) {
                    alternative = Create<AlternativeSystem> (assignedTo.SystemIdentifier);
                }
                assignment.SystemReference = alternative;
            }
            
            foreach (var child in assignedTo.Values) {
                if (child is IdentifierExpression | child is NameExpression) {
                    Agent agent;
                    if (!Get (child, out agent)) {
                        agent = Create<Agent> (child);
                    }
                    assignment.Agents.Add (agent);
                    
                } else if (child is ParsedAgent) {
                    assignment.Agents.Add (fsb.BuildElementWithKeys (child));
                    BuildElement (child);

                } else {
                    
                    // TODO use string.Format
                    throw new NotImplementedException (
                        "'" + child.GetType().Name 
                        + "' is not supported in '" 
                        + assignedTo.GetType().Name + "' on '" 
                        + element.GetType().Name + "'");
                }
            }
            
            if (!assignment.IsEmpty)
                element.AgentAssignments.Add (assignment);
        }

        public void Handle (Goal element, ParsedRefinedByAttribute refinedBy)
        {
            var refinement = new GoalRefinement ();
            
            if (refinedBy.SystemIdentifier != null) {
                AlternativeSystem alternative;
                if (!Get (refinedBy.SystemIdentifier, out alternative)) {
                    alternative = Create<AlternativeSystem> (refinedBy.SystemIdentifier);
                }
                refinement.SystemReference = alternative;
            }

            foreach (var child in refinedBy.Values) {
                if (child is IdentifierExpression | child is NameExpression) {
                    DomainProperty domprop;
                    if (Get (child, out domprop)) {
                        refinement.DomainProperties.Add (domprop);
                        continue;
                    }

                    DomainHypothesis domhyp;
                    if (Get (child, out domhyp)) {
                        refinement.DomainHypotheses.Add (domhyp);
                        continue;
                    }

                    Goal goal;
                    if (!Get (child, out goal)) {
                        goal = Create<Goal> (child);
                    }
                    refinement.Subgoals.Add (goal);

                } else if (child is ParsedGoal) {
                    refinement.Subgoals.Add (fsb.BuildElementWithKeys (child));
                    BuildElement (child);
                    
                } else if (child is ParsedDomainProperty) {
                    refinement.DomainProperties.Add (fsb.BuildElementWithKeys (child));
                    BuildElement (child);
                    
                } else if (child is ParsedDomainHypothesis) {
                    refinement.DomainHypotheses.Add (fsb.BuildElementWithKeys (child));
                    BuildElement (child);
                    
                } else {
                    
                    // TODO use string.Format
                    throw new NotImplementedException (
                        "'" + child.GetType().Name 
                        + "' is not supported in '" 
                        + refinedBy.GetType().Name + "' on '" 
                        + element.GetType().Name + "'");
                }
            }
            
            if (!refinement.IsEmpty)
                element.Refinements.Add (refinement);
        }

        public void Handle (Obstacle element, ParsedRefinedByAttribute refinedBy)
        {
            var refinement = new ObstacleRefinement ();
            
            foreach (var child in refinedBy.Values) {
                if (child is IdentifierExpression | child is NameExpression) {
                    DomainProperty domprop;
                    if (Get (child, out domprop)) {
                        refinement.DomainProperties.Add (domprop);
                        continue;
                    }
                    
                    DomainHypothesis domhyp;
                    if (Get (child, out domhyp)) {
                        refinement.DomainHypotheses.Add (domhyp);
                        continue;
                    }
                    
                    Obstacle obstacle;
                    if (!Get (child, out obstacle)) {
                        obstacle = Create<Obstacle> (child);
                    }
                    refinement.Subobstacles.Add (obstacle);
                    
                } else if (child is ParsedObstacle) {
                    refinement.Subobstacles.Add (fsb.BuildElementWithKeys (child));
                    BuildElement (child);
                    
                } else if (child is ParsedDomainProperty) {
                    refinement.DomainProperties.Add (fsb.BuildElementWithKeys (child));
                    BuildElement (child);
                    
                } else if (child is ParsedDomainHypothesis) {
                    refinement.DomainHypotheses.Add (fsb.BuildElementWithKeys (child));
                    BuildElement (child);
                    
                } else {

                // TODO use string.Format
                throw new NotImplementedException (
                    "'" + child.GetType().Name 
                    + "' is not supported in '" 
                    + refinedBy.GetType().Name + "' on '" 
                    + element.GetType().Name + "'");
                }
            }
            
            if (!refinement.IsEmpty)
                element.Refinements.Add (refinement);
        }
        
        public void Handle (KAOSMetaModelElement element, ParsedAgentTypeAttribute attribute)
        {
            Handle (element, attribute.Value == ParsedAgentType.Software ? AgentType.Software : AgentType.Environment, "Type");
        }

        public void Handle (KAOSMetaModelElement element, ParsedRDSAttribute attribute)
        {
            Handle (element, attribute.Value, "RDS");
        }
        
        public void Handle (KAOSMetaModelElement element, ParsedProbabilityAttribute attribute)
        {
            Handle (element, attribute.Value, "EPS");
        }

        public void Handle (KAOSMetaModelElement element, ParsedNameAttribute name)
        {
            Handle (element, name.Value, "Name");
        }

        public void Handle (KAOSMetaModelElement element, ParsedDefinitionAttribute definition)
        {
            Handle (element, definition.Value, "Definition");
        }

        public void Handle (KAOSMetaModelElement element, ParsedDescriptionAttribute description)
        {
            Handle (element, description.Value, "Description");
        }

        public void Handle (KAOSMetaModelElement element, ParsedIdentifierAttribute identifier)
        {
            Handle (element, identifier.Value, "Identifier");
        }
        
        public void Handle (KAOSMetaModelElement element, ParsedFormalSpecAttribute formalSpec)
        {
            Handle (element, fb.BuildFormula (formalSpec.Value), "FormalSpec");
        }
        
        public void Handle (Predicate element, ParsedFormalSpecAttribute formalSpec)
        {
            Handle (element, fb.BuildPredicateFormula (element, formalSpec.Value), "FormalSpec");
        }

        public void Handle<T> (KAOSMetaModelElement element, 
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

