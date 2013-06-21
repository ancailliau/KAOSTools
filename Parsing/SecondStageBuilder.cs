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

        public SecondStageBuilder (KAOSModel model, 
                                   IDictionary<KAOSMetaModelElement, IList<Declaration>> declarations,
                                   FirstStageBuilder fsb,
                                   FormulaBuilder fb,
                                   Uri relativePath)
            : base (model, declarations, relativePath)
        {
            this.fsb = fsb;
            this.fb = fb;
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

        public void Handle (KAOSMetaModelElement element, object attribute)
        {
            throw new NotImplementedException (string.Format("'{0}' is not yet supported on '{1}'", 
                                                             attribute.GetType().Name,
                                                             element.GetType ().Name));
        }

        public void Handle (Predicate predicate, ParsedPredicateArgumentAttribute ppa)
        {
            // ignore
        }

        public void Handle (KAOSTools.MetaModel.Attribute attribute, ParsedDerivedAttribute parsedAttribute)
        {
            attribute.Derived = true;
        }

        public void Handle (Goal element, ParsedExceptionAttribute exception)
        {
            Goal goal;
            Obstacle obstacle;

            if (exception.ResolvingGoal == null) {
                goal = null;

            } else {
                if (exception.ResolvingGoal is IdentifierExpression | exception.ResolvingGoal is NameExpression) {
                    if (!Get (exception.ResolvingGoal, out goal)) {
                        goal = Create<Goal> (exception.ResolvingGoal);
                    }

                } else if (exception.ResolvingGoal is ParsedGoal) {
                        goal = fsb.BuildElementWithKeys (exception.ResolvingGoal);
                        BuildElement (exception.ResolvingGoal);
                } else {
                    throw new NotImplementedException (string.Format ("'{0}' is not supported in '{1}' on '{2}'",
                                                                      exception.ResolvingGoal.GetType().Name,
                                                                      exception.GetType().Name,
                                                                      element.GetType().Name));
                }
            }

            if (exception.ResolvedObstacle is IdentifierExpression | exception.ResolvedObstacle is NameExpression) {
                if (!Get (exception.ResolvedObstacle, out obstacle)) {
                    obstacle = Create<Obstacle> (exception.ResolvedObstacle);
                }

            } else if (exception.ResolvingGoal is ParsedGoal) {
                obstacle = fsb.BuildElementWithKeys (exception.ResolvedObstacle);
                BuildElement (exception.ResolvedObstacle);
            } else {
                throw new NotImplementedException (string.Format ("'{0}' is not supported in '{1}' on '{2}'",
                                                                  exception.ResolvedObstacle.GetType().Name,
                                                                  exception.GetType().Name,
                                                                  element.GetType().Name));
            }

            element.Exceptions.Add (new GoalException { 
                ResolvingGoal = goal,
                ResolvedObstacle = obstacle
            });
        }

        public void Handle (Goal element, ParsedAssumptionAttribute assumption)
        {
            if (assumption.Value is IdentifierExpression | assumption.Value is NameExpression) {

                DomainHypothesis domHyp;
                if (Get (assumption.Value, out domHyp)) {
                    element.Assumptions.Add (new DomainHypothesisAssumption {
                        Assumed = domHyp
                    });

                } else {
                    Goal goal;
                    if (!Get (assumption.Value, out goal)) {
                        goal = Create<Goal> (assumption.Value);
                    }
                    element.Assumptions.Add (new GoalAssumption {
                        Assumed = goal
                    });
                }

            } else if (assumption.Value is ParsedGoal) {
                element.Assumptions.Add (new GoalAssumption {
                    Assumed = fsb.BuildElementWithKeys (assumption.Value)
                });
                BuildElement (assumption.Value);
                
            } else if (assumption.Value is ParsedDomainHypothesis) {
                element.Assumptions.Add (new DomainHypothesisAssumption {
                    Assumed = fsb.BuildElementWithKeys (assumption.Value)
                });
                BuildElement (assumption.Value);

            } else {
                throw new NotImplementedException (string.Format ("'{0}' is not supported in '{1}' on '{2}'", 
                                                                  assumption.Value.GetType().Name,
                                                                  assumption.GetType().Name,
                                                                  element.GetType().Name));
            }
        }

        public void Handle (Goal element, ParsedNegativeAssumptionAttribute assumption)
        {
            if (assumption.Value is IdentifierExpression | assumption.Value is NameExpression) {
                Obstacle obstacle;
                if (!Get (assumption.Value, out obstacle)) {
                    obstacle = Create<Obstacle> (assumption.Value);
                }
                element.Assumptions.Add (new ObstacleNegativeAssumption {
                    Assumed = obstacle
                });

            } else if (assumption.Value is ParsedObstacle) {
                element.Assumptions.Add (fsb.BuildElementWithKeys (assumption.Value));
                BuildElement (assumption.Value);

            } else {
                throw new NotImplementedException (string.Format ("'{0}' is not supported in '{1}' on '{2}'", 
                                                                  assumption.Value.GetType().Name,
                                                                  assumption.GetType().Name,
                                                                  element.GetType().Name));
            }
        }

        public void Handle (Obstacle element, ParsedResolvedByAttribute resolvedBy)
        {
            Goal goal;
            if (resolvedBy.Value is IdentifierExpression 
                | resolvedBy.Value is NameExpression) {
                if (!Get (resolvedBy.Value, out goal)) {
                    goal = Create<Goal> (resolvedBy.Value);
                }

            } else if (resolvedBy.Value is ParsedGoal) {
                goal = fsb.BuildElementWithKeys (resolvedBy.Value);
                BuildElement (resolvedBy.Value);

            } else {
                throw new NotImplementedException (string.Format ("'{0}' is not supported in '{1}' on '{2}'", 
                                                                  resolvedBy.Value.GetType().Name,
                                                                  resolvedBy.GetType().Name,
                                                                  element.GetType().Name));
            }

            var resolution = new Resolution (model);
            resolution.SetObstacle (element);
            resolution.SetResolvingGoal (goal);

            if (resolvedBy.Pattern != null) {
                if (resolvedBy.Pattern.Name == "substitution")
                    resolution.ResolutionPattern = ResolutionPattern.GoalSubstitution;

                else if (resolvedBy.Pattern.Name == "prevention")
                    resolution.ResolutionPattern = ResolutionPattern.ObstaclePrevention;

                else if (resolvedBy.Pattern.Name == "obstacle_reduction")
                    resolution.ResolutionPattern = ResolutionPattern.ObstacleReduction;

                else if (resolvedBy.Pattern.Name == "restoration")
                    resolution.ResolutionPattern = ResolutionPattern.GoalRestoration;

                else if (resolvedBy.Pattern.Name == "weakening")
                    resolution.ResolutionPattern = ResolutionPattern.GoalWeakening;

                else if (resolvedBy.Pattern.Name == "weak_mitigation")
                    resolution.ResolutionPattern = ResolutionPattern.ObstacleWeakMitigation;

                else if (resolvedBy.Pattern.Name == "strong_mitigation")
                    resolution.ResolutionPattern = ResolutionPattern.ObstacleStrongMitigation;

                else
                    throw new NotImplementedException ();

                foreach (var parameter in resolvedBy.Pattern.Parameters) {
                    DomainHypothesis hypothesis;
                    if (!Get (parameter, out hypothesis)) {
                        Goal goalAsParameter;
                        if (!Get (parameter, out goalAsParameter)) {
                            goalAsParameter = Create<Goal> (parameter);
                        }
                        resolution.Parameters.Add (goalAsParameter);
                    } else {
                        resolution.Parameters.Add (hypothesis);
                    }
                }
            }

            model.Add (resolution);

        }

        public void Handle (Entity element, ParsedAttributeAttribute attribute)
        {
            var e = new KAOSTools.MetaModel.Attribute (model);

            Handle (e, new ParsedNameAttribute() { 
                Value = attribute.Name
            });

            Handle (e, new ParsedAttributeEntityTypeAttribute() { 
                Value = attribute.Type
            });

            element.Attributes.Add (e);

            declarations.Add (e, new List<Declaration> {
                new Declaration (attribute.Line, attribute.Col, attribute.Filename, relativePath, DeclarationType.Declaration)
            });
        }

        public void Handle (Entity element, ParsedAttributeDeclaration attribute)
        {
            var e = new KAOSTools.MetaModel.Attribute (model);

            foreach (dynamic attr in attribute.Attributes) {
                Handle (e, attr);
            }

            element.Attributes.Add (e);

            declarations.Add (e, new List<Declaration> {
                new Declaration (attribute.Line, attribute.Col, attribute.Filename, relativePath, DeclarationType.Declaration)
            });
        }

        public void Handle (KAOSTools.MetaModel.Attribute element, ParsedAttributeEntityTypeAttribute attribute) {
            GivenType givenType = null;
            if (attribute.Value != null) {
                if (attribute.Value is IdentifierExpression | attribute.Value is NameExpression) {
                    if (!Get (attribute.Value, out givenType)) {
                        givenType = Create<GivenType> (attribute.Value);
                    }

                } else if (attribute.Value is ParsedGivenType) {
                    givenType = fsb.BuildElementWithKeys (attribute.Value);
                    BuildElement (attribute.Value);

                } else {
                    throw new NotImplementedException (string.Format ("'{0}' is not supported in '{1}' on '{2}'", 
                                                                      attribute.Value.GetType().Name,
                                                                      attribute.GetType().Name,
                                                                      element.GetType().Name));
                }
            }

            element.Type = givenType;
        }

        public void Handle (Goal element, ParsedObstructedByAttribute obstructedBy)
        {
            var obstruction = new Obstruction (model);
            obstruction.SetObstructedGoal (element);

            if (obstructedBy.Value is IdentifierExpression | obstructedBy.Value is NameExpression) {
                Obstacle obstacle;
                if (!Get (obstructedBy.Value, out obstacle)) {
                    obstacle = Create<Obstacle> (obstructedBy.Value);
                }
                obstruction.SetObstacle (obstacle);

            } else if (obstructedBy.Value is ParsedObstacle) {
                obstruction.SetObstacle (fsb.BuildElementWithKeys (obstructedBy.Value));
                BuildElement (obstructedBy.Value);

            } else {
                
                // TODO use string.Format
                throw new NotImplementedException (
                    "'" + obstructedBy.Value.GetType().Name 
                    + "' is not supported in '" 
                    + obstructedBy.GetType().Name + "' on '" 
                    + element.GetType().Name + "'");
            }

            model.Add (obstruction);
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
            var assignment = new GoalAgentAssignment (model);
            assignment.GoalIdentifier = element.Identifier;

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
                    assignment.Add (agent);

                } else if (child is ParsedAgent) {
                    assignment.Add (fsb.BuildElementWithKeys (child));
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
                model.Add (assignment);
        }
        
        public void Handle (AntiGoal element, ParsedAssignedToAttribute assignedTo)
        {
            var assignment = new AntiGoalAgentAssignment (model);
            assignment.AntiGoalIdentifier = element.Identifier;

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
                    assignment.Add (agent);

                } else if (child is ParsedAgent) {
                    assignment.Add (fsb.BuildElementWithKeys (child));
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
                model.Add (assignment);
        }

        private void Handle (Obstacle element, ParsedAssignedToAttribute assignedTo)
        {
            var assignment = new ObstacleAgentAssignment (model);
            assignment.ObstacleIdentifier = element.Identifier;

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
                    assignment.Add (agent);

                } else if (child is ParsedAgent) {
                    assignment.Add (fsb.BuildElementWithKeys (child));
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
                model.Add (assignment);
        }

        public void Handle (Goal element, ParsedRefinedByAttribute refinedBy)
        {
            var refinement = new GoalRefinement (model);
            refinement.SetParentGoal(element);

            if (refinedBy.SystemIdentifier != null) {
                AlternativeSystem alternative;
                if (!Get<AlternativeSystem> (refinedBy.SystemIdentifier, out alternative)) {
                    alternative = Create<AlternativeSystem> (refinedBy.SystemIdentifier);
                }
                refinement.SetSystemReference(alternative);
            }

            foreach (var child in refinedBy.Values) {
                if (child is IdentifierExpression | child is NameExpression) {
                    DomainProperty domprop;
                    if (Get (child, out domprop)) {
                        refinement.Add (domprop);
                        continue;
                    }

                    DomainHypothesis domhyp;
                    if (Get (child, out domhyp)) {
                        refinement.Add (domhyp);
                        continue;
                    }

                    Goal goal;
                    if (!Get (child, out goal)) {
                        goal = Create<Goal> (child);
                    }
                    refinement.Add (goal);

                } else if (child is ParsedGoal) {
                    refinement.Add (fsb.BuildElementWithKeys (child));
                    BuildElement (child);
                    
                } else if (child is ParsedDomainProperty) {
                    refinement.Add (fsb.BuildElementWithKeys (child));
                    BuildElement (child);
                    
                } else if (child is ParsedDomainHypothesis) {
                    refinement.Add (fsb.BuildElementWithKeys (child));
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

            if (refinedBy.RefinementPattern != null) {
                if (refinedBy.RefinementPattern.Name == ParsedRefinementPatternName.Milestone) {
                    refinement.RefinementPattern = RefinementPattern.Milestone;
                }

                else if (refinedBy.RefinementPattern.Name == ParsedRefinementPatternName.Case) {
                    refinement.RefinementPattern = RefinementPattern.Case;
                    var caseProbability = refinedBy.RefinementPattern.Parameters.Single() as ParsedFloat;
                    refinement.Parameters.Add (caseProbability.Value);
                }

                else if (refinedBy.RefinementPattern.Name == ParsedRefinementPatternName.IntroduceGuard) {
                    refinement.RefinementPattern = RefinementPattern.IntroduceGuard;
                }

                else if (refinedBy.RefinementPattern.Name == ParsedRefinementPatternName.DivideAndConquer) { 
                    refinement.RefinementPattern = RefinementPattern.DivideAndConquer;
                }

                else if (refinedBy.RefinementPattern.Name == ParsedRefinementPatternName.Uncontrollability) { 
                    refinement.RefinementPattern = RefinementPattern.Uncontrollability;
                }

                else if (refinedBy.RefinementPattern.Name ==  ParsedRefinementPatternName.Unmonitorability) { 
                    refinement.RefinementPattern = RefinementPattern.Unmonitorability;
                }

                else {
                    throw new NotImplementedException ();
                }
            }

            if (!refinement.IsEmpty)
                model.Add (refinement);
        }

        public void Handle (AntiGoal element, ParsedRefinedByAntiGoalAttribute refinedBy)
        {
            var refinement = new AntiGoalRefinement (model);
            refinement.SetParentAntiGoal (element);

            if (refinedBy.SystemIdentifier != null) {
                AlternativeSystem alternative;
                if (!Get<AlternativeSystem> (refinedBy.SystemIdentifier, out alternative)) {
                    alternative = Create<AlternativeSystem> (refinedBy.SystemIdentifier);
                }
                refinement.SetSystemReference (alternative);
            }

            foreach (var child in refinedBy.Values) {
                if (child is IdentifierExpression | child is NameExpression) {
                    DomainProperty domprop;
                    if (Get (child, out domprop)) {
                        refinement.Add (domprop);
                        continue;
                    }

                    DomainHypothesis domhyp;
                    if (Get (child, out domhyp)) {
                        refinement.Add (domhyp);
                        continue;
                    }
                    
                    Obstacle obstacle;
                    if (Get (child, out obstacle)) {
                        refinement.Add (obstacle);
                        continue;
                    }

                    AntiGoal antigoal;
                    if (!Get (child, out antigoal)) {
                        antigoal = Create<AntiGoal> (child);
                    }
                    refinement.Add (antigoal);

                } else if (child is ParsedAntiGoal) {
                    refinement.Add (fsb.BuildElementWithKeys (child));
                    BuildElement (child);

                } else if (child is ParsedObstacle) {
                    refinement.Add (fsb.BuildElementWithKeys (child));
                    BuildElement (child);

                } else if (child is ParsedDomainProperty) {
                    refinement.Add (fsb.BuildElementWithKeys (child));
                    BuildElement (child);

                } else if (child is ParsedDomainHypothesis) {
                    refinement.Add (fsb.BuildElementWithKeys (child));
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
                model.Add (refinement);
        }

        public void Handle (Obstacle element, ParsedRefinedByAttribute refinedBy)
        {
            var refinement = new ObstacleRefinement (model);
            refinement.SetParentObstacle(element);
            
            foreach (var child in refinedBy.Values) {
                if (child is IdentifierExpression | child is NameExpression) {
                    DomainProperty domprop;
                    if (Get (child, out domprop)) {
                        refinement.Add (domprop);
                        continue;
                    }
                    
                    DomainHypothesis domhyp;
                    if (Get (child, out domhyp)) {
                        refinement.Add (domhyp);
                        continue;
                    }
                    
                    Obstacle obstacle;
                    if (!Get (child, out obstacle)) {
                        obstacle = Create<Obstacle> (child);
                    }
                    refinement.Add (obstacle);
                    
                } else if (child is ParsedObstacle) {
                    refinement.Add (fsb.BuildElementWithKeys (child));
                    BuildElement (child);
                    
                } else if (child is ParsedDomainProperty) {
                    refinement.Add (fsb.BuildElementWithKeys (child));
                    BuildElement (child);
                    
                } else if (child is ParsedDomainHypothesis) {
                    refinement.Add (fsb.BuildElementWithKeys (child));
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
                model.Add (refinement);
        }
        
        public void Handle (Entity element, ParsedIsAAttribute attribute)
        {
            if (attribute.Value is IdentifierExpression | attribute.Value is NameExpression) {
                Entity entity;
                if (!Get (attribute.Value, out entity)) {
                    entity = Create<Entity> (attribute.Value);
                }
                element.Parents.Add (entity);
                
            } else if (attribute.Value is ParsedEntity) {
                element.Parents.Add (fsb.BuildElementWithKeys (attribute.Value));
                BuildElement (attribute.Value);
                
            } else {
                throw new NotImplementedException (string.Format ("'{0}' is not supported in '{1}' on '{2}'", 
                                                                  attribute.Value.GetType().Name,
                                                                  attribute.GetType().Name,
                                                                  element.GetType().Name));
            }
        }
        
        public void Handle (Relation element, ParsedLinkAttribute attribute)
        {
            Entity entity;
            if (attribute.Target is IdentifierExpression | attribute.Target is NameExpression) {
                if (!Get (attribute.Target, out entity)) {
                    entity = Create<Entity> (attribute.Target);
                }
                
            } else if (attribute.Target is ParsedEntity) {
                entity = fsb.BuildElementWithKeys (attribute.Target);
                BuildElement (attribute.Target);
                
            } else {
                throw new NotImplementedException (string.Format ("'{0}' is not supported in '{1}' on '{2}'", 
                                                                  attribute.Target.GetType().Name,
                                                                  attribute.GetType().Name,
                                                                  element.GetType().Name));
            }

            element.Links.Add (new Link (model) { Target = entity, Multiplicity = attribute.Multiplicity });
        }

        public void Handle (KAOSMetaModelElement element, ParsedAgentTypeAttribute attribute)
        {
            if (attribute.Value == ParsedAgentType.Environment)
                Handle (element, AgentType.Environment, "Type");

            else if (attribute.Value == ParsedAgentType.Software)
                Handle (element, AgentType.Software, "Type");

            else if (attribute.Value == ParsedAgentType.Malicious)
                Handle (element, AgentType.Malicious, "Type");

            else 
                throw new NotImplementedException ();
        }

        public void Handle (KAOSMetaModelElement element, ParsedEntityTypeAttribute attribute)
        {
            EntityType type;
            if (attribute.Value == ParsedEntityType.Software) {
                type = EntityType.Software;
            } else if (attribute.Value == ParsedEntityType.Environment) {
                type = EntityType.Environment;
            } else if (attribute.Value == ParsedEntityType.Shared) {
                type = EntityType.Shared;
            } else {
                type = EntityType.None;
            }

            Handle (element, type, "Type");
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
            Handle (element, Sanitize (name.Value), "Name");
        }

        public void Handle (KAOSMetaModelElement element, ParsedDefinitionAttribute definition)
        {
            Handle (element, definition.Value.Verbatim ? definition.Value.Value : Sanitize (definition.Value.Value), "Definition");
        }

        public void Handle (KAOSMetaModelElement element, ParsedIdentifierAttribute identifier)
        {
            Handle (element, identifier.Value, "Identifier");
        }
        
        public void Handle (Predicate element, ParsedFormalSpecAttribute formalSpec)
        {
            // ignore, see third stage
        }
        
        public void Handle (KAOSMetaModelElement element, ParsedSignatureAttribute attribute)
        {
            Handle (element, attribute.Value, "Signature");
        }

        public void Handle (KAOSMetaModelElement element, ParsedFormalSpecAttribute formalSpec)
        {
            // ignore, see third stage
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

